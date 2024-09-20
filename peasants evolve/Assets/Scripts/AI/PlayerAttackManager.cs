using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackManager : MonoBehaviour
{
    public List<GameObject> playerUnits = new List<GameObject>();
    public float checkInterval = 1f;
    public float attackRange = 10f;

    private void Start()
    {
        StartCoroutine(CheckForEnemies());
    }

    private void Update()
    {
        playerUnits = SelectionManager.Instance.playerUnits;
    }

    private IEnumerator CheckForEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            playerUnits.RemoveAll(unit => unit == null);

            foreach (var unit in playerUnits)
            {
                if (unit == null) continue;

                // Find nearest enemy target (unit or building)
                GameObject enemyTarget = FindNearestEnemyTarget(unit);

                if (enemyTarget != null)
                {
                    UnitStateMachine stateMachine = unit.GetComponent<UnitStateMachine>();
                    if (stateMachine != null)
                    {
                        stateMachine.SetTarget(enemyTarget);
                    }
                }
            }
        }
    }

    private GameObject FindNearestEnemyTarget(GameObject playerUnit)
    {
        List<GameObject> enemyUnits = SelectionManager.Instance.enemyUnits;
        List<GameObject> enemyBuildings = SelectionManager.Instance.enemyBuildings;

        GameObject nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        // Check for the nearest enemy unit
        foreach (var unit in enemyUnits)
        {
            if (unit == null) continue;
            float distance = Vector3.Distance(unit.transform.position, playerUnit.transform.position);
            if (distance < nearestDistance && distance <= attackRange)
            {
                nearestDistance = distance;
                nearestTarget = unit;
            }
        }

        // Check for the nearest enemy building
        foreach (var building in enemyBuildings)
        {
            if (building == null) continue;
            float distance = Vector3.Distance(building.transform.position, playerUnit.transform.position);
            if (distance < nearestDistance && distance <= attackRange)
            {
                nearestDistance = distance;
                nearestTarget = building;
            }
        }

        return nearestTarget;
    }
}
