using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace WMoSS.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Movie> Movies { get; set; }
        public IEnumerable<FeatureSlide> FeatureSlides { get; set; }

        public void OnGet()
        {
            Movies = _context.Movies.ToList();
            FeatureSlides = new FeatureSlide[]
            {
                new FeatureSlide { ImageUrl = "images/1.jpg" },
                new FeatureSlide { ImageUrl = "images/2.jpg" },
                new FeatureSlide { ImageUrl = "images/3.jpg" }
            };
        }


        // For testing setup
        public IActionResult OnPost(IFormCollection formData)
        {
            return new JsonResult(formData);
        }
    }

    public class FeatureSlide
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string CallToActionLink { get; set; }
    }
}
