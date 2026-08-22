using System.Collections.Concurrent;
using System.Text.Json;
using Website_of_Everything.Models;

namespace Website_of_Everything.Services;

public sealed class SpellService
{
    /*
     * The folder determines the spell level.
     * No Level property is needed in the JSON.
     */
    private static readonly IReadOnlyDictionary<string, int> LevelFolders =
        new Dictionary<string, int>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["cantrips"] = 0,
            ["1st"] = 1,
            ["2nd"] = 2,
            ["3rd"] = 3,
            ["4th"] = 4,
            ["5th"] = 5,
            ["6th"] = 6,
            ["7th"] = 7,
            ["8th"] = 8,
            ["9th"] = 9
        };


    private readonly IWebHostEnvironment _environment;


    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };


    /*
     * SpellService is a singleton now, so this cache is shared by
     * every visitor instead of rebuilding the spell catalogue once
     * per Blazor circuit.
     */
    private readonly ConcurrentDictionary<string, SpellData>
        _spellCache =
            new(StringComparer.OrdinalIgnoreCase);


    /*
     * Only one full-catalogue load can run at a time.
     * Everyone else awaits the same task.
     */
    private readonly object _allSpellsLock =
        new();


    private Task<IReadOnlyList<SpellData>>?
        _allSpellsTask;


    public SpellService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }


    // =====================================
    // GET ALL SPELLS
    // =====================================

    public Task<IReadOnlyList<SpellData>>
        GetAllSpellsAsync()
    {
        lock (_allSpellsLock)
        {
            return
                _allSpellsTask
                ??=
                LoadAllSpellsAsync();
        }
    }


    private async Task<IReadOnlyList<SpellData>>
        LoadAllSpellsAsync()
    {
        var results =
            new List<SpellData>();


        var root =
            Path.Combine(
                _environment.WebRootPath,
                "data",
                "spells");


        if (!Directory.Exists(root))
        {
            return results;
        }


        foreach (var level in LevelFolders)
        {
            var folder =
                Path.Combine(
                    root,
                    level.Key);


            if (!Directory.Exists(folder))
            {
                continue;
            }


            foreach (
                var file in
                Directory.EnumerateFiles(
                    folder,
                    "*.json",
                    SearchOption.TopDirectoryOnly))
            {
                if (
                    Path.GetFileName(file)
                        .Equals(
                            "_manifest.json",
                            StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }


                var slug =
                    Path.GetFileNameWithoutExtension(
                        file);


                var cacheKey =
                    BuildCacheKey(
                        level.Key,
                        slug);


                if (
                    _spellCache.TryGetValue(
                        cacheKey,
                        out var cachedSpell))
                {
                    results.Add(
                        cachedSpell);

                    continue;
                }


                var spell =
                    await LoadSpellFileAsync(
                        file,
                        level.Key,
                        level.Value,
                        slug);


                if (spell is null)
                {
                    continue;
                }


                _spellCache.TryAdd(
                    cacheKey,
                    spell);


                results.Add(
                    spell);
            }
        }


        return
            results
                .OrderBy(
                    spell =>
                        spell.Level)
                .ThenBy(
                    spell =>
                        spell.Name,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();
    }


    // =====================================
    // GET ONE SPELL
    //
    // IMPORTANT PERFORMANCE CHANGE:
    //
    // The old version loaded every spell in the site before it could
    // display one spell-detail page. This version opens only the one
    // requested JSON file unless the full catalogue is already cached.
    // =====================================

    public async Task<SpellData?>
        GetSpellAsync(
            string levelFolder,
            string slug)
    {
        if (
            string.IsNullOrWhiteSpace(levelFolder)
            ||
            string.IsNullOrWhiteSpace(slug))
        {
            return null;
        }


        levelFolder =
            levelFolder.Trim();


        slug =
            slug.Trim();


        if (
            !LevelFolders.TryGetValue(
                levelFolder,
                out var level))
        {
            return null;
        }


        if (!IsSafePathPart(levelFolder)
            ||
            !IsSafePathPart(slug))
        {
            return null;
        }


        var cacheKey =
            BuildCacheKey(
                levelFolder,
                slug);


        if (
            _spellCache.TryGetValue(
                cacheKey,
                out var cachedSpell))
        {
            return cachedSpell;
        }


        var folder =
            Path.Combine(
                _environment.WebRootPath,
                "data",
                "spells",
                levelFolder);


        if (!Directory.Exists(folder))
        {
            return null;
        }


        /*
         * Try the normal exact path first. This avoids directory
         * enumeration for the overwhelmingly common case.
         */
        var file =
            Path.Combine(
                folder,
                $"{slug}.json");


        if (!File.Exists(file))
        {
            /*
             * Linux paths are case-sensitive. Keep a small fallback
             * so old links still work if a filename has different case.
             */
            file =
                Directory
                    .EnumerateFiles(
                        folder,
                        "*.json",
                        SearchOption.TopDirectoryOnly)
                    .FirstOrDefault(
                        path =>
                            string.Equals(
                                Path.GetFileNameWithoutExtension(path),
                                slug,
                                StringComparison.OrdinalIgnoreCase));


            if (file is null)
            {
                return null;
            }
        }


        var resolvedSlug =
            Path.GetFileNameWithoutExtension(
                file);


        var spell =
            await LoadSpellFileAsync(
                file,
                levelFolder,
                level,
                resolvedSlug);


        if (spell is null)
        {
            return null;
        }


        _spellCache.TryAdd(
            cacheKey,
            spell);


        /*
         * Also cache the filename-derived spelling in case the incoming
         * route used different casing.
         */
        _spellCache.TryAdd(
            BuildCacheKey(
                levelFolder,
                resolvedSlug),
            spell);


        return spell;
    }


    // =====================================
    // READ ONE SPELL FILE
    // =====================================

    private async Task<SpellData?>
        LoadSpellFileAsync(
            string file,
            string levelFolder,
            int level,
            string slug)
    {
        try
        {
            await using var stream =
                new FileStream(
                    file,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 16 * 1024,
                    useAsync: true);


            var spell =
                await JsonSerializer
                    .DeserializeAsync<SpellData>(
                        stream,
                        _jsonOptions);


            if (
                spell is null
                ||
                string.IsNullOrWhiteSpace(
                    spell.Name))
            {
                return null;
            }


            spell.Level =
                level;


            spell.LevelFolder =
                levelFolder;


            spell.Slug =
                slug;


            return spell;
        }
        catch (
            JsonException)
        {
            return null;
        }
        catch (
            IOException)
        {
            return null;
        }
    }


    // =====================================
    // HELPERS
    // =====================================

    private static string BuildCacheKey(
        string levelFolder,
        string slug)
    {
        return
            $"{levelFolder.Trim().ToLowerInvariant()}/" +
            slug.Trim().ToLowerInvariant();
    }


    private static bool IsSafePathPart(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }


        return
            value.IndexOfAny(
                new[]
                {
                    '/',
                    '\\',
                    ':',
                    '*',
                    '?',
                    '"',
                    '<',
                    '>',
                    '|'
                })
            < 0
            &&
            value != "."
            &&
            value != "..";
    }
}
