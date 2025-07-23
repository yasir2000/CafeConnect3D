// PrefabCreator.cs - Runtime prefab creation for testing
using UnityEngine;
using Mirror;
using CafeConnect3D.Networking;

public class PrefabCreator : MonoBehaviour
{
    [Header("Auto-Create Prefabs")]
    public bool createPlayerPrefab = true;
    public bool createCustomerPrefab = true;

    void Start()
    {
        if (createPlayerPrefab)
            CreatePlayerPrefab();

        if (createCustomerPrefab)
            CreateCustomerPrefab();
    }

    void CreatePlayerPrefab()
    {
        // Create a basic player prefab for testing
        GameObject player = new GameObject("Player");

        // Add essential components
        player.AddComponent<CharacterController>();
        player.AddComponent<NetworkIdentity>();
        player.AddComponent<NetworkPlayer>();

        // Add visual representation
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.transform.parent = player.transform;
        visual.transform.localPosition = Vector3.zero;
        visual.name = "Visual";

        // Make it blue to distinguish from customers
        Renderer renderer = visual.GetComponent<Renderer>();
        renderer.material.color = Color.blue;

        // Set as player prefab in GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.playerPrefab = player;
        }

        // Register with Network Manager
        CafeNetworkManager nm = FindObjectOfType<CafeNetworkManager>();
        if (nm != null)
        {
            nm.playerPrefab = player;
        }

        Debug.Log("Created Player Prefab");
    }

    void CreateCustomerPrefab()
    {
        // Create a basic customer prefab for testing
        GameObject customer = new GameObject("Customer");

        // Add essential components
        customer.AddComponent<UnityEngine.AI.NavMeshAgent>();
        customer.AddComponent<NetworkIdentity>();
        customer.AddComponent<Customer>();

        // Add visual representation
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        visual.transform.parent = customer.transform;
        visual.transform.localPosition = Vector3.zero;
        visual.name = "Visual";

        // Make it red to distinguish from players
        Renderer renderer = visual.GetComponent<Renderer>();
        renderer.material.color = Color.red;

        // Add name display
        GameObject nameCanvas = new GameObject("NameCanvas");
        nameCanvas.transform.parent = customer.transform;
        nameCanvas.transform.localPosition = new Vector3(0, 2.5f, 0);

        Canvas canvas = nameCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.sortingOrder = 10;

        nameCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        nameCanvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        GameObject nameText = new GameObject("NameText");
        nameText.transform.parent = nameCanvas.transform;
        nameText.transform.localPosition = Vector3.zero;

        var textMesh = nameText.AddComponent<TMPro.TextMeshProUGUI>();
        textMesh.text = "Customer";
        textMesh.fontSize = 5;
        textMesh.alignment = TMPro.TextAlignmentOptions.Center;

        // Set canvas size
        RectTransform canvasRect = nameCanvas.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(3, 1);

        RectTransform textRect = nameText.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(3, 1);
        textRect.anchoredPosition = Vector3.zero;

        // Set as customer prefab in GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.customerPrefab = customer;
        }

        // Register with Network Manager as spawnable
        CafeNetworkManager nm = FindObjectOfType<CafeNetworkManager>();
        if (nm != null)
        {
            // Add to spawnable prefabs list
            var spawnablePrefabs = new System.Collections.Generic.List<GameObject>(nm.spawnPrefabs);
            if (!spawnablePrefabs.Contains(customer))
            {
                spawnablePrefabs.Add(customer);
                nm.spawnPrefabs = spawnablePrefabs;
            }
        }

        Debug.Log("Created Customer Prefab");
    }
}
