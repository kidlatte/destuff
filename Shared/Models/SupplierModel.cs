using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface ISupplierModel
{
    string Id { get; set; }
    string? Name { get; set; }
}

public class SupplierCreateModel
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

public class SupplierModel : SupplierCreateModel, ISupplierModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }

    public SupplierCreateModel ToCreate()
    {
        return new SupplierCreateModel
        {
            ShortName = ShortName,
            Name = Name,
            Phone = Phone,
            Address = Address,
            Notes = Notes,
        };
    }
}

public class SupplierListModel: ISupplierModel
{
    public required string Id { get; set; }
    public required string Slug { get; set; }

    public string? ShortName { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
}