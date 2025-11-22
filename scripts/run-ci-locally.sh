#!/bin/bash

# Script to run GitHub workflows locally using act

set -e

echo "🚀 Running GitHub workflows locally with act"
echo ""

# Check if act is installed
if ! command -v act &> /dev/null; then
    echo "❌ act is not installed. Install it with:"
    echo "   brew install act"
    exit 1
fi

# Check if Docker is running
if ! docker info &> /dev/null; then
    echo "❌ Docker is not running. Please start Docker Desktop."
    exit 1
fi

echo "Available workflows:"
echo "1. CI workflow (lint, build, test)"
echo "2. Release workflow (build, docs, publish)"
echo "3. Just the build job"
echo "4. Just the test job"
echo "5. Just the lint job"
echo ""

read -p "Which workflow would you like to run? (1-5): " choice

case $choice in
    1)
        echo "🔄 Running full CI workflow..."
        act -W .github/workflows/ci.yml
        ;;
    2)
        echo "🔄 Running release workflow..."
        echo "⚠️  Note: This will skip actual publishing (dummy tokens)"
        act -W .github/workflows/release.yml --input tag=25.10.26
        ;;
    3)
        echo "🔄 Running build job only..."
        act -W .github/workflows/ci.yml -j build
        ;;
    4)
        echo "🔄 Running test job only..."
        act -W .github/workflows/ci.yml -j test
        ;;
    5)
        echo "🔄 Running lint job only..."
        act -W .github/workflows/ci.yml -j lint
        ;;
    *)
        echo "❌ Invalid choice. Please run the script again."
        exit 1
        ;;
esac

echo ""
echo "✅ Workflow execution completed!"