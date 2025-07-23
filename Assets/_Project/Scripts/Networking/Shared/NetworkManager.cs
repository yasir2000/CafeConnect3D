// NetworkManager.cs
using Mirror;
using UnityEngine;

public class CafeNetworkManager : NetworkManager
{
    [Header("Cafe Settings")]
    public GameObject playerPrefab;
    public GameObject coffeShopPrefab;
    public int maxPlayersPerRoom = 20;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        Vector3 spawnPos = GetRandomSpawnPoint();
        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        NetworkServer.AddPlayerForConnection(conn, player);
    }

    Vector3 GetRandomSpawnPoint()
    {
        // Define spawn points around the coffee shop entrance
        Vector3[] spawnPoints = {
            new Vector3(10f, 0f, 5f),
            new Vector3(12f, 0f, 5f),
            new Vector3(8f, 0f, 7f)
        };
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
