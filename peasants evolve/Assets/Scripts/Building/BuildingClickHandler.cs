using UnityEngine;

public class BuildingClickHandler : MonoBehaviour
{
    private UnitProductionCanvasController canvasController;
    private MarketUI marketUI;
    private SchoolUI schoolUI;

    public static BuildingClickHandler currentlySelectedBuilding;

    private void Start()
    {
        canvasController = FindObjectOfType<UnitProductionCanvasController>();
        marketUI = FindObjectOfType<MarketUI>();
        schoolUI = FindObjectOfType<SchoolUI>();
    }

    void OnMouseDown()
    {
        var buildingMenuController = FindObjectOfType<BuildingMenuController>();

        // Handle selection indicator
        if (currentlySelectedBuilding != null && currentlySelectedBuilding != this)
        {
            //currentlySelectedBuilding.DeselectBuilding();
        }

        if (buildingMenuController != null)
        {
            buildingMenuController.CloseBuildingUIOnBuildingClick();
        }

        // Handle Market UI
        if (gameObject.CompareTag("Market"))
        {
            // Close School UI and Unit Production Panel if they are open
            if (schoolUI != null)
            {
                schoolUI.HideSchoolPanel();
            }
            if (canvasController != null)
            {
                canvasController.HideUnitProductionPanel();
            }

            // Show the Market UI
            if (marketUI != null)
            {
                marketUI.ShowMarketPanel(gameObject);
            }
        }

        // Handle School UI
        else if (gameObject.CompareTag("School"))
        {
            // Close Market UI and Unit Production Panel if they are open
            if (marketUI != null)
            {
                marketUI.HideMarketPanel();
            }
            if (canvasController != null)
            {
                canvasController.HideUnitProductionPanel();
            }

            // Show the School UI
            if (schoolUI != null)
            {
                schoolUI.ShowSchoolPanel(gameObject);
            }
        }

        // Handle Unit Production Panel for other buildings
        else
        {
            // Close Market UI and School UI if they are open
            if (marketUI != null)
            {
                marketUI.HideMarketPanel();
            }
            if (schoolUI != null)
            {
                schoolUI.HideSchoolPanel();
            }

            // Show Unit Production Panel for other buildings
            if (canvasController != null)
            {
                BuildingProgress buildingProgress = gameObject.GetComponent<BuildingProgress>();
                if (buildingProgress != null)
                {
                    canvasController.ShowUnitProductionPanel(gameObject);
                }
            }
        }

        currentlySelectedBuilding = this;
    }
}
