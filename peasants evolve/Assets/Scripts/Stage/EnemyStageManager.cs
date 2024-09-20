using System.Collections.Generic;
using UnityEngine;

public class EnemyStageManager : MonoBehaviour
{
    public int currentStage = 3;  // Enemy starts at stage 3 by default
    private BuildingManager enemyBuildingManager;

    private Dictionary<int, string[]> stageBuildingRequirements = new Dictionary<int, string[]>
    {
        { 2, new string[] { "CommandPost", "Barracks", "Mine", "Quarry", "Farm" } },
        { 3, new string[] { "Market", "Cavalry", "School" } }
    };

    void Start()
    {
        enemyBuildingManager = FindObjectOfType<BuildingManager>();

        if (enemyBuildingManager == null)
        {
            Debug.LogError("Enemy BuildingManager not found in the scene.");
            return;
        }

        // Enemy logic can automatically check for advancement
        InvokeRepeating("CheckForStageAdvance", 10f, 10f);  // Example of timed checks every 10 seconds
    }

    void CheckForStageAdvance()
    {
        if (CanAdvanceToNextStage())
        {
            currentStage++;
            Debug.Log("Enemy advanced to stage: " + currentStage);
        }
    }

    public bool CanAdvanceToNextStage()
    {
        if (currentStage >= 3)
        {
            return false;
        }

        if (stageBuildingRequirements.ContainsKey(currentStage + 1))
        {
            string[] requiredBuildings = stageBuildingRequirements[currentStage + 1];

            foreach (string buildingName in requiredBuildings)
            {
                if (enemyBuildingManager.GetBuildingCount(buildingName) == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
