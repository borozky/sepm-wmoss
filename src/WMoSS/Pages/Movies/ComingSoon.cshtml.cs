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

        public async Task OnGetAsync()
        {
            Movies = await _dbcontext.Movies
                .AsNoTracking()
                .Where(m => m.MovieSessions.Count() == 0)
                .Where(m => m.ReleaseDate != null)
                .Where(m => DateTime.Now.AddMonths(3) > m.ReleaseDate.Value)
                .ToListAsync();
        }
    }
}