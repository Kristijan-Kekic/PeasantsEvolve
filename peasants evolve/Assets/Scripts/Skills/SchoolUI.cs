using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SchoolUI : MonoBehaviour
{
    public GameObject schoolPanel;
    private bool isOpen = false;   
    public TextMeshProUGUI buildingHpText;

    private GameObject currentBuilding;

    public GameObject selectionIndicatorPrefab;
    private GameObject currentIndicator;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
        {
            HideSchoolPanel();
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
                    HideSchoolPanel();
                }
            }
            else
            {
                HideSchoolPanel();
            }
        }

        if (currentBuilding != null)
        {
            UpdateBuildingHpDisplay();
        }
    }
    public void ShowSchoolPanel(GameObject building)
    {
        BuildingProgress buildingProgress = building.GetComponent<BuildingProgress>();

        if (buildingProgress != null && !buildingProgress.IsCompleted())
        {
            return;
        }

        if (schoolPanel != null && !isOpen)
        {
            schoolPanel.SetActive(true);
            isOpen = true;
            currentBuilding = building;
        }

        Collider buildingCollider = currentBuilding.GetComponent<Collider>();

        if (currentIndicator == null && selectionIndicatorPrefab != null && buildingCollider != null)
        {
            currentIndicator = Instantiate(selectionIndicatorPrefab, currentBuilding.transform);

            Vector3 colliderSize = buildingCollider.bounds.size;
            currentIndicator.transform.localScale = new Vector3(colliderSize.x / 1000, 1, colliderSize.z / 1000);

            Vector3 colliderBottomCenter = buildingCollider.bounds.center;
            colliderBottomCenter.y = buildingCollider.bounds.min.y + 0.1f;

            currentIndicator.transform.position = colliderBottomCenter;
            currentIndicator.transform.rotation = currentBuilding.transform.rotation;

            currentIndicator.transform.rotation *= Quaternion.Euler(90, 0, 0);
        }
    }


    public void HideSchoolPanel()
    {
        if (schoolPanel != null && isOpen)
        {
            schoolPanel.SetActive(false);
            isOpen = false;

            if (currentIndicator != null)
            {
                Destroy(currentIndicator);
                currentIndicator = null;
                Debug.Log("Selection indicator destroyed for School.");
            }
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
