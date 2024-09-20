using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class EnemyUnitManager : MonoBehaviour
{
    public float spacing = 2f;

    public void MoveTroopsToTarget(List<GameObject> units, Vector3 targetPosition)
    {
        foreach (var unit in units)
        {
            if (unit == null) continue; // Skip destroyed units

            NavMeshAgent agent = unit.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.SetDestination(targetPosition);
            }
        }
    }
}
