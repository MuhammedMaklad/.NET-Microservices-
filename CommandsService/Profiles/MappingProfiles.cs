using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService;

public class MappingProfiles: Profile
{
  public MappingProfiles()
  {
    CreateMap<PlatformReadDto, Platform>().ReverseMap();
    CreateMap<CommandCreateDto, Command>().ReverseMap();
    CreateMap<CommandReadDto, Platform>().ReverseMap();
    CreateMap<PlatformPublishDto, Platform>()
    .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(
      src => src.Id
    ));
  }
} 
