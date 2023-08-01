using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class RunData
{
    [JsonProperty("enemy_scaling")]
    public EnemyScaling EnemyScaling { get; set; }

    [JsonProperty("nb_of_waves")]
    public int NbOfWaves { get; set; }

    [JsonProperty("zone")]
    public int Zone { get; set; }

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("xp")]
    public decimal Xp { get; set; }

    [JsonProperty("max_weapons")]
    public int MaxWeapons { get; set; }

    [JsonProperty("wave")]
    public int Wave { get; set; }

    [JsonProperty("gold")]
    public int Gold { get; set; }

    [JsonProperty("bonus_gold")]
    public int BonusGold { get; set; }

    [JsonProperty("won")]
    public bool Won { get; set; }

    [JsonProperty("endless")]
    public bool Endless { get; set; }

    [JsonProperty("elites_spawn")]
    public List<object> ElitesSpawn { get; set; }

    [JsonProperty("character")]
    public string Character { get; set; }

    [JsonProperty("current_background")]
    public string CurrentBackground { get; set; }

    [JsonProperty("difficulty_unlocked")]
    public int DifficultyUnlocked { get; set; }

    [JsonProperty("max_endless_wave_record_beaten")]
    public int MaxEndlessWaveRecordBeaten { get; set; }

    [JsonProperty("appearances_displayed")]
    public List<AppearancesDisplayed> AppearancesDisplayed { get; set; }

    [JsonProperty("weapons")]
    public List<string> Weapons { get; set; }

    [JsonProperty("items")]
    public List<string> Items { get; set; }

    [JsonProperty("challenges_completed_this_run")]
    public List<string> ChallengesCompletedThisRun { get; set; }

    [JsonProperty("active_set_effects")]
    public List<object> ActiveSetEffects { get; set; }

    [JsonProperty("effects")]
    public Effects Effects { get; set; }

    [JsonProperty("current_character")]
    public string CurrentCharacter { get; set; }

    [JsonProperty("starting_weapon")]
    public string? StartingWeapon { get; set; }

    [JsonProperty("rounds")]
    public Dictionary<int, Dictionary<string, decimal>> Rounds { get; set; }
}


