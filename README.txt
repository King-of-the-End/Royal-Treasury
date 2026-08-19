MAC LOCAL FIXER COMMAND PATCH
=============================

Your error showed MSBuild trying to execute:

    dotnetrun --project ...

instead of:

    dotnet run --project ...

Replace Local.Build.targets with the included file and add:

    .localdev/run-monster-fixer.sh

The MSBuild target now invokes /bin/sh, and the shell script runs
the dotnet command. This avoids the command-tokenization problem.

Both files remain local-only because your .gitignore already contains:

    Local.Build.targets
    .localdev/

Then run:

    dotnet run

Expected first build line:

    Running local Legendary Resistance → Legendary Reactions JSON fixer...

If the fixer project is missing, the shell script will print the exact
missing path instead of producing the ambiguous "dotnetrun" error.
