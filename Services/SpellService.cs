using System.Text.Json;
using Website_of_Everything.Models;

namespace Website_of_Everything.Services;

public sealed class SpellService
{
    /*
     * The folder determines the spell level.
     *
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
     * Cache the spell files after they have
     * been read once.
     */
    private IReadOnlyList<SpellData>? _cache;


    public SpellService(
        IWebHostEnvironment environment)
    {
        _environment = environment;
    }


    public async Task<IReadOnlyList<SpellData>>
        GetAllSpellsAsync()
    {
        if (_cache is not null)
        {
            return _cache;
        }


        var results =
            new List<SpellData>();


        var root =
            Path.Combine(
                _environment.WebRootPath,
                "data",
                "spells");


        if (!Directory.Exists(root))
        {
            _cache = results;

            return _cache;
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
                    "*.json"))
            {
                /*
                 * Do not attempt to load
                 * _manifest.json as a spell.
                 */
                if (
                    Path.GetFileName(file)
                        .Equals(
                            "_manifest.json",
                            StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }


                try
                {
                    var json =
                        await File.ReadAllTextAsync(
                            file);


                    var spell =
                        JsonSerializer.Deserialize<SpellData>(
                            json,
                            _jsonOptions);


                    if (
                        spell is null ||
                        string.IsNullOrWhiteSpace(
                            spell.Name))
                    {
                        continue;
                    }


                    /*
                     * Level comes from folder.
                     */
                    spell.Level =
                        level.Value;


                    /*
                     * Example:
                     *
                     * cantrips
                     */
                    spell.LevelFolder =
                        level.Key;


                    /*
                     * Example:
                     *
                     * acid-splash.json
                     *
                     * becomes:
                     *
                     * acid-splash
                     */
                    spell.Slug =
                        Path.GetFileNameWithoutExtension(
                            file);


                    results.Add(spell);
                }
                catch (JsonException)
                {
                    /*
                     * One malformed JSON file
                     * should not stop the entire
                     * spell compendium loading.
                     */
                }
            }
        }


        _cache =
            results
                .OrderBy(
                    spell =>
                        spell.Level)
                .ThenBy(
                    spell =>
                        spell.Name,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();


        return _cache;
    }


    public async Task<SpellData?>
        GetSpellAsync(
            string levelFolder,
            string slug)
    {
        var spells =
            await GetAllSpellsAsync();


        return spells
            .FirstOrDefault(
                spell =>
                    spell.LevelFolder.Equals(
                        levelFolder,
                        StringComparison.OrdinalIgnoreCase)
                    &&
                    spell.Slug.Equals(
                        slug,
                        StringComparison.OrdinalIgnoreCase));
    }
}