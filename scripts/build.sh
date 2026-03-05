#!/usr/bin/env sh
set -e

dotnet build src/PKHeX/PKHeX.csproj -c Release --nologo -v q

rm -rf dist
mkdir -p dist

npx jco transpile src/PKHeX/bin/Release/net10.0/wasi-wasm/native/PKHeX.wasm \
  -o dist/ --name pkhex

npx tsc

dotnet test tests/PKHeX.Tests/PKHeX.Tests.csproj --nologo -v q
npm test --silent

echo "Package size: $(du -sh dist/ | cut -f1)"
