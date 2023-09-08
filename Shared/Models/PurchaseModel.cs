namespace Destuff.Shared.Models;

public interface IPurchaseModel : IModel
{
    DateTime? Receipt { get; set; }
    DateTime? Received { get; set; }
    SupplierBasicModel? Supplier { get; set; }
}

public class PurchaseRequest : IRequest
{
    public string? SupplierId { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public string? Notes { get; set; }
}

public class PurchaseModel : PurchaseRequest, IPurchaseModel
{
    public required string Id { get; set; }
    public decimal Price { get; set; }
    public int ItemCount { get; set; }
    public SupplierBasicModel? Supplier { get; set; }

    public required ICollection<UploadModel> Uploads { get; set; }

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

public class PurchaseBasicModel : IModel
{
    public required string Id { get; set; }
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public decimal Price { get; set; }
    public int ItemCount { get; set; }
}

public class PurchaseListItem: PurchaseBasicModel, IPurchaseModel
{
    public SupplierBasicModel? Supplier { get; set; }
}
