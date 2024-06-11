namespace Api.Model.Entities.Note
{
    public class Note
    {
        public string Id { get; set; }
        public string Author { get; set; }
        public string CreationDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public byte[] FileContent { get; set; } = Array.Empty<byte>(); 
    }
}
