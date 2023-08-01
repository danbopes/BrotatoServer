using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class Effects
{
    [JsonProperty("gain_stat_max_hp")]
    public int GainStatMaxHp { get; set; }

    [JsonProperty("gain_stat_armor")]
    public int GainStatArmor { get; set; }

    [JsonProperty("gain_stat_crit_chance")]
    public int GainStatCritChance { get; set; }

    [JsonProperty("gain_stat_luck")]
    public int GainStatLuck { get; set; }

    [JsonProperty("gain_stat_attack_speed")]
    public int GainStatAttackSpeed { get; set; }

    [JsonProperty("gain_stat_elemental_damage")]
    public int GainStatElementalDamage { get; set; }

    [JsonProperty("gain_stat_hp_regeneration")]
    public int GainStatHpRegeneration { get; set; }

    [JsonProperty("gain_stat_lifesteal")]
    public int GainStatLifesteal { get; set; }

    [JsonProperty("gain_stat_melee_damage")]
    public int GainStatMeleeDamage { get; set; }

    [JsonProperty("gain_stat_percent_damage")]
    public int GainStatPercentDamage { get; set; }

    [JsonProperty("gain_stat_dodge")]
    public int GainStatDodge { get; set; }

    [JsonProperty("gain_stat_engineering")]
    public int GainStatEngineering { get; set; }

    [JsonProperty("gain_stat_range")]
    public int GainStatRange { get; set; }

    [JsonProperty("gain_stat_ranged_damage")]
    public int GainStatRangedDamage { get; set; }

    [JsonProperty("gain_stat_speed")]
    public int GainStatSpeed { get; set; }

    [JsonProperty("gain_stat_harvesting")]
    public int GainStatHarvesting { get; set; }

    [JsonProperty("no_melee_weapons")]
    public int NoMeleeWeapons { get; set; }

    [JsonProperty("no_ranged_weapons")]
    public int NoRangedWeapons { get; set; }

    [JsonProperty("hp_start_wave")]
    public int HpStartWave { get; set; }

    [JsonProperty("hp_start_next_wave")]
    public int HpStartNextWave { get; set; }

    [JsonProperty("no_min_range")]
    public int NoMinRange { get; set; }

    [JsonProperty("pacifist")]
    public int Pacifist { get; set; }

    [JsonProperty("cryptid")]
    public int Cryptid { get; set; }

    [JsonProperty("gain_pct_gold_start_wave")]
    public int GainPctGoldStartWave { get; set; }

    [JsonProperty("torture")]
    public int Torture { get; set; }

    [JsonProperty("recycling_gains")]
    public int RecyclingGains { get; set; }

    [JsonProperty("one_shot_trees")]
    public int OneShotTrees { get; set; }

    [JsonProperty("cant_stop_moving")]
    public int CantStopMoving { get; set; }

    [JsonProperty("max_ranged_weapons")]
    public int MaxRangedWeapons { get; set; }

    [JsonProperty("max_melee_weapons")]
    public int MaxMeleeWeapons { get; set; }

    [JsonProperty("group_structures")]
    public int GroupStructures { get; set; }

    [JsonProperty("double_hp_regen")]
    public int DoubleHpRegen { get; set; }

    [JsonProperty("can_attack_while_moving")]
    public int CanAttackWhileMoving { get; set; }

    [JsonProperty("trees")]
    public int Trees { get; set; }

    [JsonProperty("trees_start_wave")]
    public int TreesStartWave { get; set; }

    [JsonProperty("min_weapon_tier")]
    public int MinWeaponTier { get; set; }

    [JsonProperty("max_weapon_tier")]
    public int MaxWeaponTier { get; set; }

    [JsonProperty("hp_shop")]
    public int HpShop { get; set; }

    [JsonProperty("free_rerolls")]
    public int FreeRerolls { get; set; }

    [JsonProperty("instant_gold_attracting")]
    public int InstantGoldAttracting { get; set; }

    [JsonProperty("inflation")]
    public int Inflation { get; set; }

    [JsonProperty("double_boss")]
    public int DoubleBoss { get; set; }

    [JsonProperty("diff_gold_drops")]
    public int DiffGoldDrops { get; set; }

    [JsonProperty("gain_explosion_damage")]
    public int GainExplosionDamage { get; set; }

    [JsonProperty("gain_piercing_damage")]
    public int GainPiercingDamage { get; set; }

    [JsonProperty("gain_bounce_damage")]
    public int GainBounceDamage { get; set; }

    [JsonProperty("gain_damage_against_bosses")]
    public int GainDamageAgainstBosses { get; set; }

    [JsonProperty("neutral_gold_drops")]
    public int NeutralGoldDrops { get; set; }

    [JsonProperty("enemy_gold_drops")]
    public int EnemyGoldDrops { get; set; }

    [JsonProperty("wandering_bots")]
    public int WanderingBots { get; set; }

    [JsonProperty("dmg_when_pickup_gold")]
    public List<object> DmgWhenPickupGold { get; set; }

    [JsonProperty("dmg_when_death")]
    public List<object> DmgWhenDeath { get; set; }

    [JsonProperty("dmg_when_heal")]
    public List<object> DmgWhenHeal { get; set; }

    [JsonProperty("dmg_on_dodge")]
    public List<object> DmgOnDodge { get; set; }

    [JsonProperty("heal_on_dodge")]
    public List<object> HealOnDodge { get; set; }

    [JsonProperty("remove_speed")]
    public List<object> RemoveSpeed { get; set; }

    [JsonProperty("starting_item")]
    public List<object> StartingItem { get; set; }

    [JsonProperty("starting_weapon")]
    public List<object> StartingWeapon { get; set; }

    [JsonProperty("projectiles_on_death")]
    public List<object> ProjectilesOnDeath { get; set; }

    [JsonProperty("burn_chance")]
    public BurnChance BurnChance { get; set; }

    [JsonProperty("weapon_class_bonus")]
    public List<object> WeaponClassBonus { get; set; }

    [JsonProperty("weapon_bonus")]
    public List<object> WeaponBonus { get; set; }

    [JsonProperty("unique_weapon_effects")]
    public List<object> UniqueWeaponEffects { get; set; }

    [JsonProperty("additional_weapon_effects")]
    public List<object> AdditionalWeaponEffects { get; set; }

    [JsonProperty("tier_iv_weapon_effects")]
    public List<object> TierIvWeaponEffects { get; set; }

    [JsonProperty("tier_i_weapon_effects")]
    public List<object> TierIWeaponEffects { get; set; }

    [JsonProperty("gold_on_crit_kill")]
    public List<object> GoldOnCritKill { get; set; }

    [JsonProperty("heal_on_crit_kill")]
    public int HealOnCritKill { get; set; }

    [JsonProperty("temp_stats_while_not_moving")]
    public List<object> TempStatsWhileNotMoving { get; set; }

    [JsonProperty("temp_stats_while_moving")]
    public List<object> TempStatsWhileMoving { get; set; }

    [JsonProperty("temp_stats_on_hit")]
    public List<object> TempStatsOnHit { get; set; }

    [JsonProperty("temp_stats_stacking")]
    public List<object> TempStatsStacking { get; set; }

    [JsonProperty("stats_end_of_wave")]
    public List<object> StatsEndOfWave { get; set; }

    [JsonProperty("stat_links")]
    public List<List<object>> StatLinks { get; set; }

    [JsonProperty("structures")]
    public List<object> Structures { get; set; }

    [JsonProperty("explode_on_hit")]
    public List<object> ExplodeOnHit { get; set; }

    [JsonProperty("convert_stats_end_of_wave")]
    public List<object> ConvertStatsEndOfWave { get; set; }

    [JsonProperty("explode_on_death")]
    public List<object> ExplodeOnDeath { get; set; }

    [JsonProperty("alien_eyes")]
    public List<object> AlienEyes { get; set; }

    [JsonProperty("double_hp_regen_below_half_health")]
    public int DoubleHpRegenBelowHalfHealth { get; set; }

    [JsonProperty("upgrade_random_weapon")]
    public List<object> UpgradeRandomWeapon { get; set; }

    [JsonProperty("minimum_weapons_in_shop")]
    public int MinimumWeaponsInShop { get; set; }

    [JsonProperty("destroy_weapons")]
    public int DestroyWeapons { get; set; }

    [JsonProperty("extra_enemies_next_wave")]
    public int ExtraEnemiesNextWave { get; set; }

    [JsonProperty("extra_loot_aliens_next_wave")]
    public int ExtraLootAliensNextWave { get; set; }

    [JsonProperty("stats_next_wave")]
    public List<object> StatsNextWave { get; set; }

    [JsonProperty("consumable_stats_while_max")]
    public List<object> ConsumableStatsWhileMax { get; set; }

    [JsonProperty("explode_on_consumable")]
    public List<object> ExplodeOnConsumable { get; set; }

    [JsonProperty("structures_cooldown_reduction")]
    public List<object> StructuresCooldownReduction { get; set; }

    [JsonProperty("temp_pct_stats_start_wave")]
    public List<object> TempPctStatsStartWave { get; set; }

    [JsonProperty("temp_pct_stats_stacking")]
    public List<object> TempPctStatsStacking { get; set; }

    [JsonProperty("convert_stats_half_wave")]
    public List<object> ConvertStatsHalfWave { get; set; }

    [JsonProperty("stats_on_level_up")]
    public List<object> StatsOnLevelUp { get; set; }

    [JsonProperty("temp_stats_on_dodge")]
    public List<object> TempStatsOnDodge { get; set; }

    [JsonProperty("no_heal")]
    public int NoHeal { get; set; }

    [JsonProperty("tree_turrets")]
    public int TreeTurrets { get; set; }

    [JsonProperty("stats_below_half_health")]
    public List<object> StatsBelowHalfHealth { get; set; }

    [JsonProperty("guaranteed_shop_items")]
    public List<object> GuaranteedShopItems { get; set; }

    [JsonProperty("special_enemies_last_wave")]
    public int SpecialEnemiesLastWave { get; set; }

    [JsonProperty("specific_items_price")]
    public List<object> SpecificItemsPrice { get; set; }

    [JsonProperty("accuracy")]
    public int Accuracy { get; set; }

    [JsonProperty("projectiles")]
    public int Projectiles { get; set; }

    [JsonProperty("upgraded_baits")]
    public int UpgradedBaits { get; set; }

    [JsonProperty("stat_max_hp")]
    public int StatMaxHp { get; set; }

    [JsonProperty("stat_armor")]
    public int StatArmor { get; set; }

    [JsonProperty("stat_crit_chance")]
    public int StatCritChance { get; set; }

    [JsonProperty("stat_luck")]
    public int StatLuck { get; set; }

    [JsonProperty("stat_attack_speed")]
    public int StatAttackSpeed { get; set; }

    [JsonProperty("stat_elemental_damage")]
    public int StatElementalDamage { get; set; }

    [JsonProperty("stat_hp_regeneration")]
    public int StatHpRegeneration { get; set; }

    [JsonProperty("stat_lifesteal")]
    public int StatLifesteal { get; set; }

    [JsonProperty("stat_melee_damage")]
    public int StatMeleeDamage { get; set; }

    [JsonProperty("stat_percent_damage")]
    public int StatPercentDamage { get; set; }

    [JsonProperty("stat_dodge")]
    public int StatDodge { get; set; }

    [JsonProperty("stat_engineering")]
    public int StatEngineering { get; set; }

    [JsonProperty("stat_range")]
    public int StatRange { get; set; }

    [JsonProperty("stat_ranged_damage")]
    public int StatRangedDamage { get; set; }

    [JsonProperty("stat_speed")]
    public int StatSpeed { get; set; }

    [JsonProperty("stat_harvesting")]
    public int StatHarvesting { get; set; }

    [JsonProperty("xp_gain")]
    public int XpGain { get; set; }

    [JsonProperty("number_of_enemies")]
    public int NumberOfEnemies { get; set; }

    [JsonProperty("consumable_heal")]
    public int ConsumableHeal { get; set; }

    [JsonProperty("burning_cooldown_reduction")]
    public int BurningCooldownReduction { get; set; }

    [JsonProperty("burning_spread")]
    public int BurningSpread { get; set; }

    [JsonProperty("piercing")]
    public int Piercing { get; set; }

    [JsonProperty("piercing_damage")]
    public int PiercingDamage { get; set; }

    [JsonProperty("pickup_range")]
    public int PickupRange { get; set; }

    [JsonProperty("chance_double_gold")]
    public int ChanceDoubleGold { get; set; }

    [JsonProperty("bounce")]
    public int Bounce { get; set; }

    [JsonProperty("bounce_damage")]
    public int BounceDamage { get; set; }

    [JsonProperty("heal_when_pickup_gold")]
    public int HealWhenPickupGold { get; set; }

    [JsonProperty("item_box_gold")]
    public int ItemBoxGold { get; set; }

    [JsonProperty("knockback")]
    public int Knockback { get; set; }

    [JsonProperty("hp_cap")]
    public int HpCap { get; set; }

    [JsonProperty("speed_cap")]
    public int SpeedCap { get; set; }

    [JsonProperty("lose_hp_per_second")]
    public int LoseHpPerSecond { get; set; }

    [JsonProperty("map_size")]
    public int MapSize { get; set; }

    [JsonProperty("dodge_cap")]
    public int DodgeCap { get; set; }

    [JsonProperty("gold_drops")]
    public int GoldDrops { get; set; }

    [JsonProperty("enemy_health")]
    public int EnemyHealth { get; set; }

    [JsonProperty("enemy_damage")]
    public int EnemyDamage { get; set; }

    [JsonProperty("enemy_speed")]
    public int EnemySpeed { get; set; }

    [JsonProperty("boss_strength")]
    public int BossStrength { get; set; }

    [JsonProperty("explosion_size")]
    public int ExplosionSize { get; set; }

    [JsonProperty("explosion_damage")]
    public int ExplosionDamage { get; set; }

    [JsonProperty("damage_against_bosses")]
    public int DamageAgainstBosses { get; set; }

    [JsonProperty("giant_crit_damage")]
    public int GiantCritDamage { get; set; }

    [JsonProperty("weapon_slot")]
    public int WeaponSlot { get; set; }

    [JsonProperty("items_price")]
    public int ItemsPrice { get; set; }

    [JsonProperty("harvesting_growth")]
    public int HarvestingGrowth { get; set; }

    [JsonProperty("hit_protection")]
    public int HitProtection { get; set; }

    [JsonProperty("weapons_price")]
    public int WeaponsPrice { get; set; }
}
