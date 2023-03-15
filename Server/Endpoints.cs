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
        var routeGroup = builder.MapGroup("/v{version:apiVersion}/Migration")
            .WithTags("Migration")
            .HasApiVersion(1, 0)
            .WithOpenApi();

        routeGroup.MapPost("/MigrateSubscription", MigrateSubscriptions)
        .WithName("Migration_MigrateSubscription")
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
                SubscriptionId = subscription.Id,
                Created = DateTimeOffset.UtcNow
            });
        }

        await context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    private static void MapCustomersVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder.MapGroup("/v{version:apiVersion}/Customers")
            .WithTags("Customers")
            .HasApiVersion(1, 0)
        //.RequireAuthorization()
            .WithOpenApi();

        routeGroup.MapGet("/{customerId}/Subscriptions", GetCustomerSubscriptions)
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
        var routeGroup = builder.MapGroup("/v{version:apiVersion}/Subscriptions")
            .WithTags("Subscriptions")
            .HasApiVersion(1, 0)
            .WithOpenApi();

        routeGroup.MapGet("/", GetSubscriptions)
        .WithName("Subscriptions_GetSubscriptions")
        .Produces<IEnumerable<Subscription>>(StatusCodes.Status200OK);
    }

    private static Subscription[] GetSubscriptions()
    {
        return new[]
        {
                new Subscription("1", "Subscription 5GB"),
                new Subscription("2", "Subscription 10GB"),
                new Subscription("3", "Subscription 30GB")
            };
    }

    public record Subscription(string Id, string Name);

    public record CustomerSubscription(string Id, string CustomerId, string Name);

    public record SubscriptionMigration(string Id, string NewSubscriptionTypeId);
}