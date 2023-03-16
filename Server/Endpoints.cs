using System;
using System.Collections.Generic;
using System.Data;
using Asp.Versioning.Builder;
using DataMigrationApp.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace DataMigrationApp.Server;

public static class Endpoints
{
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var subscriptions = app.NewVersionedApi("Subscriptions");

        MapSubscriptionsVersion1(subscriptions);

        var customers = app.NewVersionedApi("Customers");

        MapCustomersVersion1(customers);

        var migration = app.NewVersionedApi("Migration");

        MapMigrationVersion1(migration);

        return app;
    }

    private static void MapMigrationVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/Migration")
            .WithTags("Migration")
            .HasApiVersion(1, 0)
            .WithOpenApi();

        routeGroup
            .MapPost("/MigrateSubscriptions", MigrateSubscriptions)
            .WithName("Migration_MigrateSubscriptions")
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> MigrateSubscriptions([FromBody] SubscriptionMigration[] subscriptions, DataContext context, CancellationToken cancellationToken)
    {
        foreach (var subscription in subscriptions)
        {
            context.Subscriptions.Add(new Models.Subscription()
            {
                Id = Guid.NewGuid(),
                CustomerId = "Customer123",
                SubscriptionPlanId = subscription.NewSubscriptionPlanId,
                Created = DateTimeOffset.UtcNow
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    private static void MapCustomersVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/Customers")
            .WithTags("Customers")
            .HasApiVersion(1, 0)
          //.RequireAuthorization()
            .WithOpenApi();

        routeGroup
            .MapGet("/{customerId}/Subscriptions", GetCustomerSubscriptions)
            .WithName("Customers_GetSubscriptions")
            .Produces<IEnumerable<CustomerSubscription>>(StatusCodes.Status200OK);
    }

    private static CustomerSubscription[] GetCustomerSubscriptions(string customerId)
    {
        return new[]
        {
            new CustomerSubscription("1", customerId, "Extra 2GB"),
            new CustomerSubscription("2", customerId, "Test 8GB")
        };
    }

    private static void MapSubscriptionsVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder
            .MapGroup("/v{version:apiVersion}/Subscriptions")
            .WithTags("Subscriptions")
            .HasApiVersion(1, 0)
            .WithOpenApi();

        routeGroup
            .MapGet("/Plans", GetSubscriptionPlans)
            .WithName("Subscriptions_GetSubscriptionPlans")
            .Produces<IEnumerable<SubscriptionPlan>>(StatusCodes.Status200OK);
    }

    private static SubscriptionPlan[] GetSubscriptionPlans()
    {
        return new[]
        {
            new SubscriptionPlan("1", "SM 5GB"),
            new SubscriptionPlan("2", "MD 10GB"),
            new SubscriptionPlan("3", "XL 30GB")
        };
    }

    public record SubscriptionPlan(string Id, string Name);

    public record CustomerSubscription(string Id, string CustomerId, string Name);

    public record SubscriptionMigration(string Id, string NewSubscriptionPlanId);
}
