// CafeServer.cs
using Mirror;
using UnityEngine;
using System.Collections.Generic;

namespace CafeConnect3D.Networking
{
    public class CafeServer : NetworkBehaviour
{
    [Header("Server Settings")]
    public int maxPlayersPerRoom = 20;
    public float serverTickRate = 20f;

    private Dictionary<uint, PlayerData> connectedPlayers;
    private CafeGameState gameState;

    void Start()
    {
        if(isServer)
        {
            InitializeServer();
        }
    }

    void InitializeServer()
    {
        connectedPlayers = new Dictionary<uint, PlayerData>();
        gameState = new CafeGameState();

        // Set server tick rate
        Application.targetFrameRate = (int)serverTickRate;

        Debug.Log("Cafe Server initialized");
    }

    public override void OnStartServer()
    {
        RegisterNetworkHandlers();
    }

    void RegisterNetworkHandlers()
    {
        NetworkServer.RegisterHandler<PlayerJoinMessage>(OnPlayerJoin);
        NetworkServer.RegisterHandler<PlayerOrderMessage>(OnPlayerOrder);
        NetworkServer.RegisterHandler<PlayerInteractionMessage>(OnPlayerInteraction);
    }

    void OnPlayerJoin(NetworkConnectionToClient conn, PlayerJoinMessage message)
    {
        PlayerData newPlayer = new PlayerData((uint)conn.connectionId, message.playerName);
        newPlayer.position = Vector3.zero;
        newPlayer.rotation = Quaternion.identity;

        connectedPlayers[(uint)conn.connectionId] = newPlayer;

        // Send welcome message
        conn.Send(new WelcomeMessage
        {
            welcomeText = $"Welcome to Cafe Connect, {message.playerName}!",
            serverTime = NetworkTime.time
        });

        Debug.Log($"Player {message.playerName} joined. Total players: {connectedPlayers.Count}");
    }

    void OnPlayerOrder(NetworkConnectionToClient conn, PlayerOrderMessage message)
    {
        // Validate order
        if(ValidateOrder(message.orderItems))
        {
            // Process order through OrderManager
            OrderManager orderManager = FindObjectOfType<OrderManager>();
            orderManager.CmdSubmitOrder((uint)conn.connectionId, message.orderItems);
        }
        else
        {
            // Send error message
            conn.Send(new OrderErrorMessage { error = "Invalid order items" });
        }
    }

    void OnPlayerInteraction(NetworkConnectionToClient conn, PlayerInteractionMessage message)
    {
        // Handle player interactions
        Debug.Log($"Player {message.playerId} interacted with {message.targetObjectId}");
    }

    bool ValidateOrder(OrderItem[] items)
    {
        MenuManager menuManager = FindObjectOfType<MenuManager>();

        foreach(OrderItem item in items)
        {
            MenuItem menuItem = menuManager.GetMenuItem(item.menuItemId);
            if(menuItem == null || item.quantity <= 0)
            {
                return false;
            }
        }

        return true;
    }

    public override void OnStopServer()
    {
        Debug.Log("Cafe Server stopped");
    }
}

// Network Messages
public struct PlayerJoinMessage : NetworkMessage
{
    public string playerName;
    public Color playerColor;
}

public struct PlayerOrderMessage : NetworkMessage
{
    public OrderItem[] orderItems;
}

public struct WelcomeMessage : NetworkMessage
{
    public string welcomeText;
    public double serverTime;
}

public struct OrderErrorMessage : NetworkMessage
{
    public string error;
}

}
