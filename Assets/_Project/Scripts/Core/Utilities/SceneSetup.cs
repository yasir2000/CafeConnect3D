// SceneSetup.cs
using UnityEngine;
using Mirror;

public class SceneSetup : MonoBehaviour
{
    [Header("Scene Setup")]
    public GameObject gameManagerPrefab;
    public GameObject audioManagerPrefab;
    public GameObject uiManagerPrefab;
    public GameObject orderManagerPrefab;
    public GameObject menuManagerPrefab;

    [Header("Network Prefabs")]
    public GameObject playerPrefab;
    public GameObject customerPrefab;

    void Start()
    {
        SetupScene();
    }

    void SetupScene()
    {
        // Create essential managers if they don't exist
        if (GameManager.Instance == null && gameManagerPrefab != null)
        {
            Instantiate(gameManagerPrefab);
        }

        if (AudioManager.Instance == null && audioManagerPrefab != null)
        {
            Instantiate(audioManagerPrefab);
        }

        if (UIManager.Instance == null && uiManagerPrefab != null)
        {
            Instantiate(uiManagerPrefab);
        }

        if (OrderManager.Instance == null && orderManagerPrefab != null)
        {
            Instantiate(orderManagerPrefab);
        }

        if (MenuManager.Instance == null && menuManagerPrefab != null)
        {
            Instantiate(menuManagerPrefab);
        }

        Debug.Log("Coffee Shop Scene Setup Complete!");
    }
}
