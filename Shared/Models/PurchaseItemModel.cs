using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class PurchaseItemCreateModel
{
    public int Quantity { get; set; }
    public decimal Cost { get; set; }

    public DateTime DateTime { get; set; }
    public int Count { get; set; }
    public string? Summary { get; set; }
    public string? Notes { get; set; }

    [Required]
    public string? StuffId { get; set; }

    [Required]
    public string? PurchaseId { get; set; }
}

public class PurchaseItemModel : PurchaseItemCreateModel
{
    public string? Id { get; set; }
    public StuffModel? Stuff { get; set; }
}

public class PurchaseItemListModel
{
    public string? Id { get; set; }
    public StuffModel? Stuff { get; set; }
    public int Quantity { get; set; }
    public decimal Cost { get; set; }
}