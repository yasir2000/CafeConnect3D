// CafeClient.cs
using Mirror;
using UnityEngine;
using CafeConnect3D.Networking;

public class CafeClient : MonoBehaviour
{
    [Header("Connection Settings")]
    public string serverAddress = "localhost";
    public ushort serverPort = 7777;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = FindObjectOfType<CafeNetworkManager>();
        SetupClientCallbacks();
    }

    void SetupClientCallbacks()
    {
        NetworkClient.RegisterHandler<WelcomeMessage>(OnWelcomeReceived);
        NetworkClient.RegisterHandler<OrderErrorMessage>(OnOrderError);
    }

    public void ConnectToServer()
    {
        networkManager.networkAddress = serverAddress;
        networkManager.StartClient();
    }

    public void DisconnectFromServer()
    {
        networkManager.StopClient();
    }

    void OnWelcomeReceived(WelcomeMessage message)
    {
        Debug.Log($"Server welcome: {message.welcomeText}");
        FindObjectOfType<NotificationUI>().ShowNotification(message.welcomeText);

        // Sync server time
        SyncServerTime(message.serverTime);
    }

    void OnOrderError(OrderErrorMessage message)
    {
        Debug.LogError($"Order error: {message.error}");
        FindObjectOfType<NotificationUI>().ShowNotification($"Order failed: {message.error}");
    }

    void SyncServerTime(double serverTime)
    {
        // Implement time synchronization logic
        double networkDelay = (NetworkTime.rtt / 2.0);
        double syncedTime = serverTime + networkDelay;

        // Update local time offset
        GameTimeManager.Instance.SetServerTime(syncedTime);
    }
}
