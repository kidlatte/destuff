namespace Destuff.Server.Models;

public class UploadCreateModel
{
    public IFormFile? File { get; set; }
    public string? StuffId { get; set; }
    public string? LocationId { get; set; }
    public string? PurchaseId { get; set; }
    public string? EventId { get; set; }
}
