namespace Destuff.Server.Services;

public interface IFileService
{
    Task<string> Save(IFormFile file);
}

public class FileService : IFileService
{
    string DataPath { get; }

    public FileService(string dataPath)
    {
        DataPath = dataPath;
    }

    public async Task<string> Save(IFormFile file)
    {
        var path = Path.Combine(DataPath, "uploads", DateTime.UtcNow.ToString("yyyyMMdd"));
        Directory.CreateDirectory(path);

        var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}-{Guid.NewGuid().ToString().Substring(0, 5)}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(path, fileName);

        await using FileStream fs = new(filePath, FileMode.Create);
        await file.CopyToAsync(fs);

        return filePath;
    }
}