using System.ComponentModel.DataAnnotations;

namespace Destuff.Shared.Models;

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

public class SupplierModel : SupplierCreateModel
{
    public string? Id { get; set; }
}

public class SupplierListModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
}