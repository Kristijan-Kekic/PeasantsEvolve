using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private Dictionary<string, int> buildingCounts = new Dictionary<string, int>();
    public List<Transform> commandPosts = new List<Transform>();
    public float globalBuildingHPMultiplier = 1f;

    private HashSet<string> buildingsConstructed = new HashSet<string>();
    public event Action<string> OnBuildingCompleted;

    public bool CanBuild(string buildingType, int limit)
    {
        if (!buildingCounts.ContainsKey(buildingType))
        {
            buildingCounts[buildingType] = 0;
        }
        return buildingCounts[buildingType] < limit;
    }

    public void Build(string buildingType, Transform buildingTransform = null)
    {
        buildingType = buildingType.Replace("(Clone)", "").Trim();

        // Update building count
        if (buildingCounts.ContainsKey(buildingType))
        {
            buildingCounts[buildingType]++;
        }
        else
        {
            buildingCounts[buildingType] = 1;
        }

        if (buildingTransform != null)
        {
            BuildingProgress buildingProgress = buildingTransform.GetComponent<BuildingProgress>();
            if (buildingProgress != null)
            {
                buildingProgress.UpgradeHP(globalBuildingHPMultiplier);
            }
        }

        FindObjectOfType<StageManager>().UpdateUI();
    }

    public void NotifyBuildingCompleted(string buildingType)
    {
        if (!buildingsConstructed.Contains(buildingType) &&
            buildingType != "Bridge" && buildingType != "Quarry" && buildingType != "Farm" && buildingType != "Mine")
        {
            buildingsConstructed.Add(buildingType);
            OnBuildingCompleted?.Invoke(buildingType);
            Debug.Log($"Skill point granted for constructing {buildingType}.");
        }
        else
        {
            Debug.Log($"No skill point granted. {buildingType} has already been built or is excluded.");
        }
    }


    public int GetBuildingCount(string buildingType)
    {
        return buildingCounts.ContainsKey(buildingType) ? buildingCounts[buildingType] : 0;
    }
    public Transform GetNearestCommandPost(Vector3 position)
    {
        Transform nearestCommandPost = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Transform commandPost in commandPosts)
        {
            float distance = Vector3.Distance(position, commandPost.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestCommandPost = commandPost;
            }
        }

        return nearestCommandPost;
    }

    public void AssignCommandPostToUnitsWithoutPost(Transform newCommandPost)
    {
        foreach (GameObject unit in SelectionManager.Instance.unitList)
        {
            ResourceGatherer gatherer = unit.GetComponent<ResourceGatherer>();
            if (gatherer != null && gatherer.commandPost == null)
            {
                gatherer.AssignCommandPost(newCommandPost);
                Debug.Log($"{unit.name} assigned to the new command post.");
            }
        }
    }

    public void UpgradeBuildingHP(float percentage)
    {
        globalBuildingHPMultiplier += percentage / 100f;
        BuildingProgress[] allBuildings = FindObjectsOfType<BuildingProgress>();
        foreach (var building in allBuildings)
        {
            building.UpgradeHP(globalBuildingHPMultiplier);
        }
        Debug.Log("All buildings' HP have been upgraded.");
    }

    public void RemoveBuilding(string buildingType)
    {
        // Strip "(Clone)" from the buildingType if it exists
        buildingType = buildingType.Replace("(Clone)", "").Trim();

        // Update building count
        if (buildingCounts.ContainsKey(buildingType) && buildingCounts[buildingType] > 0)
        {
            buildingCounts[buildingType]--;
            Debug.Log($"{buildingType} destroyed. Remaining count: {buildingCounts[buildingType]}");
        }

        FindObjectOfType<StageManager>().UpdateUI();
    }
}
