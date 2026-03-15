#!/bin/bash

args=(
  -p:PublishTrimmed=True
  -p:EnableCompressionInSingleFile=True
  -p:PublishReadyToRun=False
  -p:PublishProtocol=FileSystem
  -p:TargetFramework=net10.0
  -p:RuntimeIdentifier=linux-x64
  -p:Platform=x64
  -p:Configuration=Release
  -p:PublishDir=release-linux-x64
  -p:PublishSingleFile=True
  -p:SelfContained=True
  -p:PublishMethod=SELF_CONTAINED
)

dotnet publish ../src/DevilDaggersInfo.Tools/DevilDaggersInfo.Tools.csproj "${args[@]}"
