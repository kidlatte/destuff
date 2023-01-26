using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class PurchaseItemCreateModel
{
    [Required]
    public int? Quantity { get; set; }
    
    [Required]
    public decimal? Cost { get; set; }
    
    public string? Notes { get; set; }

    [Required(ErrorMessage = "Stuff is required.")]
    public string? StuffId { get; set; }

    [Required]
    public string? PurchaseId { get; set; }
}

public class PurchaseItemModel : PurchaseItemCreateModel
{
    public string? Id { get; set; }
    public StuffModel? Stuff { get; set; }
}

public class PurchaseItemListItem
{
    public string? Id { get; set; }
    public StuffModel? Stuff { get; set; }
    public int Quantity { get; set; }
    public decimal Cost { get; set; }
}