using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Pages.Theaters
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext db;

        public DetailsModel(ApplicationDbContext db)
        {
            this.db = db;
        }
        
        public IEnumerable<Movie> Movies { get; set; }
        public Theater Theater { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Theater = await db.Theaters.FirstOrDefaultAsync(t => t.Id == id);
            if (Theater == null)
            {
                return NotFound();
            }

            var movieIds = await db.MovieSessions
                .AsNoTracking()
                .Where(ms => ms.TheaterId == id)
                .Select(ms => ms.MovieId)
                .ToArrayAsync();
                
            var ids = movieIds.ToHashSet();
            Movies = await db.Movies.Where(m => ids.Contains(m.Id)).ToListAsync();

            return Page();
        }
    }
}