using System.Text.Json;
using System.Text.Json.Serialization;

namespace Website_of_Everything.Models;


// =========================================
// STAT BLOCK DATA
// =========================================

public sealed class StatBlockData
{
    // =====================================
    // ID
    // =====================================

    [JsonPropertyName("id")]
    public string Id { get; set; } =
        string.Empty;


    // =====================================
    // SECTION TYPE
    // =====================================

    [JsonPropertyName("section_type")]
    public string SectionType { get; set; } =
        string.Empty;


    // =====================================
    // TITLE
    // =====================================

    [JsonPropertyName("title")]
    public string Title { get; set; } =
        string.Empty;


    // =====================================
    // INFORMATION
    //
    // This is normally inherited from the
    // outer monster document used by the
    // Bestiary.
    // =====================================

    [JsonPropertyName("information")]
    public string Information { get; set; } =
        string.Empty;


    // =====================================
    // PLACEMENT
    // =====================================

    [JsonPropertyName("placement")]
    public string Placement { get; set; } =
        string.Empty;


    // =====================================
    // MARKER
    // =====================================

    [JsonPropertyName("marker")]
    public string Marker { get; set; } =
        string.Empty;


    // =====================================
    // IMAGE
    // =====================================

    [JsonPropertyName("image")]
    public string Image { get; set; } =
        string.Empty;


    // =====================================
    // REFINEMENT
    //
    // Some monster files include additional
    // lore/reference data outside the inner
    // Stat Blocks array.
    // =====================================

    [JsonPropertyName("refinement")]
    public MonsterRefinementData? Refinement { get; set; }


    // =====================================
    // MONSTER GROUP
    //
    // Supports:
    //
    // "group": "Devil"
    //
    // Use this when the creature belongs
    // to one primary monster group.
    // =====================================

    [JsonPropertyName("group")]
    public string Group { get; set; } =
        string.Empty;


    // =====================================
    // MONSTER GROUPS
    //
    // Supports:
    //
    // "groups": [
    //   "Devil",
    //   "Infernal"
    // ]
    //
    // This can be used alongside "group".
    // =====================================

    [JsonPropertyName("groups")]
    public List<string> Groups { get; set; } =
        new();


    // =====================================
    // SOURCE
    //
    // Supports:
    //
    // "source": "Monster Manual"
    // =====================================

    [JsonPropertyName("source")]
    public string Source { get; set; } =
        string.Empty;


    // =====================================
    // SOURCES
    //
    // Supports:
    //
    // "sources": [
    //   "Monster Manual",
    //   "Royal Treasury"
    // ]
    //
    // This can be used alongside "source".
    // =====================================

    [JsonPropertyName("sources")]
    public List<string> Sources { get; set; } =
        new();


    // =====================================
    // CREATURE INFORMATION
    // =====================================

    [JsonPropertyName("creature")]
    public StatBlockCreature Creature { get; set; } =
        new();


    // =====================================
    // ARMOR CLASS
    // =====================================

    [JsonPropertyName("armor_class")]
    public string ArmorClass { get; set; } =
        string.Empty;


    // =====================================
    // THRESHOLD
    // =====================================

    [JsonPropertyName("threshold")]
    public string Threshold { get; set; } =
        string.Empty;


    // =====================================
    // HIT POINTS
    // =====================================

    [JsonPropertyName("hit_points")]
    public string HitPoints { get; set; } =
        string.Empty;


    // =====================================
    // SPEED
    //
    // Example:
    //
    // "speed": {
    //   "walk": "30 ft.",
    //   "fly": "60 ft.",
    //   "swim": "30 ft."
    // }
    // =====================================

    [JsonPropertyName("speed")]
    public Dictionary<string, string> Speed { get; set; } =
        new(
            StringComparer.OrdinalIgnoreCase);


    // =====================================
    // QUALITY TRAITS
    // =====================================

    [JsonPropertyName("quality_traits")]
    public List<string> QualityTraits { get; set; } =
        new();


    // =====================================
    // ABILITY SCORES
    //
    // Example:
    //
    // "ability_scores": {
    //   "STR": {
    //     "score": 18,
    //     "check": "+4",
    //     "save": "-"
    //   }
    // }
    // =====================================

    [JsonPropertyName("ability_scores")]
    public Dictionary<
        string,
        StatBlockAbilityScore>
        AbilityScores { get; set; } =
            new(
                StringComparer.OrdinalIgnoreCase);


    // =====================================
    // SAVING THROWS
    // =====================================

    [JsonPropertyName("saving_throws")]
    public List<string> SavingThrows { get; set; } =
        new();


    // =====================================
    // SKILLS
    // =====================================

    [JsonPropertyName("skills")]
    public List<string> Skills { get; set; } =
        new();


    // =====================================
    // RESISTANCES
    // =====================================

    [JsonPropertyName("resistances")]
    public List<string> Resistances { get; set; } =
        new();


    // =====================================
    // VULNERABILITIES
    // =====================================

    [JsonPropertyName("vulnerabilities")]
    public List<string> Vulnerabilities { get; set; } =
        new();


    // =====================================
    // IMMUNITIES
    // =====================================

    [JsonPropertyName("immunities")]
    public List<string> Immunities { get; set; } =
        new();


    // =====================================
    // CONDITION IMMUNITIES
    // =====================================

    [JsonPropertyName("condition_immunities")]
    public List<string> ConditionImmunities { get; set; } =
        new();


    // =====================================
    // SENSES
    // =====================================

    [JsonPropertyName("senses")]
    public List<string> Senses { get; set; } =
        new();


    // =====================================
    // LANGUAGES
    //
    // JsonElement allows either:
    //
    // "languages": "Common"
    //
    // or:
    //
    // "languages": [
    //   "Common",
    //   "Draconic"
    // ]
    // =====================================

    [JsonPropertyName("languages")]
    public JsonElement Languages { get; set; }


    // =====================================
    // CHALLENGE RATING
    // =====================================

    [JsonPropertyName("challenge_rating")]
    public string ChallengeRating { get; set; } =
        string.Empty;


    // =====================================
    // PROFICIENCY BONUS
    //
    // JsonElement permits values such as:
    //
    // "+2"
    //
    // or:
    //
    // 2
    // =====================================

    [JsonPropertyName("proficiency_bonus")]
    public JsonElement ProficiencyBonus { get; set; }


    // =====================================
    // TRAITS
    // =====================================

    [JsonPropertyName("traits")]
    public List<StatBlockEntry> Traits { get; set; } =
        new();


    // =====================================
    // ACTIONS
    // =====================================

    [JsonPropertyName("actions")]
    public List<StatBlockEntry> Actions { get; set; } =
        new();


    // =====================================
    // BONUS ACTIONS
    // =====================================

    [JsonPropertyName("bonus_actions")]
    public List<StatBlockEntry> BonusActions { get; set; } =
        new();


    // =====================================
    // REACTIONS
    // =====================================

    [JsonPropertyName("reactions")]
    public List<StatBlockEntry> Reactions { get; set; } =
        new();


    // =====================================
    // LEGENDARY ACTIONS
    // =====================================

    [JsonPropertyName("legendary_actions")]
    public StatBlockLegendaryActions
        LegendaryActions { get; set; } =
            new();


    // =====================================
    // LEGENDARY REACTIONS
    // =====================================

    [JsonPropertyName("legendary_reactions")]
    public StatBlockLegendaryActions
        LegendaryReactions { get; set; } =
            new();


    // =====================================
    // RAW TEXT
    // =====================================

    [JsonPropertyName("raw_text")]
    public string RawText { get; set; } =
        string.Empty;
}


// =========================================
// CREATURE INFORMATION
//
// Example:
//
// "creature": {
//   "size": "Large",
//   "type": "fiend (yokai)",
//   "alignment": "chaotic evil"
// }
// =========================================

public sealed class StatBlockCreature
{
    // =====================================
    // SIZE
    // =====================================

    [JsonPropertyName("size")]
    public string Size { get; set; } =
        string.Empty;


    // =====================================
    // TYPE
    //
    // Examples:
    //
    // beast
    // undead
    // fiend (yokai)
    // humanoid (petitioner)
    // =====================================

    [JsonPropertyName("type")]
    public string Type { get; set; } =
        string.Empty;


    // =====================================
    // ALIGNMENT
    // =====================================

    [JsonPropertyName("alignment")]
    public string Alignment { get; set; } =
        string.Empty;
}


// =========================================
// ABILITY SCORE
//
// Example:
//
// "STR": {
//   "score": 18,
//   "check": "+4",
//   "save": "-"
// }
// =========================================

public sealed class StatBlockAbilityScore
{
    // =====================================
    // SCORE
    // =====================================

    [JsonPropertyName("score")]
    public int Score { get; set; }


    // =====================================
    // CHECK
    // =====================================

    [JsonPropertyName("check")]
    public string Check { get; set; } =
        string.Empty;


    // =====================================
    // MODIFIER
    //
    // Some monster files use "modifier"
    // instead of "check".
    // =====================================

    [JsonPropertyName("modifier")]
    public string Modifier { get; set; } =
        string.Empty;


    // =====================================
    // SAVE
    // =====================================

    [JsonPropertyName("save")]
    public string Save { get; set; } =
        string.Empty;
}


// =========================================
// MONSTER REFINEMENT DATA
// =========================================

public sealed class MonsterRefinementData
{
    [JsonPropertyName("title")]
    public string Title { get; set; } =
        string.Empty;


    [JsonPropertyName("description")]
    public string Description { get; set; } =
        string.Empty;


    [JsonPropertyName("notes")]
    public List<string> Notes { get; set; } =
        new();


    [JsonPropertyName("table")]
    public List<MonsterRefinementRow> Table { get; set; } =
        new();
}


// =========================================
// MONSTER REFINEMENT TABLE ROW
// =========================================

public sealed class MonsterRefinementRow
{
    [JsonPropertyName("color")]
    public string Color { get; set; } =
        string.Empty;


    [JsonPropertyName("ability_scores")]
    public Dictionary<string, int> AbilityScores { get; set; } =
        new(
            StringComparer.OrdinalIgnoreCase);


    [JsonPropertyName("hit_points")]
    public string HitPoints { get; set; } =
        string.Empty;


    [JsonPropertyName("movement")]
    public List<string> Movement { get; set; } =
        new();


    [JsonPropertyName("immunities")]
    public List<string> Immunities { get; set; } =
        new();


    [JsonPropertyName("attack_modifier")]
    public string AttackModifier { get; set; } =
        string.Empty;


    [JsonPropertyName("breath_and_blood_dc")]
    public string BreathAndBloodDc { get; set; } =
        string.Empty;


    [JsonPropertyName("wing_dc")]
    public string WingDc { get; set; } =
        string.Empty;


    [JsonPropertyName("breath_damage")]
    public string BreathDamage { get; set; } =
        string.Empty;
}


// =========================================
// STAT BLOCK ENTRY
//
// Used for:
//
// Traits
// Actions
// Bonus Actions
// Reactions
// Legendary Actions
//
// Example:
//
// {
//   "name": "Multiattack",
//   "description": "The creature makes two attacks."
// }
// =========================================

public sealed class StatBlockEntry
{
    // =====================================
    // NAME
    // =====================================

    [JsonPropertyName("name")]
    public string Name { get; set; } =
        string.Empty;


    // =====================================
    // NAME FORMAT
    // =====================================

    [JsonPropertyName("name_format")]
    public string NameFormat { get; set; } =
        string.Empty;


    // =====================================
    // DESCRIPTION
    // =====================================

    [JsonPropertyName("description")]
    public string Description { get; set; } =
        string.Empty;


    // =====================================
    // INLINE FORMATTING
    //
    // Preserved from monster JSON so the
    // data is not discarded even though
    // GlossaryText currently handles the
    // rendered entry text.
    // =====================================

    [JsonPropertyName("inline_formatting")]
    public Dictionary<string, string>
        InlineFormatting { get; set; } =
            new(
                StringComparer.OrdinalIgnoreCase);
}
// =========================================
// LEGENDARY ACTIONS
//
// Supports both of these JSON shapes:
//
// Older/simple shape:
//
// "legendary_actions": [
//   {
//     "name": "Detect",
//     "description": "..."
//   }
// ]
//
// New/full shape:
//
// "legendary_actions": {
//   "header": "Legendary Actions",
//   "uses": 3,
//   "entries": [
//     {
//       "name": "Detect",
//       "description": "..."
//     }
//   ]
// }
//
// The class implements IReadOnlyList so the
// existing StatBlock rendering can continue
// to use:
//
// Data.LegendaryActions.Count
//
// and:
//
// foreach (var entry in Data.LegendaryActions)
// =========================================

[JsonConverter(
    typeof(
        StatBlockLegendaryActionsConverter))]
public sealed class StatBlockLegendaryActions
    : IReadOnlyList<StatBlockEntry>
{
    // =====================================
    // HEADER
    // =====================================

    [JsonPropertyName("header")]
    public string Header { get; set; } =
        string.Empty;


    // =====================================
    // USES
    // =====================================

    [JsonPropertyName("uses")]
    public int Uses { get; set; }


    // =====================================
    // ENTRIES
    // =====================================

    [JsonPropertyName("entries")]
    public List<StatBlockEntry> Entries { get; set; } =
        new();


    // =====================================
    // READ-ONLY LIST SUPPORT
    // =====================================

    [JsonIgnore]
    public int Count =>
        Entries.Count;


    public StatBlockEntry this[int index] =>
        Entries[index];


    public IEnumerator<StatBlockEntry>
        GetEnumerator()
    {
        return
            Entries.GetEnumerator();
    }


    System.Collections.IEnumerator
        System.Collections.IEnumerable
            .GetEnumerator()
    {
        return
            GetEnumerator();
    }
}


// =========================================
// LEGENDARY ACTIONS JSON CONVERTER
//
// The monster library already contains
// legendary_actions as a simple array.
//
// Newer monster files, such as the
// Amniatic Dragon, use an object containing:
//
// header
// uses
// entries
//
// This converter accepts BOTH shapes so
// older monsters do not need to be edited.
// =========================================

public sealed class
    StatBlockLegendaryActionsConverter
    : JsonConverter<StatBlockLegendaryActions>
{
    // =====================================
    // READ
    // =====================================

    public override StatBlockLegendaryActions
        Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
    {
        // =================================
        // NULL
        // =================================

        if (
            reader.TokenType
            ==
            JsonTokenType.Null)
        {
            return
                new StatBlockLegendaryActions();
        }


        // =================================
        // LEGACY ARRAY SHAPE
        //
        // "legendary_actions": [
        //   ...
        // ]
        // =================================

        if (
            reader.TokenType
            ==
            JsonTokenType.StartArray)
        {
            var entries =
                JsonSerializer
                    .Deserialize<
                        List<StatBlockEntry>>(
                            ref reader,
                            options)
                ??
                new List<StatBlockEntry>();


            return
                new StatBlockLegendaryActions
                {
                    Entries =
                        entries
                };
        }


        // =================================
        // OBJECT SHAPE
        //
        // "legendary_actions": {
        //   "header": "...",
        //   "uses": 3,
        //   "entries": [
        //     ...
        //   ]
        // }
        // =================================

        if (
            reader.TokenType
            ==
            JsonTokenType.StartObject)
        {
            using var document =
                JsonDocument.ParseValue(
                    ref reader);


            var root =
                document.RootElement;


            var result =
                new StatBlockLegendaryActions();


            // =============================
            // HEADER
            // =============================

            if (
                TryGetProperty(
                    root,
                    "header",
                    out var headerElement)
                &&
                headerElement.ValueKind
                ==
                JsonValueKind.String)
            {
                result.Header =
                    headerElement.GetString()
                    ??
                    string.Empty;
            }


            // =============================
            // USES
            //
            // Accept either:
            //
            // "uses": 3
            //
            // or:
            //
            // "uses": "3"
            // =============================

            if (
                TryGetProperty(
                    root,
                    "uses",
                    out var usesElement))
            {
                if (
                    usesElement.ValueKind
                    ==
                    JsonValueKind.Number
                    &&
                    usesElement.TryGetInt32(
                        out var uses))
                {
                    result.Uses =
                        uses;
                }
                else if (
                    usesElement.ValueKind
                    ==
                    JsonValueKind.String
                    &&
                    int.TryParse(
                        usesElement.GetString(),
                        out uses))
                {
                    result.Uses =
                        uses;
                }
            }


            // =============================
            // ENTRIES
            // =============================

            if (
                TryGetProperty(
                    root,
                    "entries",
                    out var entriesElement)
                &&
                entriesElement.ValueKind
                ==
                JsonValueKind.Array)
            {
                result.Entries =
                    JsonSerializer
                        .Deserialize<
                            List<StatBlockEntry>>(
                                entriesElement
                                    .GetRawText(),
                                options)
                    ??
                    new List<StatBlockEntry>();
            }


            return
                result;
        }


        // =================================
        // INVALID SHAPE
        // =================================

        throw new JsonException(
            "legendary_actions must be either an array or an object.");
    }


    // =====================================
    // WRITE
    //
    // When serialized, use the richer
    // object form so header / uses are not
    // discarded.
    // =====================================

    public override void
        Write(
            Utf8JsonWriter writer,
            StatBlockLegendaryActions value,
            JsonSerializerOptions options)
    {
        writer.WriteStartObject();


        // =================================
        // HEADER
        // =================================

        if (
            !string.IsNullOrWhiteSpace(
                value.Header))
        {
            writer.WriteString(
                "header",
                value.Header);
        }


        // =================================
        // USES
        // =================================

        if (
            value.Uses > 0)
        {
            writer.WriteNumber(
                "uses",
                value.Uses);
        }


        // =================================
        // ENTRIES
        // =================================

        writer.WritePropertyName(
            "entries");


        JsonSerializer.Serialize(
            writer,
            value.Entries,
            options);


        writer.WriteEndObject();
    }


    // =====================================
    // CASE-INSENSITIVE PROPERTY LOOKUP
    // =====================================

    private static bool
        TryGetProperty(
            JsonElement element,
            string propertyName,
            out JsonElement value)
    {
        foreach (
            var property
            in element.EnumerateObject())
        {
            if (
                property.Name.Equals(
                    propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                value =
                    property.Value;


                return
                    true;
            }
        }


        value =
            default;


        return
            false;
    }
}
