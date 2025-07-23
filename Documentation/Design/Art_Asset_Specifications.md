# 🎨 CafeConnect3D Art Asset Specifications

## 🎯 Art Style Guide

### Visual Style
- **Theme:** Modern, cozy coffee shop
- **Style:** Low-poly to mid-poly (2000-8000 triangles per object)
- **Color Palette:** Warm browns, creams, oranges, and greens
- **Lighting:** Warm, inviting ambient lighting
- **Textures:** 512x512 to 1024x1024 resolution
- **Mood:** Friendly, welcoming, bustling but organized

### Technical Specifications
- **Poly Count:**
  - Characters: 1500-3000 triangles
  - Furniture: 500-1500 triangles
  - Small props: 100-500 triangles
  - Environment: 2000-5000 triangles per section
- **Texture Size:** Power of 2 (512x512, 1024x1024)
- **Format:** FBX for models, PNG for textures
- **Optimization:** LOD levels for distant objects

---

## 🏪 Environment Assets

### Coffee Shop Layout
```
Main Areas:
├── Entrance/Waiting Area
├── Seating Area (Tables & Chairs)
├── Order Counter
├── Kitchen/Preparation Area
├── Storage Room
└── Restrooms
```

### Required Environment Models

#### 🏗️ Structural Elements
1. **Walls & Floors**
   - Modular wall pieces (2m x 3m)
   - Corner pieces
   - Door frames
   - Window frames
   - Wood/tile floor tiles (1m x 1m)
   - Ceiling tiles

2. **Windows & Doors**
   - Large front windows
   - Entry door with glass
   - Kitchen swing doors
   - Emergency exit door

#### 🪑 Furniture & Seating
1. **Tables**
   - 2-person round table
   - 4-person square table
   - 6-person rectangular table
   - Bar-height counter seating
   - Coffee table (for lounge area)

2. **Chairs**
   - Wooden café chair
   - Upholstered lounge chair
   - Bar stools
   - Bench seating

#### 🎛️ Equipment & Stations
1. **Coffee Equipment**
   - Espresso machine (main)
   - Coffee grinder
   - Steam wand
   - Coffee bean storage
   - Cup dispensers
   - Milk frother

2. **Kitchen Equipment**
   - Commercial refrigerator
   - Microwave
   - Sandwich press
   - Cash register/POS
   - Display case for pastries
   - Sink and counter space

#### 🎨 Decorative Elements
1. **Wall Decorations**
   - Coffee art prints
   - Menu boards (chalkboard style)
   - Shelving with coffee bags
   - Plants (hanging and potted)
   - Clock
   - Lighting fixtures

2. **Ambient Objects**
   - Coffee bean sacks
   - Storage containers
   - Cleaning supplies
   - Paper cup stacks
   - Coffee bag displays

---

## 👥 Character Assets

### Customer Models (6-8 Variations)
1. **Business Person**
   - Suit, briefcase, laptop bag
   - Male/Female variants
   - Professional appearance

2. **Student**
   - Casual clothes, backpack, books
   - Young adult appearance
   - Relaxed posture

3. **Elderly Customer**
   - Comfortable clothing, walking cane
   - Slower movement animations
   - Friendly demeanor

4. **Parent with Child**
   - Family-friendly appearance
   - Stroller or child holding hand
   - Patient, caring expressions

5. **Freelancer/Artist**
   - Creative clothing, art supplies
   - Laptop, sketchbook
   - Alternative style

6. **Tourist**
   - Camera, map, travel bag
   - Curious, exploring expressions
   - Seasonal clothing options

### Staff Models (3 Roles)
1. **Barista**
   - Apron, coffee shop uniform
   - Energetic, friendly appearance
   - Coffee stains, realistic wear

2. **Manager**
   - Business casual attire
   - Clipboard, name tag
   - Professional but approachable

3. **Cleaner/Support**
   - Cleaning uniform
   - Cleaning supplies
   - Hardworking appearance

---

## 🍽️ Props & Items

### Coffee & Food Items
1. **Beverages**
   - Coffee cups (various sizes)
   - Takeaway cups with lids
   - Tea cups and saucers
   - Cold drink glasses
   - Water bottles

2. **Food Items**
   - Croissants
   - Muffins
   - Sandwiches
   - Cookies
   - Bagels
   - Salads

3. **Serving Items**
   - Plates (various sizes)
   - Napkins
   - Utensils (forks, knives, spoons)
   - Trays
   - Sugar packets
   - Stirrers

### Technology & Modern Elements
1. **POS System**
   - Tablet-based register
   - Card reader
   - Receipt printer
   - Cash drawer

2. **Customer Tech**
   - Laptops (various brands)
   - Smartphones
   - Tablets
   - Charging cables
   - Power banks

---

## 🎭 Animation Requirements

### Character Animations
1. **Customer Animations**
   - Walking (normal, hurried, slow)
   - Sitting down/standing up
   - Ordering (pointing, talking)
   - Drinking/eating
   - Looking at menu
   - Waiting (checking phone, tapping fingers)
   - Leaving (satisfied, frustrated)

2. **Staff Animations**
   - Taking orders
   - Making coffee
   - Cleaning tables
   - Operating equipment
   - Carrying trays
   - Interacting with customers

### Object Animations
1. **Equipment**
   - Coffee machine steaming
   - Cash register opening
   - Doors opening/closing
   - Chairs moving

---

## 🎨 Texture & Material Specifications

### Material Types Needed
1. **Wood Materials**
   - Oak table tops
   - Pine chair frames
   - Bamboo flooring
   - Walnut countertops

2. **Metal Materials**
   - Stainless steel (equipment)
   - Brass fixtures
   - Chrome details
   - Copper accents

3. **Fabric Materials**
   - Chair upholstery
   - Curtains
   - Aprons
   - Uniforms

4. **Glass Materials**
   - Windows
   - Display cases
   - Drinking glasses
   - Coffee pot glass

5. **Plastic Materials**
   - Takeaway cups
   - Equipment housings
   - Signage
   - Modern fixtures

---

## 📐 Asset Creation Guidelines

### Modeling Standards
- Use consistent units (1 Unity unit = 1 meter)
- Keep geometry clean and optimized
- Use proper naming conventions
- Apply transforms before export
- Include proper UV mapping

### Naming Convention
```
CategoryType_AssetName_Variant_LOD
Examples:
- Furniture_Chair_Wooden_LOD0
- Character_Customer_Business_LOD1
- Prop_CoffeeCup_Large_LOD0
- Environment_Wall_Corner_LOD0
```

### File Organization
```
Art/
├── Models/
│   ├── Characters/
│   ├── Environment/
│   └── Props/
├── Textures/
│   ├── Diffuse/
│   ├── Normal/
│   └── Roughness/
├── Materials/
│   ├── Characters/
│   ├── Environment/
│   └── Props/
└── Animations/
    ├── Characters/
    └── Objects/
```

---

## 🚀 Implementation Priority

### Phase 1 (Essential)
1. Basic environment (walls, floor, ceiling)
2. Essential furniture (tables, chairs)
3. Basic customer models (2-3 types)
4. Coffee equipment (espresso machine, counter)
5. Basic props (cups, food items)

### Phase 2 (Enhancement)
1. Additional customer variations
2. Detailed decorations
3. Advanced equipment
4. Improved textures and materials
5. Animation polish

### Phase 3 (Polish)
1. Seasonal decorations
2. Special event items
3. Advanced lighting props
4. Particle effects for steam/smoke
5. Interactive elements

---

## 💡 Free Asset Resources

### 3D Model Sources
- **Blender** (Free 3D modeling software)
- **SketchFab** (Free models with attribution)
- **Unity Asset Store** (Free coffee shop assets)
- **OpenGameArt.org** (Creative Commons assets)
- **Poly Haven** (Free textures and HDRIs)

### Texture Sources
- **TexturesCom** (Free textures)
- **FreePBR.com** (PBR materials)
- **Substance Source** (Free tier available)
- **CC0 Textures** (Public domain)

### Animation Sources
- **Mixamo** (Free character animations)
- **Unity Asset Store** (Free animation packs)

This specification provides a complete roadmap for creating all the 3D art assets needed for your coffee shop game!
