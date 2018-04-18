using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WMoSS.Entities;

namespace WMoSS.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext db)
        {
            db.Database.EnsureCreated();

            var movies = new Movie[]
            {
                new Movie
                {
                    Title = "The Nun",
                    ReleaseDate = new DateTime(2018, 9, 7),
                    Genre = "Horror, Mystery, Thriller",
                    Classification = "MA-15",
                    Rating = null,
                    RuntimeMinutes = 120,
                    PosterFileName = null,
                    Description = "A priest named Father Burke is sent to Romania to investigate the mysterious death of a nun."
                },
                new Movie
                {
                    Title = "The Predator",
                    ReleaseDate = new DateTime(2018, 9, 14),
                    Genre = " Action, Adventure, Horror",
                    Classification = "MA-15",
                    Rating = null,
                    RuntimeMinutes = 120,
                    PosterFileName = null,
                    Description = "The story will be focusing on military veterans encountering a Predator ship in a suburban neighborhood. After denial from government personnel they band together to deal with the intergalactic hunter. Assisting them will be scientists and a young autistic boy who is a language savant."
                },
                new Movie
                {
                    Title = "White Boy Rick",
                    ReleaseDate = new DateTime(2018, 9, 21),
                    Genre = "Crime, Drama",
                    Classification = "MA",
                    Rating = null,
                    RuntimeMinutes = 110,
                    PosterFileName = null,
                    Description = "The story of teenager Richard Wershe Jr., who became an undercover informant for the FBI during the 1980s and was ultimately arrested for drug-trafficking and sentenced to life in prison."
                },
                new Movie
                {
                    Title = "Johnny English Strikes Again",
                    ReleaseDate = new DateTime(2018, 10, 12),
                    Genre = "Action, Adventure, Comedy",
                    Classification = "PG",
                    Rating = null,
                    RuntimeMinutes = 130,
                    PosterFileName = null,
                    Description = "After a cyber-attack reveals the identity of all of the active undercover agents in Britain, Johnny English is forced to come out of retirement to find the mastermind hacker."
                },
                new Movie
                {
                    Title = "The House with a Clock in Its Walls",
                    ReleaseDate = new DateTime(2018, 09, 21),
                    Genre = "Fantasy, Horror, Mystery",
                    Classification = "PG",
                    Rating = null,
                    RuntimeMinutes = 100,
                    PosterFileName = null,
                    Description = "A young orphan named Lewis Barnavelt aids his magical uncle in locating a clock with the power to bring about the end of the world."
                }
            };

            db.Movies.AddRange(movies);
            db.SaveChanges();

            var theaters = new Theater[]
            {
                new Theater
                {
                    Name = "Theater #1",
                    Capacity = 50,
                    Address = "123 Lygon Street, Melbourn"
                },
                new Theater
                {
                    Name = "Theater #2",
                    Capacity = 50,
                    Address = "123 Lygon Street, Melbourn"
                },
                new Theater
                {
                    Name = "Theater #3",
                    Capacity = 50,
                    Address = "123 Lygon Street, Melbourn"
                },
                new Theater
                {
                    Name = "Theater #4",
                    Capacity = 50,
                    Address = "123 Lygon Street, Melbourn"
                },
                new Theater
                {
                    Name = "Theater #5",
                    Capacity = 50,
                    Address = "123 Lygon Street, Melbourn"
                },
            };

            db.Theaters.AddRange(theaters);

            var movieSessions = new MovieSession[]
            {
                // movies[0]
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[0].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(3).AddHours(7.5),
                },
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[0].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(3).AddHours(10),
                },
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[0].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(3).AddHours(16),
                },
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[0].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(3).AddHours(20),
                },

                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(4).AddHours(7.5),
                },
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(4).AddHours(10),
                },
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(4).AddHours(16),
                },
                new MovieSession
                {
                    MovieId = movies[0].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[0].ReleaseDate.Value.Date.AddDays(4).AddHours(20),
                },


                // movies[1]
                new MovieSession
                {
                    MovieId = movies[1].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[1].ReleaseDate.Value.Date.AddDays(4).AddHours(8),
                },
                new MovieSession
                {
                    MovieId = movies[1].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[1].ReleaseDate.Value.Date.AddDays(4).AddHours(12),
                },
                new MovieSession
                {
                    MovieId = movies[1].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[1].ReleaseDate.Value.Date.AddDays(4).AddHours(16),
                },
                new MovieSession
                {
                    MovieId = movies[1].Id,
                    TheaterId = theaters[1].Id,
                    ScheduledAt = movies[1].ReleaseDate.Value.Date.AddDays(4).AddHours(20),
                },

                // movies[2]
                new MovieSession
                {
                    MovieId = movies[2].Id,
                    TheaterId = theaters[2].Id,
                    ScheduledAt = movies[2].ReleaseDate.Value.Date.AddDays(4).AddHours(8),
                },
                new MovieSession
                {
                    MovieId = movies[2].Id,
                    TheaterId = theaters[2].Id,
                    ScheduledAt = movies[2].ReleaseDate.Value.Date.AddDays(4).AddHours(12),
                },
                new MovieSession
                {
                    MovieId = movies[2].Id,
                    TheaterId = theaters[2].Id,
                    ScheduledAt = movies[2].ReleaseDate.Value.Date.AddDays(4).AddHours(16),
                },
                new MovieSession
                {
                    MovieId = movies[2].Id,
                    TheaterId = theaters[2].Id,
                    ScheduledAt = movies[2].ReleaseDate.Value.Date.AddDays(4).AddHours(20),
                },

                // movies[3]
                new MovieSession
                {
                    MovieId = movies[3].Id,
                    TheaterId = theaters[3].Id,
                    ScheduledAt = movies[3].ReleaseDate.Value.Date.AddDays(4).AddHours(8),
                },
                new MovieSession
                {
                    MovieId = movies[3].Id,
                    TheaterId = theaters[3].Id,
                    ScheduledAt = movies[3].ReleaseDate.Value.Date.AddDays(4).AddHours(12),
                },
                new MovieSession
                {
                    MovieId = movies[3].Id,
                    TheaterId = theaters[3].Id,
                    ScheduledAt = movies[3].ReleaseDate.Value.Date.AddDays(4).AddHours(16),
                },
                new MovieSession
                {
                    MovieId = movies[3].Id,
                    TheaterId = theaters[3].Id,
                    ScheduledAt = movies[3].ReleaseDate.Value.Date.AddDays(4).AddHours(20),
                },

                 // movies[4]
                new MovieSession
                {
                    MovieId = movies[4].Id,
                    TheaterId = theaters[4].Id,
                    ScheduledAt = movies[4].ReleaseDate.Value.Date.AddDays(1).AddHours(8),
                },
                new MovieSession
                {
                    MovieId = movies[4].Id,
                    TheaterId = theaters[4].Id,
                    ScheduledAt = movies[4].ReleaseDate.Value.Date.AddDays(1).AddHours(12),
                },
                new MovieSession
                {
                    MovieId = movies[4].Id,
                    TheaterId = theaters[4].Id,
                    ScheduledAt = movies[4].ReleaseDate.Value.Date.AddDays(1).AddHours(16),
                },
                new MovieSession
                {
                    MovieId = movies[4].Id,
                    TheaterId = theaters[4].Id,
                    ScheduledAt = movies[4].ReleaseDate.Value.Date.AddDays(1).AddHours(20),
                },

            };
            db.MovieSessions.AddRange(movieSessions);
            db.SaveChanges();

        }
    }
}
