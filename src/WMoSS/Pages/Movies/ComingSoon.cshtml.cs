using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Pages.Movies
{
    public class ComingSoonModel : PageModel
    {
        private readonly ApplicationDbContext _dbcontext;

        public ComingSoonModel(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public IEnumerable<Movie> Movies { get; set; }

        public async Task OnGetAsync([FromQuery] string sort)
        {
            Movies = await _dbcontext.Movies
                .AsNoTracking()
                .Where(m => m.MovieSessions.Count() == 0)
                .Where(m => m.ReleaseDate != null)
                .Where(m => DateTime.Now.AddMonths(3) > m.ReleaseDate.Value)
                .ToListAsync();

            if (Movies != null && Movies.Count() > 0)
            {
                switch (sort)
                {
                    case "title-a-z":
                        Movies.OrderBy(m => m.Title);
                        break;
                    case "title-z-a":
                        Movies.OrderByDescending(m => m.Title);
                        break;
                    case "rating":
                        Movies.OrderBy(m => m.Rating);
                        break;
                    default:
                        break;
                }
            }


        }
    }
}