using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Services
{
    public interface IMovieSearchService
    {
        IEnumerable<Movie> GetNowShowingMovies();
        IEnumerable<MovieSession> GetNowShowingSessions();
        IEnumerable<Movie> GetComingSoonMovies();
        IEnumerable<MovieSession> GetUpcomingSessions();
        IEnumerable<Movie> SearchMoviesByTitle(string keyword);
    }
}
