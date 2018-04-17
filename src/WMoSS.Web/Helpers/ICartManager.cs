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
        bool AddToCart(CartItem cartItem);
        bool AddMultipleItemsToCart(IEnumerable<CartItem> cartItems);
        bool AddToCart(int movieSessionId, int numTickets);
        bool RemoveFromCart(int movieSessionId);
        bool ModifyNumTickets(int movieSessionId, int numTickets);
        Cart GetCart();
    }
}
