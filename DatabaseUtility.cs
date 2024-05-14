using BlazorWebAppEFCore.Data;
using Microsoft.EntityFrameworkCore;
/*
public static class DatabaseUtility
{
    // Method to see the database. Should not be used in production: demo purposes only.
    // options: The configured options.
    // count: The number of contacts to seed.
    public static async Task EnsureDbCreatedAndSeedWithCountOfAsync(DbContextOptions<OnderdeelContext> options, int count)
    {
        // Empty to avoid logging while inserting (otherwise will flood console).
        var factory = new LoggerFactory();
        var builder = new DbContextOptionsBuilder<OnderdeelContext>(options)
            .UseLoggerFactory(factory);

        using var context = new OnderdeelContext(builder.Options);
        // Result is true if the database had to be created.
        if (await context.Database.EnsureCreatedAsync())
        {
            var seed = new SeedOnderdelen();
            await seed.SeedDatabaseWithOnderdeelCountOfAsync(context, count);
        }
    }
}
*/