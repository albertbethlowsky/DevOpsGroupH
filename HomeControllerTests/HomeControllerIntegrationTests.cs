using mvc_minitwit;
using mvc_minitwit.Data;
using mvc_minitwit.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Data.Sqlite;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using mvc_minitwit.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HomeControllerTests
{
    public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> factory;
        private readonly ITestOutputHelper output;
        private User dummyUser = new User
        {
            username = "dummy321",
            email = "dummy@dummy",
            pw_hash = "very_secure",
            pw_hash2 = "very_secure" //pw_hash val will be hashed in the API
        };
        private readonly IServiceScope _scope;
        private readonly MvcDbContext _context;
        private readonly CookieContainer cookies = new System.Net.CookieContainer();

        public HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this.output = output;
            this.factory = factory;

            _client = factory.CreateDefaultClient();
            _scope = (factory.Services.GetRequiredService<IServiceScopeFactory>()).CreateScope();
            _context = _scope.ServiceProvider.GetRequiredService<MvcDbContext>();
            // database is now shared across tests
            _context.Database.EnsureCreated();
        }

        [Fact]
        public async Task test_TimeLine()
        {
            //_client = factory.CreateClient();
            var logger = _scope.ServiceProvider.GetRequiredService<ILogger<HomeController>>();
            HomeController hc = new HomeController(logger, _context);
            hc.Timeline(dummyUser.username);

            // 2 - Act
				var actionResult = hc.Timeline(dummyUser.username) as ViewResult; // Call the edit view with no item Id (Add New).
            
            dummyUser.pw_hash = null;
            var resp = await _client.PostAsJsonAsync("/register", dummyUser);

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);


            dummyUser.pw_hash = "";
            var resp2 = await _client.PostAsJsonAsync("/register", dummyUser);
            //var strResp2 = await resp.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, resp2.StatusCode);

            //doesn't work since get this back: https://tools.ietf.org/html/rfc7231#section-6.5.1
            //which is json. Have to parse it to that. Error is gen. from ApiData model
            //Assert.Equal("You have to enter a password", strResp2);
        }

       



    }
}