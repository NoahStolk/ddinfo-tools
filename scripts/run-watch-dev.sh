#!/bin/bash

cd ../src/DevilDaggersInfo.Tools || exit 1

dotnet watch -c Debug --project DevilDaggersInfo.Tools.csproj --non-interactive --property:RunAnalyzersDuringBuild=false
