using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public class StuffPartRequest
{
    [Required]
    public string? ParentId { get; set; }

    [Required]
    public string? PartId { get; set; }
    public int Count { get; set; }
}

public class StuffPartListItem
{
    public required StuffListItem Part { get; set; }
    public int Count { get; set; }
}

public class StuffPartModel : StuffPartListItem
{
    public required StuffModel Parent { get; set; }
}
