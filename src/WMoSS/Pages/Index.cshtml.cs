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

        public void OnGet()
        {
            Movies = _context.Movies.ToList();
        }


        // For testing setup
        public IActionResult OnPost(IFormCollection formData)
        {
            return new JsonResult(formData);
        }
    }
}
