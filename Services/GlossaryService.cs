using System.Text.Json;
using Website_of_Everything.Models;

namespace Website_of_Everything.Services;


public sealed class GlossaryService
{
    private readonly IWebHostEnvironment environment;


    private IReadOnlyList<GlossaryEntry>?
        cachedEntries;


    private readonly SemaphoreSlim loadLock =
        new(
            1,
            1);


    /*
     * These glossary entries represent
     * creature types/subtypes.
     *
     * They are ONLY allowed to create
     * glossary overlays in the creature
     * line beneath a stat-block title.
     *
     * This prevents words such as:
     *
     * beast
     * fiend
     * undead
     *
     * from becoming glossary links when
     * they appear inside Actions, Traits,
     * spell descriptions, etc.
     */
    private static readonly HashSet<string>
        CreatureLineOnlyTerms =
            new(
                StringComparer.OrdinalIgnoreCase)
            {
                "Aberration",
                "Beast",
                "Celestial",
                "Construct",
                "Dragon",
                "Elemental",
                "Fey",
                "Fiend",
                "Giant",
                "Humanoid",
                "Monstrosity",
                "Ooze",
                "Plant",
                "Undead",

                // Creature subtypes / tags
                "Eidolos",
                "Petitioner",
                "Yokai"
            };


    public GlossaryService(
        IWebHostEnvironment environment)
    {
        this.environment =
            environment;
    }


    // =====================================
    // GET ENTRIES FOR A CONTEXT
    // =====================================

    public async Task<IReadOnlyList<GlossaryEntry>>
        GetEntriesAsync(
            GlossaryContext context)
    {
        var entries =
            await LoadAsync();


        var contextName =
            context.ToString();


        /*
         * First remove entries that don't
         * apply to the requested context.
         *
         * Creature types are handled by
         * IsAllowedInContext(), which makes
         * them creature-line-only.
         */
        var applicable =
            entries
                .Where(
                    entry =>
                        !string.IsNullOrWhiteSpace(
                            entry.Term))
                .Where(
                    entry =>
                        IsAllowedInContext(
                            entry,
                            context,
                            contextName))
                .ToList();


        /*
         * The same exact case-sensitive term
         * may have:
         *
         * 1. a general definition
         * 2. a context-specific definition
         *
         * If that happens, prefer the
         * context-specific entry.
         */
        var resolved =
            applicable
                .GroupBy(
                    entry =>
                        entry.Term,
                    StringComparer.Ordinal)
                .Select(
                    group =>
                        group.FirstOrDefault(
                            entry =>
                                entry.Contexts.Any(
                                    value =>
                                        string.Equals(
                                            value,
                                            contextName,
                                            StringComparison.OrdinalIgnoreCase)))
                        ??
                        group.First())
                .ToList();


        /*
         * Longest terms first.
         *
         * This ensures longer glossary
         * phrases win over shorter ones.
         *
         * Example:
         *
         * Eidomancy Burn
         *
         * before:
         *
         * Eidomancy
         */
        return
            resolved
                .OrderByDescending(
                    entry =>
                        entry.Term.Length)
                .ThenBy(
                    entry =>
                        entry.Term,
                    StringComparer.Ordinal)
                .ToList();
    }


    // =====================================
    // IS ENTRY ALLOWED IN THIS CONTEXT?
    // =====================================

    private static bool IsAllowedInContext(
        GlossaryEntry entry,
        GlossaryContext context,
        string contextName)
    {
        /*
         * Creature types and creature subtype
         * tags ONLY work in the creature line.
         *
         * Their JSON Contexts value does not
         * matter for this rule.
         */
        if (
            CreatureLineOnlyTerms.Contains(
                entry.Term.Trim()))
        {
            return
                context ==
                GlossaryContext
                    .StatBlockCreatureLine;
        }


        /*
         * Everything else uses the normal
         * glossary context system.
         */
        return
            IsApplicable(
                entry,
                contextName);
    }


    // =====================================
    // NORMAL CONTEXT CHECK
    // =====================================

    private static bool IsApplicable(
        GlossaryEntry entry,
        string contextName)
    {
        /*
         * No contexts means:
         * globally available.
         */
        if (
            entry.Contexts is null
            ||
            entry.Contexts.Count == 0)
        {
            return true;
        }


        return
            entry.Contexts.Any(
                value =>
                    string.Equals(
                        value,
                        contextName,
                        StringComparison.OrdinalIgnoreCase));
    }


    // =====================================
    // LOAD THE GLOSSARY FILE
    // =====================================

    private async Task<IReadOnlyList<GlossaryEntry>>
        LoadAsync()
    {
        if (cachedEntries is not null)
        {
            return cachedEntries;
        }


        await loadLock.WaitAsync();


        try
        {
            if (cachedEntries is not null)
            {
                return cachedEntries;
            }


            var path =
                Path.Combine(
                    environment.WebRootPath,
                    "data",
                    "glossary.json");


            if (!File.Exists(path))
            {
                cachedEntries =
                    Array.Empty<GlossaryEntry>();


                return cachedEntries;
            }


            await using var stream =
                File.OpenRead(path);


            var entries =
                await JsonSerializer
                    .DeserializeAsync<
                        List<GlossaryEntry>
                    >(
                        stream,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive =
                                true
                        });


            cachedEntries =
                entries
                ??
                new List<GlossaryEntry>();


            return cachedEntries;
        }
        finally
        {
            loadLock.Release();
        }
    }
}