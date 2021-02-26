using HomeControllerTests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using mvc_minitwit.Data;
using System;
using System.Linq;

public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private SqliteConnection Connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        //Connection = new SqliteConnection("DataSource=:memory:");
        Connection = new SqliteConnection("DataSource=:memory:");

        Connection.Open();

        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            // Unregister existing database service (SQL Server).
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<MvcDbContext>));

            if (descriptor != null) services.Remove(descriptor);

            // Register new database service (SQLite In-Memory)
            services.AddDbContext<MvcDbContext>(options => options.UseSqlite(Connection));

            // Build the service provider.
            var sp = services.BuildServiceProvider();


            // Create a scope to obtain a reference to the database contexts
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<MvcDbContext>();

                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                // Ensure the database is created.
                appDb.Database.EnsureCreated();

                try
                {
                    // Seed the database with some specific test data.
                    SeedData.PopulateTestData(appDb);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred seeding the " +
                                        "database with test messages. Error: {ex.Message}");
                }
            }
        });
    //});
       
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Connection.Close();
    }
}