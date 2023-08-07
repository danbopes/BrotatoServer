using BrotatoServer.Models;
using BrotatoServer.Models.DB;

namespace BrotatoServer.Utilities;

public static class HttpContextExtensions
{
    public static User GetUser(this HttpContext context)
    {
        return context.Items["User"] as User ?? throw new UnauthorizedAccessException("No user in context");
    }
}