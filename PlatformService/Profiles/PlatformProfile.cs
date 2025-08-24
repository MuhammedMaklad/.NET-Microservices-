using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.Protos;

namespace PlatformService;

public class PlatformProfile : Profile
{
  public PlatformProfile()
  {
    CreateMap<Platform, ReadPlatformDto>().ReverseMap();
    CreateMap<CreatePlatformDto, Platform>().ReverseMap();
    CreateMap<ReadPlatformDto, PlatformPublishDto>().ReverseMap();
    CreateMap<Platform, GrpcPlatformModel>().
    ForMember(dest => dest.PlatformId, opt => opt.MapFrom(
      src => src.Id
    )).
     ForMember(dest => dest.Name, opt => opt.MapFrom(
      src => src.Name
    )).
     ForMember(dest => dest.Publisher, opt => opt.MapFrom(
      src => src.Publisher
    ));
  }
}
