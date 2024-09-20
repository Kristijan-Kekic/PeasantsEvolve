using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public float unitProductionMultiplier = 1f;
    public float unitAttackMultiplier = 1f;
    public float unitDefenseMultiplier = 1f;
    public float unitHpMultiplier = 1f;
    public float unitRangeMultiplier = 1f;

    private List<GameObject> allUnits;

    private void Update()
    {
        RefreshAllUnits();
    }

    public void IncreaseUnitAttack(float percentage)
    {
        foreach (GameObject unitObj in allUnits)
        {
            UnitStats unit = unitObj.GetComponent<UnitStats>();
            if (unit != null)
            {
                unit.attack = Mathf.RoundToInt(unit.baseAttack * (1 + percentage / 100f));
                Debug.Log($"{unit.unitName} attack increased to {unit.attack}");
            }
        }

        unitAttackMultiplier += 1 * percentage / 100;
    }

    public void IncreaseUnitDefense(float percentage)
    {
        foreach (GameObject unitObj in allUnits)
        {
            UnitStats unit = unitObj.GetComponent<UnitStats>();
            if (unit != null)
            {
                unit.defense = Mathf.RoundToInt(unit.baseDefense * (1 + percentage / 100f));
                Debug.Log($"{unit.unitName} defense increased to {unit.defense}");
            }
        }

        unitDefenseMultiplier += 1 * percentage / 100;
    }

    public void RefreshAllUnits()
    {
        allUnits = SelectionManager.Instance.playerUnits;
    }

    public void DecreaseUnitProductionTime(float percentage)
    {
        unitProductionMultiplier *= (1 - percentage / 100f);
        Debug.Log($"Unit production time decreased by {percentage}%");
    }

    public void IncreaseUnitMoveSpeed(float percentage)
    {
        foreach (GameObject unitObj in allUnits)
        {
            UnitStats unit = unitObj.GetComponent<UnitStats>();
            if (unit != null)
            {
                unit.movementSpeed = Mathf.RoundToInt(unit.baseMovementSpeed * (1 + percentage / 100f));
                Debug.Log($"{unit.unitName} movement speed increased to {unit.movementSpeed}");
            }
        }
    }

    public void IncreaseUnitRange(float percentage)
    {
        foreach (GameObject unitObj in allUnits)
        {
            UnitStats unit = unitObj.GetComponent<UnitStats>();
            if (unit != null)
            {
                unit.attackRange = Mathf.RoundToInt(unit.baseAttackRange * (1 + percentage / 100f));
                Debug.Log($"{unit.unitName} range increased to {unit.attackRange}");
            }
        }

        unitRangeMultiplier = 1 * percentage / 100;
    }

    public void IncreaseUnitHP(float percentage)
    {
        foreach (GameObject unitObj in allUnits)
        {
            UnitStats unit = unitObj.GetComponent<UnitStats>();
            if (unit != null)
            {
                unit.hp = Mathf.RoundToInt(unit.baseHp * (1 + percentage / 100f));
                Debug.Log($"{unit.unitName} HP increased to {unit.hp}");
            }
        }

        unitHpMultiplier = 1 * percentage / 100;
    }

    public void IncreaseUnitSightRange(float percentage)
    {
        foreach (GameObject unitObj in allUnits)
        {
            UnitStats unit = unitObj.GetComponent<UnitStats>();
            if (unit != null)
            {
                unit.sightRange = Mathf.RoundToInt(unit.baseSightRange * (1 + percentage / 100f));
                Debug.Log($"{unit.unitName} sight range increased to {unit.sightRange}");
            }
        }
    }
}
