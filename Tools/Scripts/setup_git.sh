#!/bin/bash
# Git setup script for CafeConnect3D project

echo "Setting up Git for CafeConnect3D project..."

# Initialize git repository
git init

# Set up git configuration
git config --local user.name "yasir2000"
git config --local user.email "your.email@example.com"

# Set up Git LFS for large files
git lfs install
git lfs track "*.png"
git lfs track "*.jpg"
git lfs track "*.tga"
git lfs track "*.tiff"
git lfs track "*.psd"
git lfs track "*.fbx"
git lfs track "*.obj"
git lfs track "*.3ds"
git lfs track "*.dae"
git lfs track "*.blend"
git lfs track "*.wav"
git lfs track "*.mp3"
git lfs track "*.ogg"
git lfs track "*.aiff"

# Add .gitattributes for Unity
echo "Creating .gitattributes..."
cat > .gitattributes << 'EOF'
# Unity YAML
*.cs diff=csharp text
*.cginc text
*.shader text

# Unity
*.mat merge=unityyamlmerge eol=lf
*.anim merge=unityyamlmerge eol=lf
*.unity merge=unityyamlmerge eol=lf
*.prefab merge=unityyamlmerge eol=lf
*.physicsMaterial2D merge=unityyamlmerge eol=lf
*.physicMaterial merge=unityyamlmerge eol=lf
*.asset merge=unityyamlmerge eol=lf
*.meta merge=unityyamlmerge eol=lf
*.controller merge=unityyamlmerge eol=lf

# LFS
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.tiff filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.obj filter=lfs diff=lfs merge=lfs -text
*.3ds filter=lfs diff=lfs merge=lfs -text
*.dae filter=lfs diff=lfs merge=lfs -text
*.blend filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.aiff filter=lfs diff=lfs merge=lfs -text
EOF

echo "Git setup completed!"