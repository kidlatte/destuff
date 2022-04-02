using AutoMapper;
using HashidsNet;
using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(ILocationIdService locationId)
    {
        CreateMap<LocationCreateModel, Location>();
        CreateMap<Location, LocationModel>()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)));

    }
}