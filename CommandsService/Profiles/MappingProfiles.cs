using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using CommandsService.Protos;

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
    CreateMap<GrpcPlatformModel, Platform>().
    ForMember(dest => dest.ExternalId, opt => opt.MapFrom(
      src => src.PlatformId
    )).
     ForMember(dest => dest.Name, opt => opt.MapFrom(
      src => src.Name
    )).
     ForMember(dest => dest.Commands, opt => opt.Ignore());
  }
} 
