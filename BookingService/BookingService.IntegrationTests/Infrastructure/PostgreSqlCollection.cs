namespace BookingService.IntegrationTests.Infrastructure;

[CollectionDefinition("Postgres")]
public class PostgreSqlCollection : ICollectionFixture<PostgreSqlFixture>
{
}
