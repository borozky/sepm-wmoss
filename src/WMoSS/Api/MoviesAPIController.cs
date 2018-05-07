using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMoSS.Entities;
using WMoSS.Data;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Api
{
    [Produces("application/json")]
    [Route("api/Movies")]
    public class MoviesAPIController : Controller
    {

        private readonly ApplicationDbContext _context;


        public MoviesAPIController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IEnumerable<Movie>> GetAsync()
        {
            return await _context.Movies.AsNoTracking().ToListAsync();
        }

        
        [HttpGet("NowShowing")]
        public async Task<IEnumerable<Movie>> GetNowShowingAsync()
        {
            return await _context.Movies
                .Where(m => m.MovieSessions.Count() > 0)
                .AsNoTracking()
                .ToListAsync();
        }


        [HttpGet("ComingSoon")]
        public async Task<IEnumerable<Movie>> GetComingSoonAsync()
        {
            return await _context.Movies
                .AsNoTracking()
                .Where(m => m.MovieSessions.Count() == 0)
                .Where(m => m.ReleaseDate != null)
                .Where(m => DateTime.Now.AddMonths(3) > m.ReleaseDate.Value)
                .ToListAsync();
        }


    }


}