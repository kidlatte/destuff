using System.ComponentModel.DataAnnotations;

namespace Destuff.Server.Data.Entities;

public abstract class Entity
{
    [Key]
    public int Id { get; set; }

    [MaxLength(255)]
    public string? CreatedBy { get; set; }

    public DateTime Created { get; set; }

    public DateTime Updated { get; set; }
}

public interface ISluggable
{
    string Slug { get; set; }

    string ToSlug();
}
