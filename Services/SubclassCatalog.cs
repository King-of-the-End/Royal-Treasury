using System.Globalization;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Website_of_Everything.Services;

// Only metadata is kept for the class listing. Full feature data is opened
// separately when the visitor navigates to a subclass route.
internal sealed record SubclassEntry(string Name, string Slug, string Summary,
    string? ParentFeature, string FilePath);

internal static class SubclassCatalog
{
    internal static string ToSlug(string value) => string.Join("-", value.Trim().ToLowerInvariant()
        .Split(new[] { ' ', '_', '-' }, StringSplitOptions.RemoveEmptyEntries));

    internal static bool IsSafeSlug(string value) => !string.IsNullOrWhiteSpace(value)
        && value.All(c => char.IsLetterOrDigit(c) || c == '-');

    internal static string? FindClassFolder(string webRoot, string slug)
    {
        if (!IsSafeSlug(slug)) return null;
        var root = Path.Combine(webRoot, "data", "classes");
        return Directory.Exists(root) ? Directory.EnumerateDirectories(root)
            .FirstOrDefault(path => string.Equals(ToSlug(Path.GetFileName(path)), slug,
                StringComparison.OrdinalIgnoreCase)) : null;
    }

    internal static IReadOnlyList<SubclassEntry> Read(string classFolder, ILogger logger)
    {
        var entries = new List<SubclassEntry>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        try
        {
            var folder = Directory.EnumerateDirectories(classFolder).FirstOrDefault(path =>
                string.Equals(Path.GetFileName(path), "subclasses", StringComparison.OrdinalIgnoreCase));
            if (folder is null) return entries;

            foreach (var file in Directory.EnumerateFiles(folder, "*.json", SearchOption.TopDirectoryOnly)
                         .OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ThenBy(path => path, StringComparer.Ordinal))
            {
                try
                {
                    using var document = JsonDocument.Parse(File.ReadAllText(file));
                    var root = document.RootElement;
                    var name = String(root, "name");
                    var slug = String(root, "slug") ?? ToSlug(name ?? string.Empty);
                    var summary = String(root, "summary");
                    if (string.IsNullOrWhiteSpace(name) || !IsSafeSlug(slug) ||
                        string.IsNullOrWhiteSpace(summary) || !ValidContent(root))
                    {
                        logger.LogWarning("Skipping incomplete subclass JSON: {File}", file);
                        continue;
                    }
                    if (!seen.Add(slug))
                    {
                        logger.LogWarning("Skipping duplicate subclass slug: {File}", file);
                        continue;
                    }
                    entries.Add(new SubclassEntry(name, slug.ToLowerInvariant(), summary, String(root, "subclass_feature"), file));
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
                {
                    logger.LogWarning(ex, "Unable to load subclass: {File}", file);
                }
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            logger.LogWarning(ex, "Unable to enumerate subclasses in {Folder}", classFolder);
        }
        return entries.OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(entry => entry.Slug, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    internal static JsonDocument Open(SubclassEntry entry)
    {
        var document = JsonDocument.Parse(File.ReadAllText(entry.FilePath));
        var root = document.RootElement;
        var name = String(root, "name");
        var slug = String(root, "slug") ?? ToSlug(name ?? string.Empty);
        if (!ValidContent(root) || string.IsNullOrWhiteSpace(name) ||
            !string.Equals(slug, entry.Slug, StringComparison.OrdinalIgnoreCase))
        {
            document.Dispose();
            throw new JsonException("The subclass data is incomplete or its slug has changed.");
        }
        return document;
    }

    private static string? String(JsonElement root, string key) =>
        root.ValueKind == JsonValueKind.Object && root.TryGetProperty(key, out var item)
        && item.ValueKind == JsonValueKind.String ? item.GetString() : null;

    private static bool ArrayProperty(JsonElement root, string key, out JsonElement array)
    {
        array = default;
        return root.ValueKind == JsonValueKind.Object && root.TryGetProperty(key, out array)
            && array.ValueKind == JsonValueKind.Array;
    }

    internal static int FeatureLevel(JsonElement feature)
    {
        if (feature.ValueKind != JsonValueKind.Object || !feature.TryGetProperty("level", out var level))
            return int.MaxValue;
        return int.TryParse(level.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed : int.MaxValue;
    }

    private static bool ValidContent(JsonElement root) =>
        ArrayProperty(root, "introduction", out var intro) &&
        intro.EnumerateArray().All(item => item.ValueKind == JsonValueKind.String) &&
        ArrayProperty(root, "features", out var features) &&
        features.EnumerateArray().All(feature => !string.IsNullOrWhiteSpace(String(feature, "name"))
            && FeatureLevel(feature) is >= 1 and <= 20 && ArrayProperty(feature, "blocks", out _));
}
