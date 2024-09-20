using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum UnitState
{
    Idle,
    MovingToTarget,
    Attacking,
    MovingToBuilding,
    MovingToPosition
}

public class UnitStateMachine : MonoBehaviour
{
    private UnitStats unitStats;
    private NavMeshAgent navMeshAgent;
    private UnitStats currentTarget;
    private BuildingProgress currentBuildingTarget;
    private bool isAttacking = false;
    private UnitState unitState;

    public LayerMask enemyLayer;
    public LayerMask buildingLayer;
    public float detectionRange = 10f;

    private Animator animator;

    private void Start()
    {
        unitStats = GetComponent<UnitStats>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        StartCoroutine(ContinuousDetectAndAttack());
    }

    private void Update()
    {
        switch (unitState)
        {
            case UnitState.MovingToPosition:
                HandleArrivalAtDestination();
                break;

            case UnitState.MovingToTarget:
                HandleMovementToTarget();
                break;

            case UnitState.MovingToBuilding:
                HandleMovementToBuilding();
                break;

            case UnitState.Idle:
                DetectAndAttack();
                break;

            case UnitState.Attacking:
                // Attacking logic is handled in the coroutine
                break;

            default:
                break;
        }
    }

    public void ChangeState(UnitState newState)
    {
        if (unitState == newState) return;

        unitState = newState;

        switch (unitState)
        {
            case UnitState.Idle:
                navMeshAgent.isStopped = true;
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
                break;

            case UnitState.MovingToTarget:
            case UnitState.MovingToBuilding:
            case UnitState.MovingToPosition:
                navMeshAgent.isStopped = false;
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
                break;

            case UnitState.Attacking:
                navMeshAgent.isStopped = true;
                animator.SetBool("isIdle", false);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", true);
                break;
        }
    }

    public void SetTarget(GameObject target)
    {
        if (target == null) return;

        UnitStats targetStats = target.GetComponent<UnitStats>();
        BuildingProgress targetBuilding = target.GetComponent<BuildingProgress>();

        if (targetStats != null)
        {
            currentTarget = targetStats;
            currentBuildingTarget = null;
            MoveToTarget();
        }
        else if (targetBuilding != null)
        {
            currentBuildingTarget = targetBuilding;
            currentTarget = null;
            MoveToBuilding();
        }
    }

    public void MoveToPosition(Vector3 destination)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.stoppingDistance = 0.5f;
            navMeshAgent.SetDestination(destination);

            ChangeState(UnitState.MovingToPosition);
        }
    }

    private void MoveToTarget()
    {
        if (navMeshAgent != null && currentTarget != null)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.stoppingDistance = unitStats.attackRange - 0.5f;
            navMeshAgent.SetDestination(currentTarget.transform.position);
            ChangeState(UnitState.MovingToTarget);
        }
    }

    private void MoveToBuilding()
    {
        if (navMeshAgent != null && currentBuildingTarget != null)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.stoppingDistance = unitStats.attackRange - 0.5f;
            navMeshAgent.SetDestination(currentBuildingTarget.transform.position);
            ChangeState(UnitState.MovingToBuilding);
        }
    }

    private void HandleArrivalAtDestination()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    ChangeState(UnitState.Idle);
                }
            }
        }
    }

    private void HandleMovementToTarget()
    {
        if (currentTarget == null || !currentTarget.IsAlive)
        {
            Debug.Log("Target is null or no longer alive.");
            ChangeState(UnitState.Idle);
            return;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            if (currentTarget != null && currentTarget.IsAlive)
            {
                StartCoroutine(AttackEnemyUnit());
            }
            else
            {
                Debug.Log("Target was null or dead upon arrival.");
                ChangeState(UnitState.Idle);
            }
        }
    }


    private void HandleMovementToBuilding()
    {
        if (currentBuildingTarget == null || currentBuildingTarget.currentBuildPoints <= 0)
        {
            Debug.Log("Building target is null or already destroyed.");
            ChangeState(UnitState.Idle);
            return;
        }

        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            StartCoroutine(AttackEnemyBuilding());
        }
    }

    private void DetectAndAttack()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, enemyLayer);
        if (hitColliders.Length > 0)
        {
            GameObject nearestEnemy = hitColliders[0].gameObject;
            SetTarget(nearestEnemy);
        }
    }

    private IEnumerator AttackEnemyUnit()
    {
        if (isAttacking || currentTarget == null || !currentTarget.IsAlive)
        {
            ChangeState(UnitState.Idle);
            yield break;
        }

        isAttacking = true;
        ChangeState(UnitState.Attacking);

        while (currentTarget != null && currentTarget.IsAlive)
        {
            if (unitStats != null)
            {
                // Deal damage to the current target
                unitStats.DealDamage(currentTarget);
            }
            yield return new WaitForSeconds(unitStats.attackSpeed);
        }

        isAttacking = false;
        ChangeState(UnitState.Idle);
    }


    private IEnumerator AttackEnemyBuilding()
    {
        if (isAttacking || currentBuildingTarget == null || currentBuildingTarget.currentBuildPoints <= 0)
        {
            ChangeState(UnitState.Idle);
            yield break;
        }

        isAttacking = true;
        ChangeState(UnitState.Attacking);

        while (currentBuildingTarget != null && currentBuildingTarget.currentBuildPoints > 0)
        {
            if (unitStats != null)
            {
                unitStats.DealDamageToBuilding(currentBuildingTarget);
            }
            yield return new WaitForSeconds(unitStats.attackSpeed);
        }

        isAttacking = false;
        ChangeState(UnitState.Idle);
    }

    private IEnumerator ContinuousDetectAndAttack()
    {
        while (true)
        {
            DetectAndAttack();
            yield return new WaitForSeconds(0.5f);
        }
    }
}
