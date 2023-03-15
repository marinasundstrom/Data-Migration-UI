using System;
using System.Collections.Generic;
using System.Data;
using Asp.Versioning.Builder;
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
        //.RequireAuthorization()
            .WithOpenApi();

        routeGroup.MapPost("/MigrateSubscription", ([FromBody] SubscriptionMigration[] subscriptions) =>
        {
            return Results.Ok();
        })
        .WithName("Migration_MigrateSubscription")
        .Produces(StatusCodes.Status200OK);
    }

    private static void MapCustomersVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder.MapGroup("/v{version:apiVersion}/Customers")
            .WithTags("Customers")
            .HasApiVersion(1, 0)
        //.RequireAuthorization()
            .WithOpenApi();

        routeGroup.MapGet("/{customerId}/Subscriptions", (string customerId) =>
        {
            return new[]
            {
                new CustomerSubscription("1", customerId, "Extra 2GB"),
                new CustomerSubscription("2", customerId, "Test 8GB")
            };
        })
        .WithName("Customers_GetSubscriptions")
        .Produces<IEnumerable<CustomerSubscription>>(StatusCodes.Status200OK);
    }

    private static void MapSubscriptionsVersion1(IVersionedEndpointRouteBuilder builder)
    {
        var routeGroup = builder.MapGroup("/v{version:apiVersion}/Subscriptions")
            .WithTags("Subscriptions")
            .HasApiVersion(1, 0)
        //.RequireAuthorization()
            .WithOpenApi();

        routeGroup.MapGet("/", () =>
        {
            return new[]
            {
                new Subscription("1", "Subscription 5GB"),
                new Subscription("2", "Subscription 10GB"),
                new Subscription("3", "Subscription 30GB")
            };
        })
        .WithName("Subscriptions_GetSubscriptions")
        .Produces<IEnumerable<Subscription>>(StatusCodes.Status200OK);
    }

    public record Subscription(string Id, string Name);

    public record CustomerSubscription(string Id, string CustomerId, string Name);

    public record SubscriptionMigration(string Id, string NewSubscriptionTypeId);
}