using System;
using DataMigrationApp.Server.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationApp.Tests;

public static class InMemoryDb
{
    public static DataContext CreateContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var dbContextOptions = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(connection)
            .Options;

        return new DataContext(dbContextOptions);
    }
}
