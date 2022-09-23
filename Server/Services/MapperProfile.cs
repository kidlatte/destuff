using AutoMapper;
using HashidsNet;
using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(ILocationIdentifier locationId, IStuffIdentifier stuffId)
    {
        CreateMap<LocationCreateModel, Location>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => m.ParentId != null ? locationId.Decode(m.ParentId) :  default(int?)));
        CreateMap<Location, LocationModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)))
            .ForMember(m => m.ParentId, o => o.MapFrom(e => e.ParentId != null ? locationId.Encode(e.ParentId.Value) : null))
            .ForMember(m => m.Children, o => o.Ignore());
        CreateMap<Location, LocationTreeModel>()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)));
    }
}