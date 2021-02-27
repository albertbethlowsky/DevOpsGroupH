using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using mvc_minitwit;
using System;
using System.Collections.Generic;
using System.Text;


namespace MiniTwitIntegrationTests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            // Database providers are injected in WebApplicationFactoryWithPROVIDER.cs classes
            services.AddTransient<TestDataSeeder>();
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            base.Configure(app, env);
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var seeder = serviceScope.ServiceProvider.GetService<TestDataSeeder>();
            seeder.SeedToDoItems();
        }
    }
}
