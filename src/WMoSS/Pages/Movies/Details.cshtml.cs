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
        public CartItem CartItem { get; set; }
        public IEnumerable<MovieSession> MovieSessions { get; private set; }
        public List<SelectListItem> MovieSessionsOptions { get; private set; }

        public async Task<IActionResult> OnGet(int id, CancellationToken ct)
        {
            Movie = await _dbcontext.Movies
                .Include(m => m.MovieSessions)
                    .ThenInclude(ms => ms.Theater)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id, ct);
            
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
            return Page();
        }
    }
}