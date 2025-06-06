using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class DatabaseSeeder
{
    public static async Task SeedTheaters(CinemaDbContext context)
    {
        if (!context.Theaters.Any())
        {
            var theaters = new[]
            {
                new Theater { Name = "Theater 1", Capacity = 100 },
                new Theater { Name = "Theater 2", Capacity = 150 },
                new Theater { Name = "Theater 3", Capacity = 200 }
            };

            context.Theaters.AddRange(theaters);
            await context.SaveChangesAsync();
        }
    }

    public static async Task SeedMoviesAndScreenings(CinemaDbContext context)
    {
        // First ensure theaters exist
        await SeedTheaters(context);

        // Add movies if they don't exist
        var movies = new[]
        {
            new Movie 
            { 
                Title = "The Dark Knight",
                Description = "When the menace known as the Joker wreaks havoc and chaos on the people of Gotham, Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.",
                DurationMinutes = 152,
                Genre = "Action",
                ReleaseDate = new DateTime(2008, 7, 18),
                PosterImagePath = "/posters/dark-knight.jpg"
            },
            new Movie 
            { 
                Title = "Inception",
                Description = "A skilled thief with the rare ability to steal secrets from people's minds while they dream must now plant an idea into a CEO's mind in a complex heist that could change everything.",
                DurationMinutes = 148,
                Genre = "Sci-Fi",
                ReleaseDate = new DateTime(2010, 7, 16),
                PosterImagePath = "/posters/inception.jpg"
            },
            new Movie 
            { 
                Title = "The Shawshank Redemption",
                Description = "Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.",
                DurationMinutes = 142,
                Genre = "Drama",
                ReleaseDate = new DateTime(1994, 9, 23),
                PosterImagePath = "/posters/shawshank.jpg"
            },
            new Movie 
            { 
                Title = "Pulp Fiction",
                Description = "The lives of two mob hitmen, a boxer, a gangster and his wife, and a pair of diner bandits intertwine in four tales of violence and redemption.",
                DurationMinutes = 154,
                Genre = "Crime",
                ReleaseDate = new DateTime(1994, 10, 14),
                PosterImagePath = "/posters/pulp-fiction.jpg"
            },
            new Movie 
            { 
                Title = "The Matrix",
                Description = "A computer programmer discovers that reality as he knows it is a simulation created by machines, and joins a rebellion to break free.",
                DurationMinutes = 136,
                Genre = "Sci-Fi",
                ReleaseDate = new DateTime(1999, 3, 31),
                PosterImagePath = "/posters/matrix.jpg"
            },
            new Movie 
            { 
                Title = "Forrest Gump",
                Description = "The presidencies of Kennedy and Johnson, the Vietnam War, the Watergate scandal and other historical events unfold from the perspective of an Alabama man with an IQ of 75.",
                DurationMinutes = 142,
                Genre = "Drama",
                ReleaseDate = new DateTime(1994, 7, 6),
                PosterImagePath = "/posters/forrest-gump.jpg"
            },
            new Movie 
            { 
                Title = "Interstellar",
                Description = "In a future where Earth is becoming uninhabitable, a former NASA pilot leads a team through a wormhole in search of a new home for humanity while grappling with time, space, and the love for his family.",
                DurationMinutes = 169,
                Genre = "Sci-Fi",
                ReleaseDate = new DateTime(2014, 11, 7),
                PosterImagePath = "/posters/interstellar.jpg"
            },
            new Movie 
            { 
                Title = "The Godfather",
                Description = "The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.",
                DurationMinutes = 175,
                Genre = "Crime",
                ReleaseDate = new DateTime(1972, 3, 24),
                PosterImagePath = "/posters/godfather.jpg"
            },
            new Movie 
            { 
                Title = "Jurassic Park",
                Description = "A pragmatic paleontologist visiting an almost complete theme park is tasked with protecting a couple of kids after a power failure causes the park's cloned dinosaurs to run loose.",
                DurationMinutes = 127,
                Genre = "Adventure",
                ReleaseDate = new DateTime(1993, 6, 11),
                PosterImagePath = "/posters/jurassic-park.jpg"
            },
            new Movie 
            { 
                Title = "Avatar",
                Description = "A paraplegic Marine dispatched to the moon Pandora on a unique mission becomes torn between following his orders and protecting the world he feels is his home.",
                DurationMinutes = 162,
                Genre = "Sci-Fi",
                ReleaseDate = new DateTime(2009, 12, 18),
                PosterImagePath = "/posters/avatar.jpg"
            }
        };

        foreach (var movie in movies)
        {
            var existingMovie = await context.Movies.FirstOrDefaultAsync(m => m.Title == movie.Title);
            if (existingMovie == null)
            {
                context.Movies.Add(movie);
            }
            else
            {
                // Update existing movie
                existingMovie.Description = movie.Description;
                existingMovie.DurationMinutes = movie.DurationMinutes;
                existingMovie.Genre = movie.Genre;
                existingMovie.ReleaseDate = movie.ReleaseDate;
                existingMovie.PosterImagePath = movie.PosterImagePath;
                context.Movies.Update(existingMovie);
            }
        }
        await context.SaveChangesAsync();

        // Get theaters
        var theaters = await context.Theaters.ToListAsync();
        if (!theaters.Any()) return;

        // Add screenings for each movie
        var today = DateTime.Today;
        foreach (var movie in await context.Movies.ToListAsync())
        {
            for (int day = 0; day < 7; day++)
            {
                foreach (var theater in theaters)
                {
                    var screenings = new[]
                    {
                        new DateTime(today.Year, today.Month, today.Day, 14, 0, 0).AddDays(day), // 2 PM
                        new DateTime(today.Year, today.Month, today.Day, 17, 0, 0).AddDays(day), // 5 PM
                        new DateTime(today.Year, today.Month, today.Day, 20, 0, 0).AddDays(day), // 8 PM
                    };

                    foreach (var time in screenings)
                    {
                        if (!context.Screenings.Any(s => 
                            s.MovieId == movie.Id && 
                            s.TheaterId == theater.Id && 
                            s.StartTime == time))
                        {
                            context.Screenings.Add(new Screening
                            {
                                MovieId = movie.Id,
                                TheaterId = theater.Id,
                                StartTime = time
                            });
                        }
                    }
                }
            }
        }
        await context.SaveChangesAsync();
    }
} 