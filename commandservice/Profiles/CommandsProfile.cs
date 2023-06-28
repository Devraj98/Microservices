using AutoMapper;
using commandservice.Dtos;
using commandservice.Models;

namespace commandservice.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            //source -> target

            CreateMap<Platform,PlatformReadDto>();
            CreateMap<CommandCreateDto, Command>();
            CreateMap<Command, PlatformReadDto>();
        }
    }
}
