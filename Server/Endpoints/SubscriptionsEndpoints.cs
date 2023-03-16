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

public static class SubscriptionsEndpoints
{
    public static WebApplication MapSubscriptionEndpoints(this WebApplication app)
    {
        var subscriptions = app.NewVersionedApi("Subscriptions");

        MapSubscriptionsVersion1(subscriptions);

        return app;
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

    public static SubscriptionPlan[] GetSubscriptionPlans()
    {
        return new[]
        {
            new SubscriptionPlan("1", "SM 5GB"),
            new SubscriptionPlan("2", "MD 10GB"),
            new SubscriptionPlan("3", "XL 30GB")
        };
    }

    public record SubscriptionPlan(string Id, string Name);
}
