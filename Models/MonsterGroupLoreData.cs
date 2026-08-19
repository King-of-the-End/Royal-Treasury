using System.Text.Json.Serialization;

namespace Website_of_Everything.Models;


// =========================================
// MONSTER GROUP LORE
//
// Supports both:
//
// "title": { ... }
//
// and:
//
// "name": { ... }
//
// Group sections may contain:
//
// - section names
// - paragraphs
// - bullets
// - tables
// =========================================

public sealed class MonsterGroupLoreData
{
    // =====================================
    // TITLE
    // =====================================

    [JsonPropertyName("title")]
    public MonsterGroupLoreTitle Title { get; set; } =
        new();


    // =====================================
    // NAME
    // =====================================

    [JsonPropertyName("name")]
    public MonsterGroupLoreTitle Name { get; set; } =
        new();


    // =====================================
    // SECTIONS
    // =====================================

    [JsonPropertyName("sections")]
    public List<MonsterGroupLoreSection> Sections { get; set; } =
        new();


    // =====================================
    // DISPLAY TITLE
    // =====================================

    [JsonIgnore]
    public MonsterGroupLoreTitle DisplayTitle =>
        !string.IsNullOrWhiteSpace(
            Title.Text)

        ? Title

        : Name;
}


// =========================================
// FORMATTED TEXT
//
// Used for titles, section names, bullets
// and table headers.
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
    [JsonPropertyName("name")]
    public MonsterGroupLoreTitle Name { get; set; } =
        new();


    [JsonPropertyName("paragraphs")]
    public List<MonsterGroupLoreParagraph> Paragraphs { get; set; } =
        new();


    [JsonPropertyName("bullets")]
    public List<MonsterGroupLoreTitle> Bullets { get; set; } =
        new();


    [JsonPropertyName("table")]
    public MonsterGroupLoreTable? Table { get; set; }
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


// =========================================
// GROUP LORE TABLE
// =========================================

public sealed class MonsterGroupLoreTable
{
    [JsonPropertyName("headers")]
    public List<MonsterGroupLoreTitle> Headers { get; set; } =
        new();


    [JsonPropertyName("rows")]
    public List<List<string>> Rows { get; set; } =
        new();
}
