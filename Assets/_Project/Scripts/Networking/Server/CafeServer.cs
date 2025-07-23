// CafeServer.cs
using Mirror;
using UnityEngine;
using System.Collections.Generic;

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
        NetworkServer.RegisterHandler<PlayerJoinMessage>(OnPlayerJoin);
        NetworkServer.RegisterHandler<PlayerOrderMessage>(OnPlayerOrder);
        NetworkServer.RegisterHandler<PlayerInteractionMessage>(OnPlayerInteraction);
    }

    void OnPlayerJoin(NetworkConnection conn, PlayerJoinMessage message)
    {
        PlayerData newPlayer = new PlayerData
        {
            connectionId = conn.connectionId,
            playerName = message.playerName,
            joinTime = NetworkTime.time
        };

        connectedPlayers[conn.connectionId] = newPlayer;

        // Send welcome message
        conn.Send(new WelcomeMessage
        {
            welcomeText = $"Welcome to Cafe Connect, {message.playerName}!",
            serverTime = NetworkTime.time
        });

        Debug.Log($"Player {message.playerName} joined. Total players: {connectedPlayers.Count}");
    }

    void OnPlayerOrder(NetworkConnection conn, PlayerOrderMessage message)
    {
        // Validate order
        if(ValidateOrder(message.orderItems))
        {
            // Process order through OrderManager
            OrderManager orderManager = FindObjectOfType<OrderManager>();
            orderManager.CmdSubmitOrder(conn.identity.netId, message.orderItems);
        }
        else
        {
            // Send error message
            conn.Send(new OrderErrorMessage { error = "Invalid order items" });
        }
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
