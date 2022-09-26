namespace Destuff.Server.Data.Entities;

public class Purchase : Entity
{
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public string? Notes { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<PurchaseItem>? Items { get; set; }
    public ICollection<Image>? Images { get; set; }
}

public class PurchaseItem : EventItem
{
    public int Quantity { get; set; }
    public decimal Cost { get; set; }

    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
}
