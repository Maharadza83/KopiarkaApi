using Api.Model.Entities.Note;
using AutoMapper;
using Api.Dtos.Note;

namespace Api.Configurations

{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            // CreateMap<Note, PostNoteDTO>().ReverseMap();
            // CreateMap<Note, GetNoteDTO>().ReverseMap();
            // CreateMap<Note, NoteDTO>().ReverseMap();

        }
    }
}
