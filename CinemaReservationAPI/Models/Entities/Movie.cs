public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int DurationMinutes { get; set; }
    public string Genre { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string PosterImagePath { get; set; }
    public ICollection<Screening> Screenings { get; set; }
}

public class Screening
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; }
    public int TheaterId { get; set; }
    public Theater Theater { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
}

public class Theater
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Capacity { get; set; }
    public ICollection<Screening> Screenings { get; set; }
}

public class Reservation
{
    public int Id { get; set; }
    public string CustomerName { get; set; }
    public string CustomerEmail { get; set; }
    public DateTime ReservationTime { get; set; }
    public int ScreeningId { get; set; }
    public Screening Screening { get; set; }
    public int SeatNumber { get; set; }
    public bool IsConfirmed { get; set; }
    public string ConfirmationDocumentPath { get; set; }
}