using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using WMoSS.Extensions;

namespace WMoSS.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _dbcontext;

        public DetailsModel(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        
        public Movie Movie { get; private set; }
        public CartItem cartItem { get; set; }
        public IEnumerable<MovieSession> MovieSessions { get; private set; }
        public List<SelectListItem> MovieSessionsOptions { get; private set; }

        [TempData]
        public string ReturnPath { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Movie = await _dbcontext.Movies
                .Include(m => m.MovieSessions)
                    .ThenInclude(ms => ms.Theater)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (Movie == null)
            {
                return NotFound();
            }

            MovieSessions = Movie.MovieSessions;
            MovieSessionsOptions = MovieSessions.Select(ms => new SelectListItem
            {
                Value = ms.Id.ToString(),
                Text = String.Format("{0:ddd dd MMM yyyy hh:mm tt} at {1}", ms.ScheduledAt, ms.Theater.Name)
            }).ToList();

            MovieSessionsOptions.Insert(0, new SelectListItem
            {
                Value = null,
                Text = "Select movie sessions",
                Disabled = true,
                Selected = true
            });

            ReturnPath = Request.Path;

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

            var cart = HttpContext.Session.Get<Entities.Cart>("cart");
            if (cart == null)
            {
                cart = new Entities.Cart
                {
                    CartItems = new List<CartItem>()
                };

            }
            cart.Add(CartItem);
            cart.SaveTo(HttpContext.Session);

            TempData["Success"] = "Successfully added movie session to cart";
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