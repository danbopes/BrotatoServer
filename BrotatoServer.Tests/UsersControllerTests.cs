using BrotatoServer.Controllers;
using BrotatoServer.Data;
using BrotatoServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Web;
using BrotatoServer.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BrotatoServer.Tests;

public class UsersControllerTests
{
    private ControllerContext GetMockControllerContext(long? byteLen = 3)
    {
        /*
        var len = (int?)Request.Headers.ContentLength;

        if (len is null or <= 0 or > 2048)
            return BadRequest();

        var ticketBuffer = new byte[len.Value];

        await Request.Body.ReadExactlyAsync(ticketBuffer, 0, len.Value);
        */

        var byteArray = new byte[byteLen ?? 3];
        var memoryStream = new MemoryStream(byteArray);
        memoryStream.Flush();
        memoryStream.Position = 0;
        
        var mockHttpContext = new Mock<HttpContext>();
        mockHttpContext
            .Setup(x => x.Request.Body)
            .Returns(memoryStream);
        mockHttpContext
            .Setup(x => x.Request.Headers)
            .Returns(new HeaderDictionary()
            {
                ContentLength = byteLen
            });
        //var mockRequest = new Mock<HttpRequest>();
        //mockRequest.Setup(x => x.Body).Returns(memoryStream);

        return new ControllerContext(new ActionContext
        {
            HttpContext = mockHttpContext.Object,
            RouteData = new RouteData(),
            ActionDescriptor = new ControllerActionDescriptor()
        });
    }
    
    [Fact]
    public async Task ShouldCreateAndGetApiKeyForUser()
    {
        // Arrange
        var steamworksMock = new Mock<ISteamworksService>();
        steamworksMock
            .Setup(service => service.AuthenticateSessionAsync(It.IsAny<byte[]>(), 1))
            .ReturnsAsync(() => true);

        var provider = DITools.CreateDefaultServiceProvider(services =>
        {
            services
                .AddSingleton(steamworksMock.Object);
        });

        await using var scope = provider.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<BrotatoServerContext>();
        var usersController = scope.ServiceProvider.GetRequiredService<UsersController>();
        usersController.ControllerContext = GetMockControllerContext();
        
        // Act
        var result = await usersController.GetApiKeyForUser(1);
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
        
        var okObjectResult = (OkObjectResult) result;

        Assert.IsType<Guid>(okObjectResult.Value);
        var guid = (Guid) okObjectResult.Value;

        steamworksMock
            .Verify(x => x.AuthenticateSessionAsync(new byte[] {0, 0, 0}, 1), Times.Once);
        
        var user = await db.Users.SingleAsync();
        Assert.Equal(guid, user.ApiKey);
        Assert.Equal(1u, user.SteamId);
    }
    
    
    [Fact]
    public async Task ShouldReturnExistingApiKeyForUser()
    {
        // Arrange
        var steamworksMock = new Mock<ISteamworksService>();
        steamworksMock
            .Setup(service => service.AuthenticateSessionAsync(It.IsAny<byte[]>(), 1))
            .ReturnsAsync(() => true);

        var provider = DITools.CreateDefaultServiceProvider(services =>
        {
            services
                .AddSingleton(steamworksMock.Object);
        });

        await using var scope = provider.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<BrotatoServerContext>();
        var userGuid = Guid.NewGuid();

        var newUser = db.Users.Add(new User
        {
            ApiKey = userGuid,
            SteamId = 1u
        });
        await db.SaveChangesAsync();
        newUser.State = EntityState.Detached;
        
        var usersController = scope.ServiceProvider.GetRequiredService<UsersController>();
        usersController.ControllerContext = GetMockControllerContext();
        
        // Act
        var result = await usersController.GetApiKeyForUser(1);
        
        // Assert
        Assert.IsType<OkObjectResult>(result);
        
        var okObjectResult = (OkObjectResult) result;

        Assert.IsType<Guid>(okObjectResult.Value);
        var guid = (Guid) okObjectResult.Value;
        Assert.Equal(userGuid, guid);
        
        steamworksMock
            .Verify(x => x.AuthenticateSessionAsync(new byte[] {0, 0, 0}, 1), Times.Once);

        var user = await db.Users.SingleAsync();
        Assert.Equal(guid, user.ApiKey);
        Assert.Equal(1u, user.SteamId);
    }


    [Fact]
    public async Task ShouldReturnAuthFailure()
    {
        // Arrange
        var steamworksMock = new Mock<ISteamworksService>();
        steamworksMock
            .Setup(service => service.AuthenticateSessionAsync(It.IsAny<byte[]>(), 1))
            .ReturnsAsync(() => false);

        var provider = DITools.CreateDefaultServiceProvider(services =>
        {
            services
                .AddSingleton(steamworksMock.Object);
        });

        await using var scope = provider.CreateAsyncScope();

        var usersController = scope.ServiceProvider.GetRequiredService<UsersController>();
        usersController.ControllerContext = GetMockControllerContext();
        
        // Act
        var result = await usersController.GetApiKeyForUser(1);
        
        // Assert
        Assert.IsType<UnauthorizedResult>(result);
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData(0L)]
    [InlineData(2049L)]
    public async Task ShouldReturnBadRequestWhenBodyNotSpecifiedIsTooLarge(long? length)
    {
        // Arrange
        var steamworksMock = new Mock<ISteamworksService>();
        steamworksMock
            .Setup(service => service.AuthenticateSessionAsync(It.IsAny<byte[]>(), 1))
            .ReturnsAsync(() => true);

        var provider = DITools.CreateDefaultServiceProvider(services =>
        {
            services
                .AddSingleton(steamworksMock.Object);
        });

        await using var scope = provider.CreateAsyncScope();

        var usersController = scope.ServiceProvider.GetRequiredService<UsersController>();
        usersController.ControllerContext = GetMockControllerContext(length);
        
        // Act
        var result = await usersController.GetApiKeyForUser(1);
        
        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
}