LOCAL LEGENDARY REACTIONS FIXER
================================

What this does
--------------
Every local Debug build automatically scans:

    wwwroot/data/monster/**/*.json

(and a case-insensitive "monsters" directory if you use one).

For every stat block containing a trait such as:

    Legendary Resistance (3/Day)

the local fixer:

1. Removes the Legendary Resistance trait.
2. Creates or updates:

    "legendary_reactions": {
      "header": "Legendary Reactions",
      "uses": 3,
      "entries": [
        {
          "name": "TBA",
          "name_format": "plain",
          "description": ""
        }
      ]
    }

3. Preserves any real Legendary Reaction entries that already exist.
4. Changes structured text references such as
   "regains her Legendary Resistances"
   to
   "regains her Legendary Reactions".
5. Does not rewrite raw_text archival transcription.
6. Only writes JSON files that actually required a change.

Hosted-site safety
------------------
The production project only contains this harmless conditional import:

    <Import Project="Local.Build.targets"
            Condition="Exists('Local.Build.targets')" />

The two local-only items are ignored by Git:

    Local.Build.targets
    .localdev/

Therefore the hosted checkout does not receive the fixer and does not run it.

Installation
------------
Copy these into the Royal Treasury project root:

    Website of Everything.csproj
    .gitignore
    Local.Build.targets
    .localdev/MonsterJsonLocalFixer/MonsterJsonLocalFixer.csproj
    .localdev/MonsterJsonLocalFixer/Program.cs

Then run normally:

    dotnet run

The fixer runs automatically before the local Debug build.

You should see output similar to:

    Running local Legendary Resistance → Legendary Reactions JSON fixer...
    [Local Monster Fixer] Fixed wwwroot/data/monster/CR24/erd-ethnia.json
    [Local Monster Fixer] Complete. 1 stat block(s) converted in 1 file(s).

Git check
---------
Run:

    git status --ignored

Local.Build.targets and .localdev/ should appear as ignored.

If either was accidentally committed before being added to .gitignore,
remove it from the Git index without deleting the local file:

    git rm --cached Local.Build.targets
    git rm -r --cached .localdev

Then commit the .gitignore and Website of Everything.csproj changes.
