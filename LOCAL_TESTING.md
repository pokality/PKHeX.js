# Local GitHub Workflow Testing

This document explains how to run your GitHub workflows locally to test changes before pushing to GitHub.

## Quick Start

### Option 1: Manual Testing (Recommended - No Docker Required)

Run the complete CI pipeline manually:

```bash
./scripts/test-ci-manually.sh
```

This script replicates all GitHub workflow steps:
- ✅ Lint checks (.NET format validation)
- ✅ Build process (WASM compilation)
- ✅ Bundle size calculation
- ✅ .NET tests (65 tests)
- ✅ TypeScript tests (93 tests)
- ✅ Documentation generation

### Option 2: Using `act` (Docker-based)

For the most accurate GitHub Actions simulation:

```bash
./scripts/run-ci-locally.sh
```

This provides an interactive menu to run:
1. Full CI workflow
2. Release workflow
3. Individual jobs (build, test, lint)

## Prerequisites

### For Manual Testing
- .NET 9.0 SDK
- Node.js 20+
- TypeDoc (optional, for docs): `npm install -g typedoc`

### For `act` Testing
- Docker Desktop (running)
- `act` tool: `brew install act`

## Available Scripts

### `./scripts/test-ci-manually.sh`
- **Purpose**: Run all CI steps without Docker
- **Time**: ~2-3 minutes
- **Output**: Colored progress indicators and detailed results
- **Best for**: Quick validation before commits

### `./scripts/run-ci-locally.sh`
- **Purpose**: Run GitHub Actions with Docker simulation
- **Time**: ~5-10 minutes (first run downloads images)
- **Output**: Exact GitHub Actions logs
- **Best for**: Testing workflow changes

### `./scripts/generate-docs.sh`
- **Purpose**: Generate TypeScript documentation
- **Output**: HTML, PDF, EPUB, MOBI formats in `docs/`
- **Best for**: Documentation validation

## What Gets Tested

### Lint Checks
- .NET code formatting (`dotnet format --verify-no-changes`)
- Code analysis with warnings as errors
- TypeScript compilation

### Build Process
- WASM workload installation
- .NET project compilation
- WASM publishing
- Bundle size calculation (currently ~54MB)

### Tests
- **65 .NET tests**: Core PKHeX functionality
- **93 TypeScript tests**: API wrapper and integration tests
- All tests must pass for CI success

### Documentation
- TypeScript API documentation
- Multiple output formats (HTML, PDF, EPUB, MOBI)
- Example code validation

## Interpreting Results

### ✅ Success Indicators
```
✅ .NET format check passed
✅ Code analysis passed
✅ Build completed
✅ Bundle size calculated
✅ .NET tests passed
✅ TypeScript tests passed
```

### ❌ Common Issues

#### Build Failures
```
❌ Code analysis failed
```
**Solution**: Run `dotnet format` to fix formatting issues

#### Test Failures
```
❌ .NET tests failed
❌ TypeScript tests failed
```
**Solution**: Check test output for specific failures

#### Documentation Issues
```
⚠️  Documentation generation failed (non-critical)
```
**Solution**: Usually TypeScript compilation issues in examples

## Workflow Comparison

| Feature | Manual Script | act Tool |
|---------|---------------|----------|
| Speed | Fast (2-3 min) | Slower (5-10 min) |
| Accuracy | High | Exact match |
| Docker Required | No | Yes |
| GitHub Secrets | N/A | Simulated |
| Environment | Local | Ubuntu container |

## Troubleshooting

### Docker Issues (act)
```bash
# Check Docker is running
docker info

# Pull required images manually
docker pull catthehacker/ubuntu:act-latest
```

### .NET Issues
```bash
# Install WASM workload
dotnet workload install wasm-tools

# Restore packages
dotnet restore
```

### Node.js Issues
```bash
# Install dependencies
npm ci

# Check Node version
node --version  # Should be 20+
```

## Configuration Files

### `.actrc`
Configuration for `act` tool:
- Uses Ubuntu container
- Sets dummy secrets for testing
- Configures runner size

### `tsconfig.json`
Updated to include examples directory:
- `rootDir: "./"` (includes both src and examples)
- Proper TypeScript compilation for docs

## Integration with Development

### Before Committing
```bash
# Quick validation
./scripts/test-ci-manually.sh

# If all passes, commit
git add .
git commit -m "Your changes"
git push
```

### Before Releases
```bash
# Test release workflow
./scripts/run-ci-locally.sh
# Choose option 2 (Release workflow)
```

### Continuous Development
```bash
# Watch mode for TypeScript
npm run dev

# In another terminal, run tests periodically
./scripts/test-ci-manually.sh
```

## Performance Metrics

### Bundle Sizes (Current)
- `dotnet.native.wasm`: 2.8MB
- `PKHeX.Core.dll`: 16MB  
- `PKHeX.dll`: 540KB
- **Total**: ~54MB

### Test Coverage
- .NET: 65 tests covering core functionality
- TypeScript: 93 tests covering API wrapper
- Integration tests for save file operations

## Next Steps

1. **Run locally** before every commit
2. **Check bundle size** for significant changes
3. **Validate documentation** for API changes
4. **Test examples** work with new features

This local testing setup ensures your changes work correctly before GitHub Actions runs, saving time and preventing failed builds.