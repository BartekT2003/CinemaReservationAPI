using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Movie
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    public int DurationMinutes { get; set; }
    
    [Required]
    public string Genre { get; set; }
    
    public DateTime ReleaseDate { get; set; }
    
    [Required]
    public string PosterImagePath { get; set; }
    
    [JsonIgnore]
    public ICollection<Screening> Screenings { get; set; }
}

public class Screening
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public int MovieId { get; set; }
    
    [JsonIgnore]
    public Movie Movie { get; set; }
    
    public int TheaterId { get; set; }
    
    [JsonIgnore]
    public Theater Theater { get; set; }
    
    [JsonIgnore]
    public ICollection<Reservation> Reservations { get; set; }
}

public class Theater
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public int Capacity { get; set; }
    
    [JsonIgnore]
    public ICollection<Screening> Screenings { get; set; }
}

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public string CustomerName { get; set; }

    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; }

    public DateTime ReservationTime { get; set; }

    [Required]
    public int SeatNumber { get; set; }

    public bool IsConfirmed { get; set; }

    public string ConfirmationDocumentPath { get; set; }

    public int ScreeningId { get; set; }

    [JsonIgnore]
    [ForeignKey("ScreeningId")]
    public Screening Screening { get; set; }
}