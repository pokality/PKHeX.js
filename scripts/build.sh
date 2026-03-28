#!/usr/bin/env sh
set -e

# Build WASI component (componentize-dotnet handles wit-bindgen + WASI SDK + NativeAOT)
dotnet restore src/PKHeX/PKHeX.csproj --nologo -r wasi-wasm
dotnet build src/PKHeX/PKHeX.csproj -c Release --nologo --no-restore

# Transpile to JS/TS
rm -rf dist
mkdir -p dist
npx jco transpile src/PKHeX/bin/Release/net10.0/wasi-wasm/native/PKHeX.wasm \
  -o dist/ --name pkhex

npx tsc

# Run tests
dotnet test tests/PKHeX.Tests/PKHeX.Tests.csproj --nologo -v q
npm test --silent

echo "Package size: $(du -sh dist/ | cut -f1)"
