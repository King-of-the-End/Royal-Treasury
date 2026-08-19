using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

internal static class Program
{
    private static readonly Regex LegendaryResistanceNameRegex =
        new(
            @"^Legendary\s+Resistance(?:\s*\(\s*(?<uses>\d+)\s*/\s*Day\s*\))?\.?$",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex LegendaryResistancesTextRegex =
        new(
            @"\bLegendary\s+Resistances\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex LegendaryResistanceTextRegex =
        new(
            @"\bLegendary\s+Resistance\b",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly JsonSerializerOptions WriteOptions =
        new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

    public static int Main(string[] args)
    {
        var projectRoot =
            args.Length > 0
                ? Path.GetFullPath(args[0])
                : Directory.GetCurrentDirectory();

        var monsterRoots =
            FindMonsterRoots(projectRoot)
                .ToArray();

        if (monsterRoots.Length == 0)
        {
            Console.Error.WriteLine(
                $"[Local Monster Fixer] Could not find a monster data directory under " +
                $"'{Path.Combine(projectRoot, "wwwroot", "data")}'.");

            return 2;
        }

        var filesChanged = 0;
        var blocksChanged = 0;
        var parseErrors = 0;

        foreach (var monsterRoot in monsterRoots)
        {
            foreach (
                var filePath
                in Directory.EnumerateFiles(
                    monsterRoot,
                    "*.json",
                    SearchOption.AllDirectories))
            {
                if (
                    string.Equals(
                        Path.GetFileName(filePath),
                        "_manifest.json",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                try
                {
                    var original =
                        File.ReadAllText(filePath);

                    var root =
                        JsonNode.Parse(original);

                    if (root is null)
                    {
                        continue;
                    }

                    var result =
                        ProcessNode(root);

                    if (!result.Changed)
                    {
                        continue;
                    }

                    var updated =
                        root.ToJsonString(WriteOptions)
                        + Environment.NewLine;

                    File.WriteAllText(
                        filePath,
                        updated,
                        new UTF8Encoding(
                            encoderShouldEmitUTF8Identifier: false));

                    filesChanged++;
                    blocksChanged +=
                        result.ConvertedBlocks;

                    Console.WriteLine(
                        $"[Local Monster Fixer] Fixed " +
                        $"{Path.GetRelativePath(projectRoot, filePath)}");
                }
                catch (JsonException ex)
                {
                    parseErrors++;

                    Console.Error.WriteLine(
                        $"[Local Monster Fixer] JSON parse error in '{filePath}': " +
                        ex.Message);
                }
                catch (Exception ex)
                {
                    parseErrors++;

                    Console.Error.WriteLine(
                        $"[Local Monster Fixer] Failed '{filePath}': " +
                        ex.Message);
                }
            }
        }

        Console.WriteLine(
            $"[Local Monster Fixer] Complete. " +
            $"{blocksChanged} stat block(s) converted in " +
            $"{filesChanged} file(s).");

        if (parseErrors > 0)
        {
            Console.Error.WriteLine(
                $"[Local Monster Fixer] {parseErrors} file(s) could not be processed.");

            return 1;
        }

        return 0;
    }

    private static IEnumerable<string> FindMonsterRoots(
        string projectRoot)
    {
        var dataRoot =
            Path.Combine(
                projectRoot,
                "wwwroot",
                "data");

        if (!Directory.Exists(dataRoot))
        {
            yield break;
        }

        foreach (
            var directory
            in Directory.EnumerateDirectories(
                dataRoot,
                "*",
                SearchOption.TopDirectoryOnly))
        {
            var name =
                Path.GetFileName(directory);

            if (
                string.Equals(
                    name,
                    "monster",
                    StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    name,
                    "monsters",
                    StringComparison.OrdinalIgnoreCase))
            {
                yield return directory;
            }
        }
    }

    private static FixResult ProcessNode(
        JsonNode node)
    {
        var changed = false;
        var convertedBlocks = 0;

        if (node is JsonObject jsonObject)
        {
            var conversion =
                ConvertLegendaryResistance(
                    jsonObject);

            changed |=
                conversion.Changed;

            convertedBlocks +=
                conversion.ConvertedBlocks;

            foreach (
                var property
                in jsonObject.ToList())
            {
                if (property.Value is null)
                {
                    continue;
                }

                /*
                 * raw_text is retained as source/reference material.
                 * The structured fields are what StatBlock.razor renders,
                 * so do not rewrite the archival raw transcription.
                 */
                if (
                    string.Equals(
                        property.Key,
                        "raw_text",
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (
                    property.Value
                    is JsonValue jsonValue
                    &&
                    jsonValue.TryGetValue<string>(
                        out var text))
                {
                    var replaced =
                        ReplaceLegendaryResistanceReferences(
                            text);

                    if (
                        !string.Equals(
                            text,
                            replaced,
                            StringComparison.Ordinal))
                    {
                        jsonObject[property.Key] =
                            replaced;

                        changed = true;
                    }

                    continue;
                }

                var childResult =
                    ProcessNode(
                        property.Value);

                changed |=
                    childResult.Changed;

                convertedBlocks +=
                    childResult.ConvertedBlocks;
            }
        }
        else if (node is JsonArray jsonArray)
        {
            for (
                var index = 0;
                index < jsonArray.Count;
                index++)
            {
                var child =
                    jsonArray[index];

                if (child is null)
                {
                    continue;
                }

                if (
                    child
                    is JsonValue jsonValue
                    &&
                    jsonValue.TryGetValue<string>(
                        out var text))
                {
                    var replaced =
                        ReplaceLegendaryResistanceReferences(
                            text);

                    if (
                        !string.Equals(
                            text,
                            replaced,
                            StringComparison.Ordinal))
                    {
                        jsonArray[index] =
                            replaced;

                        changed = true;
                    }

                    continue;
                }

                var childResult =
                    ProcessNode(
                        child);

                changed |=
                    childResult.Changed;

                convertedBlocks +=
                    childResult.ConvertedBlocks;
            }
        }

        return
            new FixResult(
                changed,
                convertedBlocks);
    }

    private static FixResult ConvertLegendaryResistance(
        JsonObject candidate)
    {
        var traitsProperty =
            FindProperty(
                candidate,
                "traits");

        if (
            traitsProperty.Value
            is not JsonArray traits)
        {
            return
                FixResult.None;
        }

        var indexesToRemove =
            new List<int>();

        var uses =
            0;

        for (
            var index = 0;
            index < traits.Count;
            index++)
        {
            if (
                traits[index]
                is not JsonObject trait)
            {
                continue;
            }

            var name =
                GetStringProperty(
                    trait,
                    "name");

            if (
                string.IsNullOrWhiteSpace(
                    name))
            {
                continue;
            }

            var match =
                LegendaryResistanceNameRegex
                    .Match(name.Trim());

            if (!match.Success)
            {
                continue;
            }

            indexesToRemove.Add(index);

            var parsedUses =
                1;

            if (
                match.Groups["uses"].Success
                &&
                int.TryParse(
                    match.Groups["uses"].Value,
                    out var numericUses)
                &&
                numericUses > 0)
            {
                parsedUses =
                    numericUses;
            }

            uses =
                Math.Max(
                    uses,
                    parsedUses);
        }

        if (indexesToRemove.Count == 0)
        {
            return
                FixResult.None;
        }

        for (
            var index =
                indexesToRemove.Count - 1;
            index >= 0;
            index--)
        {
            traits.RemoveAt(
                indexesToRemove[index]);
        }

        EnsureLegendaryReactions(
            candidate,
            uses);

        return
            new FixResult(
                Changed: true,
                ConvertedBlocks: 1);
    }

    private static void EnsureLegendaryReactions(
        JsonObject statBlock,
        int uses)
    {
        var pluralProperty =
            FindProperty(
                statBlock,
                "legendary_reactions");

        var singularProperty =
            FindProperty(
                statBlock,
                "legendary_reaction");

        JsonObject reactions;

        if (
            pluralProperty.Value
            is JsonObject pluralObject)
        {
            reactions =
                pluralObject;
        }
        else if (
            singularProperty.Value
            is JsonObject singularObject)
        {
            reactions =
                (JsonObject)
                    singularObject.DeepClone();

            if (
                singularProperty.Key
                is not null)
            {
                statBlock.Remove(
                    singularProperty.Key);
            }

            statBlock["legendary_reactions"] =
                reactions;
        }
        else
        {
            reactions =
                new JsonObject();

            statBlock["legendary_reactions"] =
                reactions;
        }

        reactions["header"] =
            "Legendary Reactions";

        reactions["uses"] =
            uses;

        var entriesProperty =
            FindProperty(
                reactions,
                "entries");

        if (
            entriesProperty.Value
            is not JsonArray entries
            ||
            entries.Count == 0)
        {
            reactions["entries"] =
                new JsonArray
                {
                    new JsonObject
                    {
                        ["name"] = "TBA",
                        ["name_format"] = "plain",
                        ["description"] = ""
                    }
                };
        }
    }

    private static string ReplaceLegendaryResistanceReferences(
        string text)
    {
        var updated =
            LegendaryResistancesTextRegex
                .Replace(
                    text,
                    "Legendary Reactions");

        updated =
            LegendaryResistanceTextRegex
                .Replace(
                    updated,
                    "Legendary Reactions");

        return
            updated;
    }

    private static string? GetStringProperty(
        JsonObject jsonObject,
        string propertyName)
    {
        var property =
            FindProperty(
                jsonObject,
                propertyName);

        if (
            property.Value
            is JsonValue jsonValue
            &&
            jsonValue.TryGetValue<string>(
                out var text))
        {
            return
                text;
        }

        return
            null;
    }

    private static JsonPropertyMatch FindProperty(
        JsonObject jsonObject,
        string propertyName)
    {
        foreach (
            var property
            in jsonObject)
        {
            if (
                string.Equals(
                    property.Key,
                    propertyName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return
                    new JsonPropertyMatch(
                        property.Key,
                        property.Value);
            }
        }

        return
            new JsonPropertyMatch(
                null,
                null);
    }

    private readonly record struct FixResult(
        bool Changed,
        int ConvertedBlocks)
    {
        public static FixResult None =>
            new(
                Changed: false,
                ConvertedBlocks: 0);
    }

    private readonly record struct JsonPropertyMatch(
        string? Key,
        JsonNode? Value);
}
