using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitProductionCanvasController : MonoBehaviour
{
    public GameObject unitProductionPanel;
    public Transform unitButtonContainer;
    public GameObject unitButtonPrefab;

    public TextMeshProUGUI buildingHpText;

    private GameObject currentBuilding;
    private BuildingProduction currentBuildingProduction;

    private ResourceManager resourceManager;
    public UnitManager unitManager;
    public Popup popup;

    public GameObject selectionIndicatorPrefab;
    private GameObject currentIndicator;

    private void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        popup = FindObjectOfType<Popup>();
        unitManager = FindObjectOfType<UnitManager>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
        {
            HideUnitProductionPanel();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == currentBuilding)
                {
                    return;
                }
                else
                {
                    HideUnitProductionPanel();
                }
            }
            else
            {
                HideUnitProductionPanel();
            }
        }

        if (currentBuilding != null)
        {
            UpdateBuildingHpDisplay();
        }
    }

    public void ShowUnitProductionPanel(GameObject building)
    {
        BuildingProgress buildingProgress = building.GetComponent<BuildingProgress>();

        if (buildingProgress != null && !buildingProgress.IsCompleted())
        {
            return;
        }

        currentBuilding = building;
        currentBuildingProduction = building.GetComponent<BuildingProduction>();
        unitProductionPanel.SetActive(true);

        Collider buildingCollider = currentBuilding.GetComponent<Collider>();

        if (currentIndicator == null && selectionIndicatorPrefab != null && buildingCollider != null)
        {
            currentIndicator = Instantiate(selectionIndicatorPrefab, currentBuilding.transform);

            Vector3 colliderSize = buildingCollider.bounds.size;
            if (currentBuilding.name.Contains("Farm"))
                currentIndicator.transform.localScale = new Vector3(1, 1, 1);
            else
                currentIndicator.transform.localScale = new Vector3(colliderSize.x / 1000, 1, colliderSize.z / 1000);

            Vector3 colliderBottomCenter = buildingCollider.bounds.center;
            colliderBottomCenter.y = buildingCollider.bounds.min.y + 0.01f;

            currentIndicator.transform.position = colliderBottomCenter;
            currentIndicator.transform.rotation = currentBuilding.transform.rotation;

            if (currentBuilding.name.Contains("Barracks") || currentBuilding.name.Contains("Quarry") || currentBuilding.name.Contains("Artillery") || currentBuilding.name.Contains("Cavarly"))
                currentIndicator.transform.rotation *= Quaternion.Euler(90, 0, 0);
            if (currentBuilding.name.Contains("Mine"))
                currentIndicator.transform.rotation *= Quaternion.Euler(90, 90, 0);
        }

        Transform[] iconSlots = {
            unitButtonContainer.Find("IconSlot1"),
            unitButtonContainer.Find("IconSlot2"),
            unitButtonContainer.Find("IconSlot3"),
            unitButtonContainer.Find("IconSlot4")
        };
        foreach (Transform slot in iconSlots)
        {
            foreach (Transform child in slot)
            {
                Destroy(child.gameObject);
            }
            slot.gameObject.SetActive(false);
        }

        BuildingUnits buildingUnits = building.GetComponent<BuildingUnits>();
        if (buildingUnits != null)
        {
            currentBuildingProduction.unitCountTexts.Clear();
            int index = 0;
            foreach (UnitProduction unitProduction in buildingUnits.units)
            {
                if (index >= iconSlots.Length) break;

                iconSlots[index].gameObject.SetActive(true);

                GameObject iconObject = new GameObject("UnitIcon");
                iconObject.transform.SetParent(iconSlots[index], false);

                Image iconImage = iconObject.AddComponent<Image>();
                iconImage.sprite = unitProduction.unitIcon;

                RectTransform iconRect = iconObject.GetComponent<RectTransform>();
                iconRect.anchorMin = iconRect.anchorMax = iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;

                GameObject countTextObject = new GameObject("CountText");
                countTextObject.transform.SetParent(iconObject.transform, false);
                TextMeshProUGUI countText = countTextObject.AddComponent<TextMeshProUGUI>();
                countText.fontSize = 18;
                countText.alignment = TextAlignmentOptions.BottomRight;

                RectTransform countTextRect = countTextObject.GetComponent<RectTransform>();
                countTextRect.anchorMin = countTextRect.anchorMax = countTextRect.pivot = new Vector2(1f, 0f);
                countTextRect.anchoredPosition = new Vector2(-10f, 10f);

                if (currentBuildingProduction.unitQueue.ContainsKey(unitProduction))
                {
                    countText.text = currentBuildingProduction.remainingUnitsToProduce[unitProduction].ToString();
                }
                else
                {
                    countText.text = "0";
                }

                currentBuildingProduction.unitCountTexts[unitProduction] = countText;

                Button slotButton = iconSlots[index].GetComponent<Button>();
                if (slotButton == null)
                {
                    slotButton = iconSlots[index].gameObject.AddComponent<Button>();
                }

                slotButton.onClick.RemoveAllListeners();
                slotButton.onClick.AddListener(() => AddUnitToBuildingQueue(unitProduction));

                UnitItem unitItem = iconObject.AddComponent<UnitItem>();

                UnitStats unitStats = unitProduction.unitPrefab.GetComponent<UnitStats>();
                unitItem.unitStats = unitStats;

                index++;
            }
        }
    }

    public void HideUnitProductionPanel()
    {
        unitProductionPanel.SetActive(false);

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
            Debug.Log("Selection indicator destroyed for Unit Production.");
        }

        currentBuilding = null;
        currentBuildingProduction = null;
    }

    private void AddUnitToBuildingQueue(UnitProduction unitProduction)
    {
        if (currentBuildingProduction != null)
        {
            currentBuildingProduction.AddUnitToQueue(unitProduction);
        }
    }

    private void UpdateBuildingHpDisplay()
    {
        BuildingProgress buildingProgress = currentBuilding.GetComponent<BuildingProgress>();

        if (buildingProgress != null)
        {
            buildingHpText.text = $"Building HP: {buildingProgress.currentBuildPoints}/{buildingProgress.totalBuildPoints}";
        }
        else
        {
            buildingHpText.text = "Building HP: N/A";
        }
    }
}
