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

namespace HomeControllerTests
{
    public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> factory;
        private readonly ITestOutputHelper output;

        private DbConnection _connection;

        private User dummyUser = new User {
            username = "dummy321",
            email = "dummy@dummy",
            pw_hash = "very_secure",
            pw_hash2 = "very_secure" //pw_hash val will be hashed in the API
        };   

        public HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this.output = output;
            //_client = factory.CreateClient();
            this.factory = factory;
        }

        private DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            return connection;
        }

        protected DbContextOptions<MvcDbContext> ContextOptions { get; }

        public void Dispose() => _connection.Dispose();

        //[Fact]
        //public async Task CanGetPlayers()
        //{
        //    // The endpoint or route of the controller action.
        //    var httpResponse = await _client.GetAsync("/api/players");

        //    // Must be successful.
        //    httpResponse.EnsureSuccessStatusCode();

        //    // Deserialize and examine results.
        //    var stringResponse = await httpResponse.Content.ReadAsStringAsync();
        //    var players = JsonConvert.DeserializeObject<IEnumerable<Message>>(stringResponse);
        //    Assert.Contains(players, p => p.FirstName == "Wayne");
        //    Assert.Contains(players, p => p.FirstName == "Mario");
        //}

        //to see more print: dotnet test --logger:"console;verbosity=detailed"
        //docs: https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-test

        private void responsePrint(HttpResponseMessage resp)
        {
            if (resp.StatusCode != HttpStatusCode.OK)
            {
                var result = resp.Content.ReadAsStringAsync().Result;
                output.WriteLine("Http operation unsuccessful");
                output.WriteLine(string.Format("Status: '{0}'", resp.StatusCode));
                output.WriteLine(string.Format("Reason: '{0}'", resp.ReasonPhrase));
                output.WriteLine(result);
            }
        }

        //[Fact]
        //public async Task Register_Success()
        //{

        //    _client = factory.CreateClient();
        //    var response = await _client.PostAsJsonAsync("/register", dummyUser);

        //    var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();

        //    using (var scope = scopeFactory.CreateScope())
        //    {
        //        var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
        //        var lst = myDataContext.user;
        //        output.WriteLine("All users:");
        //        foreach (User u in lst)
        //            output.WriteLine(u.username);
        //        myDataContext.SaveChanges();
        //        //myDataContext.Dispose();
        //        // Query the in-memory database
        //        myDataContext.Dispose();
        //    }

        //    responsePrint(response);

        //    var stringResponse = await response.Content.ReadAsStringAsync();
        //    output.WriteLine("---> " + stringResponse);

        //    //Assert.Equal("User registered", stringResponse);
        //    response.EnsureSuccessStatusCode();
        //}

        

        //output.WriteLine("statsu:" + response.StatusCode.ToString());

        [Fact]      //make the tests for all other fail-cases:
        public async Task Register_UsernameShouldAlreadyTakenError()
        {
            _client = factory.CreateClient();
            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();


            using (var scope = scopeFactory.CreateScope())
            {
                var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
                var lst = myDataContext.user;
                output.WriteLine("InMem, All users - Before:");
                foreach (User u in lst)
                    output.WriteLine(u.username);
            }

            var initResp = await _client.PostAsJsonAsync("/register", dummyUser);
            var initStrResp = await initResp.Content.ReadAsStringAsync();
            output.WriteLine("INIT " + initStrResp);

            using (var scope = scopeFactory.CreateScope())
            {
                var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
                var lst = myDataContext.user;
                output.WriteLine("InMem, All users - After:");
                foreach (User u in lst)
                    output.WriteLine(u.username);

                //myDataContext.user.Add(dummyUser);
                myDataContext.SaveChanges();
                // Query the in-memory database

            }

            //register same user again:

            var response = await _client.PostAsJsonAsync("/register", dummyUser);
            var strResponse = await response.Content.ReadAsStringAsync();
            output.WriteLine("RESP: " + strResponse);

            

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("The username is already taken", strResponse);
            
        }


        //[Fact]
        //public async Task GetAllMessages()
        //{
        //    // The endpoint or route of the controller action.
        //    _client = factory.CreateClient();

        //    var response = await _client.GetAsync("/msgs");
            
        //    responsePrint(response);

        //    // Must be successful.
        //    response.EnsureSuccessStatusCode();

        //    // Deserialize and examine results.
            

        //    var definition = new { content = "", pub_date = "", user= "" };     // format for the anon-type received

        //    var stringResponse = await response.Content.ReadAsStringAsync();
        //    //output.WriteLine("STR RESP: " + stringResponse);
            
        //    var mess = JsonConvert.DeserializeAnonymousType(stringResponse.Substring(1, stringResponse.Length - 2), definition); //cuts of [ ] to deserialize correctly
        //    //output.WriteLine("DES: " + mess);
            
        //    Assert.Equal("seed data", mess.content);
        //    Assert.Equal("HelloKitty", mess.user);
        //}

        
    }
}
