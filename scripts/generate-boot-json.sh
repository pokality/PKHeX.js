#!/bin/bash
set -e

DIST_DIR="dist"
BOOT_FILE="$DIST_DIR/blazor.boot.json"

echo "Generating blazor.boot.json..."

# Build assembly entries
ASSEMBLIES=""
first=true
for file in "$DIST_DIR"/*.dll; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        if [ "$first" = true ]; then
            first=false
        else
            ASSEMBLIES="$ASSEMBLIES,"
        fi
        ASSEMBLIES="$ASSEMBLIES
      \"$filename\": \"\""
    fi
done

cat > "$BOOT_FILE" << JSEOF
{
  "mainAssemblyName": "PKHeX.dll",
  "resources": {
    "assembly": {${ASSEMBLIES}
    },
    "wasmNative": {
      "dotnet.native.wasm": ""
    },
    "jsModuleNative": {
      "dotnet.native.js": ""
    },
    "jsModuleRuntime": {
      "dotnet.runtime.js": ""
    }
  }
}
JSEOF

echo "Generated $BOOT_FILE"
