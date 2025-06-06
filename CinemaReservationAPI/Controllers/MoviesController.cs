using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly CinemaDbContext _context;

    public MoviesController(CinemaDbContext context)
    {
        _context = context;
    }

    // GET: api/movies
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetMovies()
    {
        try
        {
            var movies = await _context.Movies
                .Include(m => m.Screenings)
                .ThenInclude(s => s.Theater)
                .ToListAsync();

            var result = movies.Select(m => new
            {
                m.Id,
                m.Title,
                m.Description,
                m.DurationMinutes,
                m.Genre,
                m.ReleaseDate,
                m.PosterImagePath,
                Screenings = m.Screenings.Select(s => new
                {
                    s.Id,
                    s.StartTime,
                    Theater = new
                    {
                        s.Theater.Id,
                        s.Theater.Name,
                        s.Theater.Capacity
                    }
                })
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Error retrieving movies", Error = ex.Message });
        }
    }

    // GET: api/movies/5
    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetMovie(int id)
    {
        try
        {
            var movie = await _context.Movies
                .Include(m => m.Screenings)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound(new { Message = $"Movie with ID {id} not found" });
            }

            var result = new
            {
                movie.Id,
                movie.Title,
                movie.Description,
                movie.DurationMinutes,
                movie.Genre,
                movie.ReleaseDate,
                movie.PosterImagePath,
                Screenings = movie.Screenings.Select(s => new
                {
                    s.Id,
                    s.StartTime,
                    Theater = new
                    {
                        s.Theater.Id,
                        s.Theater.Name,
                        s.Theater.Capacity
                    }
                })
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving movie with ID {id}", Error = ex.Message });
        }
    }

    // GET: api/movies/5/screenings
    [HttpGet("{id}/screenings")]
    public async Task<ActionResult<IEnumerable<object>>> GetMovieScreenings(int id)
    {
        try
        {
            var movie = await _context.Movies
                .Include(m => m.Screenings)
                .ThenInclude(s => s.Theater)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                return NotFound(new { Message = $"Movie with ID {id} not found" });
            }

            var result = movie.Screenings.Select(s => new
            {
                s.Id,
                s.StartTime,
                Movie = new
                {
                    movie.Id,
                    movie.Title
                },
                Theater = new
                {
                    s.Theater.Id,
                    s.Theater.Name,
                    s.Theater.Capacity
                }
            });

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = $"Error retrieving screenings for movie with ID {id}", Error = ex.Message });
        }
    }
}