using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WMoSS.Attributes;
using WMoSS.Entities;
using WMoSS.Extensions;
using WMoSS.Repositories.Excel;
using Xunit;

namespace WMoSS.Tests.Repositories.Excel
{
    public class MovieExcelRepositoryTests : ExcelRepositoryTestCase
    {
        MovieExcelRepository moviesRepo;

        public MovieExcelRepositoryTests() : base()
        {
            moviesRepo = new MovieExcelRepository(excelPackage);
        }

        [Fact]
        public void SmokeTest_WorksheetExists()
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            Assert.NotNull(moviesSheet);
        }


        [Fact]
        public void SmokeTest_RetrieveAll()
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
            var movies = movieRowIds.Select(id => new Movie
            {
                Id = id,
                Title = moviesSheet.Cells[id, 1].GetValue<string>(),
                ReleaseDate = moviesSheet.Cells[id, 2].GetValue<string>(),
                Genre = moviesSheet.Cells[id, 3].GetValue<string>(),
                Classification = moviesSheet.Cells[id, 4].GetValue<string>(),
                Rating = moviesSheet.Cells[id, 5].GetValue<double?>(),
                PosterFileName = moviesSheet.Cells[id, 6].GetValue<string>(),
                RuntimeMinutes = moviesSheet.Cells[id, 7].GetValue<int?>(),
                Description = moviesSheet.Cells[id, 8].GetValue<string>()
            });

            Assert.NotEmpty(movies);
        }

        [Fact]
        public void SmokeTest_UpdateMovie()
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
            moviesSheet.Cells[movieRowIds.First(), 1].Value = "Some movie";
            excelPackage.Save();
            Assert.True(true);
        }

        [Fact]
        public void SmokeTest_InsertNewMovie()
        {
            var moviesSheet = excelPackage.Workbook.Worksheets["MOVIES"];
            var movieRowIds = moviesSheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(x => x).Skip(1);
            var lastRowId = movieRowIds.Max();
            var newRowId = lastRowId + 1;
            moviesSheet.InsertRow(newRowId, 1);

            var movie = new Movie
            {
                Title = "Hello World",
                ReleaseDate = String.Format("{0:yyyy-MM-dd}", DateTime.Now),
                Genre = "Education",
                Classification = null,
                Rating = null,
                PosterFileName = null,
                RuntimeMinutes = null,
                Description = "This is a description"
            };

            moviesSheet.Cells[newRowId, 1].Value = movie.Title;
            moviesSheet.Cells[newRowId, 2].Value = movie.ReleaseDate;
            moviesSheet.Cells[newRowId, 3].Value = movie.Genre;
            moviesSheet.Cells[newRowId, 4].Value = movie.Classification;
            moviesSheet.Cells[newRowId, 5].Value = movie.Rating;
            moviesSheet.Cells[newRowId, 6].Value = movie.PosterFileName;
            moviesSheet.Cells[newRowId, 7].Value = movie.RuntimeMinutes;
            moviesSheet.Cells[newRowId, 8].Value = movie.Description;

            excelPackage.Save();

            Assert.True(true);

        }

        [Fact]
        public void Test_FindAll()
        {
            var movies = moviesRepo.FindAll();
            Assert.NotEmpty(movies);
        }


        [Fact]
        public void Test_FindById_Returns_Movie_When_2_Is_Passed()
        {
            var movie = moviesRepo.FindById(2);
            Assert.NotNull(movie);
        }


        [Fact]
        public void Test_FindById_Returns_Same_Id()
        {
            var movie = moviesRepo.FindById(2);
            Assert.Equal(2, movie.Id);
        }


        [Fact]
        public void Test_CreateMovie_Returns_True_For_ValidMovies()
        {
            var movie = new Movie
            {
                Title = "Hello World",
                ReleaseDate = String.Format("{0:yyyy-MM-dd}", DateTime.Now),
                Genre = "Fiction",
                Classification = null,
                Rating = null,
                PosterFileName = null,
                RuntimeMinutes = 120,
                Description = "Some Description"
            };

            var isSaved = moviesRepo.Create(movie);
            Assert.True(isSaved);
        }


        [Fact]
        public void Test_CreateMovie_Increases_Number_Of_Movies()
        {
            var movies = moviesRepo.FindAll();
            var prevNumMovies = movies.Count();

            var movie = new Movie
            {
                Title = "Hello World 2",
                ReleaseDate = String.Format("{0:yyyy-MM-dd}", DateTime.Now),
                Genre = "Fiction",
                Classification = null,
                Rating = null,
                PosterFileName = null,
                RuntimeMinutes = 120,
                Description = "Some Description"
            };

            moviesRepo.Create(movie);

            var newMovies = moviesRepo.FindAll();
            Assert.Equal(newMovies.Count(), prevNumMovies + 1);
        }


        [Fact]
        public void Test_CreateMovie_Increments_Previous_Latest_Movie()
        {
            var movies = moviesRepo.FindAll();
            var latestMovieId = movies.Select(m => m.Id).Max();

            var movie = new Movie
            {
                Title = "Hello World 3",
                ReleaseDate = String.Format("{0:yyyy-MM-dd}", DateTime.Now),
                Genre = "Fiction",
                Classification = null,
                Rating = null,
                PosterFileName = null,
                RuntimeMinutes = 120,
                Description = "Some Description"
            };

            moviesRepo.Create(movie);
            Assert.Equal(movie.Id, latestMovieId + 1);
        }


        [Fact]
        public void Test_CreateMovie_Can_Be_Retrieved()
        {
            var movie = new Movie
            {
                Title = "Hello World 4",
                ReleaseDate = String.Format("{0:yyyy-MM-dd}", DateTime.Now),
                Genre = "Fiction",
                Classification = null,
                Rating = null,
                PosterFileName = null,
                RuntimeMinutes = 120,
                Description = "Some Description"
            };

            moviesRepo.Create(movie);

            var foundMovie = moviesRepo.FindById(movie.Id);
            Assert.Equal(foundMovie.Id, movie.Id);
            Assert.Equal(foundMovie.Title, movie.Title);
        }

        [Fact]
        public void Test_UpdateMovie_Updates_Movie_Details()
        {
            var movies = moviesRepo.FindAll();
            var chosenMovie = movies.ElementAt(0);

            chosenMovie.Title = "World Hello 5";
            var isUpdated = moviesRepo.Update(chosenMovie.Id, chosenMovie);
            Assert.True(isUpdated);
        }

        [Fact]
        public void Test_UpdatedMovie_Updated_Movie_In_Memory_Same_As_In_Excel()
        {
            var movies = moviesRepo.FindAll();
            var chosenMovie = movies.ElementAt(0);

            chosenMovie.Title = "Hello World 6";
            var isUpdated = moviesRepo.Update(chosenMovie.Id, chosenMovie);

            var foundMovie = moviesRepo.FindById(chosenMovie.Id);
            Assert.Equal(chosenMovie.Id, foundMovie.Id);
            Assert.Equal(chosenMovie.Title, foundMovie.Title);
        }
    }
}
