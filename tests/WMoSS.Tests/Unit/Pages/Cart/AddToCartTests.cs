using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMoSS.Data;
using Xunit;

namespace WMoSS.Tests.Unit.Pages.Cart
{
    public class AddToCartTests : PageModelTestFixture<WMoSS.Pages.Cart.IndexModel>
    {
        private WMoSS.Pages.Cart.IndexModel CartIndexModel;

        public AddToCartTests()
        {
            CartIndexModel = new WMoSS.Pages.Cart.IndexModel(db)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(pageContext)
            };
        }

        [Fact]
        public void Test_WhenAddToCartIsSuccessful_RedirectsToCartPage()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                DbInitializer.Initialize(db);
                CartIndexModel = new WMoSS.Pages.Cart.IndexModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };

                // ARRANGE
                CartIndexModel.CartItem = new WMoSS.Entities.CartItem
                {
                    MovieSessionId = 1,
                    TicketQuantity = 1
                };

                // ACT
                var result = CartIndexModel.OnPostAddToCart();

                // ASSERT
                var redirect = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/Cart/Index", redirect.PageName);
            }
        }

        [Fact]
        public void Test_WhenAddToCartHasModelStateErrors_ItRedirectsBasedOnReturnUrl()
        {
            // ARRANGE
            CartIndexModel.CartItem = new Mock<WMoSS.Entities.CartItem>().Object;
            CartIndexModel.ReturnUrl = "/Index";
            CartIndexModel.ModelState.AddModelError("", "Invalid");

            // ACT
            var result = CartIndexModel.OnPostAddToCart();

            // ASSERT
            var redirect = Assert.IsType<RedirectResult>(result);
            Assert.Equal(CartIndexModel.ReturnUrl, redirect.Url);
        }

        [Fact]
        public void TestAddToCart_WhenSessionDoesntExists_Returns404()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                DbInitializer.Initialize(db);

                CartIndexModel = new WMoSS.Pages.Cart.IndexModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };

                // ARRANGE
                CartIndexModel.CartItem = new WMoSS.Entities.CartItem
                {
                    MovieSessionId = 1000,
                    TicketQuantity = 1
                };

                // ACT
                var result = CartIndexModel.OnPostAddToCart();

                // ASSERT
                Assert.IsType<NotFoundResult>(result);

            }
        }

        [Fact]
        public async Task TestAddToCart_WhenSessionIsAboutToStart_RejectAndRedirectBack()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                var movieSession = new WMoSS.Entities.MovieSession
                {
                    ScheduledAt = DateTime.Now.AddMinutes(59),
                    TheaterId = 1,
                    MovieId = 1,
                    Theater = new WMoSS.Entities.Theater
                    {
                        Capacity = 50,
                        Address = "123 Lygon Street",
                        Name = "Theater #1"
                    },
                    TicketPrice = 20,
                    ScheduledById = Guid.NewGuid().ToString()
                };
                db.MovieSessions.Add(movieSession);
                await db.SaveChangesAsync();

                CartIndexModel = new WMoSS.Pages.Cart.IndexModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };
            
                // ARRANGE
                CartIndexModel.CartItem = new WMoSS.Entities.CartItem
                {
                    MovieSessionId = 1,
                    TicketQuantity = 1
                };

                CartIndexModel.ReturnUrl = $"/Movies/Details/{movieSession.MovieId}";

                // ACT
                var result = CartIndexModel.OnPostAddToCart();

                // ASSERT
                var redirect = Assert.IsType<RedirectResult>(result);
                Assert.Equal(CartIndexModel.ReturnUrl, redirect.Url);
            }
        }

        [Fact]
        public async Task TestAddToCart_WhenSessionCannotAccomodateEnoughTickets_RejectAndRedirectBack()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                var movieSession = new WMoSS.Entities.MovieSession
                {
                    ScheduledAt = DateTime.Now.AddDays(5),
                    TheaterId = 1,
                    MovieId = 1,
                    Theater = new WMoSS.Entities.Theater
                    {
                        Capacity = 50,
                        Address = "123 Lygon Street",
                        Name = "Theater #1"
                    },
                    TicketPrice = 20,
                    ScheduledById = Guid.NewGuid().ToString()
                };

                db.MovieSessions.Add(movieSession);

                await db.SaveChangesAsync();

                CartIndexModel = new WMoSS.Pages.Cart.IndexModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };
                
                var tickets = new List<WMoSS.Entities.Ticket>();
                for (int i = 0; i < 45; i++)
                {
                    var ticket = new WMoSS.Entities.Ticket
                    {
                        MovieSessionId = movieSession.Id,
                        OrderId = 1,
                        SeatNumber = "A1"
                    };
                    tickets.Add(ticket);
                }

                db.Tickets.AddRange(tickets);
                await db.SaveChangesAsync();

                // ARRANGE
                CartIndexModel.CartItem = new WMoSS.Entities.CartItem
                {
                    MovieSessionId = 1,
                    TicketQuantity = 6
                };

                CartIndexModel.ReturnUrl = $"/Movies/Details/{movieSession.MovieId}";

                // ACT
                var result = CartIndexModel.OnPostAddToCart();

                // ASSERT
                var redirect = Assert.IsType<RedirectResult>(result);
                Assert.Equal(CartIndexModel.ReturnUrl, redirect.Url);
            }
        }
    }
}
