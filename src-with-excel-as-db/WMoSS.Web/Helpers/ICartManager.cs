using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMoSS.Entities;

namespace WMoSS.Web.Helpers
{
    public interface ICartManager
    {
        void AddToCart(CartItem cartItem);
        void AddMultipleItemsToCart(IEnumerable<CartItem> cartItems);
        void AddToCart(int movieSessionId, int numTickets);
        bool RemoveFromCart(int movieSessionId);
        bool ModifyNumTickets(int movieSessionId, int numTickets);
        Cart GetCart();
    }
}
