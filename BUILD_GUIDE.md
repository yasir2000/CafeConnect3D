# 🏗️ CafeConnect3D Build & Compile Guide

## 🚀 Quick Build Methods

### Method 1: Unity Editor Build (Recommended)

#### **1. Open Unity Project**
```bash
# First, ensure you're using the clean project path
cd C:\CafeConnect3D
```

1. **Open Unity Hub**
2. **Add Project** → Navigate to `C:\CafeConnect3D`
3. **Open with Unity 2022.3.45f1**

#### **2. Install Required Packages**
In Unity, go to **Window → Package Manager** and install:
- **Mirror Networking** (for multiplayer)
- **TextMeshPro** (for UI)
- **ProBuilder** (optional, for level design)

#### **3. Build Using Custom Build Menu**
1. **Top Menu Bar → CafeConnect3D → Build**
2. Choose your build option:
   - **Build Client** - Creates playable game executable
   - **Build Server** - Creates dedicated server
   - **Build All** - Creates both client and server

**Output Locations:**
- Client: `Build/Client/CafeConnect3D.exe`
- Server: `Build/Server/CafeConnect3D_Server.exe`

### Method 2: Unity Standard Build

#### **For Client Build:**
1. **File → Build Settings**
2. **Add Open Scenes** (SampleScene)
3. **Platform:** PC, Mac & Linux Standalone
4. **Target Platform:** Windows
5. **Architecture:** x86_64
6. **Click "Build"**
7. **Save to:** `Build/Client/`

#### **For Server Build:**
1. **File → Build Settings**
2. **Add Open Scenes** (SampleScene)
3. **Platform:** PC, Mac & Linux Standalone
4. **Target Platform:** Windows
5. **Architecture:** x86_64
6. **Server Build:** ✅ (Enable)
7. **Click "Build"**
8. **Save to:** `Build/Server/`

---

## 🖥️ Command Line Building

### **Method 3: Unity Command Line Build**

Create a batch file for automated building:

```batch
@echo off
echo Building CafeConnect3D...

REM Set Unity installation path
set UNITY_PATH="C:\Program Files\Unity\Hub\Editor\2022.3.45f1\Editor\Unity.exe"

REM Set project path
set PROJECT_PATH="C:\CafeConnect3D"

echo Building Client...
%UNITY_PATH% -projectPath %PROJECT_PATH% -executeMethod CafeConnect3D.Editor.BuildScript.BuildClient -quit -batchmode -logFile build_client.log

echo Building Server...
%UNITY_PATH% -projectPath %PROJECT_PATH% -executeMethod CafeConnect3D.Editor.BuildScript.BuildServer -quit -batchmode -logFile build_server.log

echo Build complete!
pause
```

---

## 📦 Build Outputs

### **Client Build** (`Build/Client/`)
```
CafeConnect3D.exe           ← Main game executable
CafeConnect3D_Data/         ← Game data folder
UnityPlayer.dll             ← Unity runtime
```

### **Server Build** (`Build/Server/`)
```
CafeConnect3D_Server.exe    ← Dedicated server
CafeConnect3D_Server_Data/  ← Server data
StartServer.bat             ← Auto-generated server launcher
```

---

## 🎮 Running the Built Game

### **Single Player / Local Testing:**
1. Run `Build/Client/CafeConnect3D.exe`
2. Press **H** to host a local game
3. Game starts immediately with AI customers

### **Multiplayer Setup:**

#### **Server:**
1. Run `Build/Server/StartServer.bat`
2. Server starts in headless mode
3. Listens on default port (7777)

#### **Clients:**
1. Run `Build/Client/CafeConnect3D.exe`
2. Press **C** to connect
3. Enter server IP address
4. Press **Connect**

---

## ⚙️ Build Configuration

### **Development Build:**
- **File → Build Settings**
- ✅ **Development Build**
- ✅ **Script Debugging**
- ✅ **Wait for Managed Debugger** (optional)

### **Release Build:**
- ❌ Development Build
- **Compression Method:** LZ4HC
- **Stripping Level:** Medium

### **Server Build:**
- ✅ **Server Build**
- ❌ Development Build (for performance)
- **Headless Mode:** Automatic in server builds

---

## 🛠️ Build Optimization

### **For Smaller File Size:**
1. **Player Settings → Publishing Settings**
2. **IL2CPP** code generation
3. **Strip Engine Code**
4. **Managed Stripping Level:** Medium/High

### **For Faster Builds:**
1. **Player Settings → Other Settings**
2. **Scripting Backend:** Mono
3. **Api Compatibility Level:** .NET Standard 2.1

---

## 🐛 Troubleshooting Build Issues

### **"Build Failed" Errors:**
```
1. Check Console for specific errors
2. Window → Console → Clear on Play (disable)
3. Fix any compilation errors first
4. Ensure all scenes are added to Build Settings
```

### **"Missing References" Errors:**
```
1. Window → Package Manager
2. Reinstall Mirror Networking
3. Reinstall TextMeshPro
4. Assets → Reimport All
```

### **"Platform Not Supported":**
```
1. File → Build Settings
2. Click "Switch Platform" for Windows
3. Wait for reimport to complete
```

### **Build Path Issues:**
```
1. Use simple paths without spaces
2. Avoid deep folder structures
3. Use forward slashes: C:/CafeConnect3D/Build
```

---

## 📋 Pre-Build Checklist

Before building, ensure:
- ✅ No compilation errors in Console
- ✅ All required packages installed
- ✅ Scenes added to Build Settings
- ✅ Player Settings configured
- ✅ Build output folder exists and is writable

---

## 🚀 Quick Build Commands

### **Development Testing:**
```
CafeConnect3D → Build → Build Client
```

### **Production Release:**
```
1. Switch to Release configuration
2. CafeConnect3D → Build → Build All
3. Test both client and server builds
```

### **CI/CD Pipeline:**
```bash
# Unity command line for automated builds
Unity.exe -projectPath "C:\CafeConnect3D" -executeMethod CafeConnect3D.Editor.BuildScript.BuildAll -quit -batchmode
```

---

**🎉 Your multiplayer coffee shop game is now ready to build and deploy!**

The build system supports both client and dedicated server builds, making it perfect for hosting multiplayer coffee shop sessions. The automated build scripts handle all the configuration, so you can focus on gameplay! ☕
