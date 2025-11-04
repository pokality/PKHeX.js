#!/usr/bin/env sh
set -e

dotnet clean src/PKHeX/PKHeX.csproj --nologo -v q
dotnet format src/PKHeX/PKHeX.csproj -v q

rm -rf dist
mkdir -p dist

dotnet publish src/PKHeX/PKHeX.csproj -c Release --nologo -v q
cp -r src/PKHeX/bin/Release/browser-wasm/publish/* dist/

./scripts/generate-boot-json.sh

npx tsc src/index.ts src/api-wrapper.ts --outDir dist --module esnext --target es2020 --moduleResolution bundler --declaration 2>/dev/null
rm -f dist/*.a dist/*.dat

dotnet test tests/PKHeX.Tests/PKHeX.Tests.csproj --nologo -v q
npm test --silent

echo "Package size: $(du -sh dist/ | cut -f1)"
