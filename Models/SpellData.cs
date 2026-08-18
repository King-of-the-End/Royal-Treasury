using System.Text.Json;
using System.Text.Json.Serialization;

namespace Website_of_Everything.Models;


public sealed class SpellData
{
    public string Name { get; set; } =
        string.Empty;


    [JsonPropertyName("Spell School")]
    public string SpellSchool { get; set; } =
        string.Empty;


    [JsonPropertyName("Spell Group(s)")]
    public List<string> SpellGroups { get; set; } =
        new();


    [JsonPropertyName("Casting Time")]
    public string CastingTime { get; set; } =
        string.Empty;


    public string Range { get; set; } =
        string.Empty;


    public string Components { get; set; } =
        string.Empty;


    public string Duration { get; set; } =
        string.Empty;


    public string Description { get; set; } =
        string.Empty;


    [JsonPropertyName("At Higher Levels")]
    public string AtHigherLevels { get; set; } =
        string.Empty;


    public List<string> Classes { get; set; } =
        new();


    /*
     * Tags are now explicitly handled.
     *
     * This means Tags will NOT appear as
     * an ExtraFields section.
     */
    public List<string> Tags { get; set; } =
        new();


    // =====================================
    // SPECIAL NOTES
    // =====================================

    [JsonPropertyName("Special Notes")]
    public List<SpellSpecialNote>
        SpecialNotes { get; set; } =
            new();


    // =====================================
    // TABLES
    // =====================================

    public List<SpellTable> Tables { get; set; } =
        new();


    // =====================================
    // STAT BLOCKS
    //
    // THIS is what detects:
    //
    // "Stat Blocks": [...]
    //
    // in any spell JSON.
    // =====================================

    [JsonPropertyName("Stat Blocks")]
    public List<StatBlockData> StatBlocks { get; set; } =
        new();


    // =====================================
    // FUTURE JSON FIELDS
    // =====================================

    [JsonExtensionData]
    public Dictionary<string, JsonElement>?
        ExtraFields { get; set; }


    // =====================================
    // GENERATED FROM FILE / FOLDER
    // =====================================

    [JsonIgnore]
    public int Level { get; set; }


    [JsonIgnore]
    public string LevelFolder { get; set; } =
        string.Empty;


    [JsonIgnore]
    public string Slug { get; set; } =
        string.Empty;
}


// =========================================
// SPECIAL NOTE
// =========================================

public sealed class SpellSpecialNote
{
    public string Type { get; set; } =
        string.Empty;


    // TABLE SUPPORT

    [JsonPropertyName("table_id")]
    public string TableId { get; set; } =
        string.Empty;


    // STAT BLOCK SUPPORT

    [JsonPropertyName("stat_block_id")]
    public string StatBlockId { get; set; } =
        string.Empty;


    public string Placement { get; set; } =
        string.Empty;


    public string Marker { get; set; } =
        string.Empty;


    [JsonPropertyName("section_label")]
    public string SectionLabel { get; set; } =
        string.Empty;


    public string Instruction { get; set; } =
        string.Empty;
}


// =========================================
// TABLE
// =========================================

public sealed class SpellTable
{
    public string Id { get; set; } =
        string.Empty;


    public string Title { get; set; } =
        string.Empty;


    public string Placement { get; set; } =
        string.Empty;


    public string Marker { get; set; } =
        string.Empty;


    public List<string> Columns { get; set; } =
        new();


    public List<
        Dictionary<string, JsonElement>
    > Rows { get; set; } =
        new();
}