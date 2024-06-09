using Microsoft.EntityFrameworkCore;

namespace Api.Model.Entities.Note
{
    public class NoteContext : DbContext

    {
        public DbSet<Note> Notes { get; set; }
        public NoteContext(DbContextOptions<NoteContext> options) : base(options)
        {

        }
    }
}
      