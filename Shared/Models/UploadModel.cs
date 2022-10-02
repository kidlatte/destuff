namespace Destuff.Shared.Models;

public class UploadModel
{
    public string? Id { get; set; }
    public string? FileName { get; set; }
    public string? Url => Id == null ? null : $"/uploads/{Id}/{FileName}";
}
