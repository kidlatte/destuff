using Xunit;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Uploads;

public class UploadsCreateRequestShould : IntegrationTestBase
{
    public UploadsCreateRequestShould() : base(HttpMethod.Post, ApiRoutes.Uploads)
    {
    }

    [Fact]
    public async Task Create_Upload()
    {
        // Arrange
        var filePath = Path.Combine("Resources", "box.png");
        using var file = File.OpenRead(filePath);
        var fileContent = new StreamContent(file);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", "box.png");

        var request = new HttpRequestMessage(Method, Route);
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
        var stuffCreate = new StuffCreateModel { Name = "Stuff 001" };
        var stuff = await AuthorizedSendAsync<StuffModel>(stuffCreate, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff?.Id);

        var filePath = Path.Combine("Resources", "box.png");
        using var file = File.OpenRead(filePath);
        var fileContent = new StreamContent(file);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", "box.png");
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