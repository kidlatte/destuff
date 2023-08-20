using AutoMapper;
using Destuff.Server.Data.Entities;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(
        IIdentityHasher<Location> locationHasher, 
        IIdentityHasher<Stuff> stuffHasher, 
        IIdentityHasher<Upload> uploadHasher, 
        IIdentityHasher<Supplier> supplierHasher, 
        IIdentityHasher<Purchase> purchaseHasher, 
        IIdentityHasher<PurchaseItem> purchaseItemHasher)
    {
        CreateMap<LocationRequest, Location>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => m.ParentId != null ? locationHasher.Decode(m.ParentId) :  default(int?)));
        CreateEntityMap<Location, LocationModel>(locationHasher).IncludeAllDerived()
            .ForMember(m => m.ParentId, o => o.MapFrom(e => e.ParentId != null ? locationHasher.Encode(e.ParentId.Value) : null))
            .ForMember(m => m.Children, o => o.Ignore());
        CreateEntityMap<Location, LocationListItem>(locationHasher).IncludeAllDerived();
        CreateMap<Location, LocationTreeModel>();
        CreateMap<Location, LocationLookupItem>();
        CreateMap<LocationListItem, LocationLookupItem>();

        CreateMap<StuffRequest, Stuff>();
        CreateEntityMap<Stuff, StuffBasicModel>(stuffHasher).IncludeAllDerived();
        CreateMap<Stuff, StuffModel>();
        CreateMap<Stuff, StuffListItem>();

        CreateMap<StuffLocationRequest, StuffLocation>()
            .ForMember(e => e.StuffId, o => o.MapFrom(m => m.StuffId != null ? stuffHasher.Decode(m.StuffId) :  default(int?)))
            .ForMember(e => e.LocationId, o => o.MapFrom(m => m.LocationId != null ? locationHasher.Decode(m.LocationId) :  default(int?)));
        CreateMap<StuffLocation, StuffLocationModel>();

        CreateMap<SupplierRequest, Supplier>();
        CreateEntityMap<Supplier, SupplierModel>(supplierHasher).IncludeAllDerived();
        CreateEntityMap<Supplier, SupplierBasicModel>(supplierHasher).IncludeAllDerived();
        CreateMap<Supplier, SupplierListItem>().IncludeAllDerived()
            .ForMember(m => m.PurchaseCount, o => o.MapFrom(e => e.Purchases.Count()));

        CreateMap<PurchaseRequest, Purchase>()
            .ForMember(e => e.SupplierId, o => o.MapFrom(m => m.SupplierId != null ? supplierHasher.Decode(m.SupplierId) :  default(int?)));
        CreateEntityMap<Purchase, PurchaseModel>(purchaseHasher).IncludeAllDerived()
            .ForMember(m => m.SupplierId, o => o.MapFrom(e => e.SupplierId.HasValue ? supplierHasher.Encode(e.SupplierId.Value) : default));
        CreateEntityMap<Purchase, PurchaseBasicModel>(purchaseHasher).IncludeAllDerived()
            .ForMember(m => m.ItemCount, o => o.MapFrom(e => e.Items!.Count()));
        CreateEntityMap<Purchase, PurchaseListItem>(purchaseHasher);

        CreateMap<PurchaseItemRequest, PurchaseItem>()
            .ForMember(e => e.PurchaseId, o => o.MapFrom(m => purchaseHasher.Decode(m.PurchaseId)))
            .ForMember(e => e.StuffId, o => o.MapFrom(m => m.StuffId != null ? stuffHasher.Decode(m.StuffId) :  default(int?)));
        CreateEntityMap<PurchaseItem, PurchaseItemModel>(purchaseItemHasher).IncludeAllDerived()
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseHasher.Encode(e.PurchaseId)))
            .ForMember(m => m.StuffId, o => o.MapFrom(e => stuffHasher.Encode(e.StuffId))); ;
        CreateEntityMap<PurchaseItem, PurchaseItemListItem>(purchaseItemHasher).IncludeAllDerived()
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseHasher.Encode(e.PurchaseId)));
        CreateEntityMap<PurchaseItem, PurchaseItemSupplier>(purchaseItemHasher).IncludeAllDerived();

        CreateEntityMap<Upload, UploadModel>(uploadHasher).IncludeAllDerived();
    }

    private IMappingExpression<TEntity, TModel> CreateEntityMap<TEntity, TModel>(IIdentityHasher<TEntity> hasher)
        where TEntity : Entity 
        where TModel : class, IModel => CreateMap<TEntity, TModel>().ForMember(m => m.Id, o => o.MapFrom(e => hasher.Encode(e.Id)));
}

public static class MapperProfileExtensions
{
}