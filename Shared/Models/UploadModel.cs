namespace Destuff.Shared.Models;

public class UploadModel : IModel
{
    public required string Id { get; set; }
    public required string FileName { get; set; }
    public string Url => $"{ApiRoutes.UploadFiles}/{Id}/{FileName}";
}
