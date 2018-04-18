using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMoSS.Entities;
using WMoSS.Repositories;

namespace WMoSS.Services
{
    public interface IExpressBookingDataProvider
    {
        IEnumerable<Movie> GetMoviesByTheaterId(int theaterId);
        IEnumerable<MovieSession> GetMovieSessionsByMovieId(int movieId);
        IEnumerable<Theater> GetAllTheaters();
    }

    public class ExpressBookingDataProvider : IExpressBookingDataProvider
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IMovieSessionRepository _movieSessionRepository;
        private readonly ITheaterRepository _theaterRepository;

        public ExpressBookingDataProvider(
            IMovieRepository movieRepository,
            IMovieSessionRepository movieSessionRepository,
            ITheaterRepository theaterRepository)
        {
            _movieRepository = movieRepository;
            _movieSessionRepository = movieSessionRepository;
            _theaterRepository = theaterRepository;
        }

        public IEnumerable<Theater> GetAllTheaters()
        {
            return _theaterRepository.FindAll();
        }

        public IEnumerable<Movie> GetMoviesByTheaterId(int theaterId)
        {
            var movieSessions = _movieSessionRepository.FindAllByTheaterId(theaterId);
            var movieIds = movieSessions.Select(ms => ms.MovieId).Distinct();
            return _movieRepository.FindByMultipleIds(movieIds.ToArray());
        }

        public IEnumerable<MovieSession> GetMovieSessionsByMovieId(int movieId)
        {
            return _movieSessionRepository.FindAllByMovieId(movieId);
        }
    }
}
