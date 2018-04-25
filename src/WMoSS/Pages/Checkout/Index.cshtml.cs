using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;
using WMoSS.Entities;

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

        [BindProperty]
        public string CVV { get; set; }
        

        public Entities.Cart Cart;

        public async Task<IActionResult> OnGetAsync()
        {
            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            
            // No items in the cart
            if (cart.CartItems.Count() == 0)
            {
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
                TempData["Danger"] = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
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

            // Generate tickets
            var tickets = new List<Ticket>();
            foreach(var cartItem in Cart.CartItems)
            {
                for (int i = 0; i < cartItem.TicketQuantity.Value; i++)
                {
                    var ticket = new Ticket
                    {
                        MovieSessionId = cartItem.MovieSessionId.Value,
                        SeatNumber = "A1" // TODO: Change this
                    };
                    tickets.Add(ticket);
                }
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
    }

}
