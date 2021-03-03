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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace HomeControllerTests
{
    public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    //public class HomeControllerIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
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

        
        //private readonly WebApplicationFactory<Startup> factory;

        //public HomeControllerIntegrationTests(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        //{
        //    this.output = output;
        //    this.factory = factory;
        //}
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

        private void PrintResp(HttpResponseMessage resp)
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

        private void PrintUser()
        {
            output.WriteLine("Users:");
            foreach (User u in _context.user)
                output.WriteLine(u.username);
        }

        [Fact]
        public async Task Register_Error_HaveToEnterPW()
        {
            _client = factory.CreateClient();

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
            
            _client = factory.CreateClient();

            dummyUser.pw_hash = "123";
            dummyUser.pw_hash2 = "321";
            var resp = await _client.PostAsJsonAsync("/register", dummyUser);

            Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);

        }

        [Fact]
        public async Task Register_Error_InvalidEmail()
        {
            _client = factory.CreateClient();

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
            _client = factory.CreateClient();

            var initResp = await _client.PostAsJsonAsync("/register", dummyUser);
            var initStrResp = await initResp.Content.ReadAsStringAsync();
            output.WriteLine("INIT " + initStrResp);

            //register same user again:
            var response = await _client.PostAsJsonAsync("/register", dummyUser);
            var strResponse = await response.Content.ReadAsStringAsync();
            output.WriteLine("RESP " + strResponse);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            //Assert.Equal("The username is already taken", strResponse);
            
        }


        [Fact]
        public async Task Register_Success()
        {
            
            _client = factory.CreateClient();

            dummyUser.username = "123dummy";
            var response = await _client.PostAsJsonAsync("/register", dummyUser);
           
            output.WriteLine(await response.Content.ReadAsStringAsync());

            var users = _context.user.ToArray();
            output.WriteLine("CONTEXT:");
            foreach (User u in users)
                output.WriteLine(u.username);

            //Assert.Equal("User registered", stringResponse);
            response.EnsureSuccessStatusCode();

        }

        
        //string GetCookieValueFromResponse(HttpResponse response, string cookieName)
        //{
        //    foreach (var headers in response.Headers.Values)
        //        foreach (var header in headers)
        //            if (header.StartsWith($"{cookieName}="))
        //            {
        //                var p1 = header.IndexOf('=');
        //                var p2 = header.IndexOf(';');
        //                return header.Substring(p1 + 1, p2 - p1 - 1);
        //            }
        //    return null;
        //}

        [Fact]
        public async Task Login_LogOut_Success()
        {
            _client = factory.CreateClient();
            dummyUser.username = "Login_LogOut_TestUser";
            var resp = await _client.PostAsJsonAsync("/register", dummyUser);
            resp.EnsureSuccessStatusCode();
            // output.WriteLine("REGISTER: " + await resp.Content.ReadAsStringAsync());

            PrintUser();

            //var pw_hash = GetPW_hashFromUser(dummyUser.username);
            var loginResp = await _client.PostAsync("/api/SignIn?email=" + dummyUser.email + "&password=" + dummyUser.pw_hash, null);
            output.WriteLine("SING: " + await loginResp.Content.ReadAsStringAsync());
            
            loginResp.EnsureSuccessStatusCode();
            IEnumerable<string> values;
            if (loginResp.Headers.TryGetValues("Set-Cookie", out values))
            {
                string cookie = values.First();
                output.WriteLine(cookie);
            }
            Assert.True(values.Any());

            var logOutResp = await _client.GetAsync("/api/Sign_Out");

            IEnumerable<string> values1;
            if (logOutResp.Headers.TryGetValues("Set-Cookie", out values1))
            {
                string cookie = values1.First();
                output.WriteLine(cookie);
            }
            Assert.True(values1 == null);

            //var res = await homeC.SignIn(dummyUser.email, pw_hash);
            //var a = Assert.IsType<RedirectToActionResult>(res);


            //var result = await new HomeController(_context).SignIn(dummyUser.email, pw_hash) as ViewResult;
            //Console.WriteLine(result);
            //Assert.Equal("Index", result.ViewName);

        }

        [Fact]
        public async Task Login_LogOut_InvalidEmail_Or_PW()
        {
            PrintUser();
            _client = factory.CreateClient();
            var invalidEmailResp = await _client.GetAsync("api/SignIn?email=NoSuch@Mail&pw_hash=totally_legit_pw");
            //output.WriteLine("RESP: " + await invalidEmailResp.Content.ReadAsStringAsync());
            PrintResp(invalidEmailResp);

            Assert.Equal(HttpStatusCode.Redirect, invalidEmailResp.StatusCode);     //check user is redirected to sign-in 
            

            dummyUser.email = SeedData.user.email;  //valid email from seed data user
            dummyUser.pw_hash = "incorrect_pw";
            var invalidPWResp = await _client.GetAsync("api/SignIn?email=" + dummyUser.email + "NoSuch@Mail&pw_hash=" + dummyUser.pw_hash);
            PrintResp(invalidPWResp);

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


        //[Fact]
        //public async Task GetAllMessages()
        //{
        //    // The endpoint or route of the controller action.
        //    _client = factory.CreateClient();

        //    var response = await _client.GetAsync("/msgs");

        //    ResponsePrint(response);

        //    response.EnsureSuccessStatusCode();

        //    var definition = new { content = "", pub_date = "", user = "" };     // format for the anon-type received

        //    var stringResponse = await response.Content.ReadAsStringAsync();
        //    //output.WriteLine("STR RESP: " + stringResponse);

        //    var mess = JsonConvert.DeserializeAnonymousType(stringResponse.Substring(1, stringResponse.Length - 2), definition); //cuts of [ ] to deserialize correctly

        //    Assert.Equal("seed data", mess.content);
        //    Assert.Equal("SeedUser", mess.user);
        //}


    }
}
