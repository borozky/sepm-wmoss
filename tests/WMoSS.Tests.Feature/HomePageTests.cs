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
using WMoSS.Entities;
using Bogus;
using WMoSS.Tests.TestUtils;
using System.Linq;

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

        [Fact]
        public void SmokeTest_MovieSessions()
        {
            var theaters = Make<Theater>(5);
            var movies = Make<Movie>(10);
            var movieSessions = Make<MovieSession>(100);
            foreach (var movieSession in movieSessions)
            {
                var theaterIds = theaters.Select(t => t.Id).ToArray();
                var index = new Random().Next(theaterIds.Length);
                movieSession.TheaterId = theaterIds[index];

                var movieIds = movies.Select(m => m.Id).ToArray();
                var movieIndex = new Random().Next(movieIds.Length);
                movieSession.MovieId = movieIds[movieIndex];
            }

            Assert.All(movieSessions, m => 
            {
                Assert.NotEqual(0, m.MovieId);
                Assert.NotEqual(0, m.TheaterId);
            });
        }
        




    }
}
