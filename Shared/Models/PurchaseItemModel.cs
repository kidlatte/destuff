using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface IPurchaseItemModel : IModel
{
    string PurchaseId { get; set; }
    StuffModel Stuff { get; set; }
    int Quantity { get; set; }
    decimal? Price { get; set; }
    string? Notes { get; set; }

    PurchaseItemRequest ToRequest();
}

public class PurchaseItemRequest : IRequest
{
    [Required]
    public required string PurchaseId { get; set; }

    [Required(ErrorMessage = "Stuff is required.")]
    public string? StuffId { get; set; }

    [Required]
    public int Quantity { get; set; }
    
    [Required]
    public decimal? Price { get; set; }
    
    public string? Notes { get; set; }
}

public class PurchaseItemModel : PurchaseItemRequest, IPurchaseItemModel
{
    public required string Id { get; set; }
    public required StuffModel Stuff { get; set; }

    public PurchaseItemRequest ToRequest()
    {
        return new PurchaseItemRequest
        {
            StuffId = StuffId,
            PurchaseId = PurchaseId,
            Quantity = Quantity,
            Price = Price,
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
    public decimal? Price { get; set; }
    public string? Notes { get; set; }

    public PurchaseItemRequest ToRequest()
    {
        return new PurchaseItemRequest
        {
            StuffId = Stuff.Id,
            PurchaseId = PurchaseId,
            Quantity = Quantity,
            Price = Price,
            Notes = Notes,
        };
    }
}

public class PurchaseItemSupplier : IModel
{
    public required string Id { get; set; }
    public DateTime DateTime { get; set; }
    public int Quantity { get; set; }
    public decimal? Price { get; set; }

    public required PurchaseListItem Purchase { get; set; }
}

public class PurchaseItemBasicModel
{
    public required string PurchaseId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}