using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAttackManager : MonoBehaviour
{
    public float spacing = 2f;
    public List<GameObject> enemyUnits = new List<GameObject>();
    public int minUnitsToAttack = 20;
    public int maxUnitsToAttack = 40;
    public float checkInterval = 1f;
    public int minUnitsToResumeProduction = 5;
    public float noChangeDuration = 30f; // Duration for the failsafe

    private bool attackLaunched = false;
    private int unitsRequiredForAttack;
    private EnemyUnitManager enemyUnitManager;

    // Failsafe variables
    private int previousUnitCount = 0;
    private float timeSinceLastChange = 0f;

    private void Start()
    {
        unitsRequiredForAttack = Random.Range(minUnitsToAttack, maxUnitsToAttack + 1);
        enemyUnitManager = GetComponent<EnemyUnitManager>();

        if (enemyUnitManager == null)
        {
            Debug.LogError("EnemyUnitManager component is missing!");
            return;
        }

        StartCoroutine(CheckUnitCount());
    }

    private IEnumerator CheckUnitCount()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            // Remove destroyed units from the list
            enemyUnits.RemoveAll(unit => unit == null);

            if (!attackLaunched)
            {
                if (enemyUnits.Count >= unitsRequiredForAttack)
                {
                    LaunchAttack();
                }
                else
                {
                    if (enemyUnits.Count == previousUnitCount)
                    {
                        timeSinceLastChange += checkInterval;

                        if (timeSinceLastChange >= noChangeDuration)
                        {
                            LaunchAttack();
                        }
                    }
                    else
                    {
                        timeSinceLastChange = 0f;
                        previousUnitCount = enemyUnits.Count;
                    }
                }
            }
        }
    }

    private void LaunchAttack()
    {
        attackLaunched = true;
        Debug.Log("Launching attack with " + enemyUnits.Count + " units.");

        // Stop troop production
        EnemyTroopProduction[] troopProducers = FindObjectsOfType<EnemyTroopProduction>();
        foreach (EnemyTroopProduction producer in troopProducers)
        {
            producer.StopProduction();
        }

        StartCoroutine(AttackPlayerTargets());
    }

    private IEnumerator AttackPlayerTargets()
    {
        while (enemyUnits.Count > minUnitsToResumeProduction)
        {
            GameObject playerTarget = FindNearestPlayerTarget();


            if (playerTarget == null)
            {
                Debug.Log("No player targets found.");
                break;
            }

            foreach (var unit in enemyUnits)
            {
                if (unit == null)
                {
                    Debug.LogWarning("Found null enemy unit in enemyUnits list.");
                    continue;
                }

                UnitStateMachine stateMachine = unit.GetComponent<UnitStateMachine>();
                if (stateMachine != null)
                {
                    stateMachine.SetTarget(playerTarget);
                }
            }

            while (playerTarget != null && !IsTargetDestroyed(playerTarget))
            {
                if (enemyUnitManager != null)
                {
                    enemyUnitManager.MoveTroopsToTarget(enemyUnits, playerTarget.transform.position);
                }

                yield return new WaitForSeconds(0.5f);
            }

            yield return null;
        }

        EnemyTroopProduction.ResumeAllProductions();
        attackLaunched = false;

        previousUnitCount = enemyUnits.Count;
        timeSinceLastChange = 0f;
        unitsRequiredForAttack = Random.Range(minUnitsToAttack, maxUnitsToAttack + 1);
    }

    private bool IsTargetDestroyed(GameObject target)
    {
        if (target == null) return true;

        BuildingProgress buildingProgress = target.GetComponent<BuildingProgress>();
        UnitStats unitStats = target.GetComponent<UnitStats>();

        if (buildingProgress != null)
        {
            return buildingProgress.currentBuildPoints <= 0;
        }
        else if (unitStats != null)
        {
            return !unitStats.IsAlive;
        }

        return true;
    }

    private GameObject FindNearestPlayerTarget()
    {
        if (SelectionManager.Instance == null)
        {
            Debug.LogError("SelectionManager instance is null.");
            return null;
        }

        List<GameObject> playerUnits = SelectionManager.Instance.playerUnits;
        List<GameObject> playerBuildings = SelectionManager.Instance.playerBuildings;

        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        // Find the nearest player unit
        foreach (var unit in playerUnits)
        {
            if (unit == null) continue;
            float distance = Vector3.Distance(unit.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = unit;
            }
        }

        // Find the nearest player building
        foreach (var building in playerBuildings)
        {
            if (building == null) continue;
            float distance = Vector3.Distance(building.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = building;
            }
        }

        return nearestTarget;
    }

    private GameObject FindNearestEnemyTarget()
    {
        List<GameObject> enemyUnits = SelectionManager.Instance.enemyUnits;
        List<GameObject> enemyBuildings = SelectionManager.Instance.enemyBuildings;

        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        // Check for the nearest enemy unit
        foreach (var unit in enemyUnits)
        {
            if (unit == null) continue;
            float distance = Vector3.Distance(unit.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = unit;
            }
        }

        // Check for the nearest enemy building
        foreach (var building in enemyBuildings)
        {
            if (building == null) continue;
            float distance = Vector3.Distance(building.transform.position, transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = building;
            }
        }

        return nearestTarget;
    }

    public bool IsAttackInProgress()
    {
        return attackLaunched;
    }
}
