namespace Website_of_Everything.Models;


// =========================================
// WHERE A TERM IS ALLOWED TO ACTIVATE
// =========================================

public enum GlossaryContext
{
    General,
    Spell,
    StatBlock,
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
     * "Burn" does NOT match "burn"
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
     * Empty list = universal.
     *
     * Otherwise:
     *
     * ["Spell"]
     *
     * ["StatBlock"]
     *
     * ["Spell", "Subclass"]
     */
    public List<string> Contexts { get; set; } =
        new();
}