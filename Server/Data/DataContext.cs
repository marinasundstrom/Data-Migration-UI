using System;
using DataMigrationApp.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationApp.Server.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {

    }

    public DbSet<Subscription> Subscriptions { get; set; }
}

