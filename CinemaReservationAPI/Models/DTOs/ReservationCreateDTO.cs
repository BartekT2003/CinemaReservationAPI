using System.ComponentModel.DataAnnotations;

namespace CinemaReservationAPI.Models.DTOs
{
    public class ReservationCreateDTO
    {
        [Required]
        public string CustomerName { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }

        [Required]
        [Range(1, 1000)]
        public int SeatNumber { get; set; }

        [Required]
        public int ScreeningId { get; set; }
    }
}
