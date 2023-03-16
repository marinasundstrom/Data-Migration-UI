using System;

namespace DataMigrationApp.Server.Models;

public class Subscription
{
    public Guid Id { get; set; }

    public string CustomerId { get; set; } = default!;

    public string SubscriptionPlanId { get; set; } = default!;

    public DateTimeOffset Created { get; internal set; }
}

