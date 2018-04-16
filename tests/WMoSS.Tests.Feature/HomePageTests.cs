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

            int theaterId = 2;
            Define<Theater>(() =>
            {
                return new Faker<Theater>()
                .RuleFor(t => t.Id, f => theaterId++)
                .RuleFor(t => t.Name, f => $"Theater #{theaterId - 2}")
                .RuleFor(t => t.Capacity, f => 50)
                .RuleFor(t => t.Address, f => "123 Lygon Street, Melbourne")
                .Generate();
            });

            int movieId = 2;
            Define<Movie>(() =>
            {
                return new Faker<Movie>()
                .RuleFor(m => m.Id, f => movieId++)
                .RuleFor(m => m.Title, f => f.Lorem.Text())
                .RuleFor(m => m.Genre, f => f.Lorem.Word())
                .RuleFor(m => m.ReleaseYear, f => f.Random.Number(2017, 2019))
                .RuleFor(m => m.RuntimeMinutes, f => f.Random.Number(100, 200))
                .RuleFor(m => m.Description, f => f.Lorem.Paragraph())
                .RuleFor(m => m.PosterFileName, f => f.Image.Image())
                .RuleFor(m => m.Classification, f => f.PickRandom("G", "PG", "MA", "MA15+", "R18+", "X18+"))
                .Generate();
            });

            int movieSessionId = 2;
            Define<MovieSession>(() =>
            {
                Random random = new Random();
                int days = random.Next(1, 4);
                int hour = random.Next(1, 4);
                DateTime schedule = DateTime.Today.AddDays(days);
                schedule.AddHours(hour * 4.5);
                string scheduleStr = schedule.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

                return new Faker<MovieSession>()
                .RuleFor(ms => ms.Id, f => movieSessionId++)
                .RuleFor(ms => ms.TicketPrice, f => 20.00)
                .RuleFor(ms => ms.ScheduledAt, f => scheduleStr)
                .RuleFor(ms => ms.TheaterId, f => f.Random.Int(2, 6))
                .Generate();
            });
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
