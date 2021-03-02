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
        });
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Connection.Close();
    }
}