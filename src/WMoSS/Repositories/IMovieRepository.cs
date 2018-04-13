using System;
using System.Collections.Generic;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories
{
    public interface IMovieRepository
    {
        IEnumerable<Movie> FindAll();
        Movie FindById(int movieId);
        IEnumerable<Movie> FindByTitle(string title);
        IEnumerable<Movie> FindByMultipleIds(int[] ids);

        bool Create(Movie movie);
        bool Update(int movieId, Movie movie);
        bool Delete(int movieId);
    }
}
