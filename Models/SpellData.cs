using System.Text.Json;
using System.Text.Json.Serialization;

namespace Website_of_Everything.Models;

public sealed class SpellData
{
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("Spell School")]
    public string SpellSchool { get; set; } = string.Empty;

    [JsonPropertyName("Spell Group(s)")]
    public List<string> SpellGroups { get; set; } = new();

    [JsonPropertyName("Casting Time")]
    public string CastingTime { get; set; } = string.Empty;

    public string Range { get; set; } = string.Empty;

    public string Components { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("At Higher Levels")]
    public string AtHigherLevels { get; set; } = string.Empty;

    public List<string> Classes { get; set; } = new();

    [JsonPropertyName("Special Notes")]
    public List<SpellSpecialNote> SpecialNotes { get; set; } = new();

    public List<SpellTable> Tables { get; set; } = new();

    /*
     * Any future JSON properties that are not
     * explicitly listed above are kept here.
     */
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtraFields { get; set; }


    /*
     * These are NOT read from JSON.
     * They are determined from the folder/file.
     */

    [JsonIgnore]
    public int Level { get; set; }

    [JsonIgnore]
    public string LevelFolder { get; set; } = string.Empty;

    [JsonIgnore]
    public string Slug { get; set; } = string.Empty;
}


public sealed class SpellSpecialNote
{
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("table_id")]
    public string TableId { get; set; } = string.Empty;

    public string Placement { get; set; } = string.Empty;

    public string Marker { get; set; } = string.Empty;

    public string Instruction { get; set; } = string.Empty;
}


public sealed class SpellTable
{
    public string Id { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Placement { get; set; } = string.Empty;

    public string Marker { get; set; } = string.Empty;

    public List<string> Columns { get; set; } = new();

    public List<Dictionary<string, JsonElement>> Rows { get; set; } = new();
}