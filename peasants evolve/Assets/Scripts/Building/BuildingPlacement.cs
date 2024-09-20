using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class BuildingPlacement : MonoBehaviour
{
    [SerializeField] GameObject buildingPrefabToPlace;
    private GameObject currentBuildingPreview;
    private Quaternion buildingRotation = Quaternion.identity;
    public Vector3 gridSize = new Vector3(4f, 0f, 4f);
    public LayerMask terrainLayer;
    public LayerMask treeLayer;
    private float raiseAmount = 0f;

    private ResourceManager resourceManager;
    private BuildingManager buildingManager;
    private StageManager stageManager;
    public bool IsPlacingBuilding { get; private set; } = false;
    [SerializeField] private TMP_Text statusText;

    private bool isBridge = false;
    private float bridgeLength = 2f;
    public float minBridgeLength = 1f;
    public float maxBridgeLength = 4f;
    public float bridgeLengthStep = 0.5f;

    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        buildingManager = FindObjectOfType<BuildingManager>();
        stageManager = FindObjectOfType<StageManager>();
    }

    public void StartPlacingBuilding(GameObject buildingPrefab, Quaternion rotation, float raiseAmount)
    {
        buildingPrefabToPlace = buildingPrefab;
        buildingRotation = rotation;
        this.raiseAmount = raiseAmount;

        if (buildingPrefabToPlace.name.Contains("Bridge"))
        {
            isBridge = true;
            bridgeLength = 2f;

            buildingRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            isBridge = false;
        }

        if (CheckPrerequisites(buildingPrefab))
        {
            IsPlacingBuilding = true;

            if (currentBuildingPreview != null)
            {
                Destroy(currentBuildingPreview);
            }

            currentBuildingPreview = Instantiate(buildingPrefabToPlace);
            currentBuildingPreview.GetComponent<Collider>().enabled = false;
            SetBuildingPreviewOpacity(0.5f);
        }
        else
        {
            DisplayMessage("Prerequisites not met for this building.");
        }
    }

    private void Update()
    {
        if (IsPlacingBuilding)
        {
            FollowCursor();

            if (Input.GetKey(KeyCode.R))
            {
                RotateBuilding(Time.deltaTime * 100f);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateBuilding(1f);
            }

            if (Input.GetMouseButtonDown(0) && currentBuildingPreview != null && currentBuildingPreview.activeSelf)
            {
                PlaceBuilding();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBuildingPlacement();
            }

            if (isBridge)
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    AdjustBridgeLength(-bridgeLengthStep);
                }
                else if (Input.GetKeyDown(KeyCode.E))
                {
                    AdjustBridgeLength(bridgeLengthStep);
                }
            }
        }

       
    }

    private void FollowCursor()
    {
        if (!IsPlacingBuilding || currentBuildingPreview == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainLayer))
        {
            Vector3 position = SnapToGrid(hit.point);
            position.y += raiseAmount;

            if (isBridge)
            {
                if (position.y < 53f)
                {
                    currentBuildingPreview.SetActive(false);
                    return;
                }

                Vector3 bridgeStart = hit.point;
                currentBuildingPreview.transform.position = bridgeStart;

                currentBuildingPreview.SetActive(true);
            }
            else
            {
                if (IsValidPlacement(position))
                {
                    currentBuildingPreview.transform.position = position;
                    currentBuildingPreview.transform.rotation = buildingRotation;
                    currentBuildingPreview.SetActive(true);
                }
                else
                {
                    currentBuildingPreview.SetActive(false);
                }
            }
        }
    }


    private void RotateBuilding(float rotationIncrement)
{
    if (isBridge)
    {
        buildingRotation *= Quaternion.Euler(0, rotationIncrement, 0);
    }
    else if (buildingPrefabToPlace.name.Contains("CommandPost") || buildingPrefabToPlace.name.Contains("Farm"))
    {
        buildingRotation *= Quaternion.Euler(0, rotationIncrement, 0);
    }
    else if (buildingPrefabToPlace.name.Contains("Mine"))
    {
        buildingRotation *= Quaternion.Euler(rotationIncrement, 0, 0);
    }
    else
    {
        buildingRotation *= Quaternion.Euler(0, 0, rotationIncrement);
    }

    if (currentBuildingPreview != null)
    {
        currentBuildingPreview.transform.rotation = buildingRotation;
    }
}


    private void AdjustBridgeLength(float adjustment)
    {
        bridgeLength = Mathf.Clamp(bridgeLength + adjustment, minBridgeLength, maxBridgeLength);

        if (currentBuildingPreview != null)
        {
            Vector3 currentScale = currentBuildingPreview.transform.localScale;

            currentBuildingPreview.transform.localScale = new Vector3(currentScale.x, currentScale.y, bridgeLength);
        }
    }


    //private void AlignBridgeToGround()
    //{
    //    Vector3 bridgeStart = currentBuildingPreview.transform.position - (currentBuildingPreview.transform.forward * (bridgeLength / 2));
    //    Vector3 bridgeEnd = currentBuildingPreview.transform.position + (currentBuildingPreview.transform.forward * (bridgeLength / 2));

    //    bool isValidStart = Physics.Raycast(bridgeStart + Vector3.up * 10f, Vector3.down, out RaycastHit startHit, Mathf.Infinity, terrainLayer);
    //    bool isValidEnd = Physics.Raycast(bridgeEnd + Vector3.up * 10f, Vector3.down, out RaycastHit endHit, Mathf.Infinity, terrainLayer);

    //    if (isValidStart && isValidEnd)
    //    {
    //        currentBuildingPreview.transform.position = (startHit.point + endHit.point) / 2;
    //    }
    //}

    private Vector3 SnapToGrid(Vector3 position)
    {
        position.x = Mathf.Round(position.x / gridSize.x) * gridSize.x;
        position.z = Mathf.Round(position.z / gridSize.z) * gridSize.z;
        return position;
    }

    private bool IsValidPlacement(Vector3 position)
    {
        if (!IsTerrainEven(position) || IsBuildingAlreadyPresent(position))
            return false;

        if (IsTreePresent(position))
            return false;

        return CheckResourceCost(buildingPrefabToPlace);
    }

    private void PlaceBuilding()
    {
        Vector3 position = currentBuildingPreview.transform.position;
        Vector3 scale = currentBuildingPreview.transform.localScale;

        BuildingCost buildingCost = buildingPrefabToPlace.GetComponent<BuildingCost>();
        if (buildingCost != null && buildingManager.CanBuild(buildingCost.name, buildingCost.buildingLimit))
        {
            DeductResources(buildingPrefabToPlace);

            GameObject placedBuilding = Instantiate(buildingPrefabToPlace, position, buildingRotation);
            placedBuilding.transform.localScale = scale;

            if (isBridge)
            {
                NavMeshSurface navMeshSurface = placedBuilding.AddComponent<NavMeshSurface>();
                navMeshSurface.BuildNavMesh();
            }

                BuildingProgress buildingProgress = placedBuilding.GetComponent<BuildingProgress>();
            if (buildingProgress != null)
            {
                buildingProgress.StartBuilding();
                buildingProgress.AutoAssignBuilders();
            }

            NavMeshObstacle obstacle = placedBuilding.AddComponent<NavMeshObstacle>();

            Vector3 originalSize = obstacle.size;

            if(buildingPrefabToPlace.name.Contains("Farm"))
                obstacle.size = originalSize * 5;
            obstacle.carving = true;

            buildingManager.Build(buildingCost.name, placedBuilding.transform);
            if(!isBridge)
            SelectionManager.Instance.playerBuildings.Add(placedBuilding);

            Debug.Log($"Built {buildingCost.name}. Total: {buildingManager.GetBuildingCount(buildingCost.name)}");
        }
        else
        {
            Debug.Log($"Cannot build more {buildingCost.name}. Limit reached.");
        }

        Destroy(currentBuildingPreview);
        IsPlacingBuilding = false;
        buildingPrefabToPlace = null;
    }

    private void CancelBuildingPlacement()
    {
        if (currentBuildingPreview != null)
        {
            Destroy(currentBuildingPreview);
        }
        IsPlacingBuilding = false;
    }

    private void SetBuildingPreviewOpacity(float opacity)
    {
        Renderer[] renderers = currentBuildingPreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                Color color = material.color;
                color.a = opacity;
                material.color = color;

                if (material.shader.name != "Transparent/Diffuse")
                {
                    material.shader = Shader.Find("Transparent/Diffuse");
                }
            }
        }
    }

    private bool CheckResourceCost(GameObject buildingPrefab)
    {
        BuildingCost buildingCost = buildingPrefab.GetComponent<BuildingCost>();
        if (buildingCost != null)
        {
           return resourceManager.HasEnoughResources(
           buildingCost.woodCost,
           buildingCost.stoneCost,
           buildingCost.goldCost,
           buildingCost.foodCost,
           buildingCost.moneyCost,
           buildingCost.coalCost,
           buildingCost.metalCost
       );
        }
        return true;
    }

    private void DeductResources(GameObject buildingPrefab)
    {
        BuildingCost buildingCost = buildingPrefab.GetComponent<BuildingCost>();
        if (buildingCost != null)
        {
            resourceManager.DeductResources(
           buildingCost.woodCost,
           buildingCost.stoneCost,
           buildingCost.goldCost,
           buildingCost.foodCost,
           buildingCost.moneyCost,
           buildingCost.coalCost,
           buildingCost.metalCost
       );
        }
    }

    private bool IsBuildingAlreadyPresent(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, gridSize.x / 2);

        foreach (Collider collider in colliders)
        {
            BuildingInstance existingBuilding = collider.GetComponent<BuildingInstance>();
            if (existingBuilding != null)
            {
                return true;
            }
        }

        return false;
    }

    private bool IsTreePresent(Vector3 position)
    {
        float checkRadius = gridSize.x;

        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);

        if (colliders.Length > 0)
        {
            foreach (var collider in colliders)
            {
                TreeResource treeResource = collider.GetComponent<TreeResource>();
                if (treeResource != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsTerrainEven(Vector3 position)
    {
        float halfGridX = gridSize.x / 2;
        float halfGridZ = gridSize.z / 2;
        float maxHeightDifference = 0.1f;

        float centerHeight = Terrain.activeTerrain.SampleHeight(position);

        
        for (float xOffset = -halfGridX; xOffset <= halfGridX; xOffset += gridSize.x)
        {
            for (float zOffset = -halfGridZ; zOffset <= halfGridZ; zOffset += gridSize.z)
            {
                Vector3 samplePosition = position + new Vector3(xOffset, 0f, zOffset);
                float sampleHeight = Terrain.activeTerrain.SampleHeight(samplePosition);

                
                float heightDifference = Mathf.Abs(sampleHeight - centerHeight);
                if (heightDifference > maxHeightDifference)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private bool CheckPrerequisites(GameObject buildingPrefab)
    {
        BuildingPrerequisites prerequisites = buildingPrefab.GetComponent<BuildingPrerequisites>();

        if (prerequisites == null || (prerequisites.requiredBuildings.Length == 0 && prerequisites.requiredStage <= stageManager.currentStage))
        {
            return true;
        }

        if (prerequisites.requiredStage > stageManager.currentStage)
        {
            Debug.Log($"Building is locked until stage {prerequisites.requiredStage}. Current stage is {stageManager.currentStage}.");
            return false;
        }

        foreach (string requiredBuilding in prerequisites.requiredBuildings)
        {
            if (!IsBuildingConstructed(requiredBuilding))
            {
                Debug.Log($"Building is locked because {requiredBuilding} is not constructed yet.");
                return false;
            }
        }

        return true;
    }

    private bool IsBuildingConstructed(string buildingName)
    {
        BuildingInstance[] constructedBuildings = FindObjectsOfType<BuildingInstance>();

        foreach (BuildingInstance building in constructedBuildings)
        {
            if (building.gameObject.layer == LayerMask.NameToLayer("Building") && building.gameObject.name.Contains(buildingName))
            {
                return true;
            }
        }

        return false;
    }

    private void DisplayMessage(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
            StartCoroutine(HideTextAfterDelay(5));
        }
    }

    private IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (statusText != null)
        {
            statusText.text = "";
        }
    }
}