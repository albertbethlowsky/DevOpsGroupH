using Microsoft.AspNetCore.Mvc.Testing;
using mvc_minitwit;
using mvc_minitwit.Controllers;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit.Abstractions;

//Integration tests for MiniTwit

namespace HomeControllerTests
{
    //public class UnitTest1 : IClassFixture<WebApplicationFactory<Startup>>
    public class UnitTest1 : IClassFixture<WebApplicationFactory<Startup>>

    {
        //private HomeController _homeController;
        private APIController apiController;
        //public HttpClient Client { get; }
        private readonly ITestOutputHelper output;

        private HttpClient Client { get; }

        public UnitTest1(WebApplicationFactory<Startup> fixture, ITestOutputHelper output) 
        {
            Client = fixture.CreateClient();
            this.output = output;
        }


        //[Fact]
        //public async Task Get_Should_Retrieve_Forecast()
        //{
        //    var response = await Client.GetAsync("/msgs");

        //    var temp = "my class!";
        //    output.WriteLine("This is output from {0}", temp);
        //    if (response.StatusCode != HttpStatusCode.OK)
        //    {
        //        var result = response.Content.ReadAsStringAsync().Result;
        //        output.WriteLine("Http operation unsuccessful");
        //        output.WriteLine(string.Format("Status: '{0}'", response.StatusCode));
        //        output.WriteLine(string.Format("Reason: '{0}'", response.ReasonPhrase));
        //        output.WriteLine(result);
        //    }

        //    response.StatusCode.Should().Be(HttpStatusCode.OK);

        //    var forecast = JsonConvert.DeserializeObject<HomeController[]>(await response.Content.ReadAsStringAsync());
        //    forecast.Should().HaveCount(5);
        //}
    }
}
