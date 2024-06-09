using Api.Dtos.Note;
using Api.Model.Entities.Note;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteContext _context;
        private readonly ILogger<NotesController> _logger;

        public NotesController(NoteContext context, ILogger<NotesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddNote([FromBody] AddNoteDTO addNoteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string author = "Anonym";
            var token = Request.Headers["Token"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken;

                try
                {
                    jwtToken = handler.ReadToken(token) as JwtSecurityToken;
                    if (jwtToken != null)
                    {
                        author = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "sub")?.Value
                                 ?? jwtToken.Claims.FirstOrDefault(claim => claim.Type == "name")?.Value
                                 ?? "Anonym";
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing JWT token");
                }
            }

            var note = new Note
            {
                Id = Guid.NewGuid().ToString(), // Generowanie unikalnego identyfikatora
                Author = author,
                CreationDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                Name = addNoteDto.Name,
                Content = addNoteDto.Content
            };

            try
            {
                _context.Notes.Add(note);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving note to the database");
                return StatusCode(500, "Internal server error");
            }

            return Ok(note);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(string id)
        {
            Note note;
            try
            {
                note = await _context.Notes.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving note from the database");
                return StatusCode(500, "Internal server error");
            }

            if (note == null)
            {
                return NotFound();
            }

            var getNoteDto = new GetNoteDTO
            {
                Id = note.Id,
                Author = note.Author,
                CreationDate = note.CreationDate,
                Name = note.Name,
                Content = note.Content
            };

            return Ok(getNoteDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNote(string id, [FromBody] UpdateNoteDTO updateNoteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Note note;
            try
            {
                note = await _context.Notes.FindAsync(id);
                if (note == null)
                {
                    return NotFound();
                }

                note.Name = updateNoteDto.Name;
                note.Content = updateNoteDto.Content;

                _context.Notes.Update(note);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating note in the database");
                return StatusCode(500, "Internal server error");
            }

            return Ok(note);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes()
        {
            List<Note> notes;
            try
            {
                notes = await _context.Notes.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notes from the database");
                return StatusCode(500, "Internal server error");
            }

            var getNoteDtos = notes.Select(note => new GetNoteDTO
            {
                Id = note.Id,
                Author = note.Author,
                CreationDate = note.CreationDate,
                Name = note.Name,
                Content = note.Content
            }).ToList();

            return Ok(getNoteDtos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoteById(string id)
        {
            Note note;
            try
            {
                note = await _context.Notes.FindAsync(id);
                if (note == null)
                {
                    return NotFound();
                }

                _context.Notes.Remove(note);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting note from the database");
                return StatusCode(500, "Internal server error");
            }

            return NoContent();
        }
    }
}
