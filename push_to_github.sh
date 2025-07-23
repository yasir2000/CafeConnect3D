#!/bin/bash
# Quick GitHub Push Script for CafeConnect3D
# Run this after creating the repository on GitHub.com

echo "🚀 Setting up GitHub repository for CafeConnect3D..."

# Navigate to project directory
cd "c:\Users\yasir\Downloads\Caffe Connect 3d\files\CafeConnect3D"

# Check current status
echo "📋 Current Git status:"
git status

# Set main branch
git branch -M main

echo ""
echo "🌐 Next steps:"
echo "1. Go to https://github.com/new"
echo "2. Create repository named: CafeConnect3D"
echo "3. Make it Public"
echo "4. Don't add README, .gitignore, or license (we have them)"
echo "5. Click 'Create repository'"
echo ""
echo "6. Then run these commands (replace YOUR_USERNAME):"
echo "   git remote add origin https://github.com/YOUR_USERNAME/CafeConnect3D.git"
echo "   git push -u origin main"
echo ""
echo "✨ Your multiplayer coffee shop game will be live on GitHub!"
