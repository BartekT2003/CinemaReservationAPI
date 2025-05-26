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
    public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
    {
        try
        {
            return await _context.Reservations
                .Include(r => r.Screening)
                .ThenInclude(s => s.Movie)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Wystąpił błąd podczas pobierania rezerwacji", Error = ex.Message });
        }
    }

    // GET: api/reservations/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Reservation>> GetReservation(int id)
    {
        try
        {
            var reservation = await _context.Reservations
                .Include(r => r.Screening)
                .ThenInclude(s => s.Movie)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound(new { Message = $"Nie znaleziono rezerwacji o ID {id}" });
            }

            return reservation;
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Wystąpił błąd podczas pobierania rezerwacji o ID {id}", Error = ex.Message });
        }
    }

    // POST: api/reservations
    [HttpPost]
    public async Task<ActionResult<Reservation>> CreateReservation([FromBody] Reservation reservation)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            reservation.ReservationTime = DateTime.Now;
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservation), new { id = reservation.Id }, reservation);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Wystąpił błąd podczas tworzenia rezerwacji", Error = ex.Message });
        }
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
}