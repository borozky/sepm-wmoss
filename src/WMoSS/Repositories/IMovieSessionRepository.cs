using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories
{
    public interface IMovieSessionRepository
    {
        MovieSession FindById(int movieSessionId);
        IEnumerable<MovieSession> FindAll();
        IEnumerable<MovieSession> FindByMultipleIds(int[] ids);
        IEnumerable<MovieSession> FindAllByMovieId(int movieId);
        IEnumerable<MovieSession> FindAllByTheaterId(int theaterId);

        IEnumerable<MovieSession> FindAllNowShowing(int prevMonths = 2);
        IEnumerable<MovieSession> FindAllComingSoon(int nextMonths = 2);

        IEnumerable<string> FindAllAvaiableSeats();

        void Create(MovieSession movieSession);
        void Update(int movieSessionId, MovieSession movieSession);
        bool Exists(int movieSessionId);
    }
}
