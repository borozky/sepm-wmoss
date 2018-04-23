using Bogus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WMoSS.Data;
using WMoSS.Entities;
using Xunit;

namespace WMoSS.Tests.Integration
{
    public class MovieDetailsTests : TestFixture<Startup>
    {
        ApplicationDbContext db;
        HttpClient client;

        public MovieDetailsTests(): base()
        {
            client = Client;
            db = Server.Host.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            DbInitializer.Initialize(db);
        }

        [Fact]
        public async Task Test_MovieDetailsPage_WorksWhenMovieExists()
        {
            var ids = await db.Movies.Select(m => m.Id).ToArrayAsync();
            Assert.NotEmpty(ids);

            // var selectedId = ids[new Random().Next(ids.Length)];
            var selectedId = ids[0];

            var response = await client.GetAsync($"/Movies/Details/{selectedId}");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Test_MovieDetailsPage_ShouldNotWork_WhenMovieDoesntExists()
        {
            var latestId = await db.Movies.MaxAsync(m => m.Id);

            var selectedIds = latestId + 1;

            var response = await client.GetAsync($"/Movies/Details/{selectedIds}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Test_MovieSessions_Shows_Up_In_Movie_Details_Page()
        {
            var movie = await factory.CreateAsync<Movie>();
            var movieSessions = await factory.CreateAsync<MovieSession>(3, ms =>
            {
                ms.MovieId = movie.Id;
                ms.ScheduledAt = DateTime.Today.AddDays(new Random().NextDouble() * 7.0);
            });

            var response = await client.GetAsync($"/Movies/Details/{movie.Id}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var schedules = movieSessions.Select(ms => string.Format("{0:dd MMM yyyy hh:mm tt}", ms.ScheduledAt));
            Assert.All(schedules, sc => Assert.True(content.Contains(sc)) );

        }
    }
}
