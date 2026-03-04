using Bogus;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Apartments;
using Dapper;

namespace Bookify.Api.Extensions;

public static class SeedDataExtension
{
    /// <summary>
    /// Extension method used to seed the database with fake apartment data.
    /// It runs once at application startup to populate the database for testing/demo purposes.
    /// </summary>
    public static void SeedData(this IApplicationBuilder app)
    {
        // Create a new dependency injection scope
        using var scope = app.ApplicationServices.CreateScope();

        // Resolve the SQL connection factory from DI container
        var sqlConnectionFactory = scope.ServiceProvider.GetRequiredService<ISqlConnectionFactory>();

        // Create a database connection
        using var connection = sqlConnectionFactory.CreateConnection();

        // Bogus library used to generate fake data
        var faker = new Faker();

        // List that will contain all generated apartment objects
        List<object> apartments = new();

        // Generate 100 fake apartment records
        for (int i = 0; i < 100; i++)
        {
            apartments.Add(new
            {
                // Unique apartment identifier
                Id = Guid.NewGuid(),

                // Random company name used as apartment name
                Name = faker.Company.CompanyName(),

                // Static description
                Description = "Amazing view",

                // Random address data
                Country = faker.Address.Country(),
                State = faker.Address.State(),
                ZipCode = faker.Address.ZipCode(),
                City = faker.Address.City(),
                Street = faker.Address.StreetAddress(),

                // Random price between 50 and 1000
                PriceAmount = faker.Random.Decimal(50, 1000),
                PriceCurrency = "USD",

                // Random cleaning fee between 25 and 200
                CleaningFeeAmount = faker.Random.Decimal(25, 200),
                CleaningFeeCurrency = "USD",

                // Apartment amenities stored as enum values
                Amenities = new List<int>
                {
                    (int)Amanity.Parking,
                    (int)Amanity.MountainView
                },

                // Default value meaning the apartment has never been booked
                LastBookedOn = DateTime.MinValue
            });
        }

        // SQL query used to insert apartment records into PostgreSQL
        const string sql = """
                INSERT INTO public.apartments
                (id, "name", description, address_country, address_state, address_zip_code, address_city, address_street, price_amount, price_currency, cleaning_fee_amount, cleaning_fee_currency, amanities, last_booked_on_utc)
                VALUES(@Id, @Name, @Description, @Country, @State, @ZipCode, @City, @Street, @PriceAmount, @PriceCurrency, @CleaningFeeAmount, @CleaningFeeCurrency, @Amenities, @LastBookedOn);
                """;

        // Execute the insert query using Dapper
        // This will insert all apartments in a batch operation
        connection.Execute(sql, apartments);
    }
}