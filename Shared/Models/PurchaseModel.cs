namespace Destuff.Shared.Models;

public interface IPurchaseModel
{
    string Id { get; set; }
    DateTime? Receipt { get; set; }
    DateTime? Received { get; set; }
    SupplierListItem? Supplier { get; set; }
}

public class PurchaseRequest : IRequest
{
    public string? SupplierId { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public string? Notes { get; set; }
}

public class PurchaseModel : PurchaseRequest, IModel, IPurchaseModel
{
    public required string Id { get; set; }
    public decimal Price { get; set; }
    public SupplierListItem? Supplier { get; set; }

    public PurchaseRequest ToRequest()
    {
        return new PurchaseRequest
        {
            SupplierId = SupplierId,
            Receipt = Receipt,
            Received = Received,
            Notes = Notes,
        };
    }
}

public class PurchaseListItem: IPurchaseModel
{
    public required string Id { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public decimal Price { get; set; }
    public SupplierListItem? Supplier { get; set; }
}

public class PurchaseBasicModel
{
    public required string Id { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public decimal Price { get; set; }
}
