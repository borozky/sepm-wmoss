using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace WMoSS.Tests.Feature
{
    public class CartPageTests : TestCase
    {
        public CartPageTests() : base()
        {
            SeedData();
        }

        [Fact]
        public async Task Test_CartPage_Works()
        {
            var response = await client.GetAsync("/Cart");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Test_AddToCart_Works()
        {
            var data = new Dictionary<string, string>();
            data["MovieSessionId"] = "2";
            data["TicketQty"] = "1";

            var formData = await TestCase.GetRequestContentAsync(client, "/Movie/Details/2", data);

            var response = await client.PostAsync("/Cart", formData);

        }
    }
}
