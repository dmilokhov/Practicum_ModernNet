using EventService.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace EventService.IntegrationTests;

[Collection("Postgres")]
public class DatabaseTests(PostgreSqlFixture fixture) 
{
    [Fact]
    public async Task Migrations_Positive_ShouldBeApplied()
    {
        // Arrange
        await using var context = fixture.CreateContext();

        // Act
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

        // Assert
        pendingMigrations.Should().BeEmpty();
    }

    [Fact]
    public async Task Migrations_Positive_ContainsTables()
    {
        await using var context = fixture.CreateContext();

        var tables = await context.Database
            .SqlQueryRaw<string>(
                """
                SELECT table_name
                FROM information_schema.tables
                WHERE table_schema = 'public'
                """)
            .ToListAsync();

        tables.Should().Contain("events");
        tables.Should().Contain("inbox_messages");
    }
}
