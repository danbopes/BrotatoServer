using BrotatoServer.Models.JSON;

namespace BrotatoServer.Models.DB;

public class FullRun
{
    public required Guid Id { get; init; }
    public required string? TwitchClip { get; init; }
    public required DateTimeOffset Date { get; init; }
    public required bool CurrentRotation { get; init; }
    public required RunData RunData { get; init; }
    public required CustomData? CustomData { get; init; }
}