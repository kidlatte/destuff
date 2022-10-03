using System.IO;
using Destuff.Server.Services;
using Microsoft.AspNetCore.Http;

namespace Destuff.Tests.Services;

public class TestFileService : IFileService
{
    public async Task<string> Save(IFormFile file)
    {
        var dataPath = Directory.GetCurrentDirectory();
        var path = Path.Combine(dataPath, "uploads", DateTime.UtcNow.ToString("yyyyMMdd"));
        Directory.CreateDirectory(path);

        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString().Substring(0, 5)}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(path, fileName);

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.CopyToAsync(fs);

        return filePath;
    }
}