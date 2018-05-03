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
        public string ReturnUrl { get; set; }


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

            MovieSessionId = id;
            ReturnUrl = Url.Page("/Seats/Select", new { id = MovieSessionId });

            return Page();
        }

        public IActionResult OnPost(SelectSeatRequestModel request)
        {
            if (ModelState.IsValid == false)
            {
                TempData["Danger"] = ModelState.Values.Select(v => v.Errors).FirstOrDefault();
                return RedirectToLocal(ReturnUrl);
            }

            var cart = Entities.Cart.GetFrom(HttpContext.Session);
            CartItem = cart.CartItems.FirstOrDefault(ci => ci.MovieSessionId == request.MovieSessionId);
            if (CartItem == null)
            {
                return NotFound();
            }

            CartItem.Seats = request.Seats;
            cart.Modify(request.MovieSessionId, CartItem);
            cart.SaveTo(HttpContext.Session);

            TempData["Success"] = "Seats successfully updated";

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

    public class SelectSeatRequestModel
    {
        [Required]
        public string[] Seats { get; set; }

        [Required]
        public int MovieSessionId { get; set; }

        [Required]
        public string ReturnUrl { get; set; }
    }
    


}