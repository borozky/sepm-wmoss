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
using WMoSS.Repositories;
using System.ComponentModel.DataAnnotations;

namespace WMoSS.Web.Pages.Cart
{
    public class IndexModel : PageModel
    {
        private IMovieRepository movieRepository;
        private IMovieSessionRepository movieSessionRepository;
        private ITheaterRepository theaterRepository;
        private ICartManager cartManager;

        public IndexModel(IMovieRepository movieRepository, 
            IMovieSessionRepository movieSessionRepository, 
            ITheaterRepository theaterRepository)
        {
            this.movieRepository = movieRepository;
            this.movieSessionRepository = movieSessionRepository;
            this.theaterRepository = theaterRepository;

            // DO NOT instantiate CartManager here because HttpContext is null at this stage
            // HttpContext.Session is loaded when request started being processed.
        }

        Entities.Cart Cart;

        public void OnGet()
        {
            SetupCartManager();
            Cart = cartManager.GetCart();
        }

        private void SetupCartManager()
        {
            cartManager = new CartManager(HttpContext.Session,
                movieRepository,
                movieSessionRepository,
                theaterRepository);
        }


        [BindProperty]
        public CartItemRequest AddToCartRequest { get; set; }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid == false)
            {
                return Page();
            }

            SetupCartManager();
            int movieSessionId = AddToCartRequest.MovieSessionId.Value;
            int ticketQty = AddToCartRequest.TicketQty.Value;

            cartManager.AddToCart(movieSessionId, ticketQty);

            return RedirectToPage();
        }
    }

    public class CartItemRequest
    {
        [Required]
        [Range(2, int.MaxValue)]
        public int? MovieSessionId { get; set; }

        [Required]
        [Range(1, 50)]
        public int? TicketQty { get; set; }
    }
}