using AutoMapper;
using HashidsNet;
using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(ILocationIdService locationId)
    {
        CreateMap<LocationCreateModel, Location>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => m.ParentId != null ? locationId.Decode(m.ParentId) :  default(int?)));
        CreateMap<Location, LocationModel>()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)))
            .ForMember(m => m.ParentId, o => o.MapFrom(e => e.ParentId != null ? locationId.Encode(e.ParentId.Value) : null));
    }
}