using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace BrotatoServer.Data.Game;

public static class Assets
{
    public static ReadOnlyDictionary<string, Weapon> Weapons { get; }
    public static ReadOnlyDictionary<string, Item> Items { get; }
    public static ReadOnlyDictionary<string, Stat> Stats { get; }

    static Assets()
    {
        var weaponJson = File.ReadAllText("Data/Game/weapons.json");
        Weapons = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, Weapon>>(weaponJson)!;

        var itemJson = File.ReadAllText("Data/Game/items.json");
        Items = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, Item>>(itemJson)!;

        var statJson = File.ReadAllText("Data/Game/stats.json");
        Stats = JsonConvert.DeserializeObject<ReadOnlyDictionary<string, Stat>>(statJson)!;
    }
}
