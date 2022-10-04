using Microsoft.AspNetCore.StaticFiles;

namespace Destuff.Server.Services;

public interface IFileService
{
    string GetContentType(string fileName);
    Task<string> Save(IFormFile file);
    void Delete(string path);
}

public class FileService : IFileService
{
    string DataPath { get; }
    FileExtensionContentTypeProvider ContentType { get; }

    public FileService(string dataPath)
    {
        DataPath = dataPath;
        ContentType = new FileExtensionContentTypeProvider();
    }

    public string GetContentType(string fileName) => 
        ContentType.TryGetContentType(fileName, out string? value) ? 
            value : "application/octet-stream";

    public async virtual Task<string> Save(IFormFile file)
    {
        var path = Path.Combine(DataPath, "uploads", DateTime.UtcNow.ToString("yyyyMMdd"));
        Directory.CreateDirectory(path);

        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString().Substring(0, 5)}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(path, fileName);

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.CopyToAsync(fs);

        return filePath;
    }

    public virtual void Delete(string path) => System.IO.File.Delete(path);
}