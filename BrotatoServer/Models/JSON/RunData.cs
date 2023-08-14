using BrotatoServer.Utilities;
using Newtonsoft.Json;

namespace BrotatoServer.Models.JSON;

public class RunData
{
    [JsonProperty("enemy_scaling")]
    public required EnemyScaling EnemyScaling { get; set; }

    [JsonProperty("nb_of_waves")]
    public required int NbOfWaves { get; set; }

    [JsonProperty("zone")]
    public required int Zone { get; set; }

    [JsonProperty("level")]
    public required int Level { get; set; }

    [JsonProperty("xp")]
    public required decimal Xp { get; set; }

    [JsonProperty("max_weapons")]
    public required int MaxWeapons { get; set; }

    [JsonProperty("wave")]
    public required int Wave { get; set; }

    [JsonProperty("gold")]
    public required int Gold { get; set; }

    [JsonProperty("bonus_gold")]
    public required int BonusGold { get; set; }

    [JsonProperty("won")]
    public required bool Won { get; set; }

    [JsonProperty("endless")]
    public required bool Endless { get; set; }

    [JsonProperty("elites_spawn")]
    public required List<object> ElitesSpawn { get; set; }

    [JsonProperty("character")]
    public required string Character { get; set; }

    [JsonProperty("current_background")]
    public required string CurrentBackground { get; set; }

    [JsonProperty("difficulty_unlocked")]
    public required int DifficultyUnlocked { get; set; }

    [JsonProperty("max_endless_wave_record_beaten")]
    public required int MaxEndlessWaveRecordBeaten { get; set; }

    [JsonProperty("appearances_displayed")]
    public required List<AppearancesDisplayed> AppearancesDisplayed { get; set; }

    [JsonProperty("weapons")]
    public required Dictionary<int, ItemData> Weapons { get; set; }

    [JsonProperty("items")]
    public required Dictionary<int, ItemData> Items { get; set; }

    [JsonProperty("challenges_completed_this_run")]
    public required List<string> ChallengesCompletedThisRun { get; set; }

    [JsonProperty("active_set_effects")]
    public required List<object> ActiveSetEffects { get; set; }

    [JsonProperty("effects")]
    public required Effects Effects { get; set; }
    
    [JsonProperty("stats")]
    public required Dictionary<string, int>? Stats { get; set; }

    [JsonProperty("current_character")]
    public required string CurrentCharacter { get; set; }

    [JsonProperty("starting_weapon")]
    public required string? StartingWeapon { get; set; }

    [JsonProperty("rounds")]
    public required Dictionary<int, Dictionary<string, decimal>> Rounds { get; set; }

    [JsonIgnore]
    public string CharacterName {
        get
        {
            var characterData = Items.Values.FirstOrDefault(item => item.Id == Character);
            var niceName = characterData?.Name;
        
            if (niceName is not null)
                return niceName;

            // There should always be a character in the items list, but just in case
            var charName = Character.Replace("character_", "");
            var niceCharName = string.Join(' ', charName.Split('_').Select(word => word.UcFirst()));

            return niceCharName;
        }
    }
}