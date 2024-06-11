using Api.Dtos.Note;
using Api.Model.Entities.Note;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Api.DTOs.Note;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteContext _context;

        public NotesController(NoteContext context)
        {
            _context = context;
        }

        [HttpPost]
      //  public async Task<IActionResult> AddNote([FromForm] NoteWithFileDTO addNoteDto)
        public async Task<IActionResult> AddNote([FromForm] NoteWithFileDTO addNoteDto)
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
                catch
                {
                    // Token is invalid or couldn't be parsed, keep author as "Anonym"
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

            if (addNoteDto.FormFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await addNoteDto.FormFile.CopyToAsync(memoryStream);
                    note.FileContent = memoryStream.ToArray();
                }
            }

            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return Ok(note);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNoteById(string id)
        {
            var note = await _context.Notes.FindAsync(id);
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
                Content = note.Content,
                FileContent = note.FileContent
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

            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            note.Name = updateNoteDto.Name;
            note.Content = updateNoteDto.Content;

            _context.Notes.Update(note);
            await _context.SaveChangesAsync();

            return Ok(note);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNotes(int page = 1, int pageSize = 10, string sortBy = "CreationDate", string sortOrder = "asc")
        {
            var totalCount = await _context.Notes.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            if (page < 1 || page > totalPages)
            {
                return BadRequest("Invalid page number.");
            }

            IQueryable<Note> notesQuery = _context.Notes;

            switch (sortBy.ToLower())
            {
                case "name":
                    notesQuery = sortOrder.ToLower() == "desc" ? notesQuery.OrderByDescending(n => n.Name) : notesQuery.OrderBy(n => n.Name);
                    break;
                case "author":
                    notesQuery = sortOrder.ToLower() == "desc" ? notesQuery.OrderByDescending(n => n.Author) : notesQuery.OrderBy(n => n.Author);
                    break;
                case "creationdate":
                default:
                    notesQuery = sortOrder.ToLower() == "desc" ? notesQuery.OrderByDescending(n => n.CreationDate) : notesQuery.OrderBy(n => n.CreationDate);
                    break;
            }

            var notes = await notesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var getNoteDtos = notes.Select(note => new GetNoteDTO
            {
                Id = note.Id,
                Author = note.Author,
                CreationDate = note.CreationDate,
                Name = note.Name,
                Content = note.Content,
                FileContent = note.FileContent
            }).ToList();

            var response = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = page,
                PageSize = pageSize,
                Notes = getNoteDtos
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNoteById(string id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
