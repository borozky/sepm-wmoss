using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories.Excel
{
    public class MovieExcelRepository : IMovieRepository
    {
        ExcelPackage excelPackage;

        public MovieExcelRepository(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
        }

        public IEnumerable<Movie> FindAll()
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
            return movieRowIds.Select(id => GetMovieByRowId(moviesSheet, id));
        }

        public Movie FindById(int movieId)
        {
            if (Exists(movieId))
            {
                var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
                return GetMovieByRowId(moviesSheet, movieId);
            }

            return null;
        }

        public bool Exists(int movieId)
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
            return movieRowIds.Contains(movieId);
        }

        public IEnumerable<Movie> FindByMultipleIds(int[] ids)
        {
            return ids.Select(id => FindById(id)).Where(m => m != null);
        }

        public IEnumerable<Movie> FindByTitle(string title)
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
            var foundMovieIdsByTitle = movieRowIds.Where(id => moviesSheet.Cells[id, 1].GetValue<string>().ToLower() == title.ToLower());
            return foundMovieIdsByTitle.Select(movieId => GetMovieByRowId(moviesSheet, movieId));
        }
        
        public bool Create(Movie movie)
        {
            try
            {
                var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
                var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
                var lastRowId = movieRowIds.Max();
                var newRowId = lastRowId + 1;
                moviesSheet.InsertRow(newRowId, 1);

                moviesSheet.Cells[newRowId, 1].Value = movie.Title;
                moviesSheet.Cells[newRowId, 2].Value = movie.ReleaseYear;
                moviesSheet.Cells[newRowId, 3].Value = movie.Genre;
                moviesSheet.Cells[newRowId, 4].Value = movie.Classification;
                moviesSheet.Cells[newRowId, 5].Value = movie.Rating;
                moviesSheet.Cells[newRowId, 6].Value = movie.PosterFileName;
                moviesSheet.Cells[newRowId, 7].Value = movie.RuntimeMinutes;
                moviesSheet.Cells[newRowId, 8].Value = movie.Description;

                excelPackage.Save();
                movie.Id = newRowId;
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public bool Update(int movieId, Movie movie)
        {
            try
            {
                if (movieId != movie.Id)
                {
                    throw new Exception("Movie IDs mismatch");
                }

                if (Exists(movieId) == false)
                {
                    throw new Exception(String.Format("Movie with ID {0} doesn't exists", movieId));
                }

                var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
                var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);

                moviesSheet.Cells[movieId, 1].Value = movie.Title;
                moviesSheet.Cells[movieId, 2].Value = movie.ReleaseYear;
                moviesSheet.Cells[movieId, 3].Value = movie.Genre;
                moviesSheet.Cells[movieId, 4].Value = movie.Classification;
                moviesSheet.Cells[movieId, 5].Value = movie.Rating;
                moviesSheet.Cells[movieId, 6].Value = movie.PosterFileName;
                moviesSheet.Cells[movieId, 7].Value = movie.RuntimeMinutes;
                moviesSheet.Cells[movieId, 8].Value = movie.Description;

                excelPackage.Save();
                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                return false;
            }
        }

        public bool Delete(int movieId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Helper method. Retrieves movie information from specific row in a worksheet. 
        /// Make sure rowId exists in excel otherwise this will return movie with empty details
        /// </summary>
        /// <param name="moviesSheet"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        private Movie GetMovieByRowId(ExcelWorksheet moviesSheet, int rowId)
        {
            return new Movie
            {
                Id = rowId,
                Title = moviesSheet.Cells[rowId, 1].GetValue<string>(),
                ReleaseYear = moviesSheet.Cells[rowId, 2].GetValue<int?>(),
                Genre = moviesSheet.Cells[rowId, 3].GetValue<string>(),
                Classification = moviesSheet.Cells[rowId, 4].GetValue<string>(),
                Rating = moviesSheet.Cells[rowId, 5].GetValue<double?>(),
                PosterFileName = moviesSheet.Cells[rowId, 6].GetValue<string>(),
                RuntimeMinutes = moviesSheet.Cells[rowId, 7].GetValue<int?>(),
                Description = moviesSheet.Cells[rowId, 8].GetValue<string>()
            };
        }
    }
}
