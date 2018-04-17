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

        }

        [Fact]
        public async Task Test_CartPage_Works()
        {
            var response = await client.GetAsync("/Cart");
            response.EnsureSuccessStatusCode();
        }
    }
}
