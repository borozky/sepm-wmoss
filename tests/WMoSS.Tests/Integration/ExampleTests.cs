using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WMoSS.Tests.Integration
{
    public class ExampleTests : TestFixture<Startup>
    {

        [Fact]
        public async Task SmokeTest_GetRequest()
        {
            var response = await Client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            //var content = await response.Content.ReadAsStringAsync();
        }


        // post requests to a razor page model will result a 400 Bad Request error 
        // if request verification tokens were not included in the request body
        [Fact]
        public async Task SmokeTest_PostRequest()
        {
            var data = new Dictionary<string, string>();
            data["prop1"] = 1.ToString();
            data["prop2"] = "Two".ToString();

            var formData = await Utilities.GetRequestContentAsync(Client, "/", data);
            //var formData = new FormUrlEncodedContent(data); 

            var response = await Client.PostAsync("/", formData);
            var content = response.Content.ReadAsStringAsync();
            response.EnsureSuccessStatusCode();
        }
        
    }
}
