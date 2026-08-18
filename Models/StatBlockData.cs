using System.Text.Json;
using System.Text.Json.Serialization;

namespace Website_of_Everything.Models;


public sealed class StatBlockData
{
    // =====================================
    // IDENTIFICATION / PLACEMENT
    // =====================================

    public string Id { get; set; } =
        string.Empty;


    [JsonPropertyName("section_type")]
    public string SectionType { get; set; } =
        string.Empty;


    public string Title { get; set; } =
        string.Empty;


    public string Placement { get; set; } =
        string.Empty;


    public string Marker { get; set; } =
        string.Empty;


    // =====================================
    // CREATURE
    // =====================================

    public StatBlockCreature Creature { get; set; } =
        new();


    // =====================================
    // CORE STATS
    // =====================================

    [JsonPropertyName("armor_class")]
    public string ArmorClass { get; set; } =
        string.Empty;


    [JsonPropertyName("hit_points")]
    public string HitPoints { get; set; } =
        string.Empty;


    /*
     * Example:
     *
     * "speed": {
     *   "walk": "30 ft.",
     *   "climb": "30 ft. (land only)",
     *   "fly": "60 ft. (air only)",
     *   "swim": "30 ft. (water only)"
     * }
     *
     * Dictionary keeps this flexible if a
     * creature later gains burrow or another
     * movement type.
     */
    public Dictionary<string, string> Speed { get; set; } =
        new();


    // =====================================
    // ABILITY SCORES
    // =====================================

    [JsonPropertyName("ability_scores")]
    public Dictionary<string, StatBlockAbility>
        AbilityScores { get; set; } =
            new();


    // =====================================
    // INFO
    // =====================================

    public List<string> Skills { get; set; } =
        new();


    public List<string> Resistances { get; set; } =
        new();


    public List<string> Vulnerabilities { get; set; } =
        new();


    public List<string> Immunities { get; set; } =
        new();


    public List<string> Senses { get; set; } =
        new();


    /*
     * JsonElement allows this to be either:
     *
     * "Languages": "Common"
     *
     * or:
     *
     * "Languages": ["Common", "Draconic"]
     */
    public JsonElement Languages { get; set; }


    [JsonPropertyName("proficiency_bonus")]
    public JsonElement ProficiencyBonus { get; set; }


    // =====================================
    // STAT BLOCK SECTIONS
    // =====================================

    public List<StatBlockEntry> Traits { get; set; } =
        new();


    public List<StatBlockEntry> Actions { get; set; } =
        new();


    [JsonPropertyName("bonus_actions")]
    public List<StatBlockEntry> BonusActions { get; set; } =
        new();


    public List<StatBlockEntry> Reactions { get; set; } =
        new();


    [JsonPropertyName("legendary_actions")]
    public List<StatBlockEntry> LegendaryActions { get; set; } =
        new();


    // =====================================
    // FALLBACK SOURCE TEXT
    //
    // Stored, but deliberately NOT rendered.
    // The structured fields above are used.
    // =====================================

    [JsonPropertyName("raw_text")]
    public string RawText { get; set; } =
        string.Empty;
}


// =========================================
// CREATURE IDENTITY
// =========================================

public sealed class StatBlockCreature
{
    public string Size { get; set; } =
        string.Empty;


    public string Type { get; set; } =
        string.Empty;


    public string Alignment { get; set; } =
        string.Empty;
}


// =========================================
// ABILITY
// =========================================

public sealed class StatBlockAbility
{
    public int Score { get; set; }


    public string Check { get; set; } =
        "-";


    public string Save { get; set; } =
        "-";
}


// =========================================
// TRAIT / ACTION
// =========================================

public sealed class StatBlockEntry
{
    public string Name { get; set; } =
        string.Empty;


    public string Description { get; set; } =
        string.Empty;
}