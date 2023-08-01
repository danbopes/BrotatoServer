using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectibleCardEngine;

public interface ICardEngine
{
    int CardCount { get; }
    public string Name { get; }
    Task InitializeAsync();
    Task<ICardResults<ICollectibleCard>> FindAsync(string name);
    Task<ICollectibleCard?> GetRandomAsync();
}
