using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMoSS.ViewComponents
{
    [ViewComponent]
    public class CartButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            var numCartItems = cart.CartItems.Count;
            return View(numCartItems);
        }
    }
}
