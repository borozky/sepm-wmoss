using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WMoSS.Data;
using WMoSS.Entities;

namespace WMoSS.Pages.Movies
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public IEnumerable<Movie> Movies { get; set; }

        // GET: /Movies?sortBy=session
        public async Task<IActionResult> OnGetAsync([FromQuery] string sort)
        {
            Movies = await _db.Movies
                .OrderBy(m => m.Title)
                .AsNoTracking()
                .ToListAsync();

            if (Movies != null && Movies.Count() > 0)
            {
                switch (sort)
                {
                    case "title-a-z":
                        Movies = Movies.OrderBy(m => m.Title);
                        break;
                    case "title-z-a":
                        Movies = Movies.OrderByDescending(m => m.Title);
                        break;
                    case "rating":
                        Movies = Movies.OrderBy(m => m.Rating);
                        break;
                    default:
                        break;
                }
            }

            return Page();
        }
    }
}