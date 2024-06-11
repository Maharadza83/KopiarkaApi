using Api.Dtos.Note;

namespace Api.DTOs.Note
{
    public class NoteWithFileDTO : AddNoteDTO
    {
        public IFormFile? FormFile { get; set; }
//        public string FileExtension { get; set; }

    }
}
