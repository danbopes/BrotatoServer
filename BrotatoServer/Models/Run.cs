using BrotatoServer.Models.JSON;

namespace BrotatoServer.Models;

public class Run
{
    public Guid Id { get; set; }
    public ulong UserId { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public string RunInformation { get; set; }
    public virtual User? User { get; set; }
}

public class FullRun
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public RunData RunData { get; set; }
}