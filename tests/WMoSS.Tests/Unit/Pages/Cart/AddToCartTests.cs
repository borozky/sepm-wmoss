using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
