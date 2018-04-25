using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using WMoSS.Extensions;

namespace WMoSS.Tests.Unit.Pages.Cart
{
    public class CartTests : IDisposable
    {
        private Entities.Cart cart;
        private Mock<ISession> mockSession;

        public CartTests()
        {
            cart = new Entities.Cart();
            mockSession = new Mock<ISession>();
        }

        [Fact]
        public void Test_Add()
        {
            // ARRANGE
            var cartItem = new Entities.CartItem
            {
                MovieSessionId = 1,
                TicketQuantity = 1
            };

            // ACT
            cart.Add(cartItem);

            // ASSERT
            Assert.Equal(1, cart.CartItems.Count);
        }


        [Fact]
        public void Test_Modify()
        {
            // ARRANGE
            cart = new Entities.Cart
            {
                CartItems = new Entities.CartItem[]
                {
                    new Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1
                    }
                }
            };

            // ACT
            var isModified = cart.Modify(1, new Entities.CartItem
            {
                MovieSessionId = 1,
                TicketQuantity = 2
            });

            // ASSERT
            Assert.True(isModified);
            Assert.Equal(2, cart.CartItems.First().TicketQuantity);
        }

        [Fact]
        public void Test_Modify_ReturnsFalse_WhenCartIsNotModified()
        {
            // ARRANGE
            cart = new Entities.Cart
            {
                CartItems = new Entities.CartItem[]
                {
                    new Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1
                    }
                }
            };

            // ACT
            var isModified = cart.Modify(2, new Entities.CartItem
            {
                MovieSessionId = 2,
                TicketQuantity = 2
            });

            // ASSERT
            Assert.False(isModified);
            Assert.Equal(1, cart.CartItems.First().TicketQuantity);
        }


        [Fact]
        public void Test_Remove()
        {
            // ARRANGE
            cart = new Entities.Cart
            {
                CartItems = (new Entities.CartItem[]
                {
                    new Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1
                    }
                }).ToList()
            };

            // ACT
            var isRemoved = cart.Remove(1);

            // ASSERT
            Assert.True(isRemoved);
            Assert.Equal(0, cart.CartItems.Count);
        }


        [Fact]
        public void Test_Remove_ReturnsFalse_When_MovieSessionNotFound()
        {
            // ARRANGE
            cart = new Entities.Cart
            {
                CartItems = new Entities.CartItem[]
                {
                    new Entities.CartItem
                    {
                        MovieSessionId = 1,
                        TicketQuantity = 1
                    }
                }.ToList()
            };

            // ACT
            var isRemoved = cart.Remove(2);

            // ASSERT
            Assert.False(isRemoved);
            Assert.Equal(1, cart.CartItems.Count);
        }

        [Fact]
        public void Test_SaveTo_Calls_Session_Get()
        {
            cart.SaveTo(mockSession.Object);
            mockSession.Verify(s => s.Set("cart", It.IsAny<byte[]>()), Times.Once());
        }


        public void Dispose()
        {

        }
    }
}
