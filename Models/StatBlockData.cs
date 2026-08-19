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
    // VARIANT TAG
    //
    // Optional short label for a stat-block
    // variant tab.
    //
    // Example:
    //
    // "tag": "Ambush"
    //
    // If omitted, MonsterDetails derives a
    // short label from the stat block title.
    // =====================================

    [JsonPropertyName("tag")]
    public string Tag { get; set; } =
        string.Empty;


    // =====================================
    // MONSTER SET NAME
    //
    // Runtime-only name inherited from the
    // outer monster document.
    //
    // Example:
    //
    // "Name": "Belzers"
    //
    // with several objects in "Stat Blocks"
    // causes those stat blocks to share the
    // same MonsterSetName.
    //
    // It is not written to / read from JSON.
    // =====================================

    [JsonIgnore]
    public string MonsterSetName { get; set; } =
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
    // IMAGE VARIANTS
    //
    // The outer monster document can contain:
    //
    // "Image Variants": [
    //   "https://...",
    //   "https://..."
    // ]
    //
    // MonsterService normalizes the outer
    // property name and copies the URLs into
    // this list.
    // =====================================

    [JsonPropertyName("image_variants")]
    public List<string> ImageVariants { get; set; } =
        new();


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
    // BARDING TABLE
    //
    // Reuses the same generic lore-table
    // structure used by group lore:
    //
    // "Barding": {
    //   "headers": [
    //     { "text": "Barding", "bold": true },
    //     { "text": "Armor Class", "bold": true }
    //   ],
    //   "rows": [
    //     [ "Leather", "15 (...)" ]
    //   ]
    // }
    // =====================================

    [JsonPropertyName("barding")]
    public MonsterGroupLoreTable? Barding { get; set; }


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
    // LEGENDARY REACTION
    //
    // A small number of monster files use
    // the singular property name:
    //
    // "legendary_reaction": { ... }
    //
    // Keep it as an alias of the plural
    // runtime property so both JSON forms
    // render through the same section.
    // =====================================

    [JsonPropertyName("legendary_reaction")]
    public StatBlockLegendaryActions
        LegendaryReaction
    {
        get =>
            LegendaryReactions;

        set
        {
            if (value is not null)
            {
                LegendaryReactions =
                    value;
            }
        }
    }


    // =====================================
    // MYTHIC ACTIONS
    // =====================================

    [JsonPropertyName("mythic_actions")]
    public StatBlockLegendaryActions
        MythicActions { get; set; } =
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


    // =====================================
    // SPELLCASTING DETAILS
    //
    // Monster JSON stores spell lists as a
    // nested object beneath the Spellcasting
    // or Innate Spellcasting trait.
    // =====================================

    [JsonPropertyName("spellcasting")]
    public StatBlockSpellcasting?
        Spellcasting { get; set; }


    // =====================================
    // NESTED OPTIONS
    //
    // Several monster abilities contain
    // follow-up options. The options array
    // can contain either strings or normal
    // entry-shaped objects.
    // =====================================

    [JsonPropertyName("options")]
    [JsonConverter(
        typeof(
            StatBlockEntryOptionsConverter))]
    public List<StatBlockEntry>
        Options { get; set; } =
            new();
}


// =========================================
// SPELLCASTING DETAILS
// =========================================

public sealed class StatBlockSpellcasting
{
    [JsonPropertyName("spellcasting_level")]
    public JsonElement SpellcastingLevel { get; set; }


    [JsonPropertyName("caster_level")]
    public JsonElement CasterLevel { get; set; }


    [JsonPropertyName("spellcasting_ability")]
    public string SpellcastingAbility { get; set; } =
        string.Empty;


    [JsonPropertyName("material_components_required")]
    public bool? MaterialComponentsRequired { get; set; }


    [JsonPropertyName("components")]
    public string Components { get; set; } =
        string.Empty;


    [JsonPropertyName("spells")]
    public List<StatBlockSpellFrequency>
        Spells { get; set; } =
            new();


    [JsonPropertyName("prepared_spells")]
    public List<StatBlockPreparedSpellLevel>
        PreparedSpells { get; set; } =
            new();


    [JsonPropertyName("additional_text")]
    public string AdditionalText { get; set; } =
        string.Empty;


    [JsonPropertyName("casting_level_note")]
    public string CastingLevelNote { get; set; } =
        string.Empty;


    [JsonPropertyName("notes")]
    public List<string> Notes { get; set; } =
        new();
}


// =========================================
// SPELLCASTING FREQUENCY
// =========================================

public sealed class StatBlockSpellFrequency
{
    [JsonPropertyName("frequency")]
    public string Frequency { get; set; } =
        string.Empty;


    [JsonPropertyName("frequency_format")]
    public string FrequencyFormat { get; set; } =
        string.Empty;


    [JsonPropertyName("spells")]
    public List<string> Spells { get; set; } =
        new();
}


// =========================================
// PREPARED SPELL LEVEL
// =========================================

public sealed class StatBlockPreparedSpellLevel
{
    [JsonPropertyName("level")]
    public string Level { get; set; } =
        string.Empty;


    [JsonPropertyName("slots")]
    public JsonElement Slots { get; set; }


    [JsonPropertyName("spells")]
    public List<string> Spells { get; set; } =
        new();
}


// =========================================
// NESTED ENTRY OPTIONS JSON CONVERTER
//
// Accepts both:
//
// "options": [
//   "Plain text option"
// ]
//
// and:
//
// "options": [
//   {
//     "name": "Option Name",
//     "description": "..."
//   }
// ]
//
// Nested options are supported recursively.
// =========================================

public sealed class
    StatBlockEntryOptionsConverter
    : JsonConverter<List<StatBlockEntry>>
{
    public override List<StatBlockEntry>
        Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
    {
        var result =
            new List<StatBlockEntry>();


        if (
            reader.TokenType
            ==
            JsonTokenType.Null)
        {
            return result;
        }


        if (
            reader.TokenType
            !=
            JsonTokenType.StartArray)
        {
            throw new JsonException(
                "Entry options must be an array.");
        }


        while (reader.Read())
        {
            if (
                reader.TokenType
                ==
                JsonTokenType.EndArray)
            {
                return result;
            }


            if (
                reader.TokenType
                ==
                JsonTokenType.String)
            {
                result.Add(
                    new StatBlockEntry
                    {
                        Description =
                            reader.GetString()
                            ??
                            string.Empty
                    });


                continue;
            }


            if (
                reader.TokenType
                ==
                JsonTokenType.StartObject)
            {
                var entry =
                    JsonSerializer
                        .Deserialize<StatBlockEntry>(
                            ref reader,
                            options);


                if (entry is not null)
                {
                    result.Add(
                        entry);
                }


                continue;
            }


            using var ignored =
                JsonDocument.ParseValue(
                    ref reader);
        }


        throw new JsonException(
            "Entry options array was not closed.");
    }


    public override void
        Write(
            Utf8JsonWriter writer,
            List<StatBlockEntry> value,
            JsonSerializerOptions options)
    {
        writer.WriteStartArray();


        foreach (
            var entry
            in value)
        {
            JsonSerializer.Serialize(
                writer,
                entry,
                options);
        }


        writer.WriteEndArray();
    }
}


// =========================================
// LEGENDARY / MYTHIC ACTION COLLECTION
//
// This shared model supports all action
// collection shapes currently used by the
// monster library:
//
// Legacy array:
//
// "legendary_actions": [ ... ]
//
// Header / uses / entries:
//
// "legendary_actions": {
//   "header": "Legendary Actions",
//   "uses": 3,
//   "entries": [ ... ]
// }
//
// Actions-per-round / options:
//
// "legendary_actions": {
//   "actions_per_round": 3,
//   "options": [ ... ]
// }
//
// Mythic actions:
//
// "mythic_actions": {
//   "intro": "...",
//   "options": [ ... ]
// }
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
    // INTRO
    //
    // Used by mythic action sections.
    // =====================================

    [JsonPropertyName("intro")]
    public string Intro { get; set; } =
        string.Empty;


    // =====================================
    // USES
    //
    // Numeric uses such as:
    //
    // "uses": 3
    // =====================================

    [JsonPropertyName("uses")]
    public int Uses { get; set; }


    // =====================================
    // USE TEXT
    //
    // Non-numeric uses such as:
    //
    // "uses": "2/Day"
    // =====================================

    [JsonIgnore]
    public string UsesText { get; set; } =
        string.Empty;


    // =====================================
    // ACTIONS PER ROUND
    //
    // Used by Embered Avatar-style
    // legendary action objects.
    // =====================================

    [JsonPropertyName("actions_per_round")]
    public int ActionsPerRound { get; set; }


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
// LEGENDARY / MYTHIC ACTION JSON CONVERTER
//
// Accepts every collection shape described
// above so individual monster JSON files do
// not need one-off rewrites.
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
        // OBJECT SHAPES
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
            //
            // Accept either:
            //
            // "header"
            // "title"
            // =============================

            if (
                TryReadString(
                    root,
                    "header",
                    out var header)
                ||
                TryReadString(
                    root,
                    "title",
                    out header))
            {
                result.Header =
                    header;
            }


            // =============================
            // INTRO
            // =============================

            if (
                TryReadString(
                    root,
                    "intro",
                    out var intro))
            {
                result.Intro =
                    intro;
            }


            // =============================
            // USES
            //
            // Accept:
            //
            // 3
            // "3"
            // "2/Day"
            // =============================

            if (
                TryGetProperty(
                    root,
                    "uses",
                    out var usesElement))
            {
                if (
                    TryReadInt(
                        usesElement,
                        out var uses))
                {
                    result.Uses =
                        uses;
                }
                else if (
                    usesElement.ValueKind
                    ==
                    JsonValueKind.String)
                {
                    result.UsesText =
                        usesElement.GetString()
                        ??
                        string.Empty;
                }
            }


            // =============================
            // ACTIONS PER ROUND
            // =============================

            if (
                TryGetProperty(
                    root,
                    "actions_per_round",
                    out var roundElement)
                &&
                TryReadInt(
                    roundElement,
                    out var actionsPerRound))
            {
                result.ActionsPerRound =
                    actionsPerRound;
            }


            // =============================
            // ENTRIES / OPTIONS
            //
            // Both names are used by current
            // monster files.
            // =============================

            if (
                (
                    TryGetProperty(
                        root,
                        "entries",
                        out var entriesElement)
                    ||
                    TryGetProperty(
                        root,
                        "options",
                        out entriesElement)
                )
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


        throw new JsonException(
            "Legendary/Mythic actions must be either an array or an object.");
    }


    // =====================================
    // WRITE
    // =====================================

    public override void
        Write(
            Utf8JsonWriter writer,
            StatBlockLegendaryActions value,
            JsonSerializerOptions options)
    {
        writer.WriteStartObject();


        if (
            !string.IsNullOrWhiteSpace(
                value.Header))
        {
            writer.WriteString(
                "header",
                value.Header);
        }


        if (
            !string.IsNullOrWhiteSpace(
                value.Intro))
        {
            writer.WriteString(
                "intro",
                value.Intro);
        }


        if (
            value.ActionsPerRound > 0)
        {
            writer.WriteNumber(
                "actions_per_round",
                value.ActionsPerRound);
        }
        else if (
            !string.IsNullOrWhiteSpace(
                value.UsesText))
        {
            writer.WriteString(
                "uses",
                value.UsesText);
        }
        else if (
            value.Uses > 0)
        {
            writer.WriteNumber(
                "uses",
                value.Uses);
        }


        writer.WritePropertyName(
            "entries");


        JsonSerializer.Serialize(
            writer,
            value.Entries,
            options);


        writer.WriteEndObject();
    }


    // =====================================
    // READ STRING
    // =====================================

    private static bool
        TryReadString(
            JsonElement element,
            string propertyName,
            out string value)
    {
        if (
            TryGetProperty(
                element,
                propertyName,
                out var property)
            &&
            property.ValueKind
            ==
            JsonValueKind.String)
        {
            value =
                property.GetString()
                ??
                string.Empty;


            return true;
        }


        value =
            string.Empty;


        return false;
    }


    // =====================================
    // READ INTEGER
    // =====================================

    private static bool
        TryReadInt(
            JsonElement element,
            out int value)
    {
        if (
            element.ValueKind
            ==
            JsonValueKind.Number
            &&
            element.TryGetInt32(
                out value))
        {
            return true;
        }


        if (
            element.ValueKind
            ==
            JsonValueKind.String
            &&
            int.TryParse(
                element.GetString(),
                out value))
        {
            return true;
        }


        value =
            0;


        return false;
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


                return true;
            }
        }


        value =
            default;


        return false;
    }
}
