#!/bin/bash
set -e

DIST_DIR="dist"
BOOT_JSON="$DIST_DIR/blazor.boot.json"

echo "Generating blazor.boot.json..."

cat > "$BOOT_JSON" << 'EOF'
{
  "mainAssemblyName": "PKHeX.dll",
  "resources": {
    "assembly": {
EOF

first=true
for file in "$DIST_DIR"/*.dll; do
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
    }
  }
}
EOF

echo "Generated $BOOT_JSON"
