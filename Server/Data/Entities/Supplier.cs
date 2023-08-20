using Destuff.Server.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

[Index(nameof(Slug), IsUnique = true)]
public class Supplier : Entity, ISluggable
{
    [Required, MaxLength(255)]
    public required string Slug { get; set; }

    [Required, MaxLength(255)]
    public required string ShortName { get; set; }

    [Required, MaxLength(255)]
    public required string Name { get; set; }

    [MaxLength(1023)]
    public string? Url { get; set; }

    [MaxLength(255)]
    public string? Phone { get; set; }

    [MaxLength(255)]
    public string? Address { get; set; }

    public string? Notes { get; set; }

    public int PurchaseCount { get; set; }

    public required ICollection<Purchase> Purchases { get; set; }

    public string ToSlug() => ShortName.ToSlug();

}