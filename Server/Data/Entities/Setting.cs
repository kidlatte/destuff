using System.ComponentModel.DataAnnotations;

public class Setting
{
    [Key]
    public string Key { get; set; } = null!;

    public string? Value { get; set; }
}
