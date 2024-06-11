using Api.Dtos.Note;

namespace Api.DTOs.Note
{
    public class NoteWithFileDTO : BaseNoteDTO
    {
        public IFormFile FormFile { get; set; }
    }
}
