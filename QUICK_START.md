# 🚀 Quick Start Guide - CafeConnect3D

## Running the Game (5 Minutes Setup)

### Option A: Quick Test (No Setup Required)
1. Open Unity Hub
2. Click "Open" → Navigate to `CafeConnect3D` folder
3. Wait for Unity to load and compile scripts
4. In Project window, navigate to `Assets/_Project/Scenes/`
5. Create a new scene: Right-click → Create → Scene → Name it "MainScene"
6. **Add essential objects to scene:**
   ```
   - Create Empty GameObject → Name: "NetworkManager"
   - Add Component: CafeNetworkManager
   - Add Component: MainScene (the script we created)

   - Create Empty GameObject → Name: "SceneSetup"
   - Add Component: SceneSetup
   ```
7. Press **PLAY** button in Unity

### Option B: Full Setup (Recommended)

#### 1. Create Scene Hierarchy
```
Scene: MainScene
├── NetworkManager (CafeNetworkManager component)
├── GameManager (GameManager component)
├── AudioManager (AudioManager component)
├── UIManager (UIManager component)
├── OrderManager (OrderManager component)
├── MenuManager (MenuManager component)
├── SceneSetup (SceneSetup component)
├── Main Camera
├── Directional Light
├── Floor (Plane scaled 20x20x20)
└── Spawn Points & Seats (auto-created by MainScene script)
```

#### 2. Setup Network Manager
- **Network Address**: localhost
- **Max Connections**: 20
- **Player Prefab**: (Create later)
- **Registered Spawnable Prefabs**: (Add customer prefab later)

#### 3. Quick Play Test
1. Press **PLAY**
2. Click **"Host"** in Network Manager HUD
3. Game should start running!

## 🎮 Controls & Testing

### Basic Controls (when game is running):
- **H**: Start hosting (in NetworkManager HUD)
- **C**: Connect as client (in NetworkManager HUD)
- **WASD**: Move player (when player is spawned)
- **E**: Interact with objects
- **Tab**: Toggle order board

### Testing the Game:
1. **Host a game**: Press H in NetworkManager HUD
2. **Check console**: Should see "Game Started" messages
3. **Watch for customers**: Should spawn automatically every 5 seconds
4. **UI Elements**: Should see game UI appear

## 🛠️ Troubleshooting

### Common Issues:

**1. Script Compilation Errors:**
- Check Console window for errors
- Ensure all required packages are installed (Mirror, TextMeshPro)
- Restart Unity if needed

**2. Missing References:**
- All scripts are designed to work without prefab references
- MainScene script will create necessary objects automatically

**3. Network Issues:**
- Ensure Mirror package is properly installed
- Check NetworkManager component is attached

**4. No Customers Spawning:**
- Check GameManager has spawn points assigned
- Ensure customer prefab is assigned (can be created later)

### Quick Fixes:
```
// If GameManager is null:
- Add GameManager script to a GameObject in scene

// If customers don't spawn:
- Check customerSpawnPoints array in GameManager
- MainScene script should create these automatically

// If UI doesn't show:
- Check UIManager is in scene
- UI will be created when UIManager starts
```

## 📋 Next Steps

1. **Create Basic Prefabs:**
   - Player prefab with NetworkPlayer script
   - Customer prefab with Customer script
   - Basic furniture and equipment

2. **Setup Scene Objects:**
   - Coffee machines
   - Tables and chairs
   - Order counter
   - Kitchen area

3. **Test Multiplayer:**
   - Build the game
   - Run multiple instances
   - Test client-server connection

## 🔧 Development Mode

For development and testing:
1. Keep Console window open
2. Watch for debug messages
3. Use Scene view to see customer spawning
4. Check NetworkManager HUD for connection status

**The game is designed to work immediately with minimal setup!**
All managers will auto-initialize and create necessary objects.
