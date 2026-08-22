namespace Website_of_Everything.Models;


// =========================================
// WHERE A TERM IS ALLOWED TO ACTIVATE
// =========================================

public enum GlossaryContext
{
    General,
    Spell,
    StatBlock,

    /*
     * Special context used ONLY for the
     * size/type/alignment line directly
     * beneath a creature's name.
     *
     * Example:
     *
     * Small beast, unaligned
     *
     * Creature-type glossary entries are
     * restricted to this context.
     */
    StatBlockCreatureLine,

    /*
     * Special context used ONLY for the
     * Quality Traits field inside a
     * creature stat block.
     *
     * Example:
     *
     * Quality Traits  Anchored, Magic Resist
     *
     * This lets glossary terms such as
     * Anchored and Magic Resist activate in
     * the Quality Traits row without turning
     * those same words into glossary links in
     * actions, lore, spells, or other fields.
     */
    StatBlockQualityTrait,

    Subclass,

    /*
     * Text rendered on /classes/{slug}.
     *
     * Class-page include/exclude rules are
     * applied in addition to this context.
     */
    Class
}


// =========================================
// GLOSSARY ENTRY
// =========================================

public sealed class GlossaryEntry
{
    public string Term { get; set; } =
        string.Empty;


    public string Definition { get; set; } =
        string.Empty;


    /*
     * true:
     *
     * "Burn" matches "Burn"
     *
     * but does NOT match:
     *
     * burn
     * BURN
     *
     *
     * false:
     *
     * "Burn" matches:
     *
     * Burn
     * burn
     * BURN
     * BuRn
     */
    public bool CaseSensitive { get; set; } =
        true;


    /*
     * true:
     *
     * The phrase is still detected by the
     * glossary matcher, but it receives no
     * highlight and no tooltip.
     *
     * This is useful for blocking a shorter
     * glossary term inside a longer phrase.
     */
    public bool NoOverlay { get; set; } =
        false;


    /*
     * Enables X/Y/Z/Type-style wildcard
     * placeholders in Term and Aliases in
     * any glossary context.
     *
     * Example:
     *
     * Term: Hemorrhaging(x-y)
     *
     * can match:
     *
     * Hemorrhaging(18-20)
     * Hemorrhaging (17–20)
     *
     * when matching aliases are supplied.
     *
     * Quality-trait glossary entries retain
     * their existing wildcard behaviour even
     * when this is false.
     */
    public bool Wildcard { get; set; } =
        false;


    /*
     * Empty list = universal.
     *
     * Examples:
     *
     * []
     *
     * ["Spell"]
     *
     * ["StatBlock"]
     *
     * ["Spell", "Subclass"]
     *
     *
     * Creature-type entries are handled
     * specially by GlossaryService and only
     * appear in StatBlockCreatureLine even
     * when their Contexts list is empty.
     */
    public List<string> Contexts { get; set; } =
        new();

    /*
     * Optional class-page whitelist.
     *
     * This only affects GlossaryContext.Class.
     *
     * Empty / missing:
     * the entry may appear on every class page
     * (unless ExcludedClassPages blocks it).
     *
     * Non-empty:
     * the entry appears only on the listed
     * class pages.
     *
     * Values may be display names or slugs:
     *
     * "Blood Minister"
     * "blood-minister"
     *
     * are treated as the same class.
     */
    public List<string> ClassPages { get; set; } =
        new();


    /*
     * Optional class-page blacklist.
     *
     * This only affects GlossaryContext.Class.
     * Exclusions always win over ClassPages.
     *
     * Example:
     *
     * ClassPages: []
     * ExcludedClassPages: ["Wizard"]
     *
     * means the term appears on every class
     * page except Wizard.
     */
    public List<string> ExcludedClassPages { get; set; } =
        new();


    /*
     * Optional display title for the popup.
     *
     * Term remains the text used for matching.
     * When this is set, the tooltip heading
     * uses DisplayTitle instead.
     *
     * Example:
     *
     * Term:
     * Belt of Hill Giant Strength
     *
     * DisplayTitle:
     * Belt of Giant Strength
     */
    public string? DisplayTitle { get; set; }


    /*
     * Optional rich-content blocks for the
     * glossary popup.
     *
     * If Blocks is empty, GlossaryText falls
     * back to the ordinary Definition string.
     *
     * Supported block types:
     *
     * source
     * item
     * paragraph
     * list
     * table
     */
    public List<GlossaryBlock> Blocks { get; set; } =
        new();


    /*
     * Optional alternate match patterns.
     *
     * The popup title still uses Term.
     *
     * This is useful when the source data has
     * more than one spelling/form for the same
     * quality trait.
     *
     * Example:
     *
     * Term:
     * Keen Senses (X)
     *
     * Alias:
     * Keen Sense (X)
     *
     * Both open a tooltip titled:
     * Keen Senses (X)
     */
    public List<string> Aliases { get; set; } =
        new();

}


// =========================================
// GLOSSARY RICH-CONTENT BLOCK
// =========================================

public sealed class GlossaryBlock
{
    /*
     * Supported values:
     *
     * source
     * item
     * paragraph
     * list
     * table
     */
    public string Type { get; set; } =
        "paragraph";


    /*
     * Used by source, item, and paragraph.
     */
    public string Text { get; set; } =
        string.Empty;


    /*
     * Used by list.
     */
    public List<string> Items { get; set; } =
        new();


    /*
     * Used by table.
     */
    public List<GlossaryTableColumn> Columns { get; set; } =
        new();


    /*
     * Used by table.
     *
     * Each dictionary is one row. The keys
     * correspond to GlossaryTableColumn.Key.
     */
    public List<Dictionary<string, string>> Rows { get; set; } =
        new();
}


// =========================================
// GLOSSARY TABLE COLUMN
// =========================================

public sealed class GlossaryTableColumn
{
    public string Key { get; set; } =
        string.Empty;


    public string Label { get; set; } =
        string.Empty;
}
