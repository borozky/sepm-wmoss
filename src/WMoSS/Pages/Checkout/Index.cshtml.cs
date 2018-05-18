using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;
using WMoSS.Entities;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace WMoSS.Pages.Checkout
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Entities.Order Order { get; set; }

        public Entities.Cart Cart;

        public async Task<IActionResult> OnGetAsync()
        {
            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            
            // No items in the cart
            if (cart.CartItems.Count() == 0)
            {
                return RedirectToPage("/Cart/Index");
            }

            // If there are remaining seats to be filled in
            if (cart.RemainingSeats != 0)
            {
                TempData["Danger"] = $"There are still {cart.RemainingSeats} seat{(cart.RemainingSeats != 1 ? "s" : "")} remaining to be filled in";
                return RedirectToPage("/Cart/Index");
            }

            //// If a seat is missing, redirect back to cart page
            //var hasSelectedAllSeats = cart.CartItems.All(
            //    ci => ci.Seats != null && 
            //    ci.Seats.Count(s => !string.IsNullOrWhiteSpace(s)) == ci.TicketQuantity
            //);
            //if (hasSelectedAllSeats == false)
            //{
            //    TempData["Danger"] = "You can only proceed to cart after you have selected all your seats";
            //    return RedirectToPage("/Cart/Index");
            //}

            // Get all movie sessions
            var movieSessionIds = cart.CartItems.Select(ci => ci.MovieSessionId);
            var movieSessions = await _context.MovieSessions
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theater)
                .Where(ms => movieSessionIds.Contains(ms.Id))
                .AsNoTracking()
                .ToListAsync();
            foreach (var cartItem in cart.CartItems)
            {
                cartItem.MovieSession = movieSessions.FirstOrDefault(ms => ms.Id == cartItem.MovieSessionId);
            }

            Cart = cart;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Cart = Entities.Cart.GetFrom(HttpContext.Session);

            // Cart must not be empty
            if (Cart.CartItems.Count == 0)
            {
                return NotFound();
            }

            // Validate order
            if (!ModelState.IsValid)
            {
                // Get movie sessions based on cart's movie session ids
                var sessionIds = Cart.CartItems.Select(ci => ci.MovieSessionId);
                var sessions = await _context.MovieSessions
                    .Include(ms => ms.Movie)
                    .Include(ms => ms.Theater)
                    .Where(ms => sessionIds.Contains(ms.Id))
                    .AsNoTracking()
                    .ToListAsync();
                foreach (var cartItem in Cart.CartItems)
                {
                    cartItem.MovieSession = sessions.FirstOrDefault(ms => ms.Id == cartItem.MovieSessionId);
                }

                var modelErrors = GetModelErrors(ModelState);
                TempData["Danger"] = modelErrors.FirstOrDefault();
                return Page();
            }

            // Get movie sessions based on cart's movie session ids
            var movieSessionIds = Cart.CartItems.Select(ci => ci.MovieSessionId);
            var movieSessions = await _context.MovieSessions
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theater)
                .Where(ms => movieSessionIds.Contains(ms.Id))
                .AsNoTracking()
                .ToListAsync();


            var checkoutErrorList = new CheckoutErrorList();

            foreach (var movieSession in movieSessions)
            {
                var errorMessage = "";
                if (DateTime.Now.AddMinutes(60) > movieSession.ScheduledAt)
                {
                    errorMessage = string.Format("Cannot book into {0}{1: hh:mm tt ddd dd MMMM} " +
                        "movie session at {2} because it will start within next 60 minutes",
                        movieSession.Movie.Title, movieSession.ScheduledAt, movieSession.Theater.Name);
                    checkoutErrorList.AddError(movieSession, errorMessage, true);
                }

                var numTickets = await _context.Tickets.CountAsync(t => t.MovieSessionId == movieSession.Id);
                var remainingSeats = movieSession.Theater.Capacity - numTickets;
                var selectedQuantity = Cart.CartItems.First(ci => ci.MovieSessionId == movieSession.Id).TicketQuantity;
                if (remainingSeats < selectedQuantity)
                {
                    errorMessage = string.Format("Cannot book into {0}{1: hh:mm tt ddd dd MMMM} movie session " +
                        "because there are {2} seats remaining in this session which you have book for {3} seats", 
                        movieSession.Movie.Title, movieSession.ScheduledAt, remainingSeats, selectedQuantity);
                    checkoutErrorList.AddError(movieSession, errorMessage, remainingSeats == 0);
                }
            }

            if (checkoutErrorList.CheckoutErrors.Count > 0)
            {
                foreach (var checkoutError in checkoutErrorList.CheckoutErrors)
                {
                    if (checkoutError.ShouldDelete)
                    {
                        Cart.Remove(checkoutError.MovieSession.Id);
                    }
                }

                Cart.SaveTo(HttpContext.Session);

                var errors = new List<string>();
                checkoutErrorList.CheckoutErrors.ForEach(ce => errors.AddRange(ce.Errors));
                TempData["Danger"] = JsonConvert.SerializeObject(errors);

                return RedirectToPage("/Cart/Index");
            }
            

            // Generate tickets
            var tickets = new List<Ticket>();
            foreach(var cartItem in Cart.CartItems)
            {
                tickets.AddRange(cartItem.Seats.Select(seat => new Ticket
                {
                    MovieSessionId = cartItem.MovieSessionId.Value,
                    SeatNumber = seat
                }));
            }

            Order.TotalPrice = Cart.TotalPrice;
            Order.Tickets = tickets;

            _context.Orders.Add(Order);
            await _context.SaveChangesAsync();

            // clear the cart
            HttpContext.Session.Remove("cart");

            TempData["Success"] = "Successfully booked";
            return RedirectToPage("/Order/Details", new { id = Order.Id });
        }

        private string[] GetModelErrors(ModelStateDictionary _modelState)
        {
            var modelErrors = new List<string>();
            foreach (var modelState in _modelState.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    modelErrors.Add(modelError.ErrorMessage);
                }
            }
            return modelErrors.ToArray();
        }
    }

    public class CheckoutError
    {
        public MovieSession MovieSession { get; set; }
        public List<string> Errors { get; set; }
        public bool ShouldDelete { get; set; } = false;
    }

    public class CheckoutErrorList
    {
        public List<CheckoutError> CheckoutErrors { get; private set; } = new List<CheckoutError>();
        public void AddError(MovieSession movieSession, string errorMessage, bool shouldDelete = false)
        {
            var checkoutErrors = CheckoutErrors
                .Where(ce => ce.MovieSession.Id == movieSession.Id);

            if (checkoutErrors.Count() == 0)
            {
                CheckoutErrors.Add(new CheckoutError
                {
                    MovieSession = movieSession,
                    Errors = new string[] { errorMessage }.ToList(),
                    ShouldDelete = shouldDelete
                });
            }
            else
            {
                foreach(var ce in checkoutErrors)
                {
                    ce.Errors.Add(errorMessage);
                    ce.ShouldDelete = ce.ShouldDelete == true ? true : shouldDelete;
                }
            }    
        }
    }

}
