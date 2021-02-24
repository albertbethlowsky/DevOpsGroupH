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
using Xunit;
using Xunit.Abstractions;

namespace HomeControllerTests
{
    public class HomeControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper output;

        public HomeControllerIntegrationTests(CustomWebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            this.output = output;
            _client = factory.CreateClient();
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
        public async Task Register()
        {
            var user = new User { username = "Bo", email = "bo@bo", 
                                    pw_hash = "very_secure", pw_hash2 = "very_secure"};   //pw_hash val will be hashed
            var response = _client.PostAsJsonAsync("register", user).Result;

            responsePrint(response);


            var stringResponse = await response.Content.ReadAsStringAsync();

            output.WriteLine("DES: " + stringResponse);
            response.EnsureSuccessStatusCode();


        }


        [Fact]
        public async Task CanGetPlayerById()
        {
            // The endpoint or route of the controller action.
            var response = await _client.GetAsync("/msgs");
            //if (response.StatusCode != HttpStatusCode.OK)
            //{
            //    var result = response.Content.ReadAsStringAsync().Result;
            //    output.WriteLine("Http operation unsuccessful");
            //    output.WriteLine(string.Format("Status: '{0}'", response.StatusCode));
            //    output.WriteLine(string.Format("Reason: '{0}'", response.ReasonPhrase));
            //    output.WriteLine(result);
            //}
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
