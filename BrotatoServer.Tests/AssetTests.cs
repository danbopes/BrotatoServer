using BrotatoServer.Data.Game;

namespace BrotatoServer.Tests;

public class AssetTests
{
    [Fact]
    public void ShouldLoadWeaponsCorrectly()
    {
        var weapons = Assets.Weapons;
        Assert.NotNull(weapons);
        Assert.NotEmpty(weapons);
        Assert.True(weapons.ContainsKey("weapon_fist_1"));
        var fist = weapons["weapon_fist_1"];
        Assert.Equal("weapon_fist", fist.WeaponId);
        Assert.Equal("res://weapons/melee/fist/fist_icon.png", fist.Icon);
        Assert.Equal(0, fist.Tier);
        Assert.Equal("Fist", fist.Name);
    }
    
    
    [Fact]
    public void ShouldLoadItemsCorrectly()
    {
        var items = Assets.Items;
        Assert.NotNull(items);
        Assert.NotEmpty(items);
        Assert.True(items.ContainsKey("item_acid"));
        var acid = items["item_acid"];
        Assert.Equal("res://items/all/acid/acid_icon.png", acid.Icon);
        Assert.Equal(1, acid.Tier);
        Assert.Equal("Acid", acid.Name);
        Assert.Equal("[color=#00ff00]+8[/color] Max HP\n[color=red]-4[/color] % Dodge", acid.EffectsText);
        Assert.Null(acid.TrackingText);
    }
    
    [Fact]
    public void ShouldLoadStatsCorrectly()
    {
        /*     
  "stat_max_hp": {
    "icon": "res://items/upgrades/health/health.png",
    "small_icon": "res://items/stats/max_hp.png",
    "name": "Max HP"
  },
         */
        var stats = Assets.Stats;
        Assert.NotNull(stats);
        Assert.NotEmpty(stats);
        Assert.True(stats.ContainsKey("stat_max_hp"));
        var hp = stats["stat_max_hp"];
        Assert.Equal("Max HP", hp.Name);
        Assert.Equal("res://items/upgrades/health/health.png", hp.Icon);
        Assert.Equal("res://items/stats/max_hp.png", hp.SmallIcon);
    }
}