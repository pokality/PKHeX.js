#!/usr/bin/env sh

set -e

if ! command -v typedoc &> /dev/null; then
    echo "TypeDoc not found. Installing..." > /dev/stderr
    npm install -g typedoc
fi

typedoc src/index.d.ts \
  --out docs \
  --name "PKHeX WASM API Documentation" \
  --readme README.md \
  --includeVersion

# Generate PDF using WeasyPrint (no Chromium/Google dependencies)
if command -v weasyprint &> /dev/null; then
    weasyprint docs/index.html docs/PKHeX-API-Documentation.pdf
else
    echo "WeasyPrint not found. Skipping PDF generation." > /dev/stderr
    echo "   Install with: pip3 install weasyprint (or brew install weasyprint on macOS)" > /dev/stderr
fi

# Generate EPUB/MOBI (requires pandoc and calibre)
if command -v pandoc &> /dev/null; then
    echo '# PKHeX.js API Documentation' > docs/api-reference.md
    
    echo '```typescript' >> docs/api-reference.md
    cat src/index.d.ts >> docs/api-reference.md
    echo '```' >> docs/api-reference.md
    
    # Generate EPUB
    pandoc docs/api-reference.md \
      -o docs/PKHeX-API-Documentation.epub \
      --metadata title="PKHeX WASM API Documentation" \
      --metadata author="PKHeX WASM Contributors" \
      --toc
    
    # Generate MOBI (requires calibre's ebook-convert)
    if command -v ebook-convert &> /dev/null; then
        ebook-convert \
          docs/PKHeX-API-Documentation.epub \
          docs/PKHeX-API-Documentation.mobi
    fi
fi
