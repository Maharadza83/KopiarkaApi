using Api.DTOs.Note;
using Api.Model.Entities.Note;
using AutoMapper;
namespace Api.Configurations

{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Note, PostNoteDTO>().ReverseMap();
            CreateMap<Note, GetNoteDTO>().ReverseMap();
            CreateMap<Note, NoteDTO>().ReverseMap();

        }
    }
}
