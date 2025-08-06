using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;

namespace PlatformService;

public class PlatformProfile : Profile
{
  public PlatformProfile()
  {
    CreateMap<Platform, ReadPlatformDto>().ReverseMap();
    CreateMap<CreatePlatformDto, Platform>().ReverseMap();
  }
}
