using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WMoSS.Data;
using WMoSS.Entities;
using Microsoft.EntityFrameworkCore;

namespace WMoSS.Api
{
    [Produces("application/json")]
    [Route("Api/ExpressBooking")]
    public class ExpressBookingAPIController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ExpressBookingAPIController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Api/ExpressBooking?movieId=5&theaterId=5
        [HttpGet]
        public async Task<IActionResult> GetData([FromQuery] int? movieId, [FromQuery] int? theaterId)
        {
            IEnumerable<Movie> movies = null;
            IEnumerable<MovieSession> movieSessions = null;
            IEnumerable<Theater> theaters = null;

            var movieIds = await _db.MovieSessions
                    .Select(ms => ms.MovieId)
                    .Distinct()
                    .ToArrayAsync();

            movies = await _db.Movies
                .Where(m => movieIds.Contains(m.Id))
                .AsNoTracking()
                .ToListAsync();
            

            if (movieId != null)
            {
                var theaterIds = await _db.MovieSessions
                    .Where(ms => ms.MovieId == movieId)
                    .Select(ms => ms.TheaterId)
                    .Distinct()
                    .ToArrayAsync();

                theaters = await _db.Theaters
                    .Where(t => theaterIds.Contains(t.Id))
                    .AsNoTracking()
                    .ToListAsync();

            }

            if (movieId != null && theaterId != null)
            {
                movieSessions = await _db.MovieSessions.AsNoTracking()
                    .Where(ms => ms.TheaterId == theaterId && ms.MovieId == movieId)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return new JsonResult(new
            {
                movies = movies,
                movieSessions = movieSessions,
                theaters = theaters
            });
        }
    }
}