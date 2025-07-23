# 🎯 Complete Running Instructions

## 🚀 FASTEST WAY TO RUN (2 Minutes)

### 1. Open Unity Project
```bash
# Navigate to your project folder
cd "c:\Users\yasir\Downloads\Caffe Connect 3d\files"

# Open Unity Hub and add project
# OR double-click CafeConnect3D folder if Unity is set as default
```

### 2. Create Test Scene (In Unity)
1. **File** → **New Scene** → **Save as** "TestScene"
2. **GameObject** → **Create Empty** → Name: "GameSetup"
3. **Add Components to GameSetup:**
   - `MainScene`
   - `PrefabCreator`
   - `SceneSetup`

### 3. Add Network Manager
1. **GameObject** → **Create Empty** → Name: "NetworkManager"
2. **Add Component:** `CafeNetworkManager`

### 4. Add Basic Floor
1. **GameObject** → **3D Object** → **Plane**
2. **Scale:** Set to (20, 1, 20) for large floor

### 5. Press PLAY!
- Game will auto-initialize everything
- NetworkManager HUD should appear
- Press **"H"** to Host or **"C"** to Connect

---

## 🎮 FULL GAME SETUP (10 Minutes)

### Scene Setup Checklist:
```
✅ NetworkManager (CafeNetworkManager)
✅ GameManager (auto-created)
✅ AudioManager (auto-created)
✅ UIManager (auto-created)
✅ OrderManager (auto-created)
✅ MenuManager (auto-created)
✅ Floor plane (scaled 20x20x20)
✅ Main Camera
✅ Directional Light
```

### Required Components Flow:
```
MainScene → Creates managers if missing
PrefabCreator → Creates player/customer prefabs
SceneSetup → Initializes all systems
CafeNetworkManager → Handles networking
```

---

## 🔧 Testing the Game

### Single Player Test:
1. Press **PLAY**
2. Press **H** (Host)
3. Watch console for "Game Started"
4. Customers should spawn every 5 seconds
5. Red capsules = Customers
6. Blue capsule = Player (when spawned)

### Multiplayer Test:
1. **Build the game:**
   - File → Build Settings
   - Add current scene
   - Build to folder
2. **Run built game** (Host)
3. **Press PLAY in Unity** (Client)
4. **Press C** to connect to localhost

---

## 🎯 Game Features You'll See:

### Immediate (No Setup):
- ✅ Customer AI spawning
- ✅ Pathfinding to seats
- ✅ Order generation
- ✅ Network synchronization
- ✅ Audio system
- ✅ UI notifications

### With Basic Prefabs:
- ✅ Player movement (WASD)
- ✅ Customer interaction (E key)
- ✅ Order taking workflow
- ✅ Real-time order board

### Advanced (Add 3D Models):
- Coffee shop environment
- Realistic customer models
- Interactive equipment
- Complete visual experience

---

## 🛠️ Troubleshooting

### "Compilation Errors":
```
1. Window → Package Manager
2. Install: Mirror Networking
3. Install: TextMeshPro
4. Wait for compilation
```

### "No Customers Spawning":
```
Check Console for:
- "Game Started" message
- Customer spawn points created
- NavMesh warnings (can ignore for testing)
```

### "Network Issues":
```
Ensure CafeNetworkManager has:
- Network Address: localhost
- Max Connections: 20
- Transport: KCP (default)
```

---

## 📱 Controls Summary

| Key | Action |
|-----|--------|
| **H** | Host game (NetworkManager) |
| **C** | Connect as client |
| **WASD** | Move player |
| **E** | Interact with customers/objects |
| **Tab** | Toggle order board |
| **ESC** | Disconnect |

---

## 🎉 Success Indicators

You'll know it's working when you see:
- ✅ NetworkManager HUD appears
- ✅ Console shows "Game Started"
- ✅ Red capsules (customers) spawn and move
- ✅ UI elements appear
- ✅ Audio plays
- ✅ No compilation errors

**The game is designed to work immediately with minimal setup!** 🚀
