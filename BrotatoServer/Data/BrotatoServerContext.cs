using Microsoft.EntityFrameworkCore;
using BrotatoServer.Models;

namespace BrotatoServer.Data
{
    public class BrotatoServerContext : DbContext
    {
        public BrotatoServerContext (DbContextOptions<BrotatoServerContext> options)
            : base(options)
        {
        }

        public DbSet<Run> Run { get; set; } = default!;
    }
}
