using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using mvc_minitwit.Data; //database context
using mvc_minitwit.Models; //model context
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Swagger;
using Serilog;
using Prometheus;

namespace mvc_minitwit
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment Environment;

        // This method gets called by the runtime. Use this method to add services to the container.
        //test trigger pipeline4
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            if(Environment.IsDevelopment()) {
            services.AddDbContext<MvcDbContext>(options =>
                 options.UseSqlite(Configuration.GetConnectionString("DefaultConnection"))); //database context
            } else {
            services.AddDbContext<MvcDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MvcDbContext"))); //production database context
            }
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "minitwit-API", Version = "1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Custom Metrics to count requests for each endpoint and the method
            var counter = Metrics.CreateCounter("minitwitAPI", "Does great monitoring on API endpoints", new CounterConfiguration
            {
            LabelNames = new[] { "method", "endpoint" }
            });

            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                return next();
            });

            // Use the Prometheus Middleware
            app.UseMetricServer();
            app.UseHttpMetrics();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "minitwit-API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Timeline}/{id?}");
            });
        }


    }
}
