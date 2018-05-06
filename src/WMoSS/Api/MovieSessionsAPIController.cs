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
    [Route("api/MovieSessions")]
    public class MovieSessionsAPIController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public MovieSessionsAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            return new JsonResult(await _context
                .MovieSessions
                .Where(ms => ms.Id == id)
                .AsNoTracking()
                .ToListAsync()
            );
        }


        [HttpGet("ByTheater/{theaterId}")]
        public async Task<IActionResult> GetByTheaterIdAsync(int theaterId)
        {
            if (theaterId <= 0)
            {
                return NotFound();
            }

            if (_context.Theaters.Count(c => c.Id == theaterId) == 0)
            {
                return NotFound();
            }

            return new JsonResult(await _context
                .MovieSessions
                .Where(ms => ms.TheaterId == theaterId)
                .AsNoTracking()
                .ToListAsync()
            );
        }

    }
}