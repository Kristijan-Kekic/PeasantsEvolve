using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class BuildingMenuController : MonoBehaviour
{
    public GameObject buildingUI;
    public GameObject descriptionContainer;
    private TextMeshProUGUI descText;
    private TextMeshProUGUI costText;
    private CanvasGroup canvasGroup;
    private bool isBuildingUIOpen = false;
    private bool isPauseMenuActive = false;

    private void Start()
    {
        TextMeshProUGUI[] textComponents = descriptionContainer.GetComponentsInChildren<TextMeshProUGUI>();
        if (textComponents.Length >= 2)
        {
            descText = textComponents[0];
            costText = textComponents[1];
        }

        canvasGroup = descriptionContainer.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = descriptionContainer.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (!isPauseMenuActive && Input.GetKeyDown(KeyCode.B))
        {
            HandleBuildingUIDeactivation();
        }
    }

    private void HandleBuildingUIDeactivation()
    {
        ToggleBuildingUI();
    }

    void ToggleBuildingUI()
    {
        isBuildingUIOpen = !isBuildingUIOpen;

        if (isBuildingUIOpen)
        {
            OpenBuildingUI();
        }
        else
        {
            CloseBuildingUI();
        }
    }

    void OpenBuildingUI()
    {
        buildingUI.SetActive(true);
    }

    void CloseBuildingUI()
    {
        buildingUI.SetActive(false);
        isBuildingUIOpen = false;
    }

    public void SetPauseMenuActive (bool isActive)
    {
        isPauseMenuActive = isActive;

        if (isActive && isBuildingUIOpen)
        {
            CloseBuildingUI();
        }
    }

    public void ShowTooltip(string cost, string description, Vector3 position)
    {
        descriptionContainer.SetActive(true);
        if (descText != null)
        {
            descText.text = description;
        }
        if (costText != null)
        {
            costText.text = cost;
        }

        position.x += 10;
        position.y -= 10;
        descriptionContainer.transform.position = position;
    }

    public void HideTooltip()
    {
        descriptionContainer.SetActive(false);
    }

    public void CloseBuildingUIOnBuildingClick()
    {
        if (isBuildingUIOpen)
        {
            CloseBuildingUI();
        }
    }
}
