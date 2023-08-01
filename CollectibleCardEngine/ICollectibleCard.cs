namespace CollectibleCardEngine;

public interface ICollectibleCard
{
    string Culture { get; }
    string Id { get; }
    string Name { get; }
    string TextOutput { get; }
}