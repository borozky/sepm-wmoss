using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMoSS.Entities;

namespace WMoSS.Repositories.Excel
{
    class MovieSessionExcelRepository : IMovieSessionRepository
    {
        private ExcelPackage excelPackage;
        private ExcelWorksheet sessionWorksheet;
        private IEnumerable<int> sessionRowIds;

        public MovieSessionExcelRepository(ExcelPackage excelPackage)
        {
            this.excelPackage = excelPackage;
            sessionWorksheet = excelPackage.Workbook.Worksheets["MOVIESESSIONS"];
            sessionRowIds = sessionWorksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
        }

        public IEnumerable<MovieSession> FindAll()
        {
            return sessionRowIds.Select(id => GetMovieSessionByRowId(id));
        }

        public IEnumerable<MovieSession> FindByMultipleIds(int[] ids) 
        {
            return ids.Select(id => FindById(id)).Where(ms => ms != null);
        }

        public IEnumerable<MovieSession> FindAllByMovieId(int movieId)
        {
            return sessionRowIds
                .Where(id => sessionWorksheet.Cells[id, 1].GetValue<int>() == movieId)
                .Select(id => GetMovieSessionByRowId(id));
        }

        public IEnumerable<MovieSession> FindAllByTheaterId(int theaterId)
        {
            return sessionRowIds
                .Where(id => sessionWorksheet.Cells[id, 2].GetValue<int>() == theaterId)
                .Select(id => GetMovieSessionByRowId(id));
        }

        public MovieSession FindById(int movieSessionId)
        {
            return Exists(movieSessionId) ? GetMovieSessionByRowId(movieSessionId) : null;
        }

        public void Create(MovieSession movieSession)
        {
            var lastRowId = sessionRowIds.Max();
            var newRowId = lastRowId + 1;
            sessionWorksheet.InsertRow(newRowId, 1);

            AssignMovieSessionValues(newRowId, movieSession);
            excelPackage.Save();
            movieSession.Id = newRowId;
        }

        public void Update(int movieSessionId, MovieSession movieSession)
        {
            if (movieSessionId != movieSession.Id)
            {
                throw new Exception("Movie session IDs mismatch");
            }

            if (Exists(movieSessionId) == false)
            {
                throw new Exception(String.Format("Movie session with ID {0} doesn't exists", movieSessionId));
            }

            AssignMovieSessionValues(movieSessionId, movieSession);
            excelPackage.Save();
        }

        public bool Exists(int movieSessionId)
        {
            return sessionRowIds.Contains(movieSessionId);
        }

        public IEnumerable<MovieSession> FindAllComingSoon(int nextMonths = 2)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MovieSession> FindAllNowShowing(int prevMonths = 2)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> FindAllAvaiableSeats()
        {
            throw new NotImplementedException();
        }

        private MovieSession GetMovieSessionByRowId(int rowId)
        {
            return new MovieSession
            {
                Id = rowId,
                MovieId = sessionWorksheet.Cells[rowId, 1].GetValue<int>(),
                TheaterId = sessionWorksheet.Cells[rowId, 2].GetValue<int>(),
                ScheduledAt = sessionWorksheet.Cells[rowId, 3].GetValue<string>(),
                TicketPrice = sessionWorksheet.Cells[rowId, 4].GetValue<double>(),
                ScheduledById = sessionWorksheet.Cells[rowId, 5].GetValue<int>(),
                CreatedAt = sessionWorksheet.Cells[rowId, 6].GetValue<string>()
            };
        }

        private void AssignMovieSessionValues(int rowId, MovieSession movieSession)
        {
            sessionWorksheet.Cells[rowId, 1].Value = movieSession.MovieId;
            sessionWorksheet.Cells[rowId, 2].Value = movieSession.TheaterId;
            sessionWorksheet.Cells[rowId, 3].Value = movieSession.ScheduledAt;
            sessionWorksheet.Cells[rowId, 4].Value = movieSession.TicketPrice;
            sessionWorksheet.Cells[rowId, 5].Value = movieSession.ScheduledById;
            sessionWorksheet.Cells[rowId, 6].Value = movieSession.CreatedAt;

        }
    }
}
