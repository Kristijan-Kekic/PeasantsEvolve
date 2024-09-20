//using System.Collections;
//using UnityEngine;

//public class UnitCombat : MonoBehaviour
//{
//    public UnitStats unitStats;  // Reference to this unit's stats
//    public UnitStats targetUnit; // Reference to the target unit's stats
//    private bool isAttacking = false;

//    private void Start()
//    {
//        unitStats = GetComponent<UnitStats>();  // Get this unit's stats
//    }

//    private void Update()
//    {
//        Check if target is set and in range, then attack
//        if (targetUnit != null && targetUnit.isAlive)
//        {
//            float distance = Vector3.Distance(transform.position, targetUnit.transform.position);
//            if (distance <= unitStats.attackRange)
//            {
//                if (!isAttacking)
//                {
//                    StartCoroutine(AttackTarget());
//                }
//            }
//        }
//    }

//    private IEnumerator AttackTarget()
//    {
//        isAttacking = true;

//        while (targetUnit != null && targetUnit.isAlive)
//        {
//            yield return new WaitForSeconds(unitStats.attackSpeed);

//            Calculate the damage dealt to the target
//            int damage = unitStats.attack - targetUnit.defense;
//            damage = Mathf.Max(0, damage); // Ensure damage is not negative

//            Apply damage to target's health
//            targetUnit.TakeDamage(damage);

//            Debug.Log($"{unitStats.unitName} dealt {damage} damage to {targetUnit.unitName}.");
//        }

//        isAttacking = false;
//    }

//    public void SetTarget(UnitStats newTarget)
//    {
//        targetUnit = newTarget;
//    }

//    public void TakeDamage(int damage)
//    {
//        unitStats.hp -= damage;
//        Debug.Log($"{unitStats.unitName} took {damage} damage. Remaining HP: {unitStats.hp}");

//        if (unitStats.hp <= 0)
//        {
//            Die();
//        }
//    }

//    private void Die()
//    {
//        Debug.Log($"{unitStats.unitName} has died.");
//        Implement logic to remove the unit from the game, like playing a death animation, disabling the unit, etc.
//       Destroy(gameObject);  // For simplicity, destroy the unit when it dies
//    }

//}
