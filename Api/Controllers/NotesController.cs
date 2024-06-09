using Api.DTOs.Note;
using Api.Model.Entities.Note;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Notes2Controller : ControllerBase
    {
        private readonly NoteContext dbContext;

        public Notes2Controller(NoteContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllNotes()
        {
            return Ok(dbContext.Notes.ToList());
        }

        [HttpGet]
        [Route("{id:int}")]
        public IActionResult GetNoteByName(int id)
        {
            var note = dbContext.Notes.Find(id);

            if (note is null)
            {
                return NotFound();
            }
            return Ok(note);
        }

        [HttpPost]
        public IActionResult AddNote(AddNoteDTO addNoteDTO)
        {
            var noteEntity = new Note()
            {
                Name = addNoteDTO.Name,
                Description = addNoteDTO.Description,
                Category = addNoteDTO.Category,
            };

            dbContext.Notes.Add(noteEntity);
            dbContext.SaveChanges();

            return Ok(noteEntity);
        }

        [HttpPut]
        [Route("{id:int}")]
        public IActionResult UpdateNote(int id, UpdateNoteDTO updateNoteDTO)
        {
            var note = dbContext.Notes.Find(id);

            if (note is null)
            {
                return NotFound();
            }

            note.Name = updateNoteDTO.Name;
            note.Description = updateNoteDTO.Description;
            note.Category = updateNoteDTO.Category;

            dbContext.SaveChanges();

            return Ok(note);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IActionResult Delete(int id)
        {
            var note = dbContext.Notes.Find(id);

            if (note is null)
            {
                return NotFound();
            }

            dbContext.Notes.Remove(note);
            dbContext.SaveChanges();
            return Ok();
        }

    }
}
