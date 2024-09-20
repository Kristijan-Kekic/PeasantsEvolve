using System;
using UnityEngine;

public class BuildingProgress : MonoBehaviour
{
    public float totalBuildPoints;
    public float currentBuildPoints;
    public float baseBuildPoints;
    public float buildRadius;

    public GameObject progressBarPrefab;
    private ProgressBar progressBar;
    private RectTransform progressBarRectTransform;
    private bool isBuilding = false;
    private bool isCompleted = false;
    private bool isDestroyed = false;

    private BuildingManager buildingManager;

    public float buildSpeedMultiplier = 1f;
    public float buildingHPMultiplier = 1f;

    public bool isAIControlled = false;

    public event Action OnBuildingComplete;
    public event Action OnBuildingDestroyed;

    private void Awake()
    {
        buildingManager = FindObjectOfType<BuildingManager>();

        if (isAIControlled)
        {

        }
        else
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            GameObject progressBarInstance = Instantiate(progressBarPrefab, canvas.transform);
            progressBar = progressBarInstance.GetComponent<ProgressBar>();
            progressBarRectTransform = progressBarInstance.GetComponent<RectTransform>();

            progressBar.maximum = Mathf.RoundToInt(totalBuildPoints);
            progressBar.current = 0;
            progressBar.gameObject.SetActive(false);
        }
    }

    public void StartBuilding()
    {
        if (isAIControlled)
        {
            return;
        }

        if (!isCompleted)
        {
            isBuilding = true;
            progressBar.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (isBuilding && !isAIControlled)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);
            progressBar.transform.position = screenPosition;

            float distanceToCamera = Vector3.Distance(Camera.main.transform.position, transform.position);
            float scaleFactor = Mathf.Clamp(1 / distanceToCamera * 50f, 0.5f, 2f);

            progressBarRectTransform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);

            if (progressBar.maximum == progressBar.current)
            {
                CompleteBuilding();
            }
        }
    }

    public void CompleteBuilding()
    {
        isBuilding = false;
        isCompleted = true;
        OnBuildingComplete?.Invoke();

        if(!isAIControlled)
            buildingManager.NotifyBuildingCompleted(gameObject.name);

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        Debug.Log("Building complete!");

        if (gameObject.CompareTag("CommandPost"))
        {
            buildingManager.AssignCommandPostToUnitsWithoutPost(transform);
        }

        HousingSpace housingSpace = GetComponent<HousingSpace>();
        if (housingSpace != null)
        {
            if (isAIControlled)
                EnemyPopulationManager.Instance.AddBuildingPopulation(housingSpace.Space);
            else
                PopulationManager.Instance.AddBuildingPopulation(housingSpace.Space);
        }
    }

    public void AutoAssignBuilders()
    {
        ResourceGatherer[] peasants = FindObjectsOfType<ResourceGatherer>();

        foreach (ResourceGatherer peasant in peasants)
        {
            float distanceToBuilding = Vector3.Distance(peasant.transform.position, transform.position);

            if (distanceToBuilding <= buildRadius && peasant.IsIdle())
            {
                peasant.GoToBuilding(this);
            }
        }
    }

    public void AddBuildPoints(float buildPoints)
    {
        if (isCompleted || isAIControlled) return;

        currentBuildPoints += buildPoints;
        currentBuildPoints = Mathf.Clamp(currentBuildPoints, 0, totalBuildPoints);

        if (progressBar != null && !isAIControlled)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.current = Mathf.RoundToInt(currentBuildPoints);
        }

        if (currentBuildPoints >= totalBuildPoints)
        {
            CompleteBuilding();
        }
    }

    public bool IsCompleted()
    {
        return isCompleted;
    }

    public float GetBuildProgress()
    {
        return currentBuildPoints / totalBuildPoints;
    }

    public void IncreaseBuildSpeed(float percentage)
    {
        buildSpeedMultiplier += percentage / 100f;
        Debug.Log("Build speed increased by: " + percentage + "%");
    }

    public void UpgradeHP(float multiplier)
    {
        buildingHPMultiplier = multiplier;

        // Calculate new total HP based on base HP and multiplier
        float newTotalBuildPoints = baseBuildPoints * buildingHPMultiplier;

        // If the building's current HP was maxed out before the upgrade, set it to the new max HP
        if (Mathf.Approximately(currentBuildPoints, totalBuildPoints))
        {
            currentBuildPoints = newTotalBuildPoints;
        }

        // Update total HP after the upgrade
        totalBuildPoints = newTotalBuildPoints;

        // Ensure current HP doesn't exceed the new total HP
        currentBuildPoints = Mathf.Min(currentBuildPoints, totalBuildPoints);

        Debug.Log($"{gameObject.name} HP upgraded to {totalBuildPoints}. Current HP: {currentBuildPoints}");
    }

    public void TakeDamage(float damage)
    {
        if (isCompleted && !isDestroyed)
        {
            currentBuildPoints -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {currentBuildPoints}");

            if (currentBuildPoints <= 0)
            {
                DestroyBuilding();  // Trigger building destruction
            }
        }
    }

    private void DestroyBuilding()
    {
        if (isDestroyed) return;  // Prevent multiple destruction calls
        isDestroyed = true;

        // Notify the BuildingManager
        buildingManager.RemoveBuilding(gameObject.name);

        // Trigger the OnBuildingDestroyed event if necessary
        OnBuildingDestroyed?.Invoke();

        if (SelectionManager.Instance != null && SelectionManager.Instance.playerBuildings.Contains(gameObject))
        {
            SelectionManager.Instance.playerBuildings.Remove(gameObject);
        }

        if (SelectionManager.Instance != null && SelectionManager.Instance.enemyBuildings.Contains(gameObject))
        {
            SelectionManager.Instance.playerBuildings.Remove(gameObject);
        }

        Debug.Log($"{gameObject.name} has been destroyed!");

        Destroy(gameObject);
    }
}
