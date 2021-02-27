using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using mvc_minitwit.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniTwitIntegrationTests
{
    public class WebApplicationFactoryWithInMemory : BaseWebApplicationFactory<TestStartup>
    {
        private readonly InMemoryDatabaseRoot _databaseRoot = new InMemoryDatabaseRoot();
        private readonly string _connectionString = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ConfigureServices(services);
                ServiceProvider = services.BuildServiceProvider();
            });
        }

        public ServiceProvider ServiceProvider { get; private set; }

        protected void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MvcDbContext>(options =>
            {
                ServiceProvider serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                options.UseInMemoryDatabase("ToDoAppDb");
                options.UseInternalServiceProvider(serviceProvider);
            });
        }
    }
    //builder.ConfigureServices(services =>
    //{
    //    services
    //        .AddEntityFrameworkInMemoryDatabase()
    //        .AddDbContext<MvcDbContext>(options =>
    //        {
    //            options.UseInMemoryDatabase(_connectionString, _databaseRoot);
    //            options.UseInternalServiceProvider(services.BuildServiceProvider());
    //        });
    //});
}
}
