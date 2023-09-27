using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Destuff.Shared;
using Destuff.Shared.Models;

namespace Destuff.Tests.Stuffs;

public class StuffsGetForInventoryRequestShould : IntegrationTestBase
{
    public StuffsGetForInventoryRequestShould() : base(HttpMethod.Get, ApiRoutes.StuffForInventory)
    {
    }

    [Fact]
    public async Task Get_Locations_List()
    {
        // Arrange
        var stuff = await AuthorizedSendAsync<StuffModel>(new StuffRequest { Name = "Stuff 001" }, HttpMethod.Post, ApiRoutes.Stuffs);
        Assert.NotNull(stuff);

        // Act
        var result = await AuthorizedSendAsync<StuffModel>();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(stuff.Name, result.Name);
    }
}