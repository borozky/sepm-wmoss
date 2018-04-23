using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WMoSS.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Tests.Integration
{
    public class HomePageTests : TestFixture<Startup>
    {
        ApplicationDbContext db;
        HttpClient client;

        public HomePageTests(): base()
        {
            client = Client;
            db = Server.Host.Services.GetService<ApplicationDbContext>();
            DbInitializer.Initialize(db);
        }

        [Fact]
        public async Task Test_HomePage_Works()
        {
            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Test_HomePage_HasMovies()
        {
            var movieTitles = await db.Movies.Select(m => m.Title).ToArrayAsync();

            var response = await client.GetAsync("/");
            var content = await response.Content.ReadAsStringAsync();

            Assert.All(movieTitles, title =>
            {
                Assert.True(content.ToLower().Contains(title.ToLower()));
            });
        }
    }
}
