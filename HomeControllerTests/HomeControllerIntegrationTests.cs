using mvc_minitwit;
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

namespace HomeControllerTests
{
    public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private HttpClient _client;
        private CustomWebApplicationFactory<Startup> factory;
        private readonly ITestOutputHelper output;
        private User dummyUser = new User {
            username = "Bo",
            email = "bo@bo",
            pw_hash = "very_secure",
            pw_hash2 = "very_secure" //pw_hash val will be hashed in the API
        };   

        public HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this.output = output;
            _client = factory.CreateClient();
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

        [Fact]
        public async Task Register_Success()
        {
            _client = factory.CreateClient();
            var response = _client.PostAsJsonAsync("/register", dummyUser).Result;

            responsePrint(response);
            //output.WriteLine("statsu:" + response.StatusCode.ToString());

            var stringResponse = await response.Content.ReadAsStringAsync();
            
            response.EnsureSuccessStatusCode();
            Assert.Equal("User registered", stringResponse);
        }

        [Fact]      //make the tests for all other fail-cases:
        public async Task Register_UsernameShouldAlreadyTakenError()
        {
            var initResp = await _client.PostAsJsonAsync("/register", dummyUser);
            var initStrResp = await initResp.Content.ReadAsStringAsync();
            output.WriteLine("INIT " + initStrResp);

            //_client.PostAsJsonAsync("/register", dummyUser).Result;
            //_client.PostAsJsonAsync("/register", dummyUser).Result;
            
            //register same user again:

            var response = _client.PostAsJsonAsync("/register", dummyUser).Result;
            var strResponse = await response.Content.ReadAsStringAsync();
            output.WriteLine("RESP: " + strResponse);

            

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Equal("The username is already taken", strResponse);
            
        }


        [Fact]
        public async Task GetAllMessages()
        {
            // The endpoint or route of the controller action.
            var response = await _client.GetAsync("/msgs");
            
            responsePrint(response);

            // Must be successful.
            response.EnsureSuccessStatusCode();

            // Deserialize and examine results.
            

            var definition = new { content = "", pub_date = "", user= "" };     // format for the anon-type received

            var stringResponse = await response.Content.ReadAsStringAsync();
            //output.WriteLine("STR RESP: " + stringResponse);
            
            var mess = JsonConvert.DeserializeAnonymousType(stringResponse.Substring(1, stringResponse.Length - 2), definition); //cuts of [ ] to deserialize correctly
            //output.WriteLine("DES: " + mess);
            
            Assert.Equal("seed data", mess.content);
            Assert.Equal("HelloKitty", mess.user);
        }

        
    }
}
