using Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WMoSS.Data;
using WMoSS.Entities;

namespace WMoSS.Tests.Integration
{
    /// <summary>
    /// Integration test setup class
    /// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/testing?view=aspnetcore-2.1
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class TestFixture<TStartup> : IDisposable
    {
        protected readonly TestServer Server;
        public HttpClient Client { get; }
        public EntityFactory factory { get; }

        public TestFixture() : this(Path.Combine("src"))
        {
            DefineTestModels();
        }

        protected TestFixture(string relativeTargetProjectParentDir)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;
            var contentRoot = GetProjectPath(relativeTargetProjectParentDir, startupAssembly);

            var builder = new WebHostBuilder()
                .UseContentRoot(contentRoot)
                .ConfigureServices(InitializeServices)
                .UseEnvironment("Development")
                .UseStartup(typeof(TStartup));

            Server = new TestServer(builder);

            Client = Server.CreateClient();

            Client.BaseAddress = new Uri("http://localhost");

            factory = new EntityFactory(Server.Host.Services.GetService<ApplicationDbContext>());
        }


        protected virtual void InitializeServices(IServiceCollection services)
        {
            var startupAssembly = typeof(TStartup).GetTypeInfo().Assembly;

            var manager = new ApplicationPartManager();
            manager.ApplicationParts.Add(new AssemblyPart(startupAssembly));
            manager.FeatureProviders.Add(new ControllerFeatureProvider());
            manager.FeatureProviders.Add(new ViewComponentFeatureProvider());

            services.AddSingleton(manager);
            services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));
        }

        /// <summary>
        /// Gets the full path to the target project that we wish to test
        /// </summary>
        /// <param name="projectRelativePath">
        /// The parent directory of the target project.
        /// e.g. src, samples, test, or test/Websites
        /// </param>
        /// <param name="startupAssembly">The target project's assembly.</param>
        /// <returns>The full path to the target project.</returns>
        private static string GetProjectPath(string projectRelativePath, Assembly startupAssembly)
        {
            // name of the target project
            var projectName = startupAssembly.GetName().Name;

            // test project path
            var applicationBasePath = System.AppContext.BaseDirectory;

            // path to the target project
            var directoryInfo = new DirectoryInfo(applicationBasePath);

            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    return Path.Combine(projectDirectoryInfo.FullName, projectName);
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}");
        }

        private void DefineTestModels()
        {
            int numTheaters = 0;
            factory.Define<Theater>(() =>
            {
                return new Faker<Theater>()
                .RuleFor(t => t.Name, f => $"Theater #{numTheaters + 1}")
                .RuleFor(t => t.Capacity, f => 50)
                .RuleFor(t => t.Address, f => "123 Lygon Street, Melbourne")
                .Generate();
            });

            factory.Define<Movie>(() => new Faker<Movie>()
                .RuleFor(m => m.Title, f => f.Lorem.Text())
                .RuleFor(m => m.Genre, f => f.Lorem.Word())
                .RuleFor(m => m.ReleaseDate, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                .RuleFor(m => m.RuntimeMinutes, f => f.Random.Number(100, 200))
                .RuleFor(m => m.Description, f => f.Lorem.Paragraph())
                .RuleFor(m => m.PosterFileName, f => f.Image.Image())
                .RuleFor(m => m.Classification, f => f.PickRandom("G", "PG", "MA", "MA15+", "R18+", "X18+"))
                .Generate());

            factory.Define<MovieSession>(() =>
            {
                Random random = new Random();
                int days = random.Next(1, 4);
                int hour = random.Next(1, 4);
                DateTime schedule = DateTime.Today.AddDays(days);
                schedule.AddHours(hour * 4.5);
                string scheduleStr = schedule.ToString("s", System.Globalization.CultureInfo.InvariantCulture);

                return new Faker<MovieSession>()
                    .RuleFor(ms => ms.TicketPrice, f => 20.00)
                    .RuleFor(ms => ms.ScheduledAt, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                    .RuleFor(ms => ms.TheaterId, f => f.Random.Int(2, 6))
                    .Generate();
            });
        }


        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}
