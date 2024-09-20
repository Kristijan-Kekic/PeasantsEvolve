using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class EnemyBuildingManager : MonoBehaviour
{
    public GameObject[] buildingsToConstruct;
    public Transform spawnPoint;  
    public float buildRadius = 200f;  
    public float maxSlopeAngle = 5f;  
    public LayerMask terrainLayer;   
    public LayerMask obstructionLayer;  

    private EnemyResourceManager resourceManager;
    private Dictionary<string, int> buildingsConstructed = new Dictionary<string, int>(); 

    public List<Transform> commandPosts = new List<Transform>();

    private void Start()
    {
        resourceManager = EnemyResourceManager.Instance;
        StartCoroutine(BuildNextStructure());
    }

    IEnumerator BuildNextStructure()
    {
        while (true)
        {
            foreach (GameObject buildingPrefab in buildingsToConstruct)
            {
                BuildingCost cost = buildingPrefab.GetComponent<BuildingCost>();

                if (!CheckBuildLimit(cost))
                {
                    continue;
                }

                if (!CheckPrerequisites(buildingPrefab))
                {
                    continue;
                }

                while (!resourceManager.HasEnoughResources(cost.woodCost, cost.stoneCost, cost.goldCost, cost.foodCost, cost.moneyCost, cost.coalCost, cost.metalCost))
                {
                    yield return new WaitForSeconds(1f);
                }

                resourceManager.DeductResources(cost.woodCost, cost.stoneCost, cost.goldCost, cost.foodCost, cost.moneyCost, cost.coalCost, cost.metalCost);
                Vector3 buildPosition = FindBuildPosition(buildingPrefab);

                if (buildPosition == Vector3.zero)
                {
                    yield return new WaitForSeconds(2f);
                    continue;
                }

                RaycastHit hit;
                if (Physics.Raycast(buildPosition + Vector3.up * 10, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
                {
                    buildPosition = hit.point;

                    Quaternion buildingRotation = GetCustomBuildingRotation(buildingPrefab);

                    GameObject builtBuilding = Instantiate(buildingPrefab, buildPosition, buildingRotation);
                    BuildingProgress buildingProgress = builtBuilding.GetComponent<BuildingProgress>();
                    if (buildingProgress != null)
                    {
                        buildingProgress.CompleteBuilding();
                    }

                    SelectionManager.Instance.enemyBuildings.Add(builtBuilding);
                    IncrementBuildingCount(cost.name);

                    if (builtBuilding.name.Contains("CommandPost"))
                    {
                        EnemyResourceGatherer resourceGatherer = FindObjectOfType<EnemyResourceGatherer>();
                        if (resourceGatherer != null)
                        {
                            resourceGatherer.SetCommandPost(builtBuilding.transform);
                        }
                    }
                }
            }
            yield return new WaitForSeconds(2f);
        }
    }






    Vector3 FindBuildPosition(GameObject buildingPrefab)
    {
        int maxAttempts = 999;
        float checkRadius = 5f; 

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPosition = spawnPoint.position + Random.insideUnitSphere * buildRadius;
            randomPosition.y = spawnPoint.position.y + 10f;

            RaycastHit hit;
            if (Physics.Raycast(randomPosition, Vector3.down, out hit, Mathf.Infinity, terrainLayer))
            {
                Vector3 buildPosition = hit.point;

                if (buildPosition.y <= 52.5f)
                {
                    continue;
                }

                float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
                if (slopeAngle <= maxSlopeAngle)
                {
                    Collider[] obstructions = Physics.OverlapSphere(buildPosition, checkRadius, obstructionLayer);
                    if (obstructions.Length == 0)
                    {
                        return buildPosition;
                    }
                    else
                    {

                    }
                }
            }
        }

        return Vector3.zero;
    }
    Quaternion GetCustomBuildingRotation(GameObject buildingPrefab)
    {
        if (buildingPrefab.name.Contains("Quarry"))
        {
            return Quaternion.Euler(-90, -90, 0);
        }
        else if (buildingPrefab.name.Contains("Mine"))
        {
            return Quaternion.Euler(0, 180, 90);
        }
        else if (buildingPrefab.name.Contains("Farm"))
        {
            return Quaternion.Euler(0, 180, 0);
        }
        else if (buildingPrefab.name.Contains("Barracks"))
        {
            return Quaternion.Euler(-90, -90, 0);
        }
        else if (buildingPrefab.name.Contains("CommandPost"))
        {
            return Quaternion.Euler(0, 180, 0);
        }
        else if (buildingPrefab.name.Contains("School"))
        {
            return Quaternion.Euler(-90, -90, 0);
        }

        else if (buildingPrefab.name.Contains("Market"))
        {
            return Quaternion.Euler(-90, 0, 0);
        }

        else if (buildingPrefab.name.Contains("Cavarly"))
        {
            return Quaternion.Euler(-90, 0, 0);
        }

        else if (buildingPrefab.name.Contains("Artillery"))
        {
            return Quaternion.Euler(-90, 0, 0);
        }

        return Quaternion.identity;
    }

    private bool CheckPrerequisites(GameObject buildingPrefab)
    {
        EnemyPrerequisites prerequisites = buildingPrefab.GetComponent<EnemyPrerequisites>();

        if (prerequisites == null || (prerequisites.requiredBuildings.Length == 0))
        {
            return true;
        }

        foreach (string requiredBuilding in prerequisites.requiredBuildings)
        {
            if (!IsBuildingConstructed(requiredBuilding))
            {
                return false;
            }
        }

        return true;
    }

    private bool CheckBuildLimit(BuildingCost cost)
    {
        string buildingName = cost.name;

        if (!buildingsConstructed.ContainsKey(buildingName))
        {
            buildingsConstructed[buildingName] = 0;
        }

        return buildingsConstructed[buildingName] < cost.buildingLimit;
    }
    private void IncrementBuildingCount(string buildingName)
    {
        if (!buildingsConstructed.ContainsKey(buildingName))
        {
            buildingsConstructed[buildingName] = 0;
        }

        buildingsConstructed[buildingName]++;
    }
    private bool IsBuildingConstructed(string buildingName)
    {
        BuildingInstance[] constructedBuildings = FindObjectsOfType<BuildingInstance>();

        foreach (BuildingInstance building in constructedBuildings)
        {
            if (building.gameObject.layer == LayerMask.NameToLayer("EnemyBuilding") && building.gameObject.name.Contains(buildingName))
            {
                return true;
            }
        }

        return false;
    }
}
