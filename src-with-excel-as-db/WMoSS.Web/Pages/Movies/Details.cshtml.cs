using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WMoSS.Repositories;
using WMoSS.Entities;
using Newtonsoft.Json;

namespace WMoSS.Web.Pages.Movies
{
    public class DetailsModel : PageModel
    {
        public IMovieRepository _movieRepository;
        public IMovieSessionRepository _movieSessionRepository;

        public DetailsModel(IMovieRepository movieRepository, 
            IMovieSessionRepository movieSessionRepository)
        {
            _movieRepository = movieRepository;
            _movieSessionRepository = movieSessionRepository;
        }

        public Movie Movie;
        public IEnumerable<MovieSession> MovieSessions;

        public IActionResult OnGet(int id)
        {
            Movie = _movieRepository.FindById(id);
            if (Movie == null)
            {
                return NotFound();
            }

            MovieSessions = _movieSessionRepository.FindAllByMovieId(id);

            return Page();
        }
    }
}