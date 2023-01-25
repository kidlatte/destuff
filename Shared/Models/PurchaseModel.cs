namespace Destuff.Shared.Models;

public interface IPurchaseModel
{
    string Id { get; set; }
    DateTime? Receipt { get; set; }
    DateTime? Received { get; set; }
    SupplierModel? Supplier { get; set; }
}

public class PurchaseCreateModel
{
    public string? SupplierId { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public string? Notes { get; set; }
}

public class PurchaseModel : PurchaseCreateModel, IPurchaseModel
{
    public required string Id { get; set; }
    public SupplierModel? Supplier { get; set; }

    public PurchaseCreateModel ToCreate()
    {
        return new PurchaseCreateModel
        {
            SupplierId = SupplierId,
            Receipt = Receipt,
            Received = Received,
            Notes = Notes,
        };
    }
}

public class PurchaseListModel: IPurchaseModel
{
    public required string Id { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public SupplierModel? Supplier { get; set; }
}