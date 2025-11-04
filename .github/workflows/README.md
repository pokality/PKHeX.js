# Workflows

## CI (`ci.yml`)
Runs on push/PR to main/develop. Lints, builds, and tests code.

## Release (`release.yml`)
Triggers on tags matching PKHeX version (e.g., `25.10.26`). Deploys to GitHub Releases, NPM, and GitHub Pages.

### Creating a Release
```bash
# Update submodule
cd lib/PKHeX && git checkout 25.10.26 && cd ../..
git add lib/PKHeX && git commit -m "Update PKHeX to 25.10.26"

# Create matching tag
git tag 25.10.26 && git push origin main 25.10.26
```

### Required Secrets
- `NPM_TOKEN` - NPM automation token for publishing
- `CODECOV_TOKEN` - Codecov token for coverage reporting (optional)

### Environments
- `build` - CI/CD build environment
- `github-pages` - GitHub Pages deployment environment
