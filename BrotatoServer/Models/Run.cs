namespace BrotatoServer.Models;

public class Run
{
    public Guid Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public bool CurrentRotation { get; set; }
    public string RunData { get; set; }
}
