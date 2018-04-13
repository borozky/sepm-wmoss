using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace WMoSS.Tests.Feature
{
    public class HomePageTests : TestCase
    {
        public HomePageTests() : base()
        {
        }

        [Fact]
        public async Task Test_HomePage_Works()
        {
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

    }
}
