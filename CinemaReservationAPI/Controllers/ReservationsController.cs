using CinemaReservationAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class ReservationsController : ControllerBase
{
    private readonly CinemaDbContext _context;
    private readonly IWebHostEnvironment _env;

    public ReservationsController(CinemaDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // GET: api/reservations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetReservations()
    {
        try
        {
            var reservations = await _context.Reservations
                .Include(r => r.Screening)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Screening)
                .ThenInclude(s => s.Theater)
                .Select(r => new
                {
                    r.Id,
                    r.CustomerName,
                    r.CustomerEmail,
                    r.SeatNumber,
                    r.ReservationTime,
                    r.IsConfirmed,
                    r.ConfirmationDocumentPath,
                    Screening = new
                    {
                        r.Screening.Id,
                        r.Screening.StartTime,
                        Movie = new
                        {
                            r.Screening.Movie.Id,
                            r.Screening.Movie.Title,
                            r.Screening.Movie.Description,
                            r.Screening.Movie.Genre
                        },
                        Theater = new
                        {
                            r.Screening.Theater.Id,
                            r.Screening.Theater.Name,
                            r.Screening.Theater.Capacity
                        }
                    }
                })
                .ToListAsync();

            return Ok(reservations);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error retrieving reservations", Error = ex.Message });
        }
    }

    [HttpGet("test-db")]
    public IActionResult TestDbConnection()
    {
        try
        {
            var canConnect = _context.Database.CanConnect();
            return Ok(new
            {
                Status = "Success",
                Database = _context.Database.GetDbConnection().Database,
                ConnectionState = _context.Database.GetDbConnection().State
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = ex.Message,
                ConnectionString = _context.Database.GetDbConnection().ConnectionString
            });
        }
    }

    // GET: api/reservations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetReservation(int id)
    {
        try
        {
            var reservation = await _context.Reservations
                .Include(r => r.Screening)
                .ThenInclude(s => s.Movie)
                .Include(r => r.Screening)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound(new { Message = $"Reservation with ID {id} not found" });
            }

            var result = new
            {
                reservation.Id,
                reservation.CustomerName,
                reservation.CustomerEmail,
                reservation.SeatNumber,
                reservation.ReservationTime,
                reservation.IsConfirmed,
                reservation.ConfirmationDocumentPath,
                Screening = new
                {
                    reservation.Screening.Id,
                    reservation.Screening.StartTime,
                    Movie = new
                    {
                        reservation.Screening.Movie.Id,
                        reservation.Screening.Movie.Title,
                        reservation.Screening.Movie.Description,
                        reservation.Screening.Movie.Genre
                    },
                    Theater = new
                    {
                        reservation.Screening.Theater.Id,
                        reservation.Screening.Theater.Name,
                        reservation.Screening.Theater.Capacity
                    }
                }
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving reservation with ID {id}", Error = ex.Message });
        }
    }

    // POST: api/reservations
    [HttpPost]
    public async Task<ActionResult<object>> CreateReservation([FromBody] ReservationCreateDTO reservationDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var screening = await _context.Screenings
            .Include(s => s.Movie)
            .Include(s => s.Theater)
            .FirstOrDefaultAsync(s => s.Id == reservationDto.ScreeningId);

        if (screening == null)
        {
            return BadRequest("Invalid screening ID");
        }

        // Check if seat number is within theater capacity
        if (reservationDto.SeatNumber > screening.Theater.Capacity)
        {
            return BadRequest($"Invalid seat number. The theater capacity is {screening.Theater.Capacity} seats.");
        }

        // Check if seat is already taken
        var isSeatTaken = await _context.Reservations
            .AnyAsync(r => r.ScreeningId == reservationDto.ScreeningId && r.SeatNumber == reservationDto.SeatNumber);

        if (isSeatTaken)
        {
            return BadRequest($"Seat {reservationDto.SeatNumber} is already taken.");
        }

        var reservation = new Reservation
        {
            CustomerName = reservationDto.CustomerName,
            CustomerEmail = reservationDto.CustomerEmail,
            SeatNumber = reservationDto.SeatNumber,
            ScreeningId = reservationDto.ScreeningId,
            ReservationTime = DateTime.Now,
            ConfirmationDocumentPath = "pending"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        var result = new
        {
            reservation.Id,
            reservation.CustomerName,
            reservation.CustomerEmail,
            reservation.SeatNumber,
            reservation.ReservationTime,
            reservation.IsConfirmed,
            reservation.ConfirmationDocumentPath,
            Screening = new
            {
                screening.Id,
                screening.StartTime,
                Movie = new
                {
                    screening.Movie.Id,
                    screening.Movie.Title,
                    screening.Movie.Description,
                    screening.Movie.Genre
                },
                Theater = new
                {
                    screening.Theater.Id,
                    screening.Theater.Name,
                    screening.Theater.Capacity
                }
            }
        };

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, result);
    }

    // PUT: api/reservations/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReservation(int id, [FromBody] Reservation reservation)
    {
        try
        {
            if (id != reservation.Id)
            {
                return BadRequest(new { Message = "Niezgodność ID w żądaniu" });
            }

            _context.Entry(reservation).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ReservationExists(id))
            {
                return NotFound(new { Message = $"Nie znaleziono rezerwacji o ID {id}" });
            }
            else
            {
                throw;
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Wystąpił błąd podczas aktualizacji rezerwacji o ID {id}", Error = ex.Message });
        }
    }

    // DELETE: api/reservations/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        try
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { Message = $"Nie znaleziono rezerwacji o ID {id}" });
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Wystąpił błąd podczas usuwania rezerwacji o ID {id}", Error = ex.Message });
        }
    }

    private bool ReservationExists(int id)
    {
        return _context.Reservations.Any(e => e.Id == id);
    }

    // GET: api/reservations/screening/{screeningId}/taken-seats
    [HttpGet("screening/{screeningId}/taken-seats")]
    public async Task<ActionResult<IEnumerable<int>>> GetTakenSeats(int screeningId)
    {
        try
        {
            Console.WriteLine($"Fetching taken seats for screening {screeningId}");
            
            // First check if the screening exists
            var screening = await _context.Screenings
                .Include(s => s.Reservations)
                .FirstOrDefaultAsync(s => s.Id == screeningId);
                
            if (screening == null)
            {
                Console.WriteLine($"Screening {screeningId} not found");
                return NotFound(new { Message = $"Screening {screeningId} not found" });
            }
            
            Console.WriteLine($"Found screening {screeningId}. Fetching reservations...");
            
            var takenSeats = await _context.Reservations
                .Where(r => r.ScreeningId == screeningId)
                .Select(r => r.SeatNumber)
                .ToListAsync();

            Console.WriteLine($"Found {takenSeats.Count} taken seats for screening {screeningId}");
            Console.WriteLine($"Taken seats: {string.Join(", ", takenSeats)}");

            return Ok(takenSeats);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetTakenSeats: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { Message = "Error retrieving taken seats", Error = ex.Message });
        }
    }

    // POST: api/reservations/5/upload
    [HttpPost("{id}/upload")]
    public async Task<IActionResult> UploadConfirmationDocument(int id, IFormFile file)
    {
        try
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound(new { Message = $"Nie znaleziono rezerwacji o ID {id}" });
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Message = "Nie przesłano pliku" });
            }

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            reservation.ConfirmationDocumentPath = $"/uploads/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Plik został pomyślnie przesłany", FilePath = reservation.ConfirmationDocumentPath });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Wystąpił błąd podczas przesyłania pliku", Error = ex.Message });
        }
    }

    // GET: api/reservations/5/download
    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadConfirmationDocument(int id)
    {
        try
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null || string.IsNullOrEmpty(reservation.ConfirmationDocumentPath))
            {
                return NotFound(new { Message = $"Nie znaleziono dokumentu potwierdzenia dla rezerwacji o ID {id}" });
            }

            var filePath = Path.Combine(_env.WebRootPath, reservation.ConfirmationDocumentPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { Message = "Plik nie istnieje" });
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, GetContentType(filePath), Path.GetFileName(filePath));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Wystąpił błąd podczas pobierania pliku", Error = ex.Message });
        }
    }

    private string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        return contentType;
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateReservationStatus(int id, [FromBody] ReservationStatusUpdateDTO statusUpdate)
    {
        try
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound(new { Message = $"Reservation with ID {id} not found" });
            }

            reservation.IsConfirmed = statusUpdate.IsConfirmed;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Reservation status updated successfully", IsConfirmed = reservation.IsConfirmed });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error updating reservation status", Error = ex.Message });
        }
    }
}