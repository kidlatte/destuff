using Microsoft.AspNetCore.StaticFiles;
using SixLabors.ImageSharp.Formats.Webp;

namespace Destuff.Server.Services;

public interface IFileService
{
    string GetContentType(string fileName);
    Task<string> SaveImage(IFormFile file);
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

        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString()[..5]}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(path, fileName);

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.CopyToAsync(fs);

        return filePath;
    }

    public async virtual Task<string> SaveImage(IFormFile file)
    {
        int width = 1080, height = 720;

        if (file.ContentType == null || !file.ContentType.Contains("image"))
            return await Save(file);

        using var image = Image.Load(file.OpenReadStream());
        var compress = image.Width > width || image.Height > height;

        var path = Path.Combine(DataPath, "uploads", DateTime.UtcNow.ToString("yyyyMMdd"));
        Directory.CreateDirectory(path);

        var ext = compress ? ".webp" : Path.GetExtension(file.FileName);
        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString()[..5]}{ext}";
        var filePath = Path.Combine(path, fileName);

        if (compress)
        {
            if (image.Height > height)
                image.Mutate(x => x.Resize(0, height, KnownResamplers.Lanczos3));
            else if (image.Width > width)
                image.Mutate(x => x.Resize(width, 0, KnownResamplers.Lanczos3));

            await image.SaveAsync(filePath, new WebpEncoder { FileFormat = WebpFileFormatType.Lossy });
        }
        else
            await image.SaveAsync(filePath);

        return filePath;
    }

    public virtual void Delete(string path)
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}