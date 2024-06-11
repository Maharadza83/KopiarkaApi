namespace Api.Dtos.Note
{
    public class BaseNoteDTO
    {
        public string Author { get; set; }
        public string CreationDate { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        //public IFormFile? FormFile { get; set; }

        public byte[] FileContent { get; set; }
    }
}
