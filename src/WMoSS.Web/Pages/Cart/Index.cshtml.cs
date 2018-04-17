using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Entities;
using WMoSS.Web.Helpers;
using Microsoft.AspNetCore.Http;
using WMoSS.Web.Extensions;

namespace WMoSS.Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

        }
    }

    public class CartManager : ICartManager
    {
        private ISession _session;

        public CartManager(ISession session)
        {
            _session = session;
        }

        public bool AddToCart(CartItem cartItem)
        {
            throw new NotImplementedException();
        }

        public bool AddToCart(int movieSessionId, int numTickets)
        {
            throw new NotImplementedException();
        }

        public bool AddMultipleItemsToCart(IEnumerable<CartItem> cartItems)
        {
            throw new NotImplementedException();
        }

        public Entities.Cart GetCart()
        {
            throw new NotImplementedException();
        }

        public bool ModifyNumTickets(int movieSessionId, int numTickets)
        {
            throw new NotImplementedException();
        }

        public bool RemoveFromCart(int movieSessionId)
        {
            throw new NotImplementedException();
        }
    }
}