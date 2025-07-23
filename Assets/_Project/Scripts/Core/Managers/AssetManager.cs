// AssetManager.cs
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class AssetManager : MonoBehaviour
{
    [Header("Asset Collections")]
    public CustomerAssets customerAssets;
    public EnvironmentAssets environmentAssets;
    public FurnitureAssets furnitureAssets;
    public PropAssets propAssets;
    public UIAssets uiAssets;

    [Header("Loading Settings")]
    public bool preloadAllAssets = true;
    public bool useAssetBundles = false;

    private Dictionary<string, Object> loadedAssets = new Dictionary<string, Object>();
    private Dictionary<string, AssetBundle> assetBundles = new Dictionary<string, AssetBundle>();

    public static AssetManager Instance { get; private set; }

    [System.Serializable]
    public class CustomerAssets
    {
        [Header("Customer Prefabs")]
        public GameObject[] businessCustomers;
        public GameObject[] studentCustomers;
        public GameObject[] elderlyCustomers;
        public GameObject[] familyCustomers;
        public GameObject[] freelancerCustomers;
        public GameObject[] touristCustomers;

        [Header("Customer Variations")]
        public Material[] skinMaterials;
        public Material[] clothingMaterials;
        public GameObject[] accessories;
    }

    [System.Serializable]
    public class EnvironmentAssets
    {
        [Header("Structural")]
        public GameObject[] wallPieces;
        public GameObject[] floorTiles;
        public GameObject[] ceilingPieces;
        public GameObject[] doorFrames;
        public GameObject[] windowFrames;

        [Header("Lighting")]
        public GameObject[] lightFixtures;
        public GameObject[] lamps;
        public Material[] lightMaterials;
    }

    [System.Serializable]
    public class FurnitureAssets
    {
        [Header("Tables")]
        public GameObject roundTable2Person;
        public GameObject squareTable4Person;
        public GameObject rectangularTable6Person;
        public GameObject counterTable;
        public GameObject coffeeTable;

        [Header("Seating")]
        public GameObject woodenChair;
        public GameObject loungeChair;
        public GameObject barStool;
        public GameObject benchSeating;

        [Header("Equipment")]
        public GameObject espressoMachine;
        public GameObject coffeeGrinder;
        public GameObject cashRegister;
        public GameObject refrigerator;
        public GameObject displayCase;
    }

    [System.Serializable]
    public class PropAssets
    {
        [Header("Beverages")]
        public GameObject[] coffeeCups;
        public GameObject[] takeawayCups;
        public GameObject[] teaCups;
        public GameObject[] coldDrinkGlasses;

        [Header("Food Items")]
        public GameObject[] pastries;
        public GameObject[] sandwiches;
        public GameObject[] salads;
        public GameObject[] snacks;

        [Header("Technology")]
        public GameObject[] laptops;
        public GameObject[] smartphones;
        public GameObject[] tablets;
        public GameObject[] chargingCables;

        [Header("Decorative")]
        public GameObject[] plants;
        public GameObject[] artPieces;
        public GameObject[] coffeeBags;
        public GameObject[] menuBoards;
    }

    [System.Serializable]
    public class UIAssets
    {
        [Header("Icons")]
        public Sprite[] menuItemIcons;
        public Sprite[] customerStateIcons;
        public Sprite[] orderStatusIcons;

        [Header("UI Elements")]
        public GameObject orderTicketPrefab;
        public GameObject notificationPrefab;
        public GameObject customerOrderPanelPrefab;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (preloadAllAssets)
            {
                StartCoroutine(PreloadAssets());
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator PreloadAssets()
    {
        Debug.Log("Preloading assets...");

        // Preload customer assets
        yield return StartCoroutine(PreloadCustomerAssets());

        // Preload environment assets
        yield return StartCoroutine(PreloadEnvironmentAssets());

        // Preload furniture assets
        yield return StartCoroutine(PreloadFurnitureAssets());

        // Preload prop assets
        yield return StartCoroutine(PreloadPropAssets());

        Debug.Log("Asset preloading complete!");
    }

    IEnumerator PreloadCustomerAssets()
    {
        if (customerAssets.businessCustomers != null)
        {
            foreach (var asset in customerAssets.businessCustomers)
            {
                if (asset != null)
                    RegisterAsset($"customer_business_{asset.name}", asset);
                yield return null;
            }
        }

        if (customerAssets.studentCustomers != null)
        {
            foreach (var asset in customerAssets.studentCustomers)
            {
                if (asset != null)
                    RegisterAsset($"customer_student_{asset.name}", asset);
                yield return null;
            }
        }

        // Continue for other customer types...
    }

    IEnumerator PreloadEnvironmentAssets()
    {
        if (environmentAssets.wallPieces != null)
        {
            foreach (var asset in environmentAssets.wallPieces)
            {
                if (asset != null)
                    RegisterAsset($"environment_wall_{asset.name}", asset);
                yield return null;
            }
        }

        if (environmentAssets.floorTiles != null)
        {
            foreach (var asset in environmentAssets.floorTiles)
            {
                if (asset != null)
                    RegisterAsset($"environment_floor_{asset.name}", asset);
                yield return null;
            }
        }
    }

    IEnumerator PreloadFurnitureAssets()
    {
        if (furnitureAssets.roundTable2Person != null)
            RegisterAsset("furniture_table_round_2p", furnitureAssets.roundTable2Person);

        if (furnitureAssets.woodenChair != null)
            RegisterAsset("furniture_chair_wooden", furnitureAssets.woodenChair);

        if (furnitureAssets.espressoMachine != null)
            RegisterAsset("equipment_espresso_machine", furnitureAssets.espressoMachine);

        yield return null;
    }

    IEnumerator PreloadPropAssets()
    {
        if (propAssets.coffeeCups != null)
        {
            for (int i = 0; i < propAssets.coffeeCups.Length; i++)
            {
                if (propAssets.coffeeCups[i] != null)
                    RegisterAsset($"prop_coffee_cup_{i}", propAssets.coffeeCups[i]);
                yield return null;
            }
        }
    }

    void RegisterAsset(string key, Object asset)
    {
        if (!loadedAssets.ContainsKey(key))
        {
            loadedAssets[key] = asset;
        }
    }

    public T GetAsset<T>(string assetKey) where T : Object
    {
        if (loadedAssets.ContainsKey(assetKey))
        {
            return loadedAssets[assetKey] as T;
        }

        Debug.LogWarning($"Asset not found: {assetKey}");
        return null;
    }

    public GameObject GetRandomCustomer(string customerType = "")
    {
        List<GameObject> availableCustomers = new List<GameObject>();

        if (string.IsNullOrEmpty(customerType) || customerType == "business")
        {
            if (customerAssets.businessCustomers != null)
                availableCustomers.AddRange(customerAssets.businessCustomers);
        }

        if (string.IsNullOrEmpty(customerType) || customerType == "student")
        {
            if (customerAssets.studentCustomers != null)
                availableCustomers.AddRange(customerAssets.studentCustomers);
        }

        if (string.IsNullOrEmpty(customerType) || customerType == "elderly")
        {
            if (customerAssets.elderlyCustomers != null)
                availableCustomers.AddRange(customerAssets.elderlyCustomers);
        }

        if (availableCustomers.Count > 0)
        {
            return availableCustomers[Random.Range(0, availableCustomers.Count)];
        }

        return null;
    }

    public GameObject GetFurniture(string furnitureType)
    {
        switch (furnitureType.ToLower())
        {
            case "table_round_2p":
                return furnitureAssets.roundTable2Person;
            case "table_square_4p":
                return furnitureAssets.squareTable4Person;
            case "chair_wooden":
                return furnitureAssets.woodenChair;
            case "chair_lounge":
                return furnitureAssets.loungeChair;
            case "espresso_machine":
                return furnitureAssets.espressoMachine;
            default:
                Debug.LogWarning($"Furniture type not found: {furnitureType}");
                return null;
        }
    }

    public GameObject GetRandomProp(string propCategory)
    {
        switch (propCategory.ToLower())
        {
            case "coffee_cup":
                return propAssets.coffeeCups?.Length > 0 ?
                    propAssets.coffeeCups[Random.Range(0, propAssets.coffeeCups.Length)] : null;
            case "pastry":
                return propAssets.pastries?.Length > 0 ?
                    propAssets.pastries[Random.Range(0, propAssets.pastries.Length)] : null;
            case "laptop":
                return propAssets.laptops?.Length > 0 ?
                    propAssets.laptops[Random.Range(0, propAssets.laptops.Length)] : null;
            default:
                return null;
        }
    }

    public void LoadAssetBundle(string bundleName, string bundlePath)
    {
        if (!assetBundles.ContainsKey(bundleName))
        {
            AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
            if (bundle != null)
            {
                assetBundles[bundleName] = bundle;
                Debug.Log($"Loaded asset bundle: {bundleName}");
            }
            else
            {
                Debug.LogError($"Failed to load asset bundle: {bundleName}");
            }
        }
    }

    public T LoadFromBundle<T>(string bundleName, string assetName) where T : Object
    {
        if (assetBundles.ContainsKey(bundleName))
        {
            return assetBundles[bundleName].LoadAsset<T>(assetName);
        }

        Debug.LogWarning($"Asset bundle not loaded: {bundleName}");
        return null;
    }

    void OnDestroy()
    {
        // Unload asset bundles
        foreach (var bundle in assetBundles.Values)
        {
            if (bundle != null)
            {
                bundle.Unload(false);
            }
        }
    }
}
