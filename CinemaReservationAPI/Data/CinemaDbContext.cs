using Microsoft.EntityFrameworkCore;
using System;

public class CinemaDbContext : DbContext
{
    public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Screening> Screenings { get; set; }
    public DbSet<Theater> Theaters { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguracja relacji
        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Movie)
            .WithMany(m => m.Screenings)
            .HasForeignKey(s => s.MovieId);

        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Theater)
            .WithMany(t => t.Screenings)
            .HasForeignKey(s => s.TheaterId);

        // Seed danych
        modelBuilder.Entity<Movie>().HasData(
            new Movie
            {
                Id = 1,
                Title = "Inception",
                Description = "Film o snach",
                DurationMinutes = 148,
                Genre = "Sci-Fi",
                ReleaseDate = new DateTime(2010, 7, 16),
                PosterImagePath = "inception-poster.jpg"
            },
            new Movie
            {
                Id = 2,
                Title = "Interstellar",
                Description = "Film o podróżach kosmicznych",
                DurationMinutes = 169,
                Genre = "Sci-Fi",
                ReleaseDate = new DateTime(2014, 11, 7),
                PosterImagePath = "interstellar-poster.jpg"
            }
        );

        modelBuilder.Entity<Theater>().HasData(
            new Theater
            {
                Id = 1,
                Name = "Sala 1",
                Capacity = 120
            },
            new Theater
            {
                Id = 2,
                Name = "Sala IMAX",
                Capacity = 250
            }
        );

        modelBuilder.Entity<Screening>().HasData(
            new Screening
            {
                Id = 1,
                StartTime = new DateTime(2024, 6, 15, 18, 0, 0),
                MovieId = 1,
                TheaterId = 1
            },
            new Screening
            {
                Id = 2,
                StartTime = new DateTime(2024, 6, 16, 20, 30, 0),
                MovieId = 2,
                TheaterId = 2
            }
        );

        modelBuilder.Entity<Reservation>().HasData(
            new Reservation
            {
                Id = 1,
                CustomerName = "Jan Kowalski",
                CustomerEmail = "jan@example.com",
                SeatNumber = 15,
                ScreeningId = 1,
                ReservationTime = new DateTime(2024, 6, 1, 12, 0, 0),
                IsConfirmed = true,
                ConfirmationDocumentPath = "documents/confirm_001.pdf"
            },
            new Reservation
            {
                Id = 2,
                CustomerName = "Anna Nowak",
                CustomerEmail = "anna@example.com",
                SeatNumber = 22,
                ScreeningId = 2,
                ReservationTime = new DateTime(2024, 6, 2, 14, 30, 0),
                IsConfirmed = false,
                ConfirmationDocumentPath = "documents/confirm_002.pdf"
            }
        );
    }
}