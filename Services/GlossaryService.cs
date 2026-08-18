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
         */
        var applicable =
            entries
                .Where(
                    entry =>
                        IsApplicable(
                            entry,
                            contextName))
                .Where(
                    entry =>
                        !string.IsNullOrWhiteSpace(
                            entry.Term))
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
         * IMPORTANT:
         *
         * Longest terms first.
         *
         * "K Corp Ampule"
         *
         * is therefore tested before:
         *
         * "Ampule"
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
    // CONTEXT CHECK
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
    // LOAD THE ONE GLOSSARY FILE
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