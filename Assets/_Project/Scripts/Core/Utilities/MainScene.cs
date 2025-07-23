// MainScene.cs - Scene initialization script
using UnityEngine;
using Mirror;

public class MainScene : MonoBehaviour
{
    [Header("Required GameObjects")]
    public GameObject gameManagerPrefab;
    public GameObject networkManagerPrefab;

    void Start()
    {
        InitializeScene();
    }

    void InitializeScene()
    {
        // Create Network Manager if it doesn't exist
        if (FindObjectOfType<CafeNetworkManager>() == null)
        {
            if (networkManagerPrefab != null)
            {
                Instantiate(networkManagerPrefab);
            }
            else
            {
                CreateDefaultNetworkManager();
            }
        }

        // Create Game Manager if it doesn't exist
        if (GameManager.Instance == null)
        {
            if (gameManagerPrefab != null)
            {
                Instantiate(gameManagerPrefab);
            }
            else
            {
                CreateDefaultGameManager();
            }
        }

        Debug.Log("Main Scene Initialized!");
    }

    void CreateDefaultNetworkManager()
    {
        GameObject nmObj = new GameObject("NetworkManager");
        CafeNetworkManager nm = nmObj.AddComponent<CafeNetworkManager>();

        // Set default network settings
        nm.networkAddress = "localhost";
        nm.maxConnections = 20;

        Debug.Log("Created default Network Manager");
    }

    void CreateDefaultGameManager()
    {
        GameObject gmObj = new GameObject("GameManager");
        GameManager gm = gmObj.AddComponent<GameManager>();

        // Create spawn points if none exist
        CreateDefaultSpawnPoints(gm);

        Debug.Log("Created default Game Manager");
    }

    void CreateDefaultSpawnPoints(GameManager gm)
    {
        // Create customer spawn points
        GameObject spawnPointsParent = new GameObject("Customer Spawn Points");
        Transform[] spawnPoints = new Transform[4];

        for (int i = 0; i < 4; i++)
        {
            GameObject spawnPoint = new GameObject($"Spawn Point {i + 1}");
            spawnPoint.transform.parent = spawnPointsParent.transform;
            spawnPoint.transform.position = new Vector3(i * 2f - 3f, 0f, -5f);
            spawnPoints[i] = spawnPoint.transform;
        }

        // Create customer seat positions
        GameObject seatsParent = new GameObject("Customer Seats");
        Transform[] seatPositions = new Transform[8];

        for (int i = 0; i < 8; i++)
        {
            GameObject seat = new GameObject($"Seat {i + 1}");
            seat.transform.parent = seatsParent.transform;
            seat.transform.position = new Vector3((i % 4) * 2f - 3f, 0f, (i / 4) * 2f);
            seatPositions[i] = seat.transform;
        }

        gm.customerSpawnPoints = spawnPoints;
        gm.customerSeatPositions = seatPositions;
    }
}
