// GameManager.cs
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    public int maxCustomers = 50;
    public float customerSpawnInterval = 5f;
    public Transform[] customerSpawnPoints;
    public Transform[] customerSeatPositions;

    [Header("Game State")]
    [SyncVar] public int totalCustomersServed = 0;
    [SyncVar] public float totalRevenue = 0f;
    [SyncVar] public GameState currentGameState = GameState.Waiting;

    [Header("Prefabs")]
    public GameObject customerPrefab;
    public GameObject playerPrefab;

    [Header("Asset Integration")]
    public AssetManager assetManager;
    public AssetPlacementTool placementTool;
    public TableManager tableManager;

    private List<Customer> activeCustomers = new List<Customer>();
    private List<NetworkPlayer> connectedPlayers = new List<NetworkPlayer>();
    private OrderManager orderManager;

    public static GameManager Instance { get; private set; }

    public enum GameState
    {
        Waiting,
        Playing,
        Paused,
        GameOver
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        orderManager = FindObjectOfType<OrderManager>();

        // Initialize asset management
        InitializeAssetSystems();

        if (isServer)
        {
            StartGame();
        }
    }

    void InitializeAssetSystems()
    {
        // Find or wait for AssetManager
        if (assetManager == null)
        {
            assetManager = AssetManager.Instance;
        }

        // Find or create AssetPlacementTool
        if (placementTool == null)
        {
            placementTool = FindObjectOfType<AssetPlacementTool>();
        }

        // Find or create TableManager
        if (tableManager == null)
        {
            tableManager = GetComponent<TableManager>();
            if (tableManager == null)
            {
                tableManager = gameObject.AddComponent<TableManager>();
            }
        }

        // Update spawn points and seating from placement tool
        UpdateSpawnPointsFromPlacement();
    }

    void UpdateSpawnPointsFromPlacement()
    {
        if (placementTool != null)
        {
            // Get customer spawn points from placement tool
            CustomerSpawn[] spawns = FindObjectsOfType<CustomerSpawn>();
            if (spawns.Length > 0)
            {
                customerSpawnPoints = new Transform[spawns.Length];
                for (int i = 0; i < spawns.Length; i++)
                {
                    customerSpawnPoints[i] = spawns[i].transform;
                }
            }

            // Get table positions for seating
            List<Transform> tables = placementTool.GetTableTransforms();
            if (tables.Count > 0)
            {
                customerSeatPositions = tables.ToArray();

                // Register with table manager
                if (tableManager != null)
                {
                    tableManager.RegisterTables(tables);
                }
            }
        }
    }

    [Server]
    public void StartGame()
    {
        currentGameState = GameState.Playing;
        InvokeRepeating(nameof(SpawnCustomer), 2f, customerSpawnInterval);
        RpcGameStarted();
    }

    [ClientRpc]
    void RpcGameStarted()
    {
        UIManager.Instance?.ShowGameUI();
        AudioManager.Instance?.PlayBackgroundMusic();
    }

    [Server]
    void SpawnCustomer()
    {
        if (activeCustomers.Count >= maxCustomers || currentGameState != GameState.Playing)
            return;

        // Use AssetManager to get random customer if available
        GameObject customerToSpawn = customerPrefab;
        if (assetManager != null)
        {
            GameObject randomCustomer = assetManager.GetRandomCustomer();
            if (randomCustomer != null)
            {
                customerToSpawn = randomCustomer;
            }
        }

        // Get spawn point (fallback to default if needed)
        Vector3 spawnPosition = Vector3.zero;
        if (customerSpawnPoints != null && customerSpawnPoints.Length > 0)
        {
            Transform spawnPoint = customerSpawnPoints[Random.Range(0, customerSpawnPoints.Length)];
            spawnPosition = spawnPoint.position;
        }
        else
        {
            // Fallback spawn position
            spawnPosition = new Vector3(Random.Range(-5f, 5f), 0f, 10f);
        }

        GameObject customerObj = Instantiate(customerToSpawn, spawnPosition, Quaternion.identity);
        NetworkServer.Spawn(customerObj);

        Customer customer = customerObj.GetComponent<Customer>();
        activeCustomers.Add(customer);
        customer.Initialize(GetAvailableSeat());
    }

    Transform GetAvailableSeat()
    {
        foreach (Transform seat in customerSeatPositions)
        {
            bool isOccupied = false;
            foreach (Customer customer in activeCustomers)
            {
                if (customer.targetSeat == seat)
                {
                    isOccupied = true;
                    break;
                }
            }
            if (!isOccupied)
                return seat;
        }
        return customerSeatPositions[0]; // Default fallback
    }

    [Server]
    public void RegisterPlayer(NetworkPlayer player)
    {
        if (!connectedPlayers.Contains(player))
        {
            connectedPlayers.Add(player);
            RpcPlayerJoined(player.playerName);
        }
    }

    [ClientRpc]
    void RpcPlayerJoined(string playerName)
    {
        UIManager.Instance?.ShowPlayerJoinedMessage(playerName);
    }

    [Server]
    public void CustomerServed(Customer customer, float orderValue)
    {
        totalCustomersServed++;
        totalRevenue += orderValue;

        if (activeCustomers.Contains(customer))
        {
            activeCustomers.Remove(customer);
        }

        RpcCustomerServed(orderValue);
    }

    [ClientRpc]
    void RpcCustomerServed(float orderValue)
    {
        UIManager.Instance?.UpdateScore(totalCustomersServed, totalRevenue);
        AudioManager.Instance?.PlayOrderCompleteSound();
    }

    public void InitializeServer()
    {
        if (!isServer) return;

        Debug.Log("[GameManager] Initializing server...");

        // Initialize game state
        currentGameState = GameState.Playing;
        totalCustomersServed = 0;
        totalRevenue = 0f;

        // Initialize managers
        if (orderManager == null)
        {
            orderManager = FindObjectOfType<OrderManager>();
        }

        // Start customer spawning
        if (customerPrefab != null)
        {
            InvokeRepeating(nameof(SpawnCustomer), 2f, customerSpawnInterval);
        }

        Debug.Log("[GameManager] Server initialization complete");
    }
}
