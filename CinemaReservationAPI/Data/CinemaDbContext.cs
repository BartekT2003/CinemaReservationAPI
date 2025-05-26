using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

public class CinemaDbContext : DbContext
{
    public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Screening> Screenings { get; set; }
    public DbSet<Theater> Theaters { get; set; }
    public DbSet<Reservation> Reservations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguracja relacji i ograniczeń
        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Movie)
            .WithMany(m => m.Screenings)
            .HasForeignKey(s => s.MovieId);

        modelBuilder.Entity<Screening>()
            .HasOne(s => s.Theater)
            .WithMany(t => t.Screenings)
            .HasForeignKey(s => s.TheaterId);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Screening)
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.ScreeningId);

        modelBuilder.Entity<Movie>().HasData(
        new Movie
         {
        Id = 1,
        Title = "Inception",
        Description = "Film o snach",
        DurationMinutes = 148,
        Genre = "Sci-Fi",
        ReleaseDate = new DateTime(2010, 7, 16)
         }
        );
    }
}