using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;
using WMoSS.Entities;

namespace WMoSS.Pages.Movies
{
    public class NowShowingModel : PageModel
    {
        private readonly ApplicationDbContext _dbcontext;

        public NowShowingModel(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IEnumerable<Movie> Movies { get; set; }

        public async Task OnGetAsync([FromQuery] string sort)
        {
            Movies = await _dbcontext.Movies
                .Include(m => m.MovieSessions)
                .Where(m => m.MovieSessions.Count() > 0)
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
        }
    }
}