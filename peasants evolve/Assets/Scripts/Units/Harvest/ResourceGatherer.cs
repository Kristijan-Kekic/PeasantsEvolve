using UnityEngine;
using UnityEngine.AI;

public class ResourceGatherer : MonoBehaviour
{
    public float chopRange = 2.0f;
    public float commandPostRange = 7.0f;
    public float chopTime = 1.5f;
    public Transform commandPost;

    private UnitStats unitStats;
    private TreeResource targetTree;
    private BuildingProgress targetBuilding;
    private NavMeshAgent navMeshAgent;
    private float workTimer = 0.0f;
    private int currentWood = 0;

    public bool isEnemy = false;

    private ResourceManager resourceManager;
    private BuildingProgress buildingProgress;
    private EnemyResourceManager enemyResourceManager;

    private enum State { Idle, MovingToTree, Harvesting, Returning, MovingToBuilding, Building }
    private State currentState = State.Idle;

    private Animator animator;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unitStats = GetComponent<UnitStats>();
        animator = GetComponent<Animator>();

        if (isEnemy)
        {
            enemyResourceManager = FindObjectOfType<EnemyResourceManager>();
        }
        else
        {
            resourceManager = FindObjectOfType<ResourceManager>();
        }

        buildingProgress = FindObjectOfType<BuildingProgress>();

        if (commandPost == null)
        {
            BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
            if (buildingManager != null)
            {
                commandPost = buildingManager.GetNearestCommandPost(transform.position);
            }
        }
    }

    public void AssignCommandPost(Transform newCommandPost)
    {
        if (commandPost == null)
        {
            commandPost = newCommandPost;
        }
    }

    private void Update()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f && (currentState == State.MovingToTree || currentState == State.Returning || currentState == State.MovingToBuilding))
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
        else if (currentState == State.Harvesting || currentState == State.Building)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", true);
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isAttacking", false);
            animator.SetBool("isIdle", true);
        }

        switch (currentState)
        {
            case State.MovingToTree:
                MoveToTree();
                break;
            case State.Harvesting:
                HarvestTree();
                break;
            case State.Returning:
                ReturnToCommandPost();
                break;
            case State.MovingToBuilding:
                MoveToBuilding();
                break;
            case State.Building:
                BuildStructure();
                break;
            case State.Idle:
                break;
        }
    }
    #region UnitStates
    public void GoToTree(TreeResource tree)
    {
        if (navMeshAgent == null)
            return;

        targetTree = tree;
        currentState = State.MovingToTree;
        navMeshAgent.SetDestination(targetTree.transform.position);
    }

    private void MoveToTree()
    {
        if (targetTree == null)
        {
            currentState = State.Idle;
            return;
        }

        float distanceToTree = Vector3.Distance(transform.position, targetTree.transform.position);
        if (distanceToTree <= chopRange)
        {
            currentState = State.Harvesting;
        }
    }

    private void HarvestTree()
    {
        if (isEnemy)
            unitStats.capacity = unitStats.baseCapacity;
        else
            unitStats.capacity = Mathf.RoundToInt(unitStats.baseCapacity * resourceManager.workerCapacityMultiplier);

        workTimer += Time.deltaTime;
        if (workTimer >= chopTime)
        {
            workTimer = 0.0f;
            targetTree.Chop();
            currentWood += targetTree.woodPerChop;

            if (currentWood >= unitStats.capacity)
            {
                navMeshAgent.SetDestination(commandPost.position);
                currentState = State.Returning;
            }

            if (targetTree.woodAmount <= 0)
            {
                targetTree = null;
                currentState = State.Idle;
            }
        }
    }

    private void ReturnToCommandPost()
    {
        if (commandPost == null)
        {
            currentState = State.Idle;
            return;
        }

        float distanceToCommandPost = Vector3.Distance(transform.position, commandPost.position);
        if (distanceToCommandPost <= commandPostRange)
        {
            if (isEnemy)
            {
                enemyResourceManager?.AddWood(currentWood, true);
            }
            else
            {
                resourceManager?.AddWood(currentWood, false);
            }

            currentWood = 0;
            currentState = State.MovingToTree;

            if (targetTree != null)
            {
                navMeshAgent.SetDestination(targetTree.transform.position);
            }
        }
    }

    private void MoveToBuilding()
    {
        if (targetBuilding == null || targetBuilding.IsCompleted())
        {
            currentState = State.Idle;
            return;
        }

        float distanceToBuilding = Vector3.Distance(transform.position, targetBuilding.transform.position);
        if (distanceToBuilding <= commandPostRange)
        {
            currentState = State.Building;
            navMeshAgent.isStopped = true;
        }
    }

    public void GoToBuilding(BuildingProgress building)
    {
        StopCurrentTask();
        targetBuilding = building;
        currentState = State.MovingToBuilding;
        navMeshAgent.SetDestination(building.transform.position);
    }

    private void BuildStructure()
    {
        if (targetBuilding == null || targetBuilding.IsCompleted())
        {
            currentState = State.Idle;
            navMeshAgent.isStopped = false;
            return;
        }

        unitStats.buildSpeed = unitStats.baseBuildSpeed * buildingProgress.buildSpeedMultiplier;

        workTimer += Time.deltaTime;
        if (workTimer >= chopTime)
        {
            workTimer = 0.0f;
            targetBuilding.AddBuildPoints(unitStats.buildSpeed);

            if (targetBuilding.IsCompleted())
            {
                currentState = State.Idle;
                targetBuilding = null;
                navMeshAgent.isStopped = false;
            }
        }
    }

    public void StopCurrentTask()
    {
        navMeshAgent.isStopped = false;
        targetTree = null;
        targetBuilding = null;
        currentState = State.Idle;
    }

    public bool IsIdle()
    {
        return currentState == State.Idle;
    }

    #endregion
}
