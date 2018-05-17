using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Entities;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        public int MovieSessionId { get; set; } = 0;
        public MovieSession MovieSession { get; set; }
        public string ReturnUrl { get; set; }


        public async Task<IActionResult> OnGetAsync(int id, string returnUrl = null)
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

            MovieSessionId = id;

            MovieSession = await db.MovieSessions
                .Include(ms => ms.Movie)
                .Include(ms => ms.Theater)
                .AsNoTracking()
                .FirstOrDefaultAsync(ms => ms.Id == id);

            ReturnUrl = returnUrl ?? Url.Page("/Seats/Select", new { id = MovieSessionId });

            return Page();
        }

        public IActionResult OnPost(SelectSeatRequestModel request)
        {
            if (ModelState.IsValid == false)
            {
                var modelErrors = GetModelErrors(ModelState);
                TempData["Danger"] = modelErrors.First();
                return RedirectToLocal(ReturnUrl);
            }

            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            CartItem = cart.CartItems.FirstOrDefault(ci => ci.MovieSessionId == request.MovieSessionId);
            if (CartItem == null)
            {
                return NotFound();
            }
            
            // pick as many seats based on number of tickets
            // excess seats are rejected
            var seats = request.Seats.Select(s => s).ToArray();
            seats = seats.Where((seat, i) => i < CartItem.TicketQuantity).ToArray();
            CartItem.Seats = seats;
            cart.Modify(request.MovieSessionId, CartItem);
            cart.SaveTo(HttpContext.Session);
            
            if (CartItem.TicketQuantity < request.Seats.Count())
            {
                TempData["Danger"] = "You have picked too many seats";
            }
            else
            {
                TempData["Success"] = "Seats successfully updated";
            }

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

    public class SelectSeatRequestModel
    {
        [Required]
        public string[] Seats { get; set; } = new string[0];

        [Required]
        public int MovieSessionId { get; set; }

        [Required]
        public string ReturnUrl { get; set; }
    }
    


}