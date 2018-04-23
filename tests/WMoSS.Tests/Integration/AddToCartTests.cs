using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WMoSS.Data;
using WMoSS.Entities;
using Xunit;

namespace WMoSS.Tests.Integration
{
    public class AddToCartTests : TestFixture<Startup>
    {
        ApplicationDbContext db;
        HttpClient client;

        public AddToCartTests() : base()
        {
            client = Client;
            db = Server.Host.Services.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            DbInitializer.Initialize(db);
        }

        [Fact]
        public async Task Test_AddToCart_Works()
        {
            var latestMovieSession = await db.MovieSessions.FirstOrDefaultAsync();
            var tickets = 2;

            var data = new Dictionary<string, string>
            {
                [$"{nameof(CartItem)}.{nameof(CartItem.MovieSessionId)}"] = latestMovieSession.Id.ToString(),
                [$"{nameof(CartItem)}.{nameof(CartItem.TicketQuantity)}"] = tickets.ToString()
            };

            var formData = await Utilities.GetRequestContentAsync(client, $"/Movies/Details/{latestMovieSession.MovieId}", data);

            var response = await client.PostAsync("./Cart/?handler=AddToCart", formData);
            var content = await response.Content.ReadAsStringAsync();
            
            Assert.Equal("/Cart", response.Headers.Location.ToString());
        }
    }
}
