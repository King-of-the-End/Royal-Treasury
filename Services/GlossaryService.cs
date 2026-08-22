using System.Collections.Concurrent;
using System.Text.Json;
using Website_of_Everything.Models;

namespace Website_of_Everything.Services;


public sealed class GlossaryService
{
    private readonly IWebHostEnvironment environment;


    private IReadOnlyList<GlossaryEntry>?
        cachedEntries;


    private DateTime
        cachedGlossaryLastWriteUtc =
            DateTime.MinValue;


    private readonly SemaphoreSlim loadLock =
        new(
            1,
            1);


    /*
     * GlossaryText can appear dozens or hundreds of times on one page.
     * The old service rebuilt the context/class-page filtered list for
     * every single component. Cache those immutable resolved lists.
     */
    private readonly ConcurrentDictionary<string, IReadOnlyList<GlossaryEntry>>
        resolvedEntriesCache =
            new(StringComparer.Ordinal);


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
                "Yokai",

                // DGR creature-line notation
                "Footprint"
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
            GlossaryContext context,
            string? classPage = null)
    {
        var entries =
            await LoadAsync();


        var contextName =
            context.ToString();


        var normalizedClassPage =
            context == GlossaryContext.Class
            &&
            !string.IsNullOrWhiteSpace(classPage)

            ? NormalizeClassPage(classPage)

            : string.Empty;


        var cacheKey =
            $"{contextName}|{normalizedClassPage}";


        if (
            resolvedEntriesCache.TryGetValue(
                cacheKey,
                out var cachedResolvedEntries))
        {
            return cachedResolvedEntries;
        }


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
                .Where(
                    entry =>
                        context !=
                            GlossaryContext.Class
                        ||
                        IsAllowedOnClassPage(
                            entry,
                            classPage))
                .ToList();


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
                .OrderByDescending(
                    entry =>
                        entry.Term.Length)
                .ThenBy(
                    entry =>
                        entry.Term,
                    StringComparer.Ordinal)
                .ToList();


        resolvedEntriesCache.TryAdd(
            cacheKey,
            resolved);


        return resolved;
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
         *
         * This includes the dedicated
         * StatBlockQualityTrait context used
         * by quality-trait glossary entries.
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
    // CLASS-PAGE INCLUDE / EXCLUDE RULES
    // =====================================

    private static bool IsAllowedOnClassPage(
        GlossaryEntry entry,
        string? classPage)
    {
        var included =
            entry.ClassPages
            ??
            new List<string>();


        var excluded =
            entry.ExcludedClassPages
            ??
            new List<string>();


        /*
         * A restricted entry should not leak
         * onto an unknown class page if the
         * caller forgot to provide its slug.
         */
        if (string.IsNullOrWhiteSpace(classPage))
        {
            return
                included.Count == 0
                &&
                excluded.Count == 0;
        }


        var normalizedPage =
            NormalizeClassPage(classPage);


        /*
         * Exclusion always wins.
         */
        if (
            excluded.Any(
                value =>
                    !string.IsNullOrWhiteSpace(value)
                    &&
                    string.Equals(
                        NormalizeClassPage(value),
                        normalizedPage,
                        StringComparison.Ordinal)))
        {
            return false;
        }


        /*
         * Empty whitelist means all classes.
         */
        if (included.Count == 0)
        {
            return true;
        }


        return
            included.Any(
                value =>
                    !string.IsNullOrWhiteSpace(value)
                    &&
                    string.Equals(
                        NormalizeClassPage(value),
                        normalizedPage,
                        StringComparison.Ordinal));
    }


    private static string NormalizeClassPage(
        string value)
    {
        return
            new string(
                value
                    .Where(char.IsLetterOrDigit)
                    .Select(char.ToLowerInvariant)
                    .ToArray());
    }


    // =====================================
    // LOAD THE GLOSSARY FILE
    // =====================================

    private async Task<IReadOnlyList<GlossaryEntry>>
        LoadAsync()
    {
        var path =
            Path.Combine(
                environment.WebRootPath,
                "data",
                "glossary.json");


        /*
         * Production data only changes on redeploy, so avoid a filesystem
         * metadata lookup for every GlossaryText component. Development
         * keeps the timestamp check so editing glossary.json still updates
         * without restarting the app.
         */
        if (
            cachedEntries is not null
            &&
            !environment.IsDevelopment())
        {
            return cachedEntries;
        }


        var lastWriteUtc =
            File.Exists(path)

            ? File.GetLastWriteTimeUtc(path)

            : DateTime.MinValue;


        /*
         * glossary.json is edited frequently
         * while the site is being built.
         *
         * Do not keep serving a stale
         * singleton cache after the file has
         * changed on disk.
         */
        if (
            cachedEntries is not null
            &&
            cachedGlossaryLastWriteUtc ==
                lastWriteUtc)
        {
            return cachedEntries;
        }


        await loadLock.WaitAsync();


        try
        {
            lastWriteUtc =
                File.Exists(path)

                ? File.GetLastWriteTimeUtc(path)

                : DateTime.MinValue;


            if (
                cachedEntries is not null
                &&
                cachedGlossaryLastWriteUtc ==
                    lastWriteUtc)
            {
                return cachedEntries;
            }


            if (!File.Exists(path))
            {
                cachedEntries =
                    Array.Empty<GlossaryEntry>();


                cachedGlossaryLastWriteUtc =
                    DateTime.MinValue;


                resolvedEntriesCache.Clear();


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


            cachedGlossaryLastWriteUtc =
                lastWriteUtc;


            resolvedEntriesCache.Clear();


            return cachedEntries;
        }
        finally
        {
            loadLock.Release();
        }
    }
}