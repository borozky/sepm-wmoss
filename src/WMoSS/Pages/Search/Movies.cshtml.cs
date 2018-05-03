using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Pages.Search
{
    public class MoviesModel : PageModel
    {
        public ApplicationDbContext _dbcontext;

        public MoviesModel(ApplicationDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        
        public SearchMoviesViewModel SearchResultsVM { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery]string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return NotFound();
            }

            SearchResultsVM = new SearchMoviesViewModel();

            SearchResultsVM.SearchResults = await _dbcontext.Movies
                .AsNoTracking()
                .Where(m => m.Title.ToLower().Contains(keyword.ToLower()))
                .ToListAsync();
            
            if (SearchResultsVM.SearchResults.Count() == 0)
            {
                return RedirectToPage("/Search/NotFound");
            }

            SearchResultsVM.Keyword = keyword;
            return Page();
        }
    }

    public class SearchMoviesViewModel
    {
        public IEnumerable<Movie> SearchResults { get; set; }
        public string Keyword { get; set; }
    }
}