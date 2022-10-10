namespace Destuff.Shared.Models;

public class PurchaseCreateModel
{
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public string? Notes { get; set; }

    public string? SupplierId { get; set; }
}

public class PurchaseModel : PurchaseCreateModel
{
    public string? Id { get; set; }
    public SupplierModel? Supplier { get; set; }
}

public class PurchaseListModel
{
    public string? Id { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public SupplierModel? Supplier { get; set; }
}