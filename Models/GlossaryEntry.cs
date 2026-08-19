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

    Subclass
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