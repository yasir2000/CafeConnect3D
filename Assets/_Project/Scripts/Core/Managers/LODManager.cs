// LODManager.cs
using UnityEngine;

public class LODManager : MonoBehaviour
{
    [Header("LOD Settings")]
    public float highDetailDistance = 10f;
    public float mediumDetailDistance = 25f;
    public float lowDetailDistance = 50f;

    [Header("LOD Objects")]
    public GameObject[] highDetailObjects;
    public GameObject[] mediumDetailObjects;
    public GameObject[] lowDetailObjects;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = Camera.main;
        InvokeRepeating(nameof(UpdateLOD), 0.5f, 0.5f); // Update LOD every 0.5 seconds
    }

    void UpdateLOD()
    {
        if(playerCamera == null) return;

        Vector3 cameraPos = playerCamera.transform.position;

        // Update high detail objects
        foreach(GameObject obj in highDetailObjects)
        {
            if(obj != null)
            {
                float distance = Vector3.Distance(cameraPos, obj.transform.position);
                obj.SetActive(distance <= highDetailDistance);
            }
        }

        // Update medium detail objects
        foreach(GameObject obj in mediumDetailObjects)
        {
            if(obj != null)
            {
                float distance = Vector3.Distance(cameraPos, obj.transform.position);
                bool shouldBeActive = distance > highDetailDistance && distance <= mediumDetailDistance;
                obj.SetActive(shouldBeActive);
            }
        }

        // Update low detail objects
        foreach(GameObject obj in lowDetailObjects)
        {
            if(obj != null)
            {
                float distance = Vector3.Distance(cameraPos, obj.transform.position);
                bool shouldBeActive = distance > mediumDetailDistance && distance <= lowDetailDistance;
                obj.SetActive(shouldBeActive);
            }
        }
    }
}
