using Api.Dtos.Note;
using Api.Model.Entities.Note;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        private readonly NoteContext _context;

        public NotesController(NoteContext context)
        {
            _context = context;
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
    }
}
