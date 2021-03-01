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

        private User dummyUser = new User {
            username = "dummy321",
            email = "dummy@dummy",
            pw_hash = "very_secure",
            pw_hash2 = "very_secure" //pw_hash val will be hashed in the API
        };   

        public HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this.output = output;
            this.factory = factory;
        }

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

        private string GetPW_hashFromUser(string username)
        {

            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
                var lst = myDataContext.user;
                var matchingUser = myDataContext.user.Where(u => u.username == username);

                if (matchingUser.Count() == 1)
                    return matchingUser.First().pw_hash;
                else
                    throw new ArgumentException("username: " + username + " not found\n" + "error - count is:" + matchingUser.Count());
            }   
        }

        private void ResponsePrint(HttpResponseMessage resp)
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

        [Fact]
        public async Task Register_Error_HaveToEnterPW()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            var _client = appF.CreateClient();

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

        [Fact]
        public async Task Register_Error_PWsDontMatch()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            var _client = appF.CreateClient();

            dummyUser.pw_hash = "123";
            dummyUser.pw_hash2 = "321";
            var resp = await _client.PostAsJsonAsync("/register", dummyUser);

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

        }

        [Fact]
        public async Task Register_Error_InvalidEmail()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            var _client = appF.CreateClient();

            dummyUser.email = "noatsign";
            var resp = await _client.PostAsJsonAsync("/register", dummyUser);
            var strResp = await resp.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
            Assert.Equal("You have to enter a valid email address", strResp);

            dummyUser.email = "";
            var resp2 = await _client.PostAsJsonAsync("/register", dummyUser);
            var strResp2 = await resp.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.BadRequest, resp2.StatusCode);
            Assert.Equal("You have to enter a valid email address", strResp2);
        } 


        [Fact]
        public async Task Register_Error_UsernameAlreadyTaken()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            var _client = appF.CreateClient();

            var initResp = await _client.PostAsJsonAsync("/register", dummyUser);
            var initStrResp = await initResp.Content.ReadAsStringAsync();
            output.WriteLine("INIT " + initStrResp);

            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
                myDataContext.Database.EnsureDeleted();
                myDataContext.Database.EnsureCreated();
                var lst = myDataContext.user;
                output.WriteLine("InMem, All users - Before:");
                foreach (User u in lst)
                    output.WriteLine(u.username);
            }

            //register same user again:
            var response = await _client.PostAsJsonAsync("/register", dummyUser);
            var strResponse = await response.Content.ReadAsStringAsync();
            output.WriteLine("RESP " + strResponse);

          

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("The username is already taken", strResponse);

        }


        [Fact]
        public async Task Register_Success()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            _client = appF.CreateClient();

            var response = await _client.PostAsJsonAsync("/register", dummyUser);
           
            ResponsePrint(response);

            var stringResponse = await response.Content.ReadAsStringAsync();
            output.WriteLine(stringResponse);

            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();
            using (var scope = scopeFactory.CreateScope())
            {
                var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
                myDataContext.Database.EnsureCreated();
                var lst = myDataContext.user;
                output.WriteLine("InMem, All users - Before:");
                foreach (User u in lst)
                    output.WriteLine(u.username);
            }

            Assert.Equal("User registered", stringResponse);
            response.EnsureSuccessStatusCode();
            
        }

        //test_login_logout 
        [Fact]
        public async Task Login_LogOut()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            _client = appF.CreateClient();
            var resp = await _client.PostAsJsonAsync("/register", dummyUser);
            resp.EnsureSuccessStatusCode();
            var stringResponse = await resp.Content.ReadAsStringAsync();
            output.WriteLine("REGISTER: " + stringResponse);

            var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                var myDataContext = scope.ServiceProvider.GetService<MvcDbContext>();
                myDataContext.Database.EnsureCreated();
                var lst = myDataContext.user;
                output.WriteLine("InMem, All users - Before:");
                foreach (User u in lst)
                    output.WriteLine(u.username);
            }

            var pw_hash = GetPW_hashFromUser(dummyUser.username);
            output.WriteLine("pw_hash: " + pw_hash);

            //var userCredentials = new StringContent($"email ={ dummyUser.email } & pw_hash ={ pw_hash }");

            var loginResp = await _client.GetAsync("/Home/SignIn?email="+dummyUser.email+"&pw_hash="+pw_hash);
            var content = await loginResp.Content.ReadAsStringAsync();
            output.WriteLine("LOGIN content: "+ content);

            loginResp.EnsureSuccessStatusCode();

        }

        [Fact]
        public async Task CreateMessageByUser_Success()
        {
            var appF = new CustomWebApplicationFactory<MvcDbContext>();
            _client = appF.CreateClient();

            await _client.PostAsJsonAsync("/register", dummyUser);

            var request = new HttpRequestMessage(HttpMethod.Post, "msgs/" + dummyUser.username);

            //request.Content = new StringContent(JsonSerializer.Serialize(new {
            //    term = "MFA",
            //    definition = "An authentication process that considers multiple factors."
            //}), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.SendAsync(request);

            var mess = new Message { author_id = 0, text = "test text", pub_date = (int)(DateTimeOffset.Now.ToUnixTimeSeconds()) };
            //_client.PostAsync("/msgs/"+dummyUser.username, mess);
        }


        [Fact]
        public async Task GetAllMessages()
        {
            // The endpoint or route of the controller action.
            _client = factory.CreateClient();

            var response = await _client.GetAsync("/msgs");

            ResponsePrint(response);

            response.EnsureSuccessStatusCode();

            var definition = new { content = "", pub_date = "", user = "" };     // format for the anon-type received

            var stringResponse = await response.Content.ReadAsStringAsync();
            //output.WriteLine("STR RESP: " + stringResponse);

            var mess = JsonConvert.DeserializeAnonymousType(stringResponse.Substring(1, stringResponse.Length - 2), definition); //cuts of [ ] to deserialize correctly

            Assert.Equal("seed data", mess.content);
            Assert.Equal("SeedUser", mess.user);
        }


    }
}
