using Destuff.Shared.Models;

namespace Destuff.Shared;

public static class ApiRoutes
{
    public const string Auth = "/api/auth";
    public const string AuthLogin = "/api/auth/login";
    public const string AuthRegister = "/api/auth/register";
    public const string AuthChangePassword = "/api/auth/change-password";
    public const string Users = "/api/users";
    public const string UserSettings = "/api/users/me";

    public const string Dashboard = "/api/dashboard";

    public const string Stuffs = "/api/stuffs";
    public const string StuffBySlug = "/api/stuffs/s";
    public const string StuffForInventory = "/api/stuffs/inventory";
    public const string StuffScrapeUrl = "/api/stuffs/scrape";
    public const string StuffLocations = "/api/stuffs/locations";
    public const string StuffParts = "/api/stuffs/parts";
    public const string StuffParents = "/api/stuffs/parents";

    public const string StuffsBySupplier = "/api/supplier/{hash}/stuffs";
    public static string QueryStuffsBySupplier(string supplierId, ListQuery query)
        => StuffsBySupplier.Replace("{hash}", supplierId) + $"?{query}";

    public const string Locations = "/api/locations";
    public const string LocationBySlug = "/api/locations/s";
    public const string LocationTree = "/api/locations/t";
    public const string LocationLookup = "/api/locations/lookup";
    public const string LocationMap = "/api/locations/map";
    public const string LocationOrderUp = "/api/locations/up";
    public const string LocationOrderDown = "/api/locations/down";

    public const string Uploads = "/api/uploads";
    public const string UploadImage = "/api/uploads/i";
    public const string UploadFiles = "/files";

    public const string Suppliers = "/api/suppliers";
    public const string SupplierBySlug = "/api/suppliers/s";

    public const string Purchases = "/api/purchases";
    public const string PurchasesBySupplier = "/api/purchases/supplier";

    public const string PurchaseItems = "/api/purchases/items";
    public const string PurchaseItemsByStuff = "/api/purchases/items/stuff";

    public const string Maintenances = "/api/maintenances";
    public const string MaintenancesByStuff = "/api/stuffs/{hash}/maintenances";
    public static string QueryMaintenancesByStuff(string stuffId, ListQuery query)
        => MaintenancesByStuff.Replace("{hash}", stuffId) + $"?{query}";

    public const string MaintenanceLogs = "/api/maintenances/logs";
    public const string MaintenanceLogsByMaintenance = "/api/maintenances/{hash}/logs";
    public static string QueryMaintenanceLogsByMaintenance(string maintenanceId)
        => MaintenanceLogsByMaintenance.Replace("{hash}", maintenanceId);

    public const string Events = "/api/events";
    public const string EventsByStuff = "/api/events/stuff";

}