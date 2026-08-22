using System.Collections.Concurrent;
using System.Text.Json;
using Website_of_Everything.Models;

namespace Website_of_Everything.Services;

public sealed class CompendiumJsonService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ConcurrentDictionary<string, Task<IReadOnlyList<CompendiumListEntry>>> _folderCache =
        new(StringComparer.OrdinalIgnoreCase);

    public CompendiumJsonService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Task<IReadOnlyList<CompendiumListEntry>> GetEntriesAsync(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName) || !IsSafeFolderName(folderName))
        {
            return Task.FromResult<IReadOnlyList<CompendiumListEntry>>(Array.Empty<CompendiumListEntry>());
        }

        return _folderCache.GetOrAdd(folderName.Trim(), LoadFolderAsync);
    }

    private async Task<IReadOnlyList<CompendiumListEntry>> LoadFolderAsync(string folderName)
    {
        var root = Path.Combine(_environment.WebRootPath, "data", folderName);

        if (!Directory.Exists(root))
        {
            return Array.Empty<CompendiumListEntry>();
        }

        var entries = new List<CompendiumListEntry>();

        foreach (var file in Directory.EnumerateFiles(root, "*.json", SearchOption.AllDirectories))
        {
            if (Path.GetFileName(file).Equals("_manifest.json", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            try
            {
                await using var stream = File.OpenRead(file);
                using var document = await JsonDocument.ParseAsync(stream);

                if (document.RootElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in document.RootElement.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.Object)
                        {
                            entries.Add(BuildEntry(item, file));
                        }
                    }
                }
                else if (document.RootElement.ValueKind == JsonValueKind.Object)
                {
                    entries.Add(BuildEntry(document.RootElement, file));
                }
            }
            catch (JsonException)
            {
                // Skip malformed JSON so one bad file does not break the whole catalogue.
            }
            catch (IOException)
            {
                // Skip unreadable files and continue loading the rest of the folder.
            }
        }

        return entries
            .Where(entry => !string.IsNullOrWhiteSpace(entry.Name))
            .GroupBy(entry => $"{entry.Name}\u001f{entry.Slug}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static CompendiumListEntry BuildEntry(JsonElement root, string file)
    {
        var slug = Path.GetFileNameWithoutExtension(file);

        return new CompendiumListEntry
        {
            Name = FirstValue(root, "Name", "Background Name", "Feat Name", "Title")
                .DefaultIfBlank(HumanizeSlug(slug)),
            Slug = slug,
            Source = FirstValue(root, "Source", "Source Book", "Book", "Publication"),
            Type = FirstValue(root, "Type", "Feat Type", "Category", "Feat Category"),
            Prerequisite = FirstValue(root, "Prerequisite", "Prerequisites", "Requirement", "Requirements"),
            AbilityScores = FirstValue(root, "Ability Scores", "Ability Score", "Ability Score Increases", "Ability Score Increase"),
            Skills = FirstValue(root, "Skill Proficiencies", "Skill Proficiency", "Skills"),
            Tools = FirstValue(root, "Tool Proficiencies", "Tool Proficiency", "Tools"),
            Languages = FirstValue(root, "Languages", "Language"),
            Feat = FirstValue(root, "Feat", "Origin Feat", "Background Feat", "Starting Feat")
        };
    }

    private static string FirstValue(JsonElement root, params string[] propertyNames)
    {
        if (root.ValueKind != JsonValueKind.Object)
        {
            return string.Empty;
        }

        foreach (var propertyName in propertyNames)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return DisplayValue(property.Value);
                }
            }
        }

        return string.Empty;
    }

    private static string DisplayValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString() ?? string.Empty,
            JsonValueKind.Number => value.ToString(),
            JsonValueKind.True => "Yes",
            JsonValueKind.False => "No",
            JsonValueKind.Array => string.Join(", ", value.EnumerateArray()
                .Select(DisplayValue)
                .Where(text => !string.IsNullOrWhiteSpace(text))),
            JsonValueKind.Object => string.Join(", ", value.EnumerateObject()
                .Select(property => DisplayValue(property.Value))
                .Where(text => !string.IsNullOrWhiteSpace(text))),
            _ => string.Empty
        };
    }

    private static string HumanizeSlug(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug))
        {
            return string.Empty;
        }

        return string.Join(" ", slug
            .Replace('_', '-')
            .Split('-', StringSplitOptions.RemoveEmptyEntries)
            .Select(part => part.Length == 0
                ? part
                : char.ToUpperInvariant(part[0]) + part[1..]));
    }

    private static bool IsSafeFolderName(string folderName) =>
        folderName.All(character => char.IsLetterOrDigit(character) || character is '-' or '_');
}

internal static class CompendiumStringExtensions
{
    public static string DefaultIfBlank(this string value, string fallback) =>
        string.IsNullOrWhiteSpace(value) ? fallback : value;
}
