using UnityEngine;
using UnityEngine.EventSystems;


public class BuildingItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private BuildingCost buildingCost;
    private BuildingMenuController buildingMenuController;
    private BuildingManager buildingManager;

    private void Start()
    {
        buildingCost = GetComponent<BuildingCost>();
        buildingMenuController = FindObjectOfType<BuildingMenuController>();
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Vector3 tooltipPosition = Input.mousePosition;
        string descriptionText = $"Description: {buildingCost.description}";
        string costText = $"Wood: {buildingCost.woodCost}\nStone: {buildingCost.stoneCost}\nGold: {buildingCost.goldCost}\nLimit: {buildingManager.GetBuildingCount(buildingCost.name)}/{buildingCost.buildingLimit}";
        buildingMenuController.ShowTooltip(descriptionText, costText, tooltipPosition);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buildingMenuController.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (buildingManager.CanBuild(buildingCost.name, buildingCost.buildingLimit))
        {
            // Logic to build the building
            buildingManager.Build(buildingCost.name);
            Debug.Log($"Built {buildingCost.name}. Total: {buildingManager.GetBuildingCount(buildingCost.name)}");
        }
        else
        {
            Debug.Log($"Cannot build more {buildingCost.name}. Limit reached.");
        }
    }
}
