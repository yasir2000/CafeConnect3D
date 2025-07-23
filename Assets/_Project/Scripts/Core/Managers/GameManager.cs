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
        if (isServer)
        {
            StartGame();
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

        Transform spawnPoint = customerSpawnPoints[Random.Range(0, customerSpawnPoints.Length)];
        GameObject customerObj = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
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
}
