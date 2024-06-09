using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Note
{
    public abstract class BaseNoteDTO
    
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
    }
}
