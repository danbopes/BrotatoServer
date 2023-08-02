using BrotatoServer.Models.JSON;

namespace BrotatoServer.Models;

public class Run
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public string RunData { get; set; }
}

public class FullRun
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public RunInformation RunData { get; set; } = new();
}