using AutoMapper;
using HashidsNet;
using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(ILocationIdentifier locationId, IStuffIdentifier stuffId, IUploadIdentifier uploadId, 
        ISupplierIdentifier supplierId, IPurchaseIdentifier purchaseId, IPurchaseItemIdentifier purchaseItemId)
    {
        CreateMap<LocationCreateModel, Location>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => m.ParentId != null ? locationId.Decode(m.ParentId) :  default(int?)));
        CreateMap<Location, LocationModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)))
            .ForMember(m => m.ParentId, o => o.MapFrom(e => e.ParentId != null ? locationId.Encode(e.ParentId.Value) : null))
            .ForMember(m => m.Children, o => o.Ignore());
        CreateMap<Location, LocationBasicModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)));
        CreateMap<Location, LocationTreeModel>();
        CreateMap<Location, LocationDataModel>();
        CreateMap<LocationBasicModel, LocationDataModel>();

        CreateMap<StuffCreateModel, Stuff>();
        CreateMap<Stuff, StuffModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => stuffId.Encode(e.Id)));
        CreateMap<Stuff, StuffListModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => stuffId.Encode(e.Id)));

        CreateMap<StuffLocationCreateModel, StuffLocation>()
            .ForMember(e => e.StuffId, o => o.MapFrom(m => m.StuffId != null ? stuffId.Decode(m.StuffId) :  default(int?)))
            .ForMember(e => e.LocationId, o => o.MapFrom(m => m.LocationId != null ? locationId.Decode(m.LocationId) :  default(int?)));
        CreateMap<StuffLocation, StuffLocationModel>();

        CreateMap<SupplierCreateModel, Supplier>();
        CreateMap<Supplier, SupplierModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => supplierId.Encode(e.Id)));
        CreateMap<Supplier, SupplierListModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => supplierId.Encode(e.Id)));

        CreateMap<PurchaseCreateModel, Purchase>()
            .ForMember(e => e.SupplierId, o => o.MapFrom(m => m.SupplierId != null ? supplierId.Decode(m.SupplierId) :  default(int?)));
        CreateMap<Purchase, PurchaseModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseId.Encode(e.Id)))
            .ForMember(m => m.SupplierId, o => o.MapFrom(e => e.SupplierId.HasValue ? supplierId.Encode(e.SupplierId.Value) : default));
        CreateMap<Purchase, PurchaseListModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseId.Encode(e.Id)));

        CreateMap<PurchaseItemCreateModel, PurchaseItem>()
            .ForMember(e => e.PurchaseId, o => o.MapFrom(m => m.PurchaseId != null ? purchaseId.Decode(m.PurchaseId) :  default(int?)))
            .ForMember(e => e.StuffId, o => o.MapFrom(m => m.StuffId != null ? stuffId.Decode(m.StuffId) :  default(int?)));
        CreateMap<PurchaseItem, PurchaseItemModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseItemId.Encode(e.Id)));
        CreateMap<PurchaseItem, PurchaseItemListModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseItemId.Encode(e.Id)));

        CreateMap<Upload, UploadModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => uploadId.Encode(e.Id)));
    }
}