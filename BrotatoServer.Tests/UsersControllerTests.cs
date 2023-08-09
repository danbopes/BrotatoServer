using BrotatoServer.Controllers;
using BrotatoServer.Data;
using BrotatoServer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using BrotatoServer.Models.DB;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace BrotatoServer.Tests;

public class UsersControllerTests
{
    private ControllerContext GetMockControllerContext(long? byteLen = 3)
    {
        var byteArray = new byte[byteLen ?? 3];
        var memoryStream = new MemoryStream(byteArray);
        memoryStream.Flush();
        memoryStream.Position = 0;
        
        var mockHttpContext = Substitute.For<HttpContext>();
        mockHttpContext
            .Request.Body.Returns(memoryStream);
        
        mockHttpContext
            .Request.Headers.Returns(new HeaderDictionary()
            {
                ContentLength = byteLen
            });
        
        //var mockRequest = new Mock<HttpRequest>();
        //mockRequest.Setup(x => x.Body).Returns(memoryStream);

        return new ControllerContext(new ActionContext
        {
            HttpContext = mockHttpContext,
            RouteData = new RouteData(),
            ActionDescriptor = new ControllerActionDescriptor()
        });
    }
    
    private (IServiceProvider serviceProvider, ISteamworksService serviceMock) GetMockServiceProvider(bool authReturn = true)
    {
        var steamworksMock = Substitute.For<ISteamworksService>();
        
        steamworksMock
            .AuthenticateSessionAsync(Arg.Any<byte[]>(), 1)
            .Returns(Task.FromResult(authReturn));

        var provider = DITools.CreateDefaultServiceProvider(services =>
        {
            services
                .AddSingleton(steamworksMock);
        });

        return (provider, steamworksMock);
    }
    
    [Fact]
    public async Task ShouldCreateAndGetApiKeyForUser()
    {
        // Arrange
        var (provider, steamworksMock) = GetMockServiceProvider();

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

        await steamworksMock
            .Received(1)
            .AuthenticateSessionAsync(Arg.Is<byte[]>(arg => arg.SequenceEqual(new byte[] {0, 0, 0})), 1);
        
        var user = await db.Users.SingleAsync();
        Assert.Equal(guid, user.ApiKey);
        Assert.Equal(1u, user.SteamId);
    }
    
    
    [Fact]
    public async Task ShouldReturnExistingApiKeyForUser()
    {
        // Arrange
        var (provider, steamworksMock) = GetMockServiceProvider();

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
        
        await steamworksMock
            .Received(1)
            .AuthenticateSessionAsync(Arg.Is<byte[]>(arg => arg.SequenceEqual(new byte[] {0, 0, 0})), 1);

        var user = await db.Users.SingleAsync();
        Assert.Equal(guid, user.ApiKey);
        Assert.Equal(1u, user.SteamId);
    }


    [Fact]
    public async Task ShouldReturnAuthFailure()
    {
        // Arrange
        var (provider, steamworksMock) = GetMockServiceProvider(false);

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
        var steamworksMock = Substitute.For<ISteamworksService>();
        
        steamworksMock
            .AuthenticateSessionAsync(Arg.Any<byte[]>(), 1)
            .Returns(Task.FromResult(true));

        var provider = DITools.CreateDefaultServiceProvider(services =>
        {
            services
                .AddSingleton(steamworksMock);
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