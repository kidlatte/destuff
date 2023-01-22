using System.ComponentModel.DataAnnotations;

public class Setting
{
    [Key]
    public required string Key { get; set; }

    public string? Value { get; set; }
}
