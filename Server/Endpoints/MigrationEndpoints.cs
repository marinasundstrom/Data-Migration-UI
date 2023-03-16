using System;
using System.Collections.Generic;
using System.Data;
using Asp.Versioning.Builder;
using DataMigrationApp.Server.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudBlazor;

namespace DataMigrationApp.Server.Endpoints;

public static class MigrationEndpoints
{
    public static WebApplication MapMigrationsEndpoints(this WebApplication app)
    {
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

    public static async Task<IResult> MigrateSubscriptions([FromBody] SubscriptionMigration[] subscriptions, DataContext context, CancellationToken cancellationToken)
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

    public record SubscriptionMigration(string Id, string NewSubscriptionPlanId);
}
