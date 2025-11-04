#!/bin/bash

# Generate blazor.boot.json for .NET WASM
# This is needed because dotnet.js expects it, even though we're not using Blazor

CONFIG="${1:-Release}"
FRAMEWORK_DIR="dist/bin/$CONFIG/AppBundle/_framework"
BOOT_JSON="$FRAMEWORK_DIR/blazor.boot.json"

echo "Generating blazor.boot.json..."

# Start JSON
cat > "$BOOT_JSON" << 'EOF'
{
  "mainAssemblyName": "PKHeX.dll",
  "resources": {
    "assembly": {
EOF

# Add all .wasm files (assemblies)
first=true
for file in "$FRAMEWORK_DIR"/*.wasm; do
    if [ -f "$file" ]; then
        filename=$(basename "$file")
        if [ "$first" = true ]; then
            first=false
        else
            echo "," >> "$BOOT_JSON"
        fi
        echo -n "      \"$filename\": \"\"" >> "$BOOT_JSON"
    fi
done

# Finish JSON
cat >> "$BOOT_JSON" << 'EOF'

    },
    "wasmNative": {
      "dotnet.native.wasm": ""
    },
    "jsModuleNative": {
      "dotnet.native.js": ""
    },
    "jsModuleRuntime": {
      "dotnet.runtime.js": ""
    },
    "icu": {
      "icudt_CJK.dat": "",
      "icudt_EFIGS.dat": "",
      "icudt_no_CJK.dat": ""
    }
  }
}
EOF

echo "Generated $BOOT_JSON"
