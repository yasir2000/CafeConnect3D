# 🎨 Asset Integration Guide

## 🚀 Quick Start: Adding Your 3D Models

### 1. **Prepare Your Assets**
```
📁 Your 3D Models Folder/
├── 👥 Characters/
│   ├── business_customer_01.fbx
│   ├── student_customer_01.fbx
│   └── staff_waiter_01.fbx
├── 🪑 Furniture/
│   ├── round_table_2p.fbx
│   ├── wooden_chair.fbx
│   └── espresso_machine.fbx
├── 🍽️ Props/
│   ├── coffee_cup.fbx
│   ├── laptop.fbx
│   └── croissant.fbx
└── 🌿 Decorations/
    ├── plant_potted.fbx
    └── wall_art.fbx
```

### 2. **Import to Unity**
1. **Drag assets** into: `Assets/_Project/Art/Models/`
2. **Set Import Settings:**
   - Model Tab: ✅ Read/Write Enabled
   - Rig Tab: Animation Type = None (for props) or Humanoid (for characters)
   - Materials Tab: ✅ Import Materials

### 3. **Auto-Assignment Method** ⚡ (FASTEST)
1. **Find AssetManager** in scene (created by SceneAutoSetup)
2. **Drag models** into the appropriate arrays:

```csharp
Customer Assets:
├── Business Customers[] ← business_customer_*.fbx
├── Student Customers[] ← student_customer_*.fbx
├── Elderly Customers[] ← elderly_customer_*.fbx
└── Staff Assets[] ← staff_*.fbx

Furniture Assets:
├── Round Table 2P ← round_table_2p.fbx
├── Wooden Chair ← wooden_chair.fbx
├── Espresso Machine ← espresso_machine.fbx
└── Cash Register ← cash_register.fbx

Prop Assets:
├── Coffee Cups[] ← coffee_cup_*.fbx
├── Pastries[] ← croissant.fbx, muffin.fbx
├── Laptops[] ← laptop_*.fbx
└── Plants[] ← plant_*.fbx
```

### 4. **Test the Integration**
1. **Press PLAY**
2. **Watch Console** for "Asset preloading complete!"
3. **Customers spawn** using your models
4. **Coffee shop appears** with your furniture
5. **Props appear** on tables and around the scene

---

## 🔧 Manual Asset Assignment

### Create Prefabs:
1. **Drag models** into scene
2. **Add required components:**
   - Customer models: `Customer.cs`, `NavMeshAgent`, `NetworkIdentity`
   - Furniture: `Collider`, `Interactable` (if needed)
   - Props: `Collider`, `Rigidbody` (if needed)
3. **Drag back to** `Assets/_Project/Prefabs/`
4. **Assign prefabs** to AssetManager arrays

### Asset Naming Convention:
```
✅ Good: customer_business_male_01
✅ Good: table_round_2person
✅ Good: coffee_cup_ceramic_white
❌ Bad: Model (1)
❌ Bad: untitled_character
```

---

## 🎯 Asset Placement Customization

### Manual Placement:
1. **Find AssetPlacementTool** in scene
2. **Configure settings:**
   - Shop Dimensions: 20x15 (default)
   - Number of Tables: 6
   - Equipment placement: ✅ enabled
3. **Right-click** component → "Place Coffee Shop"

### Custom Layout:
```csharp
// In AssetPlacementTool inspector:
Shop Dimensions: (30, 20)      // Larger shop
Number of Tables: 8            // More seating
Min Table Distance: 4f         // More spacing
Number of Plants: 10           // More decorations
```

---

## 🔄 Asset Replacement System

### Replace Assets at Runtime:
```csharp
// Get AssetManager
AssetManager assets = AssetManager.Instance;

// Replace a specific customer type
GameObject newCustomer = assets.GetRandomCustomer("business");

// Get specific furniture
GameObject table = assets.GetFurniture("table_round_2p");

// Get random props
GameObject cup = assets.GetRandomProp("coffee_cup");
```

### Dynamic Asset Loading:
```csharp
// Load from AssetBundles (for larger projects)
assets.LoadAssetBundle("coffee_shop_pack", "path/to/bundle");
GameObject model = assets.LoadFromBundle<GameObject>("coffee_shop_pack", "premium_chair");
```

---

## 🎨 Visual Quality Settings

### Optimize Your Models:
1. **Polygon Count:**
   - Characters: 2,000-5,000 triangles
   - Furniture: 500-2,000 triangles
   - Props: 100-500 triangles

2. **Texture Sizes:**
   - Characters: 1024x1024 or 2048x2048
   - Furniture: 512x512 or 1024x1024
   - Props: 256x256 or 512x512

3. **Materials:**
   - Use Unity's Standard shader
   - Keep material count low per model
   - Share materials between similar objects

---

## 🎵 Audio Integration

### Equipment Sounds:
```csharp
// AudioManager automatically gets equipment positions
AudioManager audio = AudioManager.Instance;

// Play spatial sound at espresso machine
audio.PlayEquipmentSound("espresso_machine", brewSound);
```

### Add Audio Sources:
1. **Select equipment** prefabs (espresso machine, cash register)
2. **Add AudioSource** component
3. **Set 3D spatial settings:**
   - Spatial Blend: 1 (3D)
   - Max Distance: 10
   - Rolloff: Linear

---

## 🔍 Troubleshooting

### Common Issues:

**❌ "Assets not loading"**
```
Solution: Check AssetManager has your models assigned
Check: Window → Console for "Asset preloading complete!"
```

**❌ "Customers not using my models"**
```
Solution: Ensure models are in "Customer Assets" arrays
Check: GameManager.customerPrefab still uses default prefab
```

**❌ "Furniture not appearing"**
```
Solution: Check AssetPlacementTool settings
Check: Console for "Coffee shop placement complete!"
```

**❌ "Models appear broken"**
```
Solution: Check import settings (Read/Write enabled)
Check: Materials imported correctly
```

### Debug Commands:
```csharp
// In Unity Console window:
AssetManager.Instance.GetAsset<GameObject>("customer_business_01");
AssetPlacementTool.Instance.PlaceCoffeeShopManual();
GameManager.Instance.customerPrefab = myCustomerPrefab;
```

---

## 🎉 Success Checklist

✅ Models imported with correct settings
✅ Assets assigned to AssetManager arrays
✅ AssetPlacementTool configured
✅ Game runs without errors
✅ Customers spawn using your models
✅ Furniture appears in coffee shop
✅ Props are distributed around the scene
✅ Audio plays from equipment locations

**Your coffee shop should now have a complete visual identity with your 3D assets!** 🎊
