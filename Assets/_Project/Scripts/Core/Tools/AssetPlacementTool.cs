// AssetPlacementTool.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AssetPlacementTool : MonoBehaviour
{
    [Header("Coffee Shop Layout")]
    public Vector2 shopDimensions = new Vector2(20f, 15f);
    public Vector3 shopCenter = Vector3.zero;
    public float wallThickness = 0.3f;

    [Header("Placement Settings")]
    public bool autoPlaceOnStart = true;
    public bool clearExistingAssets = true;
    public bool generateNavMesh = true;
    public bool createCustomerSpawns = true;

    [Header("Table Layout")]
    [Range(2, 8)]
    public int numberOfTables = 6;
    public float minTableDistance = 3f;
    public float wallClearance = 2f;

    [Header("Equipment Placement")]
    public Vector3 counterPosition = new Vector3(0f, 0f, -6f);
    public Vector3 counterSize = new Vector3(8f, 1f, 1.5f);
    public bool placeEspressoMachine = true;
    public bool placeCashRegister = true;
    public bool placeDisplayCase = true;

    [Header("Decoration Settings")]
    [Range(0, 20)]
    public int numberOfPlants = 5;
    [Range(0, 15)]
    public int numberOfArtPieces = 3;
    [Range(0, 10)]
    public int numberOfMenuBoards = 2;

    [Header("Customer System")]
    public Transform customerSpawnParent;
    public Transform tableParent;
    public Transform furnitureParent;
    public Transform decorationParent;
    public Transform equipmentParent;

    private AssetManager assetManager;
    private List<Vector3> tablePositions = new List<Vector3>();
    private List<Vector3> availableWallPositions = new List<Vector3>();
    private List<Transform> createdTables = new List<Transform>();

    // Placement data for integration
    public List<PlacedAsset> placedAssets = new List<PlacedAsset>();

    [System.Serializable]
    public class PlacedAsset
    {
        public string assetType;
        public string assetName;
        public Vector3 position;
        public Quaternion rotation;
        public Transform transform;
        public bool isInteractable;
        public string[] interactionTypes;
    }

    void Start()
    {
        if (autoPlaceOnStart)
        {
            StartCoroutine(AutoPlaceCoffeeShop());
        }
    }

    void Awake()
    {
        // Ensure parent objects exist
        CreateParentObjects();
    }

    void CreateParentObjects()
    {
        if (customerSpawnParent == null)
        {
            GameObject spawns = new GameObject("CustomerSpawns");
            spawns.transform.SetParent(transform);
            customerSpawnParent = spawns.transform;
        }

        if (tableParent == null)
        {
            GameObject tables = new GameObject("Tables");
            tables.transform.SetParent(transform);
            tableParent = tables.transform;
        }

        if (furnitureParent == null)
        {
            GameObject furniture = new GameObject("Furniture");
            furniture.transform.SetParent(transform);
            furnitureParent = furniture.transform;
        }

        if (decorationParent == null)
        {
            GameObject decorations = new GameObject("Decorations");
            decorations.transform.SetParent(transform);
            decorationParent = decorations.transform;
        }

        if (equipmentParent == null)
        {
            GameObject equipment = new GameObject("Equipment");
            equipment.transform.SetParent(transform);
            equipmentParent = equipment.transform;
        }
    }

    IEnumerator AutoPlaceCoffeeShop()
    {
        Debug.Log("Starting automatic coffee shop placement...");

        // Wait for AssetManager to be ready
        yield return new WaitUntil(() => AssetManager.Instance != null);
        assetManager = AssetManager.Instance;

        if (clearExistingAssets)
        {
            ClearExistingAssets();
        }

        // Step 1: Place walls and structure
        yield return StartCoroutine(PlaceWallsAndStructure());

        // Step 2: Place counter and equipment
        yield return StartCoroutine(PlaceCounterAndEquipment());

        // Step 3: Calculate table positions
        CalculateTablePositions();

        // Step 4: Place tables and chairs
        yield return StartCoroutine(PlaceTablesAndChairs());

        // Step 5: Place decorations
        yield return StartCoroutine(PlaceDecorations());

        // Step 6: Create customer spawn points
        if (createCustomerSpawns)
        {
            CreateCustomerSpawnPoints();
        }

        // Step 7: Generate NavMesh
        if (generateNavMesh)
        {
            yield return StartCoroutine(GenerateNavMesh());
        }

        // Step 8: Integrate with game systems
        IntegrateWithGameSystems();

        Debug.Log("Coffee shop placement complete!");
    }

    void ClearExistingAssets()
    {
        // Clear existing placed assets
        foreach (Transform child in tableParent)
        {
            if (child != null) DestroyImmediate(child.gameObject);
        }

        foreach (Transform child in furnitureParent)
        {
            if (child != null) DestroyImmediate(child.gameObject);
        }

        foreach (Transform child in decorationParent)
        {
            if (child != null) DestroyImmediate(child.gameObject);
        }

        foreach (Transform child in equipmentParent)
        {
            if (child != null) DestroyImmediate(child.gameObject);
        }

        placedAssets.Clear();
        createdTables.Clear();
    }

    IEnumerator PlaceWallsAndStructure()
    {
        // Create floor if not exists
        if (GameObject.Find("CoffeeShopFloor") == null)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "CoffeeShopFloor";
            floor.transform.position = shopCenter;
            floor.transform.localScale = new Vector3(shopDimensions.x / 10f, 1f, shopDimensions.y / 10f);

            // Apply floor material if available
            GameObject floorAsset = assetManager?.GetAsset<GameObject>("environment_floor_wood");
            if (floorAsset != null)
            {
                Renderer floorRenderer = floor.GetComponent<Renderer>();
                if (floorRenderer != null)
                {
                    Renderer assetRenderer = floorAsset.GetComponent<Renderer>();
                    if (assetRenderer != null)
                    {
                        floorRenderer.material = assetRenderer.material;
                    }
                }
            }
        }

        // Calculate wall positions for decoration placement
        CalculateWallPositions();

        yield return null;
    }

    void CalculateWallPositions()
    {
        availableWallPositions.Clear();

        float halfWidth = shopDimensions.x / 2f;
        float halfHeight = shopDimensions.y / 2f;

        // North wall positions
        for (float x = -halfWidth + 2f; x < halfWidth - 2f; x += 2f)
        {
            availableWallPositions.Add(new Vector3(x, 0f, halfHeight - wallThickness));
        }

        // South wall positions
        for (float x = -halfWidth + 2f; x < halfWidth - 2f; x += 2f)
        {
            availableWallPositions.Add(new Vector3(x, 0f, -halfHeight + wallThickness));
        }

        // East wall positions
        for (float z = -halfHeight + 2f; z < halfHeight - 2f; z += 2f)
        {
            availableWallPositions.Add(new Vector3(halfWidth - wallThickness, 0f, z));
        }

        // West wall positions
        for (float z = -halfHeight + 2f; z < halfHeight - 2f; z += 2f)
        {
            availableWallPositions.Add(new Vector3(-halfWidth + wallThickness, 0f, z));
        }
    }

    IEnumerator PlaceCounterAndEquipment()
    {
        // Place counter structure (if we have counter prefab)
        GameObject counterPrefab = assetManager?.GetFurniture("counter");
        if (counterPrefab != null)
        {
            GameObject counter = Instantiate(counterPrefab, counterPosition, Quaternion.identity, equipmentParent);
            counter.name = "ServiceCounter";

            RegisterPlacedAsset("equipment", "counter", counterPosition, Quaternion.identity, counter.transform, true, new string[] { "service" });
        }

        // Place espresso machine
        if (placeEspressoMachine)
        {
            GameObject espressoMachine = assetManager?.GetFurniture("espresso_machine");
            if (espressoMachine != null)
            {
                Vector3 machinePos = counterPosition + new Vector3(-2f, 1f, 0f);
                GameObject machine = Instantiate(espressoMachine, machinePos, Quaternion.identity, equipmentParent);
                machine.name = "EspressoMachine";

                RegisterPlacedAsset("equipment", "espresso_machine", machinePos, Quaternion.identity, machine.transform, true, new string[] { "brew_coffee" });
            }
        }

        // Place cash register
        if (placeCashRegister)
        {
            GameObject cashRegister = assetManager?.GetFurniture("cash_register");
            if (cashRegister == null)
            {
                // Create placeholder
                cashRegister = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cashRegister.GetComponent<Renderer>().material.color = Color.green;
                cashRegister.transform.localScale = new Vector3(0.5f, 0.3f, 0.4f);
            }

            Vector3 registerPos = counterPosition + new Vector3(2f, 1f, 0f);
            GameObject register = Instantiate(cashRegister, registerPos, Quaternion.identity, equipmentParent);
            register.name = "CashRegister";

            RegisterPlacedAsset("equipment", "cash_register", registerPos, Quaternion.identity, register.transform, true, new string[] { "payment" });
        }

        yield return null;
    }

    void CalculateTablePositions()
    {
        tablePositions.Clear();

        float halfWidth = shopDimensions.x / 2f - wallClearance;
        float halfHeight = shopDimensions.y / 2f - wallClearance;

        // Avoid counter area
        Vector3 counterMin = counterPosition - counterSize / 2f - Vector3.one * 2f;
        Vector3 counterMax = counterPosition + counterSize / 2f + Vector3.one * 2f;

        int attempts = 0;
        while (tablePositions.Count < numberOfTables && attempts < 100)
        {
            Vector3 candidatePos = new Vector3(
                Random.Range(-halfWidth, halfWidth),
                0f,
                Random.Range(-halfHeight, halfHeight)
            );

            // Check if position is valid
            bool validPosition = true;

            // Check distance from counter
            if (candidatePos.x >= counterMin.x && candidatePos.x <= counterMax.x &&
                candidatePos.z >= counterMin.z && candidatePos.z <= counterMax.z)
            {
                validPosition = false;
            }

            // Check distance from other tables
            foreach (Vector3 existingTable in tablePositions)
            {
                if (Vector3.Distance(candidatePos, existingTable) < minTableDistance)
                {
                    validPosition = false;
                    break;
                }
            }

            if (validPosition)
            {
                tablePositions.Add(candidatePos);
            }

            attempts++;
        }

        Debug.Log($"Calculated {tablePositions.Count} table positions");
    }

    IEnumerator PlaceTablesAndChairs()
    {
        for (int i = 0; i < tablePositions.Count; i++)
        {
            Vector3 tablePos = tablePositions[i];

            // Determine table type based on position and randomness
            string tableType = Random.value > 0.5f ? "table_round_2p" : "table_square_4p";
            int chairCount = tableType.Contains("2p") ? 2 : 4;

            // Place table
            GameObject tablePrefab = assetManager?.GetFurniture(tableType);
            if (tablePrefab == null)
            {
                // Create placeholder table
                tablePrefab = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                tablePrefab.GetComponent<Renderer>().material.color = Color.brown;
                tablePrefab.transform.localScale = new Vector3(1.5f, 0.1f, 1.5f);
            }

            GameObject table = Instantiate(tablePrefab, tablePos, Quaternion.identity, tableParent);
            table.name = $"Table_{i + 1}";
            createdTables.Add(table.transform);

            RegisterPlacedAsset("furniture", tableType, tablePos, Quaternion.identity, table.transform, true, new string[] { "seating", "ordering" });

            // Place chairs around table
            for (int j = 0; j < chairCount; j++)
            {
                float angle = (360f / chairCount) * j;
                Vector3 chairOffset = Quaternion.Euler(0, angle, 0) * Vector3.forward * 1.2f;
                Vector3 chairPos = tablePos + chairOffset;
                Quaternion chairRot = Quaternion.LookRotation(-chairOffset);

                GameObject chairPrefab = assetManager?.GetFurniture("chair_wooden");
                if (chairPrefab == null)
                {
                    // Create placeholder chair
                    chairPrefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    chairPrefab.GetComponent<Renderer>().material.color = Color.yellow;
                    chairPrefab.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
                }

                GameObject chair = Instantiate(chairPrefab, chairPos, chairRot, tableParent);
                chair.name = $"Chair_{i + 1}_{j + 1}";

                RegisterPlacedAsset("furniture", "chair", chairPos, chairRot, chair.transform, true, new string[] { "seating" });
            }

            yield return null; // Spread work across frames
        }
    }

    IEnumerator PlaceDecorations()
    {
        // Place plants
        for (int i = 0; i < numberOfPlants; i++)
        {
            if (availableWallPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availableWallPositions.Count);
                Vector3 plantPos = availableWallPositions[randomIndex];
                availableWallPositions.RemoveAt(randomIndex);

                GameObject plantPrefab = assetManager?.GetRandomProp("plant");
                if (plantPrefab == null)
                {
                    // Create placeholder plant
                    plantPrefab = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    plantPrefab.GetComponent<Renderer>().material.color = Color.green;
                    plantPrefab.transform.localScale = Vector3.one * 0.8f;
                }

                GameObject plant = Instantiate(plantPrefab, plantPos, Quaternion.identity, decorationParent);
                plant.name = $"Plant_{i + 1}";

                RegisterPlacedAsset("decoration", "plant", plantPos, Quaternion.identity, plant.transform, false, null);
            }

            yield return null;
        }

        // Place art pieces
        for (int i = 0; i < numberOfArtPieces; i++)
        {
            if (availableWallPositions.Count > 0)
            {
                int randomIndex = Random.Range(0, availableWallPositions.Count);
                Vector3 artPos = availableWallPositions[randomIndex] + Vector3.up * 2f;
                availableWallPositions.RemoveAt(randomIndex);

                GameObject artPrefab = assetManager?.GetRandomProp("art");
                if (artPrefab == null)
                {
                    // Create placeholder art
                    artPrefab = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    artPrefab.GetComponent<Renderer>().material.color = Random.ColorHSV();
                    artPrefab.transform.localScale = Vector3.one * 1.5f;
                }

                GameObject art = Instantiate(artPrefab, artPos, Quaternion.identity, decorationParent);
                art.name = $"Art_{i + 1}";

                RegisterPlacedAsset("decoration", "art", artPos, Quaternion.identity, art.transform, false, null);
            }

            yield return null;
        }
    }

    void CreateCustomerSpawnPoints()
    {
        // Create entrance spawn points
        Vector3[] spawnPositions = new Vector3[]
        {
            new Vector3(0f, 0f, shopDimensions.y / 2f - 1f), // Front entrance
            new Vector3(-shopDimensions.x / 2f + 1f, 0f, 0f), // Side entrance
            new Vector3(shopDimensions.x / 2f - 1f, 0f, 0f)  // Side entrance
        };

        for (int i = 0; i < spawnPositions.Length; i++)
        {
            GameObject spawn = new GameObject($"CustomerSpawn_{i + 1}");
            spawn.transform.position = spawnPositions[i];
            spawn.transform.SetParent(customerSpawnParent);

            // Add CustomerSpawn component if it exists
            var spawnComponent = spawn.AddComponent<CustomerSpawn>();
            if (spawnComponent != null)
            {
                // Configure spawn component
            }
        }
    }

    IEnumerator GenerateNavMesh()
    {
        // Note: NavMesh baking requires NavMeshSurface or manual baking
        Debug.Log("NavMesh generation would be handled by Unity's NavMesh system");
        yield return null;
    }

    void RegisterPlacedAsset(string type, string name, Vector3 pos, Quaternion rot, Transform trans, bool interactable, string[] interactions)
    {
        PlacedAsset asset = new PlacedAsset
        {
            assetType = type,
            assetName = name,
            position = pos,
            rotation = rot,
            transform = trans,
            isInteractable = interactable,
            interactionTypes = interactions ?? new string[0]
        };

        placedAssets.Add(asset);
    }

    void IntegrateWithGameSystems()
    {
        // Update GameManager with table positions
        GameManager gameManager = FindObjectOfType<GameManager>();
        if (gameManager != null)
        {
            // Pass table transforms to GameManager for customer seating
            List<Transform> tables = new List<Transform>();
            foreach (var asset in placedAssets)
            {
                if (asset.assetType == "furniture" && asset.assetName.Contains("table"))
                {
                    tables.Add(asset.transform);
                }
            }

            // If GameManager has a method to register tables
            if (gameManager.GetComponent<TableManager>() == null)
            {
                TableManager tableManager = gameManager.gameObject.AddComponent<TableManager>();
                tableManager.RegisterTables(tables);
            }
        }

        // Update AudioManager with equipment positions
        AudioManager audioManager = FindObjectOfType<AudioManager>();
        if (audioManager != null)
        {
            foreach (var asset in placedAssets)
            {
                if (asset.assetType == "equipment")
                {
                    // Register equipment for spatial audio
                    audioManager.RegisterEquipmentPosition(asset.assetName, asset.position);
                }
            }
        }

        Debug.Log($"Integrated {placedAssets.Count} assets with game systems");
    }

    // Public methods for manual placement
    [ContextMenu("Place Coffee Shop")]
    public void PlaceCoffeeShopManual()
    {
        StartCoroutine(AutoPlaceCoffeeShop());
    }

    [ContextMenu("Clear All Assets")]
    public void ClearAllAssets()
    {
        ClearExistingAssets();
    }

    public List<Transform> GetTableTransforms()
    {
        return createdTables;
    }

    public List<PlacedAsset> GetPlacedAssets()
    {
        return placedAssets;
    }
}

// Helper component for customer spawning
public class CustomerSpawn : MonoBehaviour
{
    public float spawnRadius = 1f;
    public bool isActive = true;

    public Vector3 GetSpawnPosition()
    {
        Vector3 randomOffset = Random.insideUnitCircle * spawnRadius;
        return transform.position + new Vector3(randomOffset.x, 0f, randomOffset.y);
    }
}

// Helper component for table management
public class TableManager : MonoBehaviour
{
    private List<Transform> registeredTables = new List<Transform>();

    public void RegisterTables(List<Transform> tables)
    {
        registeredTables = tables;
        Debug.Log($"Registered {tables.Count} tables with TableManager");
    }

    public Transform GetRandomAvailableTable()
    {
        if (registeredTables.Count > 0)
        {
            return registeredTables[Random.Range(0, registeredTables.Count)];
        }
        return null;
    }

    public List<Transform> GetAllTables()
    {
        return registeredTables;
    }
}
