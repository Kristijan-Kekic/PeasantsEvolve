using UnityEngine;
using UnityEngine.UI;

public class BuildingSelectionButton : MonoBehaviour
{
    public GameObject buildingPrefab;
    public BuildingPlacement buildingPlacement;
    public BridgePlacement bridgePlacement;

    public Vector3 rotationEulerAngles = Vector3.zero;
    public float raiseAmount = 0f;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SelectBuilding);
    }

    void SelectBuilding()
    {
        Quaternion rotation = Quaternion.Euler(rotationEulerAngles);
        buildingPlacement.StartPlacingBuilding(buildingPrefab, rotation, raiseAmount);
    }
}
