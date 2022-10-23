using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

public interface ISupplierModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}

public class SupplierCreateModel
{
    [Required]
    [MaxLength(255)]
    public string? ShortName { get; set; }

    [Required]
    [MaxLength(255)]
    public string? Name { get; set; }

    [MaxLength(255)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Notes { get; set; }
}

public class SupplierModel : SupplierCreateModel, ISupplierModel
{
    public string? Id { get; set; }

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
    public string? Id { get; set; }
    public string? ShortName { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }

    public string? Slug => ShortName?.ToLower();    
}