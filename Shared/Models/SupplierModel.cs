using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface ISupplierModel
{
    string Id { get; set; }
    string? Name { get; set; }
    string? ShortName { get; set; }
}

public class SupplierRequest
{
    [Required]
    [MaxLength(255)]
    public string? ShortName { get; set; }

    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(1023)]
    public string? Url { get; set; }

    [MaxLength(255)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Notes { get; set; }
}

public class SupplierModel : SupplierRequest, ISupplierModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }

    public SupplierRequest ToRequest()
    {
        return new SupplierRequest
        {
            ShortName = ShortName,
            Name = Name,
            Url = Url,
            Phone = Phone,
            Address = Address,
            Notes = Notes,
        };
    }
}

public class SupplierListItem: ISupplierModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }

    public string? ShortName { get; set; }
    public string? Name { get; set; }
    public string? Url { get; set; }
    public string? Phone { get; set; }
}