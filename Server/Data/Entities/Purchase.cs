using System.ComponentModel.DataAnnotations.Schema;

namespace Destuff.Server.Data.Entities;

public class Purchase : Entity
{
    public DateTime? Receipt { get; set; }
    public DateTime? Received { get; set; }
    public decimal Price { get; set; }
    public string? Notes { get; set; }

    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }

    public ICollection<PurchaseItem>? Items { get; set; }
    public ICollection<Upload>? Uploads { get; set; }
}

public class PurchaseItem : EventItem
{
    public int Quantity { get; set; }
    
    public decimal Price { get; set; }

    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
}
