// SceneAutoSetup.cs
using UnityEngine;
using System.Collections;

public class SceneAutoSetup : MonoBehaviour
{
    [Header("Setup Configuration")]
    public bool setupOnStart = true;
    public bool createFloor = true;
    public bool setupLighting = true;
    public bool setupAssetPlacement = true;
    public bool setupNetworking = true;

    [Header("Scene Settings")]
    public Vector2 shopDimensions = new Vector2(20f, 15f);
    public Material floorMaterial;
    public Material wallMaterial;

    private bool isSetupComplete = false;

    void Start()
    {
        if (setupOnStart)
        {
            StartCoroutine(AutoSetupScene());
        }
    }

    IEnumerator AutoSetupScene()
    {
        Debug.Log("Starting automatic scene setup...");

        yield return StartCoroutine(SetupBasicEnvironment());
        yield return StartCoroutine(SetupAssetSystems());
        yield return StartCoroutine(SetupGameManagers());
        yield return StartCoroutine(FinalizeSetup());

        isSetupComplete = true;
        Debug.Log("Scene setup complete!");
    }

    IEnumerator SetupBasicEnvironment()
    {
        if (createFloor)
        {
            CreateFloor();
        }

        if (setupLighting)
        {
            SetupLighting();
        }

        yield return null;
    }

    void CreateFloor()
    {
        if (GameObject.Find("CoffeeShopFloor") == null)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "CoffeeShopFloor";
            floor.transform.position = Vector3.zero;
            floor.transform.localScale = new Vector3(shopDimensions.x / 10f, 1f, shopDimensions.y / 10f);

            if (floorMaterial != null)
            {
                floor.GetComponent<Renderer>().material = floorMaterial;
            }
            else
            {
                // Create a simple wood-like material
                Material woodMaterial = new Material(Shader.Find("Standard"));
                woodMaterial.color = new Color(0.6f, 0.4f, 0.2f);
                floor.GetComponent<Renderer>().material = woodMaterial;
            }

            Debug.Log("Created coffee shop floor");
        }
    }

    void SetupLighting()
    {
        // Ensure we have a directional light
        if (FindObjectOfType<Light>() == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = Color.white;
            light.intensity = 1f;
            lightObj.transform.rotation = Quaternion.Euler(45f, 45f, 0f);

            Debug.Log("Created directional light");
        }

        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.5f, 0.7f, 1f);
        RenderSettings.ambientEquatorColor = new Color(0.4f, 0.4f, 0.4f);
        RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.2f);
    }

    IEnumerator SetupAssetSystems()
    {
        // Create or find AssetManager
        AssetManager assetManager = FindObjectOfType<AssetManager>();
        if (assetManager == null)
        {
            GameObject assetManagerObj = new GameObject("AssetManager");
            assetManager = assetManagerObj.AddComponent<AssetManager>();
            Debug.Log("Created AssetManager");
        }

        // Wait for AssetManager to initialize
        yield return new WaitUntil(() => AssetManager.Instance != null);

        // Create or find AssetPlacementTool
        if (setupAssetPlacement)
        {
            AssetPlacementTool placementTool = FindObjectOfType<AssetPlacementTool>();
            if (placementTool == null)
            {
                GameObject placementObj = new GameObject("AssetPlacementTool");
                placementTool = placementObj.AddComponent<AssetPlacementTool>();
                placementTool.shopDimensions = shopDimensions;
                placementTool.autoPlaceOnStart = true;
                Debug.Log("Created AssetPlacementTool");
            }
        }

        yield return null;
    }

    IEnumerator SetupGameManagers()
    {
        // Create GameManager if it doesn't exist
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            GameObject gameManagerObj = new GameObject("GameManager");
            gameManager = gameManagerObj.AddComponent<GameManager>();
            Debug.Log("Created GameManager");
        }

        // Create UIManager if it doesn't exist
        UIManager uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            GameObject uiManagerObj = new GameObject("UIManager");
            uiManager = uiManagerObj.AddComponent<UIManager>();
            Debug.Log("Created UIManager");
        }

        // Create AudioManager if it doesn't exist
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager == null)
        {
            GameObject audioManagerObj = new GameObject("AudioManager");
            audioManager = audioManagerObj.AddComponent<AudioManager>();
            Debug.Log("Created AudioManager");
        }

        // Create OrderManager if it doesn't exist
        OrderManager orderManager = FindObjectOfType<OrderManager>();
        if (orderManager == null)
        {
            GameObject orderManagerObj = new GameObject("OrderManager");
            orderManager = orderManagerObj.AddComponent<OrderManager>();
            Debug.Log("Created OrderManager");
        }

        // Create MenuManager if it doesn't exist
        MenuManager menuManager = FindObjectOfType<MenuManager>();
        if (menuManager == null)
        {
            GameObject menuManagerObj = new GameObject("MenuManager");
            menuManager = menuManagerObj.AddComponent<MenuManager>();
            Debug.Log("Created MenuManager");
        }

        yield return null;
    }

    IEnumerator FinalizeSetup()
    {
        // Setup networking if enabled
        if (setupNetworking)
        {
            SetupNetworking();
        }

        // Configure camera if needed
        SetupCamera();

        // Create basic UI Canvas
        SetupUICanvas();

        yield return null;
    }

    void SetupNetworking()
    {
        CafeNetworkManager networkManager = FindObjectOfType<CafeNetworkManager>();
        if (networkManager == null)
        {
            GameObject networkObj = new GameObject("CafeNetworkManager");
            networkManager = networkObj.AddComponent<CafeNetworkManager>();
            Debug.Log("Created CafeNetworkManager");
        }
    }

    void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            GameObject cameraObj = new GameObject("Main Camera");
            mainCamera = cameraObj.AddComponent<Camera>();
            cameraObj.tag = "MainCamera";

            // Position camera for good coffee shop view
            cameraObj.transform.position = new Vector3(0f, 15f, -10f);
            cameraObj.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            Debug.Log("Created main camera");
        }

        // Add camera controller if not present
        if (mainCamera.GetComponent<CameraController>() == null)
        {
            mainCamera.gameObject.AddComponent<CameraController>();
        }
    }

    void SetupUICanvas()
    {
        if (FindObjectOfType<Canvas>() == null)
        {
            GameObject canvasObj = new GameObject("UI Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            Debug.Log("Created UI Canvas");
        }
    }

    // Public methods for manual setup
    [ContextMenu("Setup Scene")]
    public void SetupSceneManual()
    {
        StartCoroutine(AutoSetupScene());
    }

    [ContextMenu("Reset Scene")]
    public void ResetScene()
    {
        // Clear all created objects
        AssetPlacementTool placementTool = FindObjectOfType<AssetPlacementTool>();
        if (placementTool != null)
        {
            placementTool.ClearAllAssets();
        }

        isSetupComplete = false;
    }

    public bool IsSetupComplete()
    {
        return isSetupComplete;
    }
}

// Simple camera controller for testing
public class CameraController : MonoBehaviour
{
    [Header("Camera Controls")]
    public float moveSpeed = 10f;
    public float rotationSpeed = 2f;
    public float zoomSpeed = 5f;

    void Update()
    {
        // Mouse look (right click to rotate)
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
            float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

            transform.Rotate(-mouseY, mouseX, 0f);
        }

        // WASD movement
        Vector3 moveDirection = Vector3.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection += transform.forward;
        if (Input.GetKey(KeyCode.S)) moveDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A)) moveDirection -= transform.right;
        if (Input.GetKey(KeyCode.D)) moveDirection += transform.right;
        if (Input.GetKey(KeyCode.Q)) moveDirection -= transform.up;
        if (Input.GetKey(KeyCode.E)) moveDirection += transform.up;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Mouse scroll for zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += transform.forward * scroll * zoomSpeed;
    }
}
