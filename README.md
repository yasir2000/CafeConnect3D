# CafeConnect3D - Multiplayer Coffee Shop Game

## Overview
CafeConnect3D is a 3D multiplayer coffee shop simulation game built with Unity and Mirror Networking. Players work together to serve customers, take orders, and manage a busy coffee shop in real-time.

## Game Features

### Core Gameplay
- **Multiplayer Support**: Up to 20 players can join a coffee shop
- **Role-Based Gameplay**: Waiters, Baristas, and Managers with different abilities
- **Customer AI**: Smart NPCs that enter, order, wait, and leave based on service quality
- **Order Management**: Real-time order tracking and preparation system
- **Step-by-Step Workflow**: Complete customer service from entry to payment

### Game Flow
1. **Customer Entry**: NPCs spawn and find available seats
2. **Decision Making**: Customers review menu and decide what to order
3. **Order Taking**: Players interact with customers to take orders
4. **Order Processing**: Kitchen staff prepare orders using various equipment
5. **Service Delivery**: Completed orders are delivered to customers
6. **Payment & Exit**: Satisfied customers pay and leave

### Player Roles
- **Waiter**: Take orders from customers, deliver food, clean tables
- **Barista**: Operate coffee machines, prepare beverages, fulfill orders
- **Manager**: Oversee operations, handle special requests, manage inventory

## Technical Features

### Networking
- **Mirror Networking**: Reliable multiplayer framework
- **Authority System**: Proper client-server architecture
- **Synchronized State**: Real-time game state across all clients
- **Command/RPC System**: Secure client-server communication

### AI Systems
- **Customer Behavior**: State-based AI with patience system
- **Pathfinding**: NavMesh-based movement for NPCs
- **Dynamic Spawning**: Continuous customer flow management

### Audio System
- **3D Spatial Audio**: Positional sound effects
- **Dynamic Music**: Context-aware background music
- **Ambient Sounds**: Coffee shop atmosphere audio

## Project Structure

```
Assets/_Project/
├── Art/              # 3D models, textures, materials
├── Audio/            # Music, SFX, voice files
├── Scripts/          # All C# code organized by function
│   ├── Core/         # Managers, data structures, utilities
│   ├── Networking/   # Client/server/shared network code
│   ├── Gameplay/     # Player, NPCs, interactions, orders
│   ├── UI/           # Menus, HUD, dialogs
│   └── Testing/      # Unit and integration tests
├── Prefabs/          # Reusable game objects
├── Scenes/           # Unity scene files
└── Settings/         # ScriptableObject configurations
```

## Getting Started

### Prerequisites
- Unity 2022.3 LTS or newer
- Mirror Networking package
- TextMeshPro package

### Setup Instructions
1. Open Unity Hub and add the CafeConnect3D project
2. Install required packages via Package Manager:
   - Mirror Networking
   - TextMeshPro
3. Open the Main scene in `Assets/_Project/Scenes/Main/`
4. Press Play to start the game in editor

### Building
1. Go to File > Build Settings
2. Add scenes from `Assets/_Project/Scenes/Main/`
3. Select target platform
4. Click Build

## Network Setup

### Hosting a Game
1. Start the game
2. Click "Start Game" to host
3. Game will start accepting connections

### Joining a Game
1. Start the game
2. Click "Join Game"
3. Enter server IP address
4. Click Connect

## Gameplay Controls

### Movement
- **WASD**: Move around the coffee shop
- **Mouse**: Look around

### Interactions
- **E**: Interact with customers, equipment, or objects
- **Tab**: Toggle order board
- **ESC**: Open/close game menu

### UI Navigation
- **Mouse**: Navigate menus and UI elements
- **Left Click**: Select/confirm
- **Right Click**: Cancel/back

## Development Guidelines

### Code Standards
- Follow C# naming conventions
- Use XMLDoc comments for public methods
- Implement proper error handling
- Write unit tests for core functionality

### Network Programming
- Use `[Command]` for client-to-server calls
- Use `[ClientRpc]` for server-to-client calls
- Use `[SyncVar]` for synchronized variables
- Always validate inputs on server

### Performance Considerations
- Use object pooling for frequently spawned objects
- Implement LOD system for distant objects
- Optimize UI updates to avoid frame drops
- Profile network bandwidth usage

## Contributing
1. Fork the repository
2. Create a feature branch
3. Follow coding standards
4. Write tests for new features
5. Submit a pull request

## License
This project is licensed under the MIT License - see the LICENSE file for details.

## Support
For technical support or questions:
- Check the documentation in `Documentation/`
- Review the setup guide in `Documentation/Setup/`
- Create an issue in the project repository
