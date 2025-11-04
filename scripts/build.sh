#!/usr/bin/env sh

npm install

dotnet build src/PKHeX/PKHeX.csproj
dotnet publish src/PKHeX/PKHeX.csproj -c Release

dotnet test tests/PKHeX.Tests/PKHeX.Tests.csproj 2>&1 | tail -5

scripts/generate-boot-json.sh
scripts/generate-docs.sh

npx tsc --noEmit src/index.d.ts src/helpers.ts 2>&1
