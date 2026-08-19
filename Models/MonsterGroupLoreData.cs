using System.Text.Json.Serialization;

namespace Website_of_Everything.Models;


// =========================================
// MONSTER GROUP LORE
//
// Reads files such as:
//
// wwwroot/data/monster/Groups/Abishai.json
// =========================================

public sealed class MonsterGroupLoreData
{
    [JsonPropertyName("title")]
    public MonsterGroupLoreTitle Title { get; set; } =
        new();


    [JsonPropertyName("sections")]
    public List<MonsterGroupLoreSection> Sections { get; set; } =
        new();
}


// =========================================
// GROUP TITLE
// =========================================

public sealed class MonsterGroupLoreTitle
{
    [JsonPropertyName("text")]
    public string Text { get; set; } =
        string.Empty;


    [JsonPropertyName("bold")]
    public bool Bold { get; set; }


    [JsonPropertyName("italic")]
    public bool Italic { get; set; }
}


// =========================================
// GROUP SECTION
// =========================================

public sealed class MonsterGroupLoreSection
{
    [JsonPropertyName("paragraphs")]
    public List<MonsterGroupLoreParagraph> Paragraphs { get; set; } =
        new();
}


// =========================================
// GROUP PARAGRAPH
// =========================================

public sealed class MonsterGroupLoreParagraph
{
    [JsonPropertyName("spans")]
    public List<MonsterGroupLoreSpan> Spans { get; set; } =
        new();
}


// =========================================
// FORMATTED LORE SPAN
// =========================================

public sealed class MonsterGroupLoreSpan
{
    [JsonPropertyName("text")]
    public string Text { get; set; } =
        string.Empty;


    [JsonPropertyName("bold")]
    public bool Bold { get; set; }


    [JsonPropertyName("italic")]
    public bool Italic { get; set; }
}
