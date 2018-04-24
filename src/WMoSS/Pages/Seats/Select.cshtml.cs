using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Entities;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Pages.Seats
{
    public class SelectModel : PageModel
    {
        private readonly ApplicationDbContext db;

        public CartItem CartItem { get; set; }
        public string[] UnavailableSeats { get; set; }

        public SelectModel(ApplicationDbContext db)
        {
            this.db = db;
        }


        public async Task<IActionResult> OnGetAsync(int id)
        {
            // id is the movie session id. This id must exists in the cart

            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            CartItem = cart.CartItems.FirstOrDefault(ci => ci.MovieSessionId == id);

            if (CartItem == null)
            {
                return NotFound();
            }

            // Get all seat numbers from tickets queried by movie sessions id
            UnavailableSeats = await db.Tickets
                .AsNoTracking()
                .Where(t => t.MovieSessionId == id)
                .Select(t => t.SeatNumber)
                .ToArrayAsync();

            return Page();
        }
    }
    


}