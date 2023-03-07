using Microsoft.AspNetCore.StaticFiles;
using System.IO;
using System.Net.Http.Headers;

namespace Destuff.Tests.Uploads;

public class UploadsPostRequestShould : IntegrationTestBase
{
    public UploadsPostRequestShould() : base(HttpMethod.Post, ApiRoutes.Uploads)
    {
    }

    [Fact]
    public async Task Upload_UploadFile()
    {
        // Arrange
        var fileName = "box.png";
        var filePath = Path.Combine("Resources", fileName);
        using var file = File.OpenRead(filePath);
        var fileContent = new StreamContent(file);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName);

        var request = new HttpRequestMessage(Method, ApiRoutes.Uploads);
        request.Content = content;

        // Act
        var result = await AuthorizedSendAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Upload_Image()
    {
        // Arrange
        var fileName = "office.jpg";
        var filePath = Path.Combine("Resources", fileName);
        using var file = File.OpenRead(filePath);
        var fileContent = new StreamContent(file);

        var provider = new FileExtensionContentTypeProvider();
        var contentType = provider.TryGetContentType(fileName, out string? value) ? 
            value : "application/octet-stream";
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName);

        var request = new HttpRequestMessage(Method, ApiRoutes.UploadImage);
        request.Content = content;

        // Act
        var result = await AuthorizedSendAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }

    [Fact]
    public async Task Create_StuffUpload()
    {
        // Arrange
        var stuffCreate = new StuffRequest { Name = "Stuff 001" };
        var stuff = await AuthorizedSendAsync<StuffModel>(stuffCreate, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff?.Id);

        var fileName = "box.png";
        var filePath = Path.Combine("Resources", fileName);
        using var file = File.OpenRead(filePath);
        var fileContent = new StreamContent(file);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(stuff.Id), "StuffId");

        var request = new HttpRequestMessage(Method, Route);
        request.Content = content;

        // Act
        var result = await AuthorizedSendAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}