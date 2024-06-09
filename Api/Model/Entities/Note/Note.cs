namespace Api.Model.Entities.Note
{
    public class Note
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Author { get; set; } = "System"; // Możesz zmienić na zalogowanego użytkownika
        public string CreationDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
