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
                TempData["Danger"] = ModelState.Values.Select(msVal => msVal.Errors).FirstOrDefault();
                return RedirectToLocal(ReturnUrl);
            }

            // if session doesn't exists, return 404
            var movieSession = _dbContext.MovieSessions
                .Include(m => m.Theater)
                .AsNoTracking()
                .FirstOrDefault(ms => ms.Id == CartItem.MovieSessionId);
            if (movieSession == null)
            {
                TempData["Danger"] = "Movie session cannot be found";
                return NotFound();
            }

            // if session will start within next 60 minutes, reject request to add to cart
            if (DateTime.Now.AddMinutes(60) > movieSession.ScheduledAt)
            {
                TempData["Danger"] = string.Format("The{0: hh:mm tt ddd dd MMMM} session " +
                    "cannot be added to cart anymore because " +
                    "it will start in the next 60 minutes. " +
                    "Please pick another session.",
                    movieSession.ScheduledAt);
                return RedirectToLocal(ReturnUrl);
            }


            // if number of tickets to be added to cart is less than the available seats remaining, 
            // reject request to add to cart
            var seatCapacity = movieSession.Theater.Capacity;
            var numOfSeatsBooked = _dbContext.Tickets
                .AsNoTracking()
                .Count(t => t.MovieSessionId == movieSession.Id);
            var numOfAvailableSeatsLeft = seatCapacity - numOfSeatsBooked;

            if (numOfAvailableSeatsLeft < CartItem.TicketQuantity)
            {
                TempData["Danger"] = $"Cannot add {CartItem.TicketQuantity} " +
                    $"tickets to cart because there are {numOfAvailableSeatsLeft} " +
                    $"seats remaining. Please reduce number of tickets accordingly";
                return RedirectToLocal(ReturnUrl);
            }



            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            cart.Add(CartItem);
            cart.SaveTo(HttpContext.Session);

            TempData["Success"] = "Successfully added movie session to cart";
            return RedirectToPage("/Cart/Index");
        }

        
        public IActionResult OnPostModifyCart()
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

        public IActionResult OnPostRemoveFromCart(CartItem cartItem)
        {
            if (!ModelState.IsValid)
            {
                var error = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
                TempData["Danger"] = error;
                return Page();
            }

            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            var isRemoved = cart.Remove(cartItem.MovieSessionId.HasValue ? cartItem.MovieSessionId.Value : 0);
            if (isRemoved == false)
            {
                var error = "Failed to remove movie session from the cart";
                TempData["Danger"] = error;
                return Page();
            }

            cart.SaveTo(HttpContext.Session);
            TempData["Success"] = "Successfully removed item from the cart";
            return RedirectToPage("/Cart/Index");
        }

        public IActionResult OnPostClearCart()
        {
            HttpContext.Session.Set<Entities.Cart>("cart", null);
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