// CoffeeShopBuilder.cs
using UnityEngine;

public class CoffeeShopBuilder : MonoBehaviour
{
    [Header("Shop Components")]
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject counterPrefab;
    public GameObject[] chairPrefabs;
    public GameObject[] tablePrefabs;

    void Start()
    {
        BuildCoffeeShop();
    }

    void BuildCoffeeShop()
    {
        // Create floor
        Instantiate(floorPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        // Create walls
        CreateWalls();

        // Place furniture
        PlaceFurniture();

        // Setup lighting
        SetupLighting();
    }

    void CreateWalls()
    {
        // North wall
        for(int i = 0; i < 20; i++)
        {
            Instantiate(wallPrefab, new Vector3(i * 2, 3, 20), Quaternion.identity);
        }
        // Continue for other walls...
    }

    void PlaceFurniture()
    {
        // Place tables and chairs
        for(int i = 0; i < 10; i++)
        {
            Vector3 tablePos = new Vector3(5 + i * 3, 0, 8);
            Instantiate(tablePrefabs[Random.Range(0, tablePrefabs.Length)], tablePos, Quaternion.identity);

            // Place chairs around table
            PlaceChairsAroundTable(tablePos);
        }
    }

    void SetupLighting()
    {
        // Warm coffee shop lighting
        RenderSettings.ambientLight = new Color(0.4f, 0.35f, 0.25f);

        // Add ceiling lights
        GameObject lightPrefab = Resources.Load<GameObject>("CeilingLight");
        for(int i = 0; i < 6; i++)
        {
            Vector3 lightPos = new Vector3(5 + i * 6, 4, 10);
            Instantiate(lightPrefab, lightPos, Quaternion.identity);
        }
    }

    void PlaceChairsAroundTable(Vector3 tablePosition)
    {
        if (chairPrefabs.Length == 0) return;

        // Place 4 chairs around the table
        Vector3[] chairOffsets = {
            new Vector3(1.5f, 0, 0),      // Right
            new Vector3(-1.5f, 0, 0),     // Left
            new Vector3(0, 0, 1.5f),      // Front
            new Vector3(0, 0, -1.5f)      // Back
        };

        for (int i = 0; i < chairOffsets.Length; i++)
        {
            Vector3 chairPos = tablePosition + chairOffsets[i];
            Quaternion chairRotation = Quaternion.LookRotation(-chairOffsets[i]);

            GameObject chairPrefab = chairPrefabs[Random.Range(0, chairPrefabs.Length)];
            if (chairPrefab != null)
            {
                Instantiate(chairPrefab, chairPos, chairRotation);
            }
        }
    }
}
