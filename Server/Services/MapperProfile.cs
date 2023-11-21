using AutoMapper;
using Destuff.Server.Data.Entities;
using Destuff.Server.Models;
using Destuff.Shared.Models;

namespace Destuff.Server.Services;

public class MapperProfile : Profile
{
    public MapperProfile(
        IIdentityHasher<Location> locationHasher, 
        IIdentityHasher<Stuff> stuffHasher, 
        IIdentityHasher<Supplier> supplierHasher, 
        IIdentityHasher<Purchase> purchaseHasher, 
        IIdentityHasher<PurchaseItem> purchaseItemHasher,
        IIdentityHasher<Upload> uploadHasher,
        IIdentityHasher<Event> eventHasher)
    {
        CreateMap<LocationRequest, Location>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => locationHasher.Decode(m.ParentId)));
        CreateEntityMap<Location, LocationModel>(locationHasher).IncludeAllDerived()
            .ForMember(m => m.ParentId, o => o.MapFrom(e => locationHasher.Encode(e.ParentId)));
        CreateEntityMap<Location, LocationListItem>(locationHasher).IncludeAllDerived()
            .ForMember(m => m.PathString, o => o.MapFrom(e => e.Data!.PathString));
        CreateMap<Location, LocationTreeItem>()
            .ForMember(m => m.ParentId, o => o.MapFrom(e => locationHasher.Encode(e.ParentId)))
            .ForMember(m => m.Children, o => o.Ignore());
        CreateMap<Location, LocationLookupItem>();
        CreateMap<LocationListItem, LocationLookupItem>();

        CreateMap<StuffRequest, Stuff>();
        CreateEntityMap<Stuff, StuffBasicModel>(stuffHasher).IncludeAllDerived();
        CreateMap<Stuff, StuffModel>();
        CreateMap<Stuff, StuffListItem>();

        CreateMap<StuffLocationRequest, StuffLocation>()
            .ForMember(e => e.StuffId, o => o.MapFrom(m => stuffHasher.Decode(m.StuffId)))
            .ForMember(e => e.LocationId, o => o.MapFrom(m => locationHasher.Decode(m.LocationId)));
        CreateMap<StuffLocation, StuffLocationModel>();
        CreateMap<StuffLocation, StuffLocationListItem>();

        CreateMap<StuffPartRequest, StuffPart>()
            .ForMember(e => e.ParentId, o => o.MapFrom(m => stuffHasher.Decode(m.ParentId)))
            .ForMember(e => e.PartId, o => o.MapFrom(m => stuffHasher.Decode(m.PartId)));
        CreateMap<StuffPart, StuffPartModel>();
        CreateMap<StuffPart, StuffPartListItem>();

        CreateMap<SupplierRequest, Supplier>();
        CreateEntityMap<Supplier, SupplierModel>(supplierHasher).IncludeAllDerived();
        CreateEntityMap<Supplier, SupplierBasicModel>(supplierHasher).IncludeAllDerived();
        CreateMap<Supplier, SupplierListItem>();

        CreateMap<PurchaseRequest, Purchase>()
            .ForMember(e => e.SupplierId, o => o.MapFrom(m => supplierHasher.Decode(m.SupplierId)));
        CreateEntityMap<Purchase, PurchaseModel>(purchaseHasher).IncludeAllDerived()
            .ForMember(m => m.SupplierId, o => o.MapFrom(e => supplierHasher.Encode(e.SupplierId)));
        CreateEntityMap<Purchase, PurchaseBasicModel>(purchaseHasher).IncludeAllDerived();
        CreateMap<Purchase, PurchaseListItem>();

        CreateMap<PurchaseItemRequest, PurchaseItem>()
            .ForMember(e => e.PurchaseId, o => o.MapFrom(m => purchaseHasher.Decode(m.PurchaseId)))
            .ForMember(e => e.StuffId, o => o.MapFrom(m => stuffHasher.Decode(m.StuffId)));
        CreateEntityMap<PurchaseItem, PurchaseItemModel>(purchaseItemHasher).IncludeAllDerived()
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseHasher.Encode(e.PurchaseId)))
            .ForMember(m => m.StuffId, o => o.MapFrom(e => stuffHasher.Encode(e.StuffId)));
        CreateEntityMap<PurchaseItem, PurchaseItemListItem>(purchaseItemHasher).IncludeAllDerived()
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseHasher.Encode(e.PurchaseId)));
        CreateEntityMap<PurchaseItem, PurchaseItemSupplier>(purchaseItemHasher);
        CreateMap<PurchaseItem, PurchaseItemBasicModel>()
            .ForMember(m => m.PurchaseId, o => o.MapFrom(e => purchaseHasher.Encode(e.PurchaseId)));

        CreateEntityMap<Upload, UploadModel>(uploadHasher).IncludeAllDerived();

        CreateMap<EventRequest, Event>()
            .ForMember(e => e.StuffId, o => o.MapFrom(m => stuffHasher.Decode(m.StuffId)));
        CreateEntityMap<Event, EventModel>(eventHasher).IncludeAllDerived();
        CreateEntityMap<Event, EventListItem>(eventHasher).IncludeAllDerived();

        CreateMap<Event, EventBuffer>();
        CreateMap<PurchaseItem, EventBuffer>()
            .ForMember(m => m.Type, o => o.MapFrom(e => EventType.Purchased));
        CreateMap<EventBuffer, EventListItem>()
            .ForMember(m => m.Id, o => o.MapFrom(e => eventHasher.Encode(e.Id)));
    }

    private IMappingExpression<TEntity, TModel> CreateEntityMap<TEntity, TModel>(IIdentityHasher<TEntity> hasher)
        where TEntity : Entity 
        where TModel : class, IModel => CreateMap<TEntity, TModel>().ForMember(m => m.Id, o => o.MapFrom(e => hasher.Encode(e.Id)));
}

public static class MapperProfileExtensions
{
}