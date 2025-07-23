# CafeConnect3D Development Environment Setup Guide

## Prerequisites

### Required Software
1. **Unity 2022.3 LTS**
   - Download from Unity Hub
   - Include Windows Build Support

2. **Visual Studio 2022 Community**
   - Download from Microsoft website
   - Include "Game development with Unity" workload

3. **Git**
   - Download from git-scm.com
   - Configure with your GitHub credentials

### Optional but Recommended
- **Git LFS** (for large asset files)
- **GitHub Desktop** (for easier Git management)
- **Blender** (for 3D asset creation)

## Step-by-Step Setup

### 1. Clone the Repository
```bash
git clone https://github.com/yasir2000/CafeConnect3D.git
cd CafeConnect3D
```

### 2. Install Unity Packages
1. Open Unity Hub
2. Click "Add" and select the project folder
3. Open the project with Unity 2022.3 LTS
4. Wait for Unity to import all assets

### 3. Install Mirror Networking
1. Open Window > Package Manager
2. Click "Add package from git URL"
3. Enter: `https://github.com/MirrorNetworking/Mirror.git?path=/Assets/Mirror`
4. Click "Add"

### 4. Configure Visual Studio
1. Open the project in Unity
2. Go to Edit > Preferences > External Tools
3. Set "External Script Editor" to Visual Studio 2022
4. Install recommended extensions (see VS2022_Extensions.txt)

### 5. Setup Project Structure
Run the following script to create the complete folder structure:
```bash
# On Windows (PowerShell)
./Tools/Scripts/setup_project_structure.ps1

# On macOS/Linux
./Tools/Scripts/setup_project_structure.sh
```

### 6. Configure Git LFS
```bash
git lfs install
git lfs track "*.png" "*.jpg" "*.wav" "*.mp3" "*.fbx"
git add .gitattributes
git commit -m "Configure Git LFS"
```

## Development Workflow

### Daily Workflow
1. Pull latest changes: `git pull origin main`
2. Work on your feature branch
3. Test your changes locally
4. Commit and push: `git push origin feature/your-feature`
5. Create pull request on GitHub

### Testing
- Use Unity Test Runner for unit tests
- Test networking with multiple instances
- Profile performance regularly

### Building
- Use CafeConnect3D/Build menu items
- Build client and server separately
- Test builds before committing

## Troubleshooting

### Common Issues
1. **Unity won't open project**: Check Unity version compatibility
2. **Mirror not importing**: Verify package URL and internet connection
3. **Build errors**: Check all required packages are installed
4. **Git issues**: Ensure Git LFS is properly configured

### Getting Help
- Check project documentation in `/Documentation`
- Ask questions in team chat
- Create GitHub issues for bugs

## Next Steps
1. Read the Game Design Document
2. Review the Coding Standards
3. Set up your development branch
4. Start with assigned tasks

Happy coding! 🎮☕