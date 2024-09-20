using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    public int currentStage = 1;
    public TextMeshProUGUI stageText;
    public Button advanceStageButton;

    private BuildingManager buildingManager;

    private Dictionary<int, string[]> stageBuildingRequirements = new Dictionary<int, string[]>
    {
        { 2, new string[] { "CommandPost", "Barracks", "Mine", "Quarry", "Farm" } },
        { 3, new string[] { "Market", "Cavalry", "School" } }
    };

    void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();

        if (buildingManager == null)
        {
            Debug.LogError("BuildingManager not found in the scene.");
            return;
        }

        if (advanceStageButton != null)
        {
            advanceStageButton.gameObject.SetActive(true);
            advanceStageButton.onClick.AddListener(AdvanceStage);
        }
        UpdateUI();
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
                if (buildingManager.GetBuildingCount(buildingName) == 0)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void AdvanceStage()
    {
        if (CanAdvanceToNextStage())
        {
            currentStage++;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (stageText != null)
        stageText.text = "Stage " + currentStage;

        if (advanceStageButton != null)
        {
            advanceStageButton.gameObject.SetActive(CanAdvanceToNextStage());
        }
    }
}
