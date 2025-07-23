using Mirror;
using UnityEngine;
using System.Collections.Generic;
using CafeConnect3D.Gameplay.Player;

namespace CafeConnect3D.Networking
{
    /// <summary>
    /// Main network manager for CafeConnect3D
    /// Handles player connections, room management, and server configuration
    /// </summary>
    public class CafeNetworkManager : NetworkManager
    {
        [Header("Cafe Settings")]
        [SerializeField] private new GameObject playerPrefab;
        #pragma warning disable 0414
        [SerializeField] private int maxPlayersPerCafe = 20;
        #pragma warning restore 0414
        [SerializeField] private Transform[] spawnPoints;

        [Header("Server Configuration")]
        [SerializeField] private ushort serverPort = 7777;
        #pragma warning disable 0414
        [SerializeField] private string serverAddress = "localhost";
        #pragma warning restore 0414

        // Connected players tracking
        private Dictionary<uint, PlayerInfo> connectedPlayers = new Dictionary<uint, PlayerInfo>();

        #region Server Events

        public override void OnStartServer()
        {
            base.OnStartServer();
            Debug.Log($"[CafeServer] Server started on port {serverPort}");
            InitializeServerSystems();
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            // Get spawn position
            Vector3 spawnPos = GetAvailableSpawnPoint();
            Quaternion spawnRot = Quaternion.identity;

            // Instantiate player
            GameObject player = Instantiate(playerPrefab, spawnPos, spawnRot);

            // Setup player data
            var playerController = player.GetComponent<PlayerController>();
            playerController.Initialize((uint)conn.connectionId);

            // Add to network
            NetworkServer.AddPlayerForConnection(conn, player);

            // Track player
            connectedPlayers[(uint)conn.connectionId] = new PlayerInfo
            {
                connectionId = (uint)conn.connectionId,
                playerName = $"Player_{conn.connectionId}",
                joinTime = NetworkTime.time
            };

            Debug.Log($"[CafeServer] Player {conn.connectionId} joined. Total players: {connectedPlayers.Count}");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            // Remove player tracking
            if (connectedPlayers.ContainsKey((uint)conn.connectionId))
            {
                connectedPlayers.Remove((uint)conn.connectionId);
                Debug.Log($"[CafeServer] Player {conn.connectionId} disconnected. Remaining players: {connectedPlayers.Count}");
            }

            base.OnServerDisconnect(conn);
        }

        #endregion

        #region Client Events

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("[CafeClient] Connected to server");
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("[CafeClient] Disconnected from server");
        }

        #endregion

        #region Helper Methods

        private void InitializeServerSystems()
        {
            // Initialize server-side systems here
            var gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.InitializeServer();
            }
        }

        private Vector3 GetAvailableSpawnPoint()
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                return Vector3.zero;
            }

            // Simple round-robin spawn point selection
            int spawnIndex = connectedPlayers.Count % spawnPoints.Length;
            return spawnPoints[spawnIndex].position;
        }

        #endregion
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public uint connectionId;
        public string playerName;
        public double joinTime;
    }
}
