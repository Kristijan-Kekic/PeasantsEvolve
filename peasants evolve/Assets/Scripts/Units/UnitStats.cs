using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class UnitStats : MonoBehaviour
{
    public string unitName;
    public int hp;
    public int baseHp;
    public int attack;
    public int baseAttack;
    public int defense;
    public int baseDefense;
    public int capacity;
    public int baseCapacity;
    public float buildSpeed;
    public float baseBuildSpeed;
    public float productionSpeed;
    public float baseProductionSpeed;
    public float attackRange;
    public float baseAttackRange;
    public float attackSpeed;
    public float movementSpeed;
    public float baseMovementSpeed;
    public float sightRange;
    public float baseSightRange;

    public int foodCost;
    public int stoneCost;
    public int woodCost;
    public int moneyCost;
    public int coalCost;
    public int goldCost;
    public int metalCost;

    public bool isEnemy;

    private UnitManager unitManager;
    private EnemyAttackManager enemyAttackManager;

    public bool IsAlive => hp > 0;

    private void Start()
    {
        hp = baseHp;
        unitManager = FindObjectOfType<UnitManager>();
        enemyAttackManager = GetComponent<EnemyAttackManager>();

        if (unitManager != null)
        {
            attack = Mathf.RoundToInt(baseAttack * unitManager.unitAttackMultiplier);
        }
    }

    public void TakeDamage(int damage)
    {
        if (this == null) return;

        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
    }

    public void DealDamage(UnitStats target)
    {
        if (target == null || !target.IsAlive) return;

        int damageDealt = attack - target.defense;
        if (damageDealt < 0) damageDealt = 0; // Ensure damage is not negative

        target.TakeDamage(damageDealt);
    }

    public void DealDamageToBuilding(BuildingProgress building)
    {
        if (building == null || building.currentBuildPoints <= 0) return;

        int damageDealt = attack; // Buildings don't usually have defense, so direct attack damage
        building.TakeDamage(damageDealt);
    }


    private void Die()
    {
        Debug.Log($"{unitName} has died.");

        // Split population logic between player and enemy
        if (isEnemy)
        {
            // Decrement enemy population
            if (EnemyPopulationManager.Instance != null)
            {
                EnemyPopulationManager.Instance.RemoveUnit(gameObject);
            }

            // Remove from enemyUnits list in SelectionManager
            if (SelectionManager.Instance != null && SelectionManager.Instance.enemyUnits.Contains(gameObject))
            {
                if (enemyAttackManager != null)
                {
                    enemyAttackManager.enemyUnits.Remove(gameObject);
                }

                SelectionManager.Instance.enemyUnits.Remove(gameObject);
                
            }
        }
        else
        {
            // Decrement player population
            if (PopulationManager.Instance != null)
            {
                PopulationManager.Instance.RemoveUnit(gameObject);
            }

            // Remove from playerUnits list in SelectionManager
            if (SelectionManager.Instance != null && SelectionManager.Instance.playerUnits.Contains(gameObject))
            {
                SelectionManager.Instance.playerUnits.Remove(gameObject);
            }
        }

        // Remove from the general unitList in SelectionManager
        if (SelectionManager.Instance != null && SelectionManager.Instance.unitList.Contains(gameObject))
        {
            SelectionManager.Instance.unitList.Remove(gameObject);
        }

        // Destroy the unit
        Destroy(gameObject);
    }
}