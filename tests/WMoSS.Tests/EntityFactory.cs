using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMoSS.Data;
using WMoSS.Entities;
using Xunit;

namespace WMoSS.Tests
{
    public class EntityFactory
    {
        private IDictionary<Type, Func<object>> factory = new Dictionary<Type, Func<object>>();
        private readonly ApplicationDbContext _dbContext;

        public EntityFactory(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public void Define<T>(Func<T> factory) where T : class
        {
            this.factory[typeof(T)] = factory;
        }

        public IEnumerable<T> Make<T>(int count, Action<T> overrides = null) where T : class
        {
            var entities = new List<T>();
            for(var i = 0; i < count; i++)
            {
                var factory = this.factory[typeof(T)] as Func<T>;
                if (factory != null)
                {
                    var entity = factory.Invoke();
                    if (overrides != null)
                    {
                        overrides.Invoke(entity);
                    }
                    entities.Add(entity);
                }
            }

            return entities;
        }

        public T Make<T>(Action<T> overrides = null) where T : class
        {
            return Make(1, overrides).FirstOrDefault();
        }

        public async Task<IEnumerable<T>> CreateAsync<T>(int count, Action<T> overrides = null) where T : class
        {
            var entities = Make(count, overrides);
            _dbContext.AddRange(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public async Task<T> CreateAsync<T>(Action<T> overrides = null) where T : class
        {
            return (await CreateAsync(1, overrides)).FirstOrDefault();
        }
        

    }

    public class EntityFactoryTests : IDisposable
    {
        private readonly ApplicationDbContext db;
        private readonly EntityFactory entityFactory;

        public EntityFactoryTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("InMemoryDb"));

            db = serviceCollection.BuildServiceProvider().GetService<ApplicationDbContext>();
            entityFactory = new EntityFactory(db);

            entityFactory.Define<Movie>(() => new Faker<Movie>()
                .RuleFor(m => m.Title, f => f.Lorem.Text())
                .RuleFor(m => m.Genre, f => f.Lorem.Word())
                .RuleFor(m => m.ReleaseDate, f => DateTime.UtcNow.AddDays(f.Random.Int(1, 365)))
                .RuleFor(m => m.RuntimeMinutes, f => f.Random.Number(100, 200))
                .RuleFor(m => m.Description, f => f.Lorem.Paragraph())
                .RuleFor(m => m.PosterFileName, f => f.Image.Image())
                .RuleFor(m => m.Classification, f => f.PickRandom("G", "PG", "MA", "MA15+", "R18+", "X18+"))
                .Generate());

            int numTheaters = 0;
            entityFactory.Define<Theater>(() =>
            {
                return new Faker<Theater>()
                .RuleFor(t => t.Name, f => $"Theater #{numTheaters + 1}")
                .RuleFor(t => t.Capacity, f => 50)
                .RuleFor(t => t.Address, f => "123 Lygon Street, Melbourne")
                .Generate();
            });

            entityFactory.Define<MovieSession>(() =>
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

        public void Dispose() { db.Dispose(); }

        [Fact]
        public void Test_SingleEntity()
        {
            var movie = entityFactory.Make<Movie>();
            var movieSession = entityFactory.Make<MovieSession>();
            var theater = entityFactory.Make<Theater>();

            Assert.NotNull(movie);
            Assert.NotNull(movieSession);
            Assert.NotNull(theater);
        }

        [Fact]
        public void Test_Multiple_Entities()
        {
            var movies = entityFactory.Make<Movie>(10);
            var movieSessions = entityFactory.Make<MovieSession>(10);
            var theaters = entityFactory.Make<Theater>(10);

            Assert.Equal(10, movies.Count());
            Assert.Equal(10, movieSessions.Count());
            Assert.Equal(10, theaters.Count());
        }

        [Fact]
        public async Task Test_CreateAsync_Populates_Id()
        {
            var movie = await entityFactory.CreateAsync<Movie>();
            Assert.True(movie.Id > 0);
        }
        
        [Fact]
        public async Task Test_CreateAsync_Populates_AllIds()
        {
            var movies = await entityFactory.CreateAsync<Movie>(10);
            Assert.All(movies, movie =>
            {
                Assert.True(movie.Id > 0);
            });
        }

        [Fact]
        public void Test_WhenOverrideIsProvidedInMakeMethod_TheSingleEntityThatIsCreatedCallsThatOverride()
        {
            var customTitle = "Some title";
            var movie = entityFactory.Make<Movie>(m => m.Title = customTitle);
            Assert.Equal(customTitle, movie.Title);
        }

        [Fact]
        public void Test_WhenOverrideIsProvidedInMakeMethod_TheMultipleEntityCreatedCallsThatOverride()
        {
            var customTitle = "Something";
            var movies = entityFactory.Make<Movie>(10, m => m.Title = customTitle);
            Assert.All(movies, m => Assert.True(m.Title == customTitle));
        }


    }
}
