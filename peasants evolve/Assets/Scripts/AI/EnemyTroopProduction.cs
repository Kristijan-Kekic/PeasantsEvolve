using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTroopProduction : MonoBehaviour
{
    public Transform spawnPoint;
    public float productionInterval = 5f;

    private BuildingUnits buildingUnits;
    private EnemyResourceManager resourceManager;
    private EnemyAttackManager enemyAttackManager;
    private int currentUnitIndex = 0;

    private int maxUnitsBeforePause;

    private bool isProducing = true;

    private void Start()
    {
        buildingUnits = GetComponent<BuildingUnits>();
        resourceManager = EnemyResourceManager.Instance;
        enemyAttackManager = FindObjectOfType<EnemyAttackManager>();
        maxUnitsBeforePause = Random.Range(enemyAttackManager.minUnitsToAttack, enemyAttackManager.maxUnitsToAttack + 1);

        if (buildingUnits != null && resourceManager != null && enemyAttackManager != null)
        {
            StartCoroutine(ProduceTroops());
        }
        else
        {
            Debug.LogError("BuildingUnits or EnemyResourceManager component not found.");
        }
    }

    IEnumerator ProduceTroops()
    {
        while (true)
        {
            if (!isProducing || enemyAttackManager.IsAttackInProgress())
            {
                yield return new WaitForSeconds(productionInterval);
                continue;
            }

            if (SelectionManager.Instance.enemyUnits.Count >= maxUnitsBeforePause)
            {
                yield return new WaitForSeconds(productionInterval);
                continue;
            }

            yield return new WaitForSeconds(productionInterval);

            if (buildingUnits.units.Length == 0) yield break;

            UnitProduction unitToProduce = GetNextUnit();
            if (unitToProduce != null && CanAffordUnit(unitToProduce))
            {
                ProduceUnit(unitToProduce);
            }
        }
    }

    private void ProduceUnit(UnitProduction unitProduction)
    {
        Vector3 spawnPosition = FindValidSpawnPosition(spawnPoint.position, unitProduction.unitPrefab);
        if (spawnPosition != Vector3.zero)
        {
            GameObject newUnit = Instantiate(unitProduction.unitPrefab, spawnPosition, Quaternion.identity);
            SelectionManager.Instance.enemyUnits.Add(newUnit);
            enemyAttackManager.enemyUnits.Add(newUnit);
            DeductUnitCost(unitProduction);
        }
    }

    private bool CanAffordUnit(UnitProduction unitProduction)
    {
        UnitStats unitStats = unitProduction.unitPrefab.GetComponent<UnitStats>();
        return resourceManager.HasEnoughResources(
            unitStats.woodCost,
            unitStats.stoneCost,
            unitStats.goldCost,
            unitStats.foodCost,
            unitStats.moneyCost,
            unitStats.coalCost,
            unitStats.metalCost
        );
    }

    private void DeductUnitCost(UnitProduction unitProduction)
    {
        UnitStats unitStats = unitProduction.unitPrefab.GetComponent<UnitStats>();
        resourceManager.DeductResources(
            unitStats.woodCost,
            unitStats.stoneCost,
            unitStats.goldCost,
            unitStats.foodCost,
            unitStats.moneyCost,
            unitStats.coalCost,
            unitStats.metalCost
        );
    }

    private Vector3 FindValidSpawnPosition(Vector3 initialPosition, GameObject unitPrefab)
    {
        float checkRadius = unitPrefab.GetComponent<Collider>().bounds.extents.magnitude;
        float spawnRadius = 30f;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 randomPosition = initialPosition + Random.insideUnitSphere * spawnRadius;
            randomPosition.y = initialPosition.y;

            if (randomPosition.y >= 52.5f && randomPosition.y <= 54f)
            {
                if (!Physics.CheckSphere(randomPosition, checkRadius, LayerMask.GetMask("EnemyTroops", "EnemyBuilding")))
                {
                    return randomPosition;
                }
            }
        }

        return initialPosition;
    }


    private UnitProduction GetNextUnit()
    {
        if (currentUnitIndex >= buildingUnits.units.Length)
        {
            currentUnitIndex = 0;
        }

        UnitProduction unitToProduce = buildingUnits.units[currentUnitIndex];
        currentUnitIndex++;
        return unitToProduce;
    }

    public void StopProduction()
    {
        isProducing = false;
    }

    public void StartProduction()
    {
        isProducing = true;
    }

    public static void ResumeAllProductions()
    {
        EnemyTroopProduction[] allProductions = FindObjectsOfType<EnemyTroopProduction>();
        foreach (EnemyTroopProduction production in allProductions)
        {
            production.StartProduction();
        }
    }
}
