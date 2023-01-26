using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface IPurchaseItemModel
{
    string Id { get; set; }
    string PurchaseId { get; set; }
    StuffModel Stuff { get; set; }
    int Quantity { get; set; }
    decimal? Cost { get; set; }
}

public class PurchaseItemCreateModel
{
    [Required]
    public required string PurchaseId { get; set; }

    [Required(ErrorMessage = "Stuff is required.")]
    public string? StuffId { get; set; }

    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal? Cost { get; set; }
    
    public string? Notes { get; set; }
}

public class PurchaseItemModel : PurchaseItemCreateModel, IPurchaseItemModel
{
    public required string Id { get; set; }
    public required StuffModel Stuff { get; set; }

    public PurchaseItemCreateModel ToCreate()
    {
        return new PurchaseItemCreateModel
        {
            StuffId = StuffId,
            PurchaseId = PurchaseId,
            Quantity = Quantity,
            Cost = Cost,
            Notes = Notes,
        };
    }
}

public class PurchaseItemListItem: IPurchaseItemModel
{
    public required string Id { get; set; }
    public required string PurchaseId { get; set; }
    public required StuffModel Stuff { get; set; }
    public int Quantity { get; set; }
    public decimal? Cost { get; set; }
}