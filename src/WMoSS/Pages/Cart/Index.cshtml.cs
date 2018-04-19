using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.AspNetCore.Http;
using WMoSS.Extensions;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _dbContext;

        public IndexModel(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Entities.Cart Cart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Cart = Entities.Cart.GetFrom(HttpContext.Session);

            var movieSessionIds = Cart.CartItems.Select(ci => ci.MovieSessionId);

            if (Cart.CartItems.Count() == 0)
            {
                return Page();
            }

            var movieSessions = await _dbContext.MovieSessions
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theater)
                .Where(ms => movieSessionIds.Contains(ms.Id))
                .AsNoTracking()
                .ToListAsync();
            
            foreach (var cartItem in Cart.CartItems)
            {
                cartItem.MovieSession = movieSessions.FirstOrDefault(ms => ms.Id == cartItem.MovieSessionId);
            }
            
            return Page();
            
        }

        // Add to cart functionality actually handled from '../Movie/Details.cshtml.cs:OnPostAddToCart()
        [BindProperty]
        public CartItem CartItem { get; set; }

        [BindProperty]
        public string ReturnUrl { get; set; }

        public IActionResult OnPostAddToCart()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToLocal(ReturnUrl);
            }

            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            cart.Add(CartItem);
            cart.SaveTo(HttpContext.Session);

            TempData["Success"] = "Successfully added movie session to cart";
            return RedirectToPage("/Cart/Index");
        }

        
        public IActionResult OnPostModifyCart(CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var cart = Entities.Cart.GetFrom(HttpContext.Session);

            var isModified = cart.Modify(CartItem.MovieSessionId.Value, CartItem);
            if (!isModified)
            {
                TempData["Danger"] = "Failed to update cart.";
                return Page();
            }

            cart.SaveTo(HttpContext.Session);
            TempData["Success"] = "Cart successfully updated.";
            return RedirectToPage("/Cart/Index");
            


        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToPage("/");
            }
        }
    }
}