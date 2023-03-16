using DataMigrationApp.Server;
using DataMigrationApp.Server.Data;
using DataMigrationApp.Server.Endpoints;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DataMigrationApp.Tests;

public class MigrationEndpointsTest 
{
    [Fact]
    public async Task MigrateSubscriptionsReturnsOkIfSucceeded() // GetTodoReturnsNotFoundIfNotExists
    {
        // Arrange
        await using var context = InMemoryDb.CreateContext();

        await context.Database.EnsureCreatedAsync();

        var migrations = new[]
        {
            new MigrationEndpoints.SubscriptionMigration("1", "New plan 1"),
            new MigrationEndpoints.SubscriptionMigration("2", "New plan 2")
        };

        // Act
        var okResult = (Ok)await MigrationEndpoints.MigrateSubscriptions(migrations, context, default);

        //Assert
        okResult.StatusCode
            .Should().Be(200);

        context.Subscriptions.Count()
            .Should().Be(2);
    }
}
