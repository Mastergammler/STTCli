#!/bin/bash

projectName=Stt.Prototype.csproj
dbContext=SttContext
migrationDir=Migrations

case "$1" in
    "add")
    dotnet ef migrations add $2 --context $dbContext --output-dir $migrationDir
        ;;
    "update")
    dotnet ef database update $2 --context $dbContext
        ;;
    "remove")
    dotent ef migrations remove
        ;;
    "script")
    dotnet ef migrations script
        ;;
    "list")
    dotnet ef migrations list --context $dbContext
        ;;
    *)
        echo "Unknown command '$1'"
        ;;
esac
