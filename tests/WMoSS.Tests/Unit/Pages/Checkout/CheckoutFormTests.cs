using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMoSS.Data;
using Xunit;

namespace WMoSS.Tests.Unit.Pages.Checkout
{
    public class CheckoutFormTests : PageModelTestFixture<WMoSS.Pages.Checkout.IndexModel>
    {
        private WMoSS.Pages.Checkout.IndexModel checkoutIndexModel;

        public CheckoutFormTests()
        {
            checkoutIndexModel = new WMoSS.Pages.Checkout.IndexModel(db)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(pageContext)
            };
        }

        [Fact]
        public async Task Test_OnPost_WithValidModelState_RedirectsToOrderSummary()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                DbInitializer.Initialize(db);

                checkoutIndexModel = new WMoSS.Pages.Checkout.IndexModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };

                // ARRANGE
                var cart = new WMoSS.Entities.Cart
                {
                    CartItems = new WMoSS.Entities.CartItem[]
                    {
                    new WMoSS.Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1,
                    }
                    }.ToList()
                };
                
                var order = new WMoSS.Entities.Order
                {
                    FullName = "Sample user",
                    MailingAddress = "123 Club house avenue",
                    EmailAddress = "someone123@email.com",
                    CardNumber = "4111111111111111",
                    Expiry = "10/20",
                };

                checkoutIndexModel.Order = order;
                checkoutIndexModel.CVV = "123";

                checkoutIndexModel.Cart = cart;
                var jsonCart = JsonConvert.SerializeObject(cart);
                byte[] jsonCartBytes = System.Text.Encoding.UTF8.GetBytes(jsonCart);
                mockSession.Setup(ms => ms.TryGetValue("cart", out jsonCartBytes)).Returns(true).Verifiable();

                // ACT
                var result = await checkoutIndexModel.OnPostAsync();

                // ASSERT
                var redirect = Assert.IsType<RedirectToPageResult>(result);
                Assert.Equal("/Order/Details", redirect.PageName);
            }
        }

        [Fact]
        public async Task Test_OnPost_WithINVALIDModelState_RendersTheFormAgain()
        {
            // ARRANGE
            var cart = new WMoSS.Entities.Cart
            {
                CartItems = new WMoSS.Entities.CartItem[]
                {
                    new WMoSS.Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1,
                    }
                }.ToList()
            };

            var order = new WMoSS.Entities.Order
            {
                FullName = "", // INVALID
                MailingAddress = "123 Club house avenue",
                EmailAddress = "someone123@email.com",
                CardNumber = "4111111111111111",
                Expiry = "10/20",
            };

            checkoutIndexModel.Order = order;
            checkoutIndexModel.CVV = "123";

            checkoutIndexModel.Cart = cart;
            var jsonCart = JsonConvert.SerializeObject(cart);
            byte[] jsonCartBytes = System.Text.Encoding.UTF8.GetBytes(jsonCart);
            mockSession.Setup(ms => ms.TryGetValue("cart", out jsonCartBytes)).Returns(true).Verifiable();
            
            // check for validation errors
            SimulateValidation(order, checkoutIndexModel);

            // ACT
            var result = await checkoutIndexModel.OnPostAsync();

            // ASSERT
            Assert.IsType<PageResult>(result);
            Assert.NotNull(tempData["Danger"]);

        }


        [Fact]
        public async Task Test_EmptyCart_Returns_404Page()
        {
            // ARRANGE

            // cart is not given

            // ACT
            var result = await checkoutIndexModel.OnPostAsync();

            // ASSERT
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Test_OrderTicketsArePopulated_AfterSuccessfulCheckout()
        {
            using (var db = new ApplicationDbContext(Utilities.TestingDbContextOptions<ApplicationDbContext>()))
            {
                DbInitializer.Initialize(db);

                checkoutIndexModel = new WMoSS.Pages.Checkout.IndexModel(db)
                {
                    PageContext = pageContext,
                    TempData = tempData,
                    Url = new UrlHelper(pageContext)
                };

                // ARRANGE
                var cart = new WMoSS.Entities.Cart
                {
                    CartItems = new WMoSS.Entities.CartItem[]
                    {
                    new WMoSS.Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1,
                    }
                    }.ToList()
                };

                var order = new WMoSS.Entities.Order
                {
                    FullName = "Sample user",
                    MailingAddress = "123 Club house avenue",
                    EmailAddress = "someone123@email.com",
                    CardNumber = "4111111111111111",
                    Expiry = "10/20",
                };

                checkoutIndexModel.Order = order;
                checkoutIndexModel.CVV = "123";

                checkoutIndexModel.Cart = cart;
                var jsonCart = JsonConvert.SerializeObject(cart);
                byte[] jsonCartBytes = System.Text.Encoding.UTF8.GetBytes(jsonCart);
                mockSession.Setup(ms => ms.TryGetValue("cart", out jsonCartBytes)).Returns(true).Verifiable();

                // ACT
                var result = await checkoutIndexModel.OnPostAsync();

                // ASSERT
                Assert.IsType<RedirectToPageResult>(result);
                var expectedNumTickets = cart.CartItems.Sum(ci => ci.TicketQuantity);
                Assert.Equal(expectedNumTickets, checkoutIndexModel.Order.Tickets.Count());
            }
        } 
        


        


    }
}
