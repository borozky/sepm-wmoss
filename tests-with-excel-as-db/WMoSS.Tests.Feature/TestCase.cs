using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using System.IO;
using WMoSS.Entities;
using Bogus;
using System.Linq;
using WMoSS.Tests.TestUtils;
using WMoSS.Repositories.Excel;
using OfficeOpenXml;

namespace WMoSS.Tests.Feature
{

    // In razor page integration testing, tests will return a 500 response error
    // without the following lines of code in .csproj file in the WMoSS.Tests.Feature project
    //
    //<Target Name="CopyDepsFiles" AfterTargets="Build" Condition="'$(TargetFramework)'!=''">
    //  <ItemGroup>
    //      <DepsFilePaths Include="$([System.IO.Path]::ChangeExtension('%(_ResolvedProjectReferencePaths.FullPath)', '.deps.json'))" />
    //  </ItemGroup>
    //  <Copy SourceFiles="%(DepsFilePaths.FullPath)" DestinationFolder="$(OutputPath)" Condition="Exists('%(DepsFilePaths.FullPath)')" />
    //</Target>


    /// <summary>
    /// Superclass for all feature tests
    /// </summary>
    public abstract class TestCase : IDisposable
    {
        protected readonly TestServer server;
        protected readonly HttpClient client;
        protected readonly string sourceFolder;
        protected readonly string webfolder;
        protected readonly IWebHostBuilder webhostBuilder;
        protected readonly ExcelPackage excelPackage;

        private IEntityFactory factory;

        protected TestCase()
        {
            sourceFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\..\src");
            webfolder = Path.Combine(sourceFolder, @"WMoSS.Web");
            webhostBuilder = new WebHostBuilder()
                .UseEnvironment("Testing")
                .UseContentRoot(webfolder)
                .UseStartup<WMoSS.Web.Startup>();

            server = new TestServer(webhostBuilder);
            client = server.CreateClient();

            factory = new ExcelEntityFactory();

            DefineEntities();

            var excelTestFile = Path.Combine(sourceFolder, "WMOSS-EXCELDB-TEST.xlsx");
            var excelFile = new FileInfo(excelTestFile);
            excelPackage = new ExcelPackage(excelFile);
        }

        protected void Define<T>(Func<T> definition) where T : class
        {
            factory.Define<T>(definition);
        }

        
        protected T Make<T>() where T : class
        {
            return Make<T>(1).FirstOrDefault();
        }

        protected IEnumerable<T> Make<T>(int numberOfEntities = 1) where T : class
        {
            return factory.Make<T>(numberOfEntities);
        }

        [Fact]
        public void SmokeTest_EntityGeneration()
        {
            var id = 2;
            factory.Define<Movie>(() =>
            {
                return new Faker<Movie>()
                .RuleFor(m => m.Id, f => id++)
                .RuleFor(m => m.Title, f => f.Lorem.Text())
                .RuleFor(m => m.Genre, f => f.Lorem.Word())
                .RuleFor(m => m.ReleaseDate, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                .RuleFor(m => m.RuntimeMinutes, f => f.Random.Number(100, 200))
                .RuleFor(m => m.Description, f => f.Lorem.Paragraph())
                .RuleFor(m => m.PosterFileName, f => f.Image.Image())
                .RuleFor(m => m.Classification, f => f.PickRandom("G", "PG", "MA", "MA15+", "R18+", "X18+"))
                .Generate();
            });

            var movie = factory.Make<Movie>();
            Assert.NotNull(movie);
        }

        [Fact]
        public void SmokeTest_MultipleEntityGeneration()
        {
            var id = 2;
            factory.Define<Movie>(() =>
            {
                return new Faker<Movie>()
                .RuleFor(m => m.Id, f => id++)
                .RuleFor(m => m.Title, f => f.Lorem.Text())
                .RuleFor(m => m.Genre, f => f.Lorem.Word())
                .RuleFor(m => m.ReleaseDate, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                .RuleFor(m => m.RuntimeMinutes, f => f.Random.Number(100, 200))
                .RuleFor(m => m.Description, f => f.Lorem.Paragraph())
                .RuleFor(m => m.PosterFileName, f => f.Image.Image())
                .RuleFor(m => m.Classification, f => f.PickRandom("G", "PG", "MA", "MA15+", "R18+", "X18+"))
                .Generate();
            });

            var movies = factory.Make<Movie>(2);
            Assert.Equal(movies.Count(), 2);
        }

        private void DefineEntities()
        {
            int theaterId = 2;
            Define<Theater>(() =>
            {
                return new Faker<Theater>()
                .RuleFor(t => t.Id, f => theaterId++)
                .RuleFor(t => t.Name, f => $"Theater #{theaterId - 2}")
                .RuleFor(t => t.Capacity, f => 50)
                .RuleFor(t => t.Address, f => "123 Lygon Street, Melbourne")
                .Generate();
            });

            int movieId = 2;
            Define<Movie>(() =>
            {
                return new Faker<Movie>()
                .RuleFor(m => m.Id, f => movieId++)
                .RuleFor(m => m.Title, f => f.Lorem.Text())
                .RuleFor(m => m.Genre, f => f.Lorem.Word())
                .RuleFor(m => m.ReleaseDate, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                .RuleFor(m => m.RuntimeMinutes, f => f.Random.Number(100, 200))
                .RuleFor(m => m.Description, f => f.Lorem.Paragraph())
                .RuleFor(m => m.PosterFileName, f => f.Image.Image())
                .RuleFor(m => m.Classification, f => f.PickRandom("G", "PG", "MA", "MA15+", "R18+", "X18+"))
                .Generate();
            });

            int movieSessionId = 2;
            Define<MovieSession>(() =>
            {
                Random random = new Random();
                int days = random.Next(1, 4);
                int hour = random.Next(1, 4);
                DateTime schedule = DateTime.Today.AddDays(days);
                schedule.AddHours(hour * 4.5);
                string scheduleStr = schedule.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

                return new Faker<MovieSession>()
                .RuleFor(ms => ms.Id, f => movieSessionId++)
                .RuleFor(ms => ms.TicketPrice, f => 20.00)
                .RuleFor(ms => ms.ScheduledAt, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                .RuleFor(ms => ms.TheaterId, f => f.Random.Int(2, 6))
                .Generate();
            });
        }

        protected void SeedData()
        {
            var theaters = Make<Theater>(5);
            var worksheet = excelPackage.Workbook.Worksheets["THEATERS"];
            var rowIds = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
            foreach (var theater in theaters)
            {
                var lastRowId = rowIds.Count() > 0 ? rowIds.Max() : 1;
                var newRowId = lastRowId + 1;
                worksheet.InsertRow(newRowId, 1);
                worksheet.Cells[newRowId, 1].Value = theater.Name;
                worksheet.Cells[newRowId, 2].Value = theater.Capacity;
                worksheet.Cells[newRowId, 3].Value = theater.Address;
                theater.Id = newRowId;
            }

            var movies = Make<Movie>(10);
            worksheet = excelPackage.Workbook.Worksheets["MOVIES"];
            rowIds = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
            foreach(var movie in movies)
            {
                var lastRowId = rowIds.Count() > 0 ? rowIds.Max() : 1;
                var newRowId = lastRowId + 1;
                worksheet.InsertRow(newRowId, 1);
                worksheet.Cells[newRowId, 1].Value = movie.Title;
                worksheet.Cells[newRowId, 2].Value = movie.ReleaseDate;
                worksheet.Cells[newRowId, 3].Value = movie.Genre;
                worksheet.Cells[newRowId, 4].Value = movie.Classification;
                worksheet.Cells[newRowId, 5].Value = movie.Rating;
                worksheet.Cells[newRowId, 6].Value = movie.PosterFileName;
                worksheet.Cells[newRowId, 7].Value = movie.RuntimeMinutes;
                worksheet.Cells[newRowId, 8].Value = movie.Description;
                movie.Id = newRowId;
            }

            var movieSessions = Make<MovieSession>(100);
            worksheet = excelPackage.Workbook.Worksheets["MOVIESESSIONS"];
            rowIds = worksheet.Cells.Select(cell => cell.Start.Row).Distinct().OrderBy(id => id).Skip(1);
            foreach (var movieSession in movieSessions)
            {
                var theaterIds = theaters.Select(t => t.Id).ToArray();
                var index = new Random().Next(theaterIds.Length);
                movieSession.TheaterId = theaterIds[index];

                var movieIds = movies.Select(m => m.Id).ToArray();
                var movieIndex = new Random().Next(movieIds.Length);
                movieSession.MovieId = movieIds[movieIndex];

                var lastRowId = rowIds.Count() > 0 ? rowIds.Max() : 1;
                var newRowId = lastRowId + 1;
                worksheet.InsertRow(newRowId, 1);
                worksheet.Cells[newRowId, 1].Value = movieSession.MovieId;
                worksheet.Cells[newRowId, 2].Value = movieSession.TheaterId;
                worksheet.Cells[newRowId, 3].Value = movieSession.ScheduledAt;
                worksheet.Cells[newRowId, 4].Value = movieSession.TicketPrice;
                worksheet.Cells[newRowId, 5].Value = movieSession.ScheduledById;
                worksheet.Cells[newRowId, 6].Value = movieSession.CreatedAt;
                movieSession.Id = newRowId;
            }

            excelPackage.Save();
            excelPackage.Dispose();
        }


        /// <summary>
        /// Converts dictionary into url-encoded form data. See
        /// https://docs.microsoft.com/en-us/aspnet/core/testing/razor-pages-testing?view=aspnetcore-2.1
        /// </summary>
        /// <param name="_client"></param>
        /// <param name="path"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<FormUrlEncodedContent> GetRequestContentAsync(
            HttpClient _client, string path, IDictionary<string, string> data)
        {
            // Make a request for the resource.
            var getResponse = await _client.GetAsync(path);

            // Set the response's antiforgery cookie on the HttpClient.
            _client.DefaultRequestHeaders.Add("Cookie",
                getResponse.Headers.GetValues("Set-Cookie"));

            // Obtain the request verification token from the response.
            // Any <form> element in the response contains a token, and
            // they're all the same within a single response.
            //
            // This method uses Regex to parse the element and its value
            // from the response markup. A better approach in a production
            // app would be to use an HTML parser (for example, 
            // HtmlAgilityPack: http://html-agility-pack.net/).
            var responseMarkup = await getResponse.Content.ReadAsStringAsync();
            var regExp_RequestVerificationToken = new Regex(
                "<input name=\"__RequestVerificationToken\" type=\"hidden\" value=\"(.*?)\" \\/>",
                RegexOptions.Compiled);
            var matches = regExp_RequestVerificationToken.Matches(responseMarkup);
            // Group[1] represents the captured characters, represented
            // by (.*?) in the Regex pattern string.
            var token = matches?.FirstOrDefault().Groups[1].Value;

            // Add the token to the form data for the request.
            data.Add("__RequestVerificationToken", token);

            return new FormUrlEncodedContent(data);
        }

        public void Dispose()
        {
            var excelSampleFile = Path.Combine(sourceFolder, "WMOSS-EXCELDB-SAMPLE.xlsx");
            var excelTestFile = Path.Combine(sourceFolder, "WMOSS-EXCELDB-TEST.xlsx");
            File.Copy(excelSampleFile, excelTestFile, true);

            client.Dispose();
            server.Dispose();
        }

    }

}
