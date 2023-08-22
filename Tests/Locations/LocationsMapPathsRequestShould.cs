namespace Destuff.Tests.Locations;

public class LocationsMapPathsRequestShould : IntegrationTestBase
{
    public LocationsMapPathsRequestShould() : base(HttpMethod.Put, ApiRoutes.LocationMap)
    {
    }

    [Fact]
    public async Task Update_Location()
    {
        // Arrange
        var parentCreate = new LocationRequest { Name = "Parent Location" };
        var parent = await AuthorizedSendAsync<LocationModel>(parentCreate, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(parent);

        var childCreate = new LocationRequest { ParentId = parent.Id, Name = "Child Location" };
        var child = await AuthorizedSendAsync<LocationModel>(childCreate, HttpMethod.Post, ApiRoutes.Locations);
        Assert.NotNull(child);

        // Act & Assert
        await AuthorizedSendAsync();

        parent = await AuthorizedGetAsync<LocationModel>($"{ApiRoutes.LocationBySlug}/{parent.Slug}");
        Assert.NotNull(parent?.Data?.Path);
        Assert.False(parent.Data.Path.Any());
        
        child = await AuthorizedGetAsync<LocationModel>($"{ApiRoutes.LocationBySlug}/{child.Slug}");
        Assert.NotNull(child?.Data?.Path);
        Assert.Single(child.Data.Path);
        Assert.Equal(parent.Id, child.Data.Path.First().Id);
    }

}