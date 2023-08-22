namespace Destuff.Shared;

public class ApiRoutes
{
    public const string Users = "/api/users";
    public const string Auth = "/api/auth";
    public const string AuthLogin = "/api/auth/login";
    public const string AuthRegister = "/api/auth/register";
    public const string AuthChangePassword = "/api/auth/change-password";

    public const string Locations = "/api/locations";
    public const string LocationBySlug = "/api/locations/s";
    public const string LocationTree = "/api/locations/t";
    public const string LocationLookup = "/api/locations/lookup";
    public const string LocationMap = "/api/locations/map";

    public const string Stuffs = "/api/stuffs";
    public const string StuffBySlug = "/api/stuffs/s";
    public const string StuffsBySupplier = "/api/stuffs/supplier";
    public const string StuffLocations = "/api/stuff-locations";

    public const string Uploads = "/api/uploads";
    public const string UploadImage = "/api/uploads/i";
    public const string UploadFiles = "/files";

    public const string Suppliers = "/api/suppliers";
    public const string SupplierBySlug = "/api/suppliers/s";

    public const string Purchases = "/api/purchases";
    public const string PurchasesBySupplier = "/api/purchases/supplier";

    public const string PurchaseItems = "/api/purchase-items";
    public const string PurchaseItemsByStuff = "/api/purchase-items/stuff";

    public const string Events = "/api/events";
    public const string EventsByStuff = "/api/events/stuff";
    public const string Inventories = "/api/inventories";
    public const string InventoryStuff = "/api/inventory";
}