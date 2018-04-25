using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace WMoSS.Tests.Unit.Pages.Cart
{
    public class AddToCartTests : TestCase
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
            // ARRANGE
            CartIndexModel.CartItem = new Mock<Entities.CartItem>().Object;

            // ACT
            var result = CartIndexModel.OnPostAddToCart();

            // ASSERT
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Cart/Index", redirect.PageName);
        }

        [Fact]
        public void Test_WhenAddToCartHasModelStateErrors_ItRedirectsBasedOnReturnUrl()
        {
            // ARRANGE
            CartIndexModel.CartItem = new Mock<Entities.CartItem>().Object;
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
