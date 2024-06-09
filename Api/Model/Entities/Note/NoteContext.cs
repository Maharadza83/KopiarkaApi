using Microsoft.EntityFrameworkCore;

namespace Api.Model.Entities.Note
{
    public class NoteContext : DbContext
    {
        public NoteContext(DbContextOptions<NoteContext> options) : base(options) { }

        public DbSet<Note> Notes { get; set; }
    }
}
