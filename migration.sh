#!/bin/bash

# relative to script directory
projectName=src/Stt.Prototype.csproj
dbContext=SttContext
# relative to .csproj file
migrationDir=Migrations

case "$1" in
    "add")
    dotnet ef migrations add $2 --context $dbContext --output-dir $migrationDir --project $projectName
        ;;
    "update")
    dotnet ef database update $2 --context $dbContext --project $projectName
        ;;
    "remove")
    dotnet ef migrations remove --project $projectName
        ;;
    "script")
    dotnet ef migrations script --project $projectName
        ;;
    "list")
    dotnet ef migrations list --context $dbContext --project $projectName
        ;;
    *)
        echo "Unknown command '$1'"
        ;;
esac
