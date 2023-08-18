using AutoMapper;
using HashidsNet;
using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(
        IIdentityHasher<Location> locationId, 
        IIdentityHasher<Stuff> stuffId, 
        IIdentityHasher<Upload> uploadId, 
        IIdentityHasher<Supplier> supplierId, 
        IIdentityHasher<Purchase> purchaseId, 
        IIdentityHasher<PurchaseItem> purchaseItemId)
    {
        CreateMap<LocationRequest, Location>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => m.ParentId != null ? locationId.Decode(m.ParentId) :  default(int?)));
        CreateMap<Location, LocationModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)))
            .ForMember(m => m.ParentId, o => o.MapFrom(e => e.ParentId != null ? locationId.Encode(e.ParentId.Value) : null))
            .ForMember(m => m.Children, o => o.Ignore());
        CreateMap<Location, LocationListItem>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => locationId.Encode(e.Id)));
        CreateMap<Location, LocationTreeModel>();
        CreateMap<Location, LocationLookupItem>();
        CreateMap<LocationListItem, LocationLookupItem>();

        CreateMap<StuffRequest, Stuff>();
        CreateMap<Stuff, StuffModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => stuffId.Encode(e.Id)));
        CreateMap<Stuff, StuffListItem>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => stuffId.Encode(e.Id)));

        CreateMap<StuffLocationRequest, StuffLocation>()
            .ForMember(e => e.StuffId, o => o.MapFrom(m => m.StuffId != null ? stuffId.Decode(m.StuffId) :  default(int?)))
            .ForMember(e => e.LocationId, o => o.MapFrom(m => m.LocationId != null ? locationId.Decode(m.LocationId) :  default(int?)));
        CreateMap<StuffLocation, StuffLocationModel>();

        CreateMap<SupplierRequest, Supplier>();
        CreateMap<Supplier, SupplierModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => supplierId.Encode(e.Id)));
        CreateMap<Supplier, SupplierListItem>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => supplierId.Encode(e.Id)));

        CreateMap<PurchaseRequest, Purchase>()
            .ForMember(e => e.SupplierId, o => o.MapFrom(m => m.SupplierId != null ? supplierId.Decode(m.SupplierId) :  default(int?)));
        CreateMap<Purchase, PurchaseModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseId.Encode(e.Id)))
            .ForMember(m => m.SupplierId, o => o.MapFrom(e => e.SupplierId.HasValue ? supplierId.Encode(e.SupplierId.Value) : default));
        CreateMap<Purchase, PurchaseListItem>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseId.Encode(e.Id)));
        CreateMap<Purchase, PurchaseBasicModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseId.Encode(e.Id)));

        CreateMap<PurchaseItemRequest, PurchaseItem>()
            .ForMember(e => e.PurchaseId, o => o.MapFrom(m => purchaseId.Decode(m.PurchaseId)))
            .ForMember(e => e.StuffId, o => o.MapFrom(m => m.StuffId != null ? stuffId.Decode(m.StuffId) :  default(int?)));
        CreateMap<PurchaseItem, PurchaseItemModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseItemId.Encode(e.Id)))
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseId.Encode(e.PurchaseId)))
            .ForMember(m => m.StuffId, o => o.MapFrom(e => stuffId.Encode(e.StuffId))); ;
        CreateMap<PurchaseItem, PurchaseItemListItem>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseItemId.Encode(e.Id)))
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseId.Encode(e.PurchaseId)));
        CreateMap<PurchaseItem, PurchaseItemSupplier>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => purchaseItemId.Encode(e.Id)));

        CreateMap<Upload, UploadModel>().IncludeAllDerived()
            .ForMember(m => m.Id, o => o.MapFrom(e => uploadId.Encode(e.Id)));
    }
}