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

public static class CustomersEndpoints
{
    public static WebApplication MapCustomersEndpoints(this WebApplication app)
    {
        var customers = app.NewVersionedApi("Customers");

        MapCustomersVersion1(customers);

        return app;
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

    public static CustomerSubscription[] GetCustomerSubscriptions(string customerId)
    {
        return new[]
        {
            new CustomerSubscription("1", customerId, "Extra 2GB"),
            new CustomerSubscription("2", customerId, "Test 8GB")
        };
    }

    public record CustomerSubscription(string Id, string CustomerId, string Name);
}
