using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectibleCardEngine;

public interface ISpoilerEngine
{
    Task<IEnumerable<ICollectibleCard>> GetLatestAsync(int amount = 5);
}
