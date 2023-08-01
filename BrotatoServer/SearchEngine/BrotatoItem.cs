using CollectibleCardEngine;

namespace BrotatoServer.SearchEngine;

public class BrotatoItem : ICollectibleCard
{
    public required string Culture { get; init; }

    public required string Id { get; init; }

    public required string Name { get; init; }

    public required string TextOutput { get; init; }
}