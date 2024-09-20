using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildingProduction : MonoBehaviour
{
    public Transform spawnPoint;

    public Dictionary<UnitProduction, int> unitQueue = new Dictionary<UnitProduction, int>();
    private float currentProductionTime = 0f;
    private UnitProduction currentUnitProduction = null;
    public Dictionary<UnitProduction, int> remainingUnitsToProduce = new Dictionary<UnitProduction, int>();
    public Dictionary<UnitProduction, TextMeshProUGUI> unitCountTexts = new Dictionary<UnitProduction, TextMeshProUGUI>();

    private ResourceManager resourceManager;
    private UnitManager unitManager;

    private void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        unitManager = FindObjectOfType<UnitManager>();

        if (spawnPoint == null)
        {
            spawnPoint = transform.Find("SpawnPoint");
        }
    }

    private void Update()
    {
        if (currentUnitProduction != null && currentProductionTime > 0)
        {
            currentProductionTime -= Time.deltaTime;

            if (currentProductionTime <= 0)
            {
                CompleteUnitProduction();
            }
        }
        else if (currentUnitProduction == null && unitQueue.Count > 0)
        {
            StartNextUnitInQueue();
        }
    }

    public void AddUnitToQueue(UnitProduction unitProduction)
    {
        UnitStats unitStats = unitProduction.unitPrefab.GetComponent<UnitStats>();

        if (resourceManager.HasEnoughResources(
            unitStats.woodCost,
            unitStats.stoneCost,
            unitStats.goldCost,
            unitStats.foodCost,
            unitStats.moneyCost,
            unitStats.coalCost,
            unitStats.metalCost
        ))
        {
            resourceManager.DeductResources(
                unitStats.woodCost,
                unitStats.stoneCost,
                unitStats.goldCost,
                unitStats.foodCost,
                unitStats.moneyCost,
                unitStats.coalCost,
                unitStats.metalCost
            );

            if (unitQueue.ContainsKey(unitProduction))
            {
                unitQueue[unitProduction]++;
            }
            else
            {
                unitQueue[unitProduction] = 1;
            }

            if (remainingUnitsToProduce.ContainsKey(unitProduction))
            {
                remainingUnitsToProduce[unitProduction]++;
            }
            else
            {
                remainingUnitsToProduce[unitProduction] = 1;
            }

            if (unitCountTexts.ContainsKey(unitProduction))
            {
                unitCountTexts[unitProduction].text = remainingUnitsToProduce[unitProduction].ToString();
            }
        }
        else
        {

        }
    }

    private void StartNextUnitInQueue()
    {
        if (unitQueue.Count == 0)
        {
            return;
        }

        foreach (var unitProduction in unitQueue.Keys)
        {
            currentUnitProduction = unitProduction;
            break;
        }

        if (currentUnitProduction != null)
        {
            UnitStats unitStats = currentUnitProduction.unitPrefab.GetComponent<UnitStats>();

            if (unitStats != null)
            {
                unitStats.productionSpeed = unitStats.baseProductionSpeed * unitManager.unitProductionMultiplier;
                currentProductionTime = unitStats.productionSpeed;
            }

            DecreaseQueueCount(currentUnitProduction);
        }
    }

    private void CompleteUnitProduction()
    {
        if (spawnPoint != null)
        {
            UnitMovement unitMovement = currentUnitProduction.unitPrefab.GetComponent<UnitMovement>();
            Vector3 spawnPosition = FindValidSpawnPosition(spawnPoint.position, currentUnitProduction.unitPrefab);
            if (spawnPosition != Vector3.zero)
            {
                GameObject newUnit = Instantiate(currentUnitProduction.unitPrefab, spawnPosition, Quaternion.identity);
                PopulationManager.Instance.AddUnit(newUnit, false);
                SelectionManager.Instance.playerUnits.Add(newUnit);
                //unitMovement.enabled = false;
            }
        }

        if (remainingUnitsToProduce.ContainsKey(currentUnitProduction))
        {
            remainingUnitsToProduce[currentUnitProduction]--;
            if (remainingUnitsToProduce[currentUnitProduction] <= 0)
            {
                remainingUnitsToProduce.Remove(currentUnitProduction);
            }

            if (unitCountTexts.ContainsKey(currentUnitProduction))
            {
                unitCountTexts[currentUnitProduction].text = remainingUnitsToProduce.ContainsKey(currentUnitProduction)
                    ? remainingUnitsToProduce[currentUnitProduction].ToString()
                    : "0";
            }
        }

        currentUnitProduction = null;
        currentProductionTime = 0f;

        if (unitQueue.Count > 0)
        {
            StartNextUnitInQueue();
        }
    }

    private void DecreaseQueueCount(UnitProduction unitProduction)
    {
        if (unitQueue.ContainsKey(unitProduction))
        {
            unitQueue[unitProduction]--;

            if (unitQueue[unitProduction] <= 0)
            {
                unitQueue.Remove(unitProduction);
            }
        }
    }

    private Vector3 FindValidSpawnPosition(Vector3 initialPosition, GameObject unitPrefab)
    {
        float checkRadius = 0.5f;
        float initialSpawnRadius = 1f;
        float radiusIncrement = 1f;
        int maxAttemptsPerRadius = 10;
        int maxRadiusExpansions = 5;

        float currentSpawnRadius = initialSpawnRadius;

        for (int expansion = 0; expansion < maxRadiusExpansions; expansion++)
        {
            for (int i = 0; i < maxAttemptsPerRadius; i++)
            {
                Vector3 randomPosition = GetRandomPositionAround(initialPosition, currentSpawnRadius);
                if (!IsPositionOccupied(randomPosition, checkRadius))
                {
                    return randomPosition;
                }
            }

            currentSpawnRadius += radiusIncrement;
        }

        return Vector3.zero;
    }

    private bool IsPositionOccupied(Vector3 position, float radius)
    {
        LayerMask unitLayerMask = LayerMask.GetMask("PlayerTroops");

        Collider[] colliders = Physics.OverlapSphere(position, radius, unitLayerMask);
        return colliders.Length > 0;
    }

    private Vector3 GetRandomPositionAround(Vector3 originalPosition, float radius)
    {
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(0.5f, radius);

        Vector3 offset = new Vector3(Mathf.Sin(randomAngle) * randomDistance, 0f, Mathf.Cos(randomAngle) * randomDistance);
        return originalPosition + offset;
    }
}
