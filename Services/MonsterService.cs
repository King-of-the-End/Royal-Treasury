using System.Text.Json;
using Website_of_Everything.Models;

namespace Website_of_Everything.Services;


public sealed class MonsterService
{
    // =====================================
    // ENVIRONMENT
    // =====================================

    private readonly IWebHostEnvironment
        environment;


    // =====================================
    // JSON OPTIONS
    // =====================================

    private readonly JsonSerializerOptions
        jsonOptions =
            new()
            {
                PropertyNameCaseInsensitive =
                    true
            };


    // =====================================
    // CACHE
    //
    // Each CR file/folder is cached
    // separately.
    // =====================================

    private readonly Dictionary<
        string,
        IReadOnlyList<StatBlockData>>
        cache =
            new(
                StringComparer.OrdinalIgnoreCase);


    // =====================================
    // MONSTER GROUP LORE CACHE
    //
    // Keyed by a normalized group name.
    //
    // Example:
    //
    // Abishai
    //
    // reads:
    //
    // wwwroot/data/monster/Groups/Abishai.json
    // =====================================

    private readonly Dictionary<
        string,
        MonsterGroupLoreData?>
        groupLoreCache =
            new(
                StringComparer.OrdinalIgnoreCase);


    // =====================================
    // CONSTRUCTOR
    // =====================================

    public MonsterService(
        IWebHostEnvironment environment)
    {
        this.environment =
            environment;
    }


    // =====================================
    // GET MONSTERS FOR CHALLENGE RATING
    //
    // Expected requested values:
    //
    // CR0
    // CR0.125
    // CR0.25
    // CR0.5
    // CR1
    // CR2
    // ...
    // CR30
    //
    // Supports either:
    //
    // wwwroot/data/monster/CR3.json
    //
    // or:
    //
    // wwwroot/data/monster/CR3/*.json
    // =====================================

    public async Task<
        IReadOnlyList<StatBlockData>>
        GetMonstersForChallengeRatingAsync(
            string requestedName)
    {
        if (
            string.IsNullOrWhiteSpace(
                requestedName))
        {
            return
                Array.Empty<StatBlockData>();
        }


        requestedName =
            requestedName.Trim();


        // =================================
        // CACHE
        // =================================

        if (
            cache.TryGetValue(
                requestedName,
                out var cached))
        {
            return cached;
        }


        var results =
            new List<StatBlockData>();


        // =================================
        // FIND MONSTER DATA ROOT
        // =================================

        var monsterRoot =
            FindMonsterRoot();


        if (monsterRoot is null)
        {
            cache[requestedName] =
                Array.Empty<StatBlockData>();


            return cache[requestedName];
        }


        // =================================
        // GET ALL POSSIBLE CR NAMES
        //
        // Example:
        //
        // CR0.25
        //
        // becomes:
        //
        // CR0.25
        // 0.25
        // =================================

        var possibleNames =
            GetPossibleNames(
                requestedName);


        // =================================
        // LOAD AGGREGATE FILE
        //
        // Supports:
        //
        // monster/CR5.json
        //
        // and:
        //
        // monster/5.json
        // =================================

        foreach (
            var possibleName
            in possibleNames)
        {
            var aggregateFile =
                FindJsonFileIgnoreCase(
                    monsterRoot,
                    $"{possibleName}.json");


            if (aggregateFile is null)
            {
                continue;
            }


            var loaded =
                await ReadMonsterFileAsync(
                    aggregateFile);


            results.AddRange(
                loaded);
        }


        // =================================
        // LOAD CR DIRECTORY
        //
        // Supports:
        //
        // monster/CR5/
        //
        // and:
        //
        // monster/5/
        //
        // Each JSON inside the folder may
        // be:
        //
        // - one raw stat block
        // - an array of stat blocks
        // - an outer monster document with
        //   Name / Group / Source /
        //   Information / Stat Blocks
        // =================================

        foreach (
            var possibleName
            in possibleNames)
        {
            var challengeRatingDirectory =
                FindDirectoryIgnoreCase(
                    monsterRoot,
                    possibleName);


            if (
                challengeRatingDirectory
                is null)
            {
                continue;
            }


            foreach (
                var file
                in EnumerateJsonFiles(
                    challengeRatingDirectory))
            {
                /*
                 * Ignore manifest/helper
                 * files if one exists.
                 */
                if (
                    Path.GetFileName(file)
                        .Equals(
                            "_manifest.json",
                            StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }


                var loaded =
                    await ReadMonsterFileAsync(
                        file);


                results.AddRange(
                    loaded);
            }
        }


        // =================================
        // REMOVE DUPLICATES
        //
        // This is useful if both:
        //
        // CR5.json
        //
        // and:
        //
        // CR5/
        //
        // exist.
        // =================================

        var finalResults =
            results
                .Where(
                    monster =>
                        monster is not null
                        &&
                        !string.IsNullOrWhiteSpace(
                            monster.Title))
                .GroupBy(
                    MonsterIdentity,
                    StringComparer.OrdinalIgnoreCase)
                .Select(
                    group =>
                        group.First())
                .OrderBy(
                    monster =>
                        monster.Title,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();


        // =================================
        // CACHE RESULT
        // =================================

        cache[requestedName] =
            finalResults;


        return finalResults;
    }


    // =====================================
    // GET ONE MONSTER
    //
    // Used by the dynamically generated
    // monster detail page:
    //
    // /bestiary/CR3/black-bloated-horker
    // =====================================

    public async Task<StatBlockData?>
        GetMonsterAsync(
            string challengeRating,
            string slug)
    {
        if (
            string.IsNullOrWhiteSpace(
                challengeRating)
            ||
            string.IsNullOrWhiteSpace(
                slug))
        {
            return null;
        }


        var monsters =
            await GetMonstersForChallengeRatingAsync(
                challengeRating);


        var normalizedSlug =
            NormalizeSlug(
                slug);


        return
            monsters.FirstOrDefault(
                monster =>
                    NormalizeSlug(
                        monster.Id)
                        .Equals(
                            normalizedSlug,
                            StringComparison.Ordinal)
                    ||
                    NormalizeSlug(
                        monster.Title)
                        .Equals(
                            normalizedSlug,
                            StringComparison.Ordinal)
                    ||
                    NormalizeSlug(
                        monster.MonsterSetName)
                        .Equals(
                            normalizedSlug,
                            StringComparison.Ordinal));
    }


    // =====================================
    // GET MONSTER STAT-BLOCK VARIANTS
    //
    // Monster documents can contain several
    // entries in their "Stat Blocks" array.
    //
    // Entries inherited from the same outer
    // document receive the same
    // MonsterSetName. This method returns the
    // complete set for the detail page's
    // Monster sub-tabs.
    // =====================================

    public async Task<
        IReadOnlyList<StatBlockData>>
        GetMonsterVariantsAsync(
            string challengeRating,
            string slug)
    {
        if (
            string.IsNullOrWhiteSpace(
                challengeRating)
            ||
            string.IsNullOrWhiteSpace(
                slug))
        {
            return
                Array.Empty<StatBlockData>();
        }


        var monsters =
            await GetMonstersForChallengeRatingAsync(
                challengeRating);


        var normalizedSlug =
            NormalizeSlug(
                slug);


        var selected =
            monsters.FirstOrDefault(
                monster =>
                    NormalizeSlug(
                        monster.Id)
                        .Equals(
                            normalizedSlug,
                            StringComparison.Ordinal)
                    ||
                    NormalizeSlug(
                        monster.Title)
                        .Equals(
                            normalizedSlug,
                            StringComparison.Ordinal)
                    ||
                    NormalizeSlug(
                        monster.MonsterSetName)
                        .Equals(
                            normalizedSlug,
                            StringComparison.Ordinal));


        if (selected is null)
        {
            return
                Array.Empty<StatBlockData>();
        }


        var setName =
            string.IsNullOrWhiteSpace(
                selected.MonsterSetName)

            ? selected.Title

            : selected.MonsterSetName;


        var normalizedSetName =
            NormalizeSlug(
                setName);


        if (
            string.IsNullOrWhiteSpace(
                normalizedSetName))
        {
            return
                new[]
                {
                    selected
                };
        }


        var variants =
            monsters
                .Where(
                    monster =>
                        NormalizeSlug(
                            string.IsNullOrWhiteSpace(
                                monster.MonsterSetName)
                            ? monster.Title
                            : monster.MonsterSetName)
                            .Equals(
                                normalizedSetName,
                                StringComparison.Ordinal))
                .ToList();


        if (variants.Count == 0)
        {
            variants.Add(
                selected);
        }


        return
            variants;
    }


    // =====================================
    // GET MONSTER GROUP LORE
    //
    // Reads the group lore files stored in:
    //
    // wwwroot/data/monster/Groups/
    //
    // The monster document's Group / Groups
    // values are matched against group JSON
    // filenames first, then against the
    // group's title inside the JSON.
    // =====================================

    public async Task<
        IReadOnlyList<MonsterGroupLoreData>>
        GetMonsterGroupLoreAsync(
            IEnumerable<string> groupNames)
    {
        if (groupNames is null)
        {
            return
                Array.Empty<MonsterGroupLoreData>();
        }


        var requestedGroups =
            groupNames
                .Where(
                    group =>
                        !string.IsNullOrWhiteSpace(
                            group))
                .Select(
                    group =>
                        group.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();


        if (requestedGroups.Count == 0)
        {
            return
                Array.Empty<MonsterGroupLoreData>();
        }


        var monsterRoot =
            FindMonsterRoot();


        if (monsterRoot is null)
        {
            return
                Array.Empty<MonsterGroupLoreData>();
        }


        var groupsRoot =
            FindDirectoryIgnoreCase(
                monsterRoot,
                "Groups")
            ??
            FindDirectoryIgnoreCase(
                monsterRoot,
                "Group");


        if (groupsRoot is null)
        {
            return
                Array.Empty<MonsterGroupLoreData>();
        }


        var results =
            new List<MonsterGroupLoreData>();


        foreach (
            var groupName
            in requestedGroups)
        {
            var lore =
                await GetMonsterGroupLoreEntryAsync(
                    groupsRoot,
                    groupName);


            if (lore is null)
            {
                continue;
            }


            if (
                results.Any(
                    existing =>
                        NormalizeSlug(
                            existing.DisplayTitle.Text)
                            .Equals(
                                NormalizeSlug(
                                    lore.DisplayTitle.Text),
                                StringComparison.Ordinal)))
            {
                continue;
            }


            results.Add(
                lore);
        }


        return
            results;
    }


    // =====================================
    // GET ONE GROUP LORE ENTRY
    // =====================================

    private async Task<MonsterGroupLoreData?>
        GetMonsterGroupLoreEntryAsync(
            string groupsRoot,
            string groupName)
    {
        var normalizedGroup =
            NormalizeSlug(
                groupName);


        if (
            string.IsNullOrWhiteSpace(
                normalizedGroup))
        {
            return null;
        }


        if (
            groupLoreCache.TryGetValue(
                normalizedGroup,
                out var cachedLore))
        {
            return cachedLore;
        }


        // =================================
        // FIRST:
        // MATCH THE JSON FILENAME.
        //
        // Abishai -> Abishai.json
        // =================================

        var matchingFile =
            EnumerateJsonFiles(
                groupsRoot)
                .FirstOrDefault(
                    file =>
                        NormalizeSlug(
                            Path.GetFileNameWithoutExtension(
                                file))
                            .Equals(
                                normalizedGroup,
                                StringComparison.Ordinal));


        if (matchingFile is not null)
        {
            var lore =
                await ReadMonsterGroupLoreFileAsync(
                    matchingFile);


            groupLoreCache[normalizedGroup] =
                lore;


            return lore;
        }


        // =================================
        // FALLBACK:
        // MATCH THE TITLE STORED INSIDE
        // EACH GROUP FILE.
        //
        // This means the file itself can use
        // a slightly different filename.
        // =================================

        foreach (
            var file
            in EnumerateJsonFiles(
                groupsRoot))
        {
            var lore =
                await ReadMonsterGroupLoreFileAsync(
                    file);


            if (lore is null)
            {
                continue;
            }


            var normalizedTitle =
                NormalizeSlug(
                    lore.DisplayTitle.Text);


            /*
             * Cache every group title we
             * discover while searching.
             */
            if (
                !string.IsNullOrWhiteSpace(
                    normalizedTitle))
            {
                groupLoreCache[normalizedTitle] =
                    lore;
            }


            if (
                normalizedTitle.Equals(
                    normalizedGroup,
                    StringComparison.Ordinal))
            {
                groupLoreCache[normalizedGroup] =
                    lore;


                return lore;
            }
        }


        groupLoreCache[normalizedGroup] =
            null;


        return null;
    }


    // =====================================
    // READ MONSTER GROUP LORE FILE
    // =====================================

    private async Task<MonsterGroupLoreData?>
        ReadMonsterGroupLoreFileAsync(
            string path)
    {
        try
        {
            var json =
                await File.ReadAllTextAsync(
                    path);


            if (
                string.IsNullOrWhiteSpace(
                    json))
            {
                return null;
            }


            var lore =
                JsonSerializer.Deserialize<
                    MonsterGroupLoreData>(
                    json,
                    jsonOptions);


            if (
                lore is null
                ||
                string.IsNullOrWhiteSpace(
                    lore.DisplayTitle.Text))
            {
                return null;
            }


            return lore;
        }
        catch (JsonException)
        {
            return null;
        }
        catch (IOException)
        {
            return null;
        }
        catch (UnauthorizedAccessException)
        {
            return null;
        }
    }


    // =====================================
    // FIND MONSTER ROOT
    //
    // Preferred:
    //
    // wwwroot/data/Monsters
    //
    // Also accepts:
    //
    // wwwroot/data/monsters
    // wwwroot/data/Monster
    // wwwroot/data/monster
    // =====================================

    private string? FindMonsterRoot()
    {
        if (
            string.IsNullOrWhiteSpace(
                environment.WebRootPath))
        {
            return null;
        }


        var dataRoot =
            Path.Combine(
                environment.WebRootPath,
                "data");


        if (!Directory.Exists(dataRoot))
        {
            return null;
        }


        var preferredNames =
            new[]
            {
                "Monsters",
                "monsters",
                "Monster",
                "monster"
            };


        // =================================
        // TRY EXACT NAMES FIRST
        // =================================

        foreach (
            var preferredName
            in preferredNames)
        {
            var path =
                Path.Combine(
                    dataRoot,
                    preferredName);


            if (Directory.Exists(path))
            {
                return path;
            }
        }


        // =================================
        // FINAL CASE-INSENSITIVE SEARCH
        // =================================

        return
            Directory
                .EnumerateDirectories(
                    dataRoot,
                    "*",
                    SearchOption.TopDirectoryOnly)
                .FirstOrDefault(
                    directory =>
                    {
                        var name =
                            Path.GetFileName(
                                directory);


                        return
                            name.Equals(
                                "Monsters",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            name.Equals(
                                "Monster",
                                StringComparison.OrdinalIgnoreCase);
                    });
    }


    // =====================================
    // POSSIBLE CR NAMES
    //
    // Input:
    //
    // CR0.125
    //
    // Returns:
    //
    // CR0.125
    // 0.125
    //
    // Input:
    //
    // 0.125
    //
    // Returns:
    //
    // 0.125
    // CR0.125
    // =====================================

    private static IReadOnlyList<string>
        GetPossibleNames(
            string requestedName)
    {
        var names =
            new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);


        var cleaned =
            requestedName.Trim();


        names.Add(
            cleaned);


        if (
            cleaned.StartsWith(
                "CR",
                StringComparison.OrdinalIgnoreCase))
        {
            var withoutCr =
                cleaned[2..]
                    .Trim();


            if (
                !string.IsNullOrWhiteSpace(
                    withoutCr))
            {
                names.Add(
                    withoutCr);
            }
        }
        else
        {
            names.Add(
                $"CR{cleaned}");
        }


        return
            names.ToList();
    }


    // =====================================
    // FIND JSON FILE IGNORING CASE
    // =====================================

    private static string?
        FindJsonFileIgnoreCase(
            string directory,
            string fileName)
    {
        if (!Directory.Exists(directory))
        {
            return null;
        }


        return
            EnumerateJsonFiles(
                directory)
                .FirstOrDefault(
                    file =>
                        Path.GetFileName(file)
                            .Equals(
                                fileName,
                                StringComparison.OrdinalIgnoreCase));
    }


    // =====================================
    // ENUMERATE JSON FILES
    //
    // The extension comparison is itself
    // case-insensitive so .JSON also works.
    // =====================================

    private static IEnumerable<string>
        EnumerateJsonFiles(
            string directory)
    {
        if (!Directory.Exists(directory))
        {
            return
                Enumerable.Empty<string>();
        }


        return
            Directory
                .EnumerateFiles(
                    directory,
                    "*",
                    SearchOption.TopDirectoryOnly)
                .Where(
                    file =>
                        Path.GetExtension(file)
                            .Equals(
                                ".json",
                                StringComparison.OrdinalIgnoreCase))
                .OrderBy(
                    file =>
                        file,
                    StringComparer.OrdinalIgnoreCase);
    }


    // =====================================
    // FIND DIRECTORY IGNORING CASE
    // =====================================

    private static string?
        FindDirectoryIgnoreCase(
            string root,
            string directoryName)
    {
        if (!Directory.Exists(root))
        {
            return null;
        }


        return
            Directory
                .EnumerateDirectories(
                    root,
                    "*",
                    SearchOption.TopDirectoryOnly)
                .FirstOrDefault(
                    directory =>
                        Path.GetFileName(directory)
                            .Equals(
                                directoryName,
                                StringComparison.OrdinalIgnoreCase));
    }


    // =====================================
    // READ MONSTER FILE
    //
    // Supports all of these structures:
    //
    // [
    //   { monster },
    //   { monster }
    // ]
    //
    // {
    //   "monsters": [...]
    // }
    //
    // {
    //   "Name": "Black Bloated Horker",
    //   "Group": "Abishai",
    //   "Source": "...",
    //   "Information": "...",
    //   "Stat Blocks": [ ... ]
    // }
    //
    // or a single raw StatBlockData object.
    // =====================================

    private async Task<
        IReadOnlyList<StatBlockData>>
        ReadMonsterFileAsync(
            string path)
    {
        try
        {
            var json =
                await File.ReadAllTextAsync(
                    path);


            if (
                string.IsNullOrWhiteSpace(
                    json))
            {
                return
                    Array.Empty<StatBlockData>();
            }


            using var document =
                JsonDocument.Parse(
                    json);


            var results =
                new List<StatBlockData>();


            ReadElement(
                document.RootElement,
                results,
                MonsterMetadata.Empty);


            return results;
        }
        catch (JsonException)
        {
            /*
             * Ignore malformed monster
             * files rather than breaking
             * the entire Bestiary.
             */
            return
                Array.Empty<StatBlockData>();
        }
        catch (IOException)
        {
            return
                Array.Empty<StatBlockData>();
        }
        catch (UnauthorizedAccessException)
        {
            return
                Array.Empty<StatBlockData>();
        }
    }


    // =====================================
    // READ ELEMENT
    //
    // This recursively handles arrays,
    // generic collection wrappers, the
    // project's "Stat Blocks" wrapper, and
    // raw stat block objects.
    // =====================================

    private void ReadElement(
        JsonElement element,
        List<StatBlockData> results,
        MonsterMetadata inheritedMetadata)
    {
        // =================================
        // ARRAY
        // =================================

        if (
            element.ValueKind ==
            JsonValueKind.Array)
        {
            foreach (
                var child
                in element.EnumerateArray())
            {
                ReadElement(
                    child,
                    results,
                    inheritedMetadata);
            }


            return;
        }


        // =================================
        // ONLY OBJECTS CAN REPRESENT A
        // MONSTER DOCUMENT / STAT BLOCK
        // =================================

        if (
            element.ValueKind !=
            JsonValueKind.Object)
        {
            return;
        }


        // =================================
        // READ OUTER METADATA
        // =================================

        var localMetadata =
            ReadMetadata(
                element);


        var metadata =
            MergeMetadata(
                inheritedMetadata,
                localMetadata);


        // =================================
        // PROJECT MONSTER DOCUMENT FORMAT
        //
        // "Stat Blocks"
        //
        // Normalized matching also accepts:
        //
        // stat_blocks
        // statBlocks
        // StatBlocks
        // stat-blocks
        // =================================

        if (
            TryGetNormalizedProperty(
                element,
                "statblocks",
                out var statBlocks))
        {
            if (
                statBlocks.ValueKind ==
                JsonValueKind.Array)
            {
                foreach (
                    var statBlock
                    in statBlocks.EnumerateArray())
                {
                    AddMonster(
                        statBlock,
                        results,
                        metadata);
                }


                return;
            }


            if (
                statBlocks.ValueKind ==
                JsonValueKind.Object)
            {
                AddMonster(
                    statBlocks,
                    results,
                    metadata);


                return;
            }
        }


        // =================================
        // GENERIC COLLECTION WRAPPERS
        // =================================

        var collectionNames =
            new[]
            {
                "monsters",
                "creatures",
                "entries"
            };


        foreach (
            var collectionName
            in collectionNames)
        {
            if (
                !TryGetNormalizedProperty(
                    element,
                    collectionName,
                    out var collection))
            {
                continue;
            }


            if (
                collection.ValueKind ==
                JsonValueKind.Array)
            {
                foreach (
                    var child
                    in collection.EnumerateArray())
                {
                    ReadElement(
                        child,
                        results,
                        metadata);
                }


                return;
            }


            if (
                collection.ValueKind ==
                JsonValueKind.Object)
            {
                ReadElement(
                    collection,
                    results,
                    metadata);


                return;
            }
        }


        // =================================
        // OTHERWISE:
        // SINGLE RAW STAT BLOCK OBJECT
        // =================================

        AddMonster(
            element,
            results,
            metadata);
    }


    // =====================================
    // DESERIALIZE ONE MONSTER STAT BLOCK
    // =====================================

    private void AddMonster(
        JsonElement element,
        List<StatBlockData> results,
        MonsterMetadata metadata)
    {
        if (
            element.ValueKind !=
            JsonValueKind.Object)
        {
            return;
        }


        try
        {
            var monster =
                element.Deserialize<
                    StatBlockData>(
                    jsonOptions);


            if (monster is null)
            {
                return;
            }


            // =================================
            // TITLE FALLBACK
            //
            // A raw stat block normally has
            // "title". If it does not, use
            // the outer document's "Name".
            // =================================

            if (
                string.IsNullOrWhiteSpace(
                    monster.Title)
                &&
                !string.IsNullOrWhiteSpace(
                    metadata.Name))
            {
                monster.Title =
                    metadata.Name;
            }


            /*
             * A title is required for the
             * Bestiary table and detail URL.
             */
            if (
                string.IsNullOrWhiteSpace(
                    monster.Title))
            {
                return;
            }


            // =================================
            // MONSTER SET NAME
            //
            // Preserve the outer monster
            // document name so several stat
            // blocks from the same JSON file
            // can be presented as variants on
            // one Monster detail page.
            // =================================

            monster.MonsterSetName =
                !string.IsNullOrWhiteSpace(
                    metadata.Name)

                ? metadata.Name.Trim()

                : monster.Title.Trim();


            // =================================
            // OUTER INFORMATION
            // =================================

            if (
                string.IsNullOrWhiteSpace(
                    monster.Information)
                &&
                !string.IsNullOrWhiteSpace(
                    metadata.Information))
            {
                monster.Information =
                    metadata.Information;
            }


            // =================================
            // OUTER INFORMATION SECTIONS
            // =================================

            monster.InformationSections ??=
                new List<MonsterInformationSection>();


            foreach (
                var informationSection
                in metadata.InformationSections)
            {
                if (
                    string.IsNullOrWhiteSpace(
                        informationSection.Text))
                {
                    continue;
                }


                monster.InformationSections.Add(
                    informationSection);
            }


            // =================================
            // OUTER IMAGE
            //
            // Monster files such as
            // black-bloated-horker.json store
            // Image beside Name / Group /
            // Source rather than inside the
            // Stat Blocks array.
            // =================================

            if (
                string.IsNullOrWhiteSpace(
                    monster.Image)
                &&
                !string.IsNullOrWhiteSpace(
                    metadata.Image))
            {
                monster.Image =
                    metadata.Image;
            }


            // =================================
            // OUTER IMAGE VARIANTS
            //
            // The standard Image remains Image 1.
            // These URLs become Image 2 onward.
            // =================================

            monster.ImageVariants ??=
                new List<string>();


            foreach (
                var imageVariant
                in metadata.ImageVariants)
            {
                if (
                    string.IsNullOrWhiteSpace(
                        imageVariant)
                    ||
                    imageVariant.Equals(
                        monster.Image,
                        StringComparison.OrdinalIgnoreCase)
                    ||
                    monster.ImageVariants.Any(
                        existing =>
                            existing.Equals(
                                imageVariant,
                                StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }


                monster.ImageVariants.Add(
                    imageVariant);
            }


            // =================================
            // OUTER REFINEMENT
            // =================================

            if (
                monster.Refinement is null
                &&
                metadata.Refinement is not null)
            {
                monster.Refinement =
                    metadata.Refinement;
            }


            // =================================
            // OUTER BARDING TABLE
            //
            // Some monster files keep a generic
            // lore table beside Information,
            // Image, Source, etc.
            // =================================

            if (
                monster.Barding is null
                &&
                metadata.Barding is not null)
            {
                monster.Barding =
                    metadata.Barding;
            }


            // =================================
            // OUTER GENERIC LORE TABLES
            //
            // These are displayed on the Lore
            // tab by MonsterDetails. This keeps
            // reference material such as the
            // Sword of Damocles Godfall Cannon
            // table out of the stat block.
            // =================================

            monster.LoreTables ??=
                new List<MonsterLoreTableData>();


            foreach (
                var loreTable
                in metadata.LoreTables)
            {
                if (
                    monster.LoreTables.Any(
                        existing =>
                            SameLoreTable(
                                existing,
                                loreTable)))
                {
                    continue;
                }


                monster.LoreTables.Add(
                    loreTable);
            }


            // =================================
            // OUTER GROUP / SOURCE
            //
            // These values live outside the
            // Stat Blocks array in files such
            // as black-bloated-horker.json.
            // =================================

            ApplyGroups(
                monster,
                metadata.Groups);


            ApplySources(
                monster,
                metadata.Sources);


            results.Add(
                monster);
        }
        catch (JsonException)
        {
            /*
             * Skip only the malformed entry.
             */
        }
    }


    // =====================================
    // READ OUTER MONSTER METADATA
    // =====================================

    private MonsterMetadata
        ReadMetadata(
            JsonElement element)
    {
        var name =
            ReadStringProperty(
                element,
                "Name");


        var information =
            ReadStringProperty(
                element,
                "Information");


        var informationSections =
            ReadInformationSectionsProperty(
                element);


        var image =
            ReadStringProperty(
                element,
                "Image");


        var imageVariants =
            DistinctValues(
                ReadStringOrArrayProperty(
                    element,
                    "Image Variants")
                .Concat(
                    ReadStringOrArrayProperty(
                        element,
                        "Images"))
                .Concat(
                    ReadImageTabUrls(
                        element))
                .Where(
                    candidate =>
                        !string.IsNullOrWhiteSpace(
                            candidate)
                        &&
                        !candidate.Equals(
                            image,
                            StringComparison.OrdinalIgnoreCase)));


        var refinement =
            ReadRefinementProperty(
                element);


        var barding =
            ReadLoreTableProperty(
                element,
                "Barding");


        var loreTables =
            ReadLoreTablesProperty(
                element);


        var groups =
            DistinctValues(
                ReadStringOrArrayProperty(
                    element,
                    "Group")
                .Concat(
                    ReadStringOrArrayProperty(
                        element,
                        "Groups")));


        var sources =
            DistinctValues(
                ReadStringOrArrayProperty(
                    element,
                    "Source")
                .Concat(
                    ReadStringOrArrayProperty(
                        element,
                        "Sources")));


        return
            new MonsterMetadata(
                name,
                information,
                informationSections,
                image,
                imageVariants,
                refinement,
                barding,
                loreTables,
                groups,
                sources);
    }


    // =====================================
    // MERGE INHERITED + LOCAL METADATA
    // =====================================

    private static MonsterMetadata
        MergeMetadata(
            MonsterMetadata inherited,
            MonsterMetadata local)
    {
        var name =
            !string.IsNullOrWhiteSpace(
                local.Name)

            ? local.Name

            : inherited.Name;


        var information =
            !string.IsNullOrWhiteSpace(
                local.Information)

            ? local.Information

            : inherited.Information;


        var informationSections =
            local.InformationSections.Count > 0

            ? local.InformationSections

            : inherited.InformationSections;


        var image =
            !string.IsNullOrWhiteSpace(
                local.Image)

            ? local.Image

            : inherited.Image;


        var imageVariants =
            DistinctValues(
                inherited.ImageVariants
                    .Concat(
                        local.ImageVariants));


        var refinement =
            local.Refinement
            ??
            inherited.Refinement;


        var barding =
            local.Barding
            ??
            inherited.Barding;


        var loreTables =
            MergeLoreTables(
                inherited.LoreTables,
                local.LoreTables);


        var groups =
            DistinctValues(
                inherited.Groups
                    .Concat(
                        local.Groups));


        var sources =
            DistinctValues(
                inherited.Sources
                    .Concat(
                        local.Sources));


        return
            new MonsterMetadata(
                name,
                information,
                informationSections,
                image,
                imageVariants,
                refinement,
                barding,
                loreTables,
                groups,
                sources);
    }


    // =====================================
    // APPLY GROUPS TO STAT BLOCK
    // =====================================

    private static void ApplyGroups(
        StatBlockData monster,
        IReadOnlyList<string> groups)
    {
        if (groups.Count == 0)
        {
            return;
        }


        monster.Groups ??=
            new List<string>();


        if (
            string.IsNullOrWhiteSpace(
                monster.Group))
        {
            monster.Group =
                groups[0];
        }


        foreach (
            var group
            in groups)
        {
            if (
                string.IsNullOrWhiteSpace(
                    group))
            {
                continue;
            }


            if (
                string.Equals(
                    monster.Group,
                    group,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }


            if (
                monster.Groups.Any(
                    existing =>
                        string.Equals(
                            existing,
                            group,
                            StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }


            monster.Groups.Add(
                group);
        }
    }


    // =====================================
    // APPLY SOURCES TO STAT BLOCK
    // =====================================

    private static void ApplySources(
        StatBlockData monster,
        IReadOnlyList<string> sources)
    {
        if (sources.Count == 0)
        {
            return;
        }


        monster.Sources ??=
            new List<string>();


        if (
            string.IsNullOrWhiteSpace(
                monster.Source))
        {
            monster.Source =
                sources[0];
        }


        foreach (
            var source
            in sources)
        {
            if (
                string.IsNullOrWhiteSpace(
                    source))
            {
                continue;
            }


            if (
                string.Equals(
                    monster.Source,
                    source,
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }


            if (
                monster.Sources.Any(
                    existing =>
                        string.Equals(
                            existing,
                            source,
                            StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }


            monster.Sources.Add(
                source);
        }
    }


    // =====================================
    // READ INFORMATION SECTIONS
    //
    // Supported outer property:
    //
    // "Information Sections": [
    //   {
    //     "Title": "...",
    //     "Text": "...",
    //     "IsQuote": true
    //   }
    // ]
    // =====================================

    private IReadOnlyList<MonsterInformationSection>
        ReadInformationSectionsProperty(
            JsonElement element)
    {
        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    "Information Sections"),
                out var value)
            ||
            value.ValueKind !=
            JsonValueKind.Array)
        {
            return
                Array.Empty<MonsterInformationSection>();
        }


        var sections =
            new List<MonsterInformationSection>();


        foreach (
            var sectionElement
            in value.EnumerateArray())
        {
            if (
                sectionElement.ValueKind !=
                JsonValueKind.Object)
            {
                continue;
            }


            var title =
                ReadStringProperty(
                    sectionElement,
                    "Title");


            var text =
                ReadStringProperty(
                    sectionElement,
                    "Text");


            if (
                string.IsNullOrWhiteSpace(
                    text))
            {
                continue;
            }


            var isQuote = false;


            if (
                TryGetNormalizedProperty(
                    sectionElement,
                    NormalizePropertyName(
                        "IsQuote"),
                    out var quoteValue))
            {
                isQuote =
                    quoteValue.ValueKind switch
                    {
                        JsonValueKind.True =>
                            true,

                        JsonValueKind.False =>
                            false,

                        JsonValueKind.String =>
                            bool.TryParse(
                                quoteValue.GetString(),
                                out var parsed)
                            && parsed,

                        _ =>
                            false
                    };
            }


            // Also recognize text that is explicitly wrapped
            // in quotation marks. This keeps older monster
            // data working even if IsQuote was omitted.
            var trimmedText =
                text.Trim();


            if (
                !isQuote
                &&
                ((trimmedText.StartsWith('“')
                  && trimmedText.EndsWith('”'))
                 ||
                 (trimmedText.StartsWith('\"')
                  && trimmedText.EndsWith('\"'))))
            {
                isQuote = true;
            }


            sections.Add(
                new MonsterInformationSection
                {
                    Title = title,
                    Text = text,
                    IsQuote = isQuote
                });
        }


        return sections;
    }


    // =====================================
    // READ REFINEMENT PROPERTY
    // =====================================

    private MonsterRefinementData?
        ReadRefinementProperty(
            JsonElement element)
    {
        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    "Refinement"),
                out var value)
            ||
            value.ValueKind !=
            JsonValueKind.Object)
        {
            return null;
        }


        try
        {
            return
                JsonSerializer.Deserialize<
                    MonsterRefinementData>(
                        value.GetRawText(),
                        jsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }


    // =====================================
    // READ GENERIC LORE TABLE PROPERTY
    //
    // Reuses MonsterGroupLoreTable for
    // outer monster tables such as Barding.
    // =====================================

    private MonsterGroupLoreTable?
        ReadLoreTableProperty(
            JsonElement element,
            string propertyName)
    {
        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    propertyName),
                out var value)
            ||
            value.ValueKind !=
            JsonValueKind.Object)
        {
            return null;
        }


        try
        {
            return
                JsonSerializer.Deserialize<
                    MonsterGroupLoreTable>(
                        value.GetRawText(),
                        jsonOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }


    // =====================================
    // READ GENERIC LORE TABLES
    //
    // Supported outer property names:
    //
    // "Tables"
    // "Lore Tables"
    // "LoreTables"
    // "lore_tables"
    //
    // A single object is also accepted for
    // convenience, though an array is the
    // preferred representation.
    // =====================================

    private IReadOnlyList<MonsterLoreTableData>
        ReadLoreTablesProperty(
            JsonElement element)
    {
        JsonElement value;


        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    "Tables"),
                out value)
            &&
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    "Lore Tables"),
                out value))
        {
            return
                Array.Empty<MonsterLoreTableData>();
        }


        try
        {
            if (
                value.ValueKind ==
                JsonValueKind.Array)
            {
                return
                    JsonSerializer.Deserialize<
                        List<MonsterLoreTableData>>(
                            value.GetRawText(),
                            jsonOptions)
                    ??
                    new List<MonsterLoreTableData>();
            }


            if (
                value.ValueKind ==
                JsonValueKind.Object)
            {
                var single =
                    JsonSerializer.Deserialize<
                        MonsterLoreTableData>(
                            value.GetRawText(),
                            jsonOptions);


                return
                    single is null

                    ? Array.Empty<MonsterLoreTableData>()

                    : new[]
                    {
                        single
                    };
            }
        }
        catch (JsonException)
        {
            // Ignore malformed optional lore tables.
        }


        return
            Array.Empty<MonsterLoreTableData>();
    }


    // =====================================
    // MERGE GENERIC LORE TABLES
    // =====================================

    private static IReadOnlyList<MonsterLoreTableData>
        MergeLoreTables(
            IEnumerable<MonsterLoreTableData> inherited,
            IEnumerable<MonsterLoreTableData> local)
    {
        var result =
            new List<MonsterLoreTableData>();


        foreach (
            var table
            in inherited.Concat(local))
        {
            if (
                result.Any(
                    existing =>
                        SameLoreTable(
                            existing,
                            table)))
            {
                continue;
            }


            result.Add(
                table);
        }


        return
            result;
    }


    // =====================================
    // SAME GENERIC LORE TABLE
    // =====================================

    private static bool SameLoreTable(
        MonsterLoreTableData left,
        MonsterLoreTableData right)
    {
        if (
            !string.IsNullOrWhiteSpace(
                left.Title)
            ||
            !string.IsNullOrWhiteSpace(
                right.Title))
        {
            return
                left.Title.Trim().Equals(
                    right.Title.Trim(),
                    StringComparison.OrdinalIgnoreCase);
        }


        return
            left.Columns.SequenceEqual(
                right.Columns,
                StringComparer.OrdinalIgnoreCase)
            &&
            left.Rows.Count ==
            right.Rows.Count;
    }


    // =====================================
    // READ STRING PROPERTY
    // =====================================

    private static string ReadStringProperty(
        JsonElement element,
        string propertyName)
    {
        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    propertyName),
                out var value))
        {
            return
                string.Empty;
        }


        if (
            value.ValueKind !=
            JsonValueKind.String)
        {
            return
                string.Empty;
        }


        return
            value.GetString()
                ?.Trim()
            ??
            string.Empty;
    }


    // =====================================
    // READ IMAGE TAB URLS
    //
    // Supports outer monster documents that
    // store image variants as:
    //
    // "Image Tabs": [
    //   { "Label": "Image 1", "Url": "..." },
    //   { "Label": "Image 2", "Url": "..." }
    // ]
    //
    // Labels are intentionally ignored here;
    // MonsterDetails currently labels the tabs
    // in image order as Image 1, Image 2, etc.
    // =====================================

    private static IReadOnlyList<string>
        ReadImageTabUrls(
            JsonElement element)
    {
        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    "Image Tabs"),
                out var tabs)
            ||
            tabs.ValueKind !=
            JsonValueKind.Array)
        {
            return
                Array.Empty<string>();
        }


        var results =
            new List<string>();


        foreach (
            var tab
            in tabs.EnumerateArray())
        {
            if (
                tab.ValueKind !=
                JsonValueKind.Object)
            {
                continue;
            }


            var url =
                ReadStringProperty(
                    tab,
                    "Url");


            if (
                string.IsNullOrWhiteSpace(
                    url)
                ||
                results.Any(
                    existing =>
                        existing.Equals(
                            url,
                            StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }


            results.Add(
                url);
        }


        return
            results;
    }


    // =====================================
    // READ STRING OR STRING ARRAY
    // =====================================

    private static IReadOnlyList<string>
        ReadStringOrArrayProperty(
            JsonElement element,
            string propertyName)
    {
        if (
            !TryGetNormalizedProperty(
                element,
                NormalizePropertyName(
                    propertyName),
                out var value))
        {
            return
                Array.Empty<string>();
        }


        // =================================
        // ONE STRING
        // =================================

        if (
            value.ValueKind ==
            JsonValueKind.String)
        {
            var text =
                value.GetString()
                    ?.Trim();


            if (
                string.IsNullOrWhiteSpace(
                    text))
            {
                return
                    Array.Empty<string>();
            }


            return
                new[]
                {
                    text
                };
        }


        // =================================
        // ARRAY OF STRINGS
        // =================================

        if (
            value.ValueKind ==
            JsonValueKind.Array)
        {
            return
                value
                    .EnumerateArray()
                    .Where(
                        item =>
                            item.ValueKind ==
                            JsonValueKind.String)
                    .Select(
                        item =>
                            item.GetString()
                                ?.Trim()
                            ??
                            string.Empty)
                    .Where(
                        text =>
                            !string.IsNullOrWhiteSpace(
                                text))
                    .Distinct(
                        StringComparer.OrdinalIgnoreCase)
                    .ToList();
        }


        return
            Array.Empty<string>();
    }


    // =====================================
    // NORMALIZED PROPERTY LOOKUP
    //
    // These all normalize to "statblocks":
    //
    // Stat Blocks
    // stat_blocks
    // stat-blocks
    // statBlocks
    // StatBlocks
    // =====================================

    private static bool
        TryGetNormalizedProperty(
            JsonElement element,
            string normalizedPropertyName,
            out JsonElement value)
    {
        foreach (
            var property
            in element.EnumerateObject())
        {
            if (
                NormalizePropertyName(
                    property.Name)
                    .Equals(
                        normalizedPropertyName,
                        StringComparison.Ordinal))
            {
                value =
                    property.Value;


                return true;
            }
        }


        value =
            default;


        return false;
    }


    // =====================================
    // NORMALIZE PROPERTY NAME
    // =====================================

    private static string
        NormalizePropertyName(
            string value)
    {
        if (
            string.IsNullOrWhiteSpace(
                value))
        {
            return
                string.Empty;
        }


        return
            new string(
                value
                    .Where(
                        char.IsLetterOrDigit)
                    .Select(
                        char.ToLowerInvariant)
                    .ToArray());
    }


    // =====================================
    // NORMALIZE MONSTER URL SLUG
    //
    // Examples:
    //
    // black_bloated_horker
    // Black Bloated Horker
    // black-bloated-horker
    //
    // all become:
    //
    // black-bloated-horker
    // =====================================

    private static string NormalizeSlug(
        string? value)
    {
        if (
            string.IsNullOrWhiteSpace(
                value))
        {
            return
                string.Empty;
        }


        var result =
            new List<char>();


        var needsSeparator =
            false;


        foreach (
            var character
            in value.Trim().ToLowerInvariant())
        {
            if (char.IsLetterOrDigit(character))
            {
                if (
                    needsSeparator
                    &&
                    result.Count > 0)
                {
                    result.Add('-');
                }


                result.Add(character);


                needsSeparator =
                    false;
            }
            else
            {
                needsSeparator =
                    result.Count > 0;
            }
        }


        return
            new string(
                result.ToArray());
    }


    // =====================================
    // DISTINCT NONEMPTY VALUES
    // =====================================

    private static IReadOnlyList<string>
        DistinctValues(
            IEnumerable<string> values)
    {
        return
            values
                .Where(
                    value =>
                        !string.IsNullOrWhiteSpace(
                            value))
                .Select(
                    value =>
                        value.Trim())
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .ToList();
    }


    // =====================================
    // DUPLICATE IDENTITY
    //
    // Prefer monster ID.
    //
    // If there is no ID, use title.
    // =====================================

    private static string MonsterIdentity(
        StatBlockData monster)
    {
        if (
            !string.IsNullOrWhiteSpace(
                monster.Id))
        {
            return
                $"id:{monster.Id.Trim()}";
        }


        return
            $"title:{monster.Title.Trim()}";
    }


    // =====================================
    // OUTER MONSTER FILE METADATA
    // =====================================

    private sealed record MonsterMetadata(
        string Name,
        string Information,
        IReadOnlyList<MonsterInformationSection> InformationSections,
        string Image,
        IReadOnlyList<string> ImageVariants,
        MonsterRefinementData? Refinement,
        MonsterGroupLoreTable? Barding,
        IReadOnlyList<MonsterLoreTableData> LoreTables,
        IReadOnlyList<string> Groups,
        IReadOnlyList<string> Sources)
    {
        public static MonsterMetadata Empty { get; } =
            new(
                string.Empty,
                string.Empty,
                Array.Empty<MonsterInformationSection>(),
                string.Empty,
                Array.Empty<string>(),
                null,
                null,
                Array.Empty<MonsterLoreTableData>(),
                Array.Empty<string>(),
                Array.Empty<string>());
    }
}
