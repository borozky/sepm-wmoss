using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Extensions;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WMoSS.Tests.Unit.Cart
{
    public class ModifyCartTests : TestCase
    {
        private Pages.Cart.IndexModel CartIndexModel;

        public ModifyCartTests()
        {
            CartIndexModel = new Pages.Cart.IndexModel(db)
            {
                PageContext = pageContext,
                TempData = tempData,
                Url = new UrlHelper(pageContext)
            };
        }

        [Fact]
        public void Test_WhenModifyCart_IsSuccessful_RedirectsToCartPage()
        {
            var cartItem = new Entities.CartItem
            {
                MovieSessionId = 1,
                TicketQuantity = 1
            };

            // ARRANGE
            // Extension methods cannot be mocked 
            // so serialize the cart into string they convert to byte array
            // Also do not mock the cart and cart items
            var cart = new Entities.Cart
            { 
                CartItems = new Entities.CartItem[] { cartItem }
            };
            var jsonCart = JsonConvert.SerializeObject(cart);
            byte[] jsonCartBytes = System.Text.Encoding.UTF8.GetBytes(jsonCart);
            mockSession.Setup(ms => ms.TryGetValue("cart", out jsonCartBytes)).Returns(true).Verifiable();


            CartIndexModel.CartItem = cart.CartItems.FirstOrDefault();

            // ACT
            var result = CartIndexModel.OnPostModifyCart();

            // ASSERT
            var redirect = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("/Cart/Index", redirect.PageName);
        }

        [Fact]
        public void Test_WhenModifyCartFailsToModifyCart_ItRendersTheCartPageAgain()
        {
            // ARRANGE
            var cartItem = new Entities.CartItem
            {
                MovieSessionId = 1,
                TicketQuantity = 1
            };
            var cartItem2 = new Entities.CartItem
            {
                MovieSessionId = 2,
                TicketQuantity = 1
            };
            var cart = new Entities.Cart
            {
                CartItems = new Entities.CartItem[] { cartItem }
            };

            var jsonCart = JsonConvert.SerializeObject(cart);
            byte[] jsonCartBytes = System.Text.Encoding.UTF8.GetBytes(jsonCart);
            mockSession.Setup(ms => ms.TryGetValue("cart", out jsonCartBytes)).Returns(true).Verifiable();

            // assume cart item submitted has different movie session id
            // this should pass the ModelState.IsValid but fails on further validation
            CartIndexModel.CartItem = cartItem2;

            // ACT
            var result = CartIndexModel.OnPostModifyCart();

            // ASSERT
            Assert.IsType<PageResult>(result);
        }


        [Fact]
        public void Test_WhenModifyCart_ModelStateIsNotValid_RendersTheCartPageAgain()
        {
            // ARRANGE
            var cartItem = new Entities.CartItem
            {
                MovieSessionId = 1,
                TicketQuantity = 0
            };

            CartIndexModel.ModelState.AddModelError(nameof(cartItem), "Ticket quantity cannot be 0");

            // ACT
            var result = CartIndexModel.OnPostModifyCart();

            // ASSERT
            Assert.IsType<PageResult>(result);
        }
    }
}
