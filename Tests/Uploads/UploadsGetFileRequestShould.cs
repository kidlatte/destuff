using System.IO;

namespace Destuff.Tests.Uploads;

public class UploadsGetFileRequestShould : IntegrationTestBase
{
    public UploadsGetFileRequestShould() : base(HttpMethod.Get, ApiRoutes.UploadFiles)
    {
    }

    [Fact]
    public async Task Get_UploadFile()
    {
        // Arrange

        var filePath = Path.Combine("Resources", "box.png");
        using var file = File.OpenRead(filePath);
        var fileContent = new StreamContent(file);

        using var content = new MultipartFormDataContent();
        content.Add(fileContent, "file", "box.png");

        var request = new HttpRequestMessage(HttpMethod.Post, ApiRoutes.Uploads);
        request.Content = content;

        var response = await AuthorizedSendAsync(request);
        var model = await response.Content.ReadFromJsonAsync<UploadModel>();
        Assert.NotNull(model?.Url);

        // Act
        var result = await Http.GetAsync(model?.Url);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessStatusCode);
    }
}