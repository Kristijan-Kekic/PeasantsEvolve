using UnityEngine;
using UnityEngine.AI;

public class ResourceGatherer : MonoBehaviour
{
    [Header("Gathering Settings")]
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

    // Flag to ensure capacity is set only once per Harvesting state
    private bool hasSetCapacity = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        unitStats = GetComponent<UnitStats>();
        animator = GetComponent<Animator>();

        if (isEnemy)
        {
            enemyResourceManager = FindObjectOfType<EnemyResourceManager>();
            Debug.Log($"{gameObject.name} is set as Enemy.");
        }
        else
        {
            resourceManager = FindObjectOfType<ResourceManager>();
            Debug.Log($"{gameObject.name} is set as Player Worker.");
        }

        buildingProgress = FindObjectOfType<BuildingProgress>();

        if (commandPost == null)
        {
            BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
            if (buildingManager != null)
            {
                commandPost = buildingManager.GetNearestCommandPost(transform.position);
                if (commandPost != null)
                {
                    Debug.Log($"{gameObject.name} assigned to Command Post at {commandPost.position}");
                }
                else
                {
                    Debug.LogWarning($"{gameObject.name}: No Command Post found.");
                }
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: BuildingManager not found.");
            }
        }
        else
        {
            Debug.Log($"{gameObject.name} has a manually assigned Command Post.");
        }
    }

    public void AssignCommandPost(Transform newCommandPost)
    {
        if (commandPost == null && newCommandPost != null)
        {
            commandPost = newCommandPost;
            Debug.Log($"{gameObject.name} assigned to new Command Post at {commandPost.position}");
        }
    }

    private void Update()
    {
        UpdateAnimatorParameters();
        HandleState();
    }

    #region State Handling

    private void HandleState()
    {
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
                // Optional: Implement idle behavior or search for new tasks
                break;
        }
    }

    public void GoToTree(TreeResource tree)
    {
        if (navMeshAgent == null || tree == null)
        {
            Debug.LogWarning($"{gameObject.name}: NavMeshAgent or TreeResource is null.");
            return;
        }

        targetTree = tree;
        currentState = State.MovingToTree;
        navMeshAgent.SetDestination(targetTree.transform.position);

        Debug.Log($"{gameObject.name} is moving to tree at {targetTree.transform.position}");
    }

    private void MoveToTree()
    {
        if (targetTree == null)
        {
            Debug.LogWarning($"{gameObject.name}: Target tree is null. Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        float distanceToTree = Vector3.Distance(transform.position, targetTree.transform.position);
        Debug.Log($"{gameObject.name}: Distance to tree: {distanceToTree}");

        if (distanceToTree <= chopRange)
        {
            Debug.Log($"{gameObject.name}: Within chop range. Switching to Harvesting.");
            currentState = State.Harvesting;
            hasSetCapacity = false; // Reset the capacity flag
        }
    }

    private void HarvestTree()
    {
        if (targetTree == null)
        {
            Debug.LogWarning($"{gameObject.name}: Harvesting with no target tree. Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        // Set capacity only once when entering Harvesting state
        if (!hasSetCapacity)
        {
            if (isEnemy)
            {
                unitStats.capacity = unitStats.baseCapacity;
            }
            else
            {
                if (resourceManager != null)
                {
                    unitStats.capacity = Mathf.RoundToInt(unitStats.baseCapacity * resourceManager.workerCapacityMultiplier);
                }
                else
                {
                    Debug.LogWarning($"{gameObject.name}: ResourceManager not found.");
                    unitStats.capacity = unitStats.baseCapacity;
                }
            }
            hasSetCapacity = true;
            Debug.Log($"{gameObject.name}: Capacity set to {unitStats.capacity}");
        }

        workTimer += Time.deltaTime;
        Debug.Log($"{gameObject.name}: Harvesting... Work Timer: {workTimer}");

        if (workTimer >= chopTime)
        {
            workTimer = 0.0f;
            targetTree.Chop();
            currentWood += targetTree.woodPerChop;

            Debug.Log($"{gameObject.name}: Chopped tree. Current wood: {currentWood}");

            if (currentWood >= unitStats.capacity)
            {
                if (commandPost != null)
                {
                    navMeshAgent.SetDestination(commandPost.position);
                    currentState = State.Returning;
                    Debug.Log($"{gameObject.name}: Wood capacity reached. Returning to Command Post at {commandPost.position}");
                }
                else
                {
                    Debug.LogWarning($"{gameObject.name}: Command Post is null. Cannot return wood.");
                    currentState = State.Idle;
                }
                return;
            }

            if (targetTree.woodAmount <= 0)
            {
                Debug.Log($"{gameObject.name}: Tree depleted. Switching to Idle.");
                targetTree = null;
                currentState = State.Idle;
            }
        }
    }

    private void ReturnToCommandPost()
    {
        if (commandPost == null)
        {
            Debug.LogWarning($"{gameObject.name}: No Command Post assigned. Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        float distanceToCommandPost = Vector3.Distance(transform.position, commandPost.position);
        Debug.Log($"{gameObject.name}: Distance to Command Post: {distanceToCommandPost}");

        if (distanceToCommandPost <= commandPostRange)
        {
            if (isEnemy)
            {
                enemyResourceManager?.AddWood(currentWood, true);
                Debug.Log($"{gameObject.name}: Returned {currentWood} wood to Enemy Resource Manager.");
            }
            else
            {
                resourceManager?.AddWood(currentWood, false);
                Debug.Log($"{gameObject.name}: Returned {currentWood} wood to Resource Manager.");
            }

            currentWood = 0;
            currentState = State.MovingToTree;

            if (targetTree != null)
            {
                navMeshAgent.SetDestination(targetTree.transform.position);
                Debug.Log($"{gameObject.name}: Moving back to tree at {targetTree.transform.position}");
            }
            else
            {
                Debug.Log($"{gameObject.name}: No target tree to return to. Switching to Idle.");
                currentState = State.Idle;
            }
        }
    }

    private void MoveToBuilding()
    {
        if (targetBuilding == null || targetBuilding.IsCompleted())
        {
            Debug.Log($"{gameObject.name}: Building target is null or completed. Switching to Idle.");
            currentState = State.Idle;
            return;
        }

        float distanceToBuilding = Vector3.Distance(transform.position, targetBuilding.transform.position);
        Debug.Log($"{gameObject.name}: Distance to Building: {distanceToBuilding}");

        if (distanceToBuilding <= commandPostRange)
        {
            currentState = State.Building;
            navMeshAgent.isStopped = true;
            Debug.Log($"{gameObject.name}: Within range to build. Switching to Building.");
        }
    }

    public void GoToBuilding(BuildingProgress building)
    {
        if (building == null)
        {
            Debug.LogWarning($"{gameObject.name}: BuildingProgress is null.");
            return;
        }

        StopCurrentTask();
        targetBuilding = building;
        currentState = State.MovingToBuilding;
        navMeshAgent.SetDestination(building.transform.position);

        Debug.Log($"{gameObject.name} is moving to building at {building.transform.position}");
    }

    private void BuildStructure()
    {
        if (targetBuilding == null || targetBuilding.IsCompleted())
        {
            Debug.LogWarning($"{gameObject.name}: Building target is null or completed. Switching to Idle.");
            currentState = State.Idle;
            navMeshAgent.isStopped = false;
            return;
        }

        // Calculate build speed
        float buildSpeed = unitStats.baseBuildSpeed * buildingProgress.buildSpeedMultiplier;

        workTimer += Time.deltaTime;
        Debug.Log($"{gameObject.name}: Building... Work Timer: {workTimer}");

        if (workTimer >= chopTime)
        {
            workTimer = 0.0f;
            targetBuilding.AddBuildPoints((int)buildSpeed);


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
        workTimer = 0.0f;
        hasSetCapacity = false;

        Debug.Log($"{gameObject.name}: Stopped current task and switched to Idle.");
    }

    public bool IsIdle()
    {
        return currentState == State.Idle;
    }

    #endregion

    #region Animator Handling

    private void UpdateAnimatorParameters()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f &&
            (currentState == State.MovingToTree || currentState == State.Returning || currentState == State.MovingToBuilding))
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttacking", false);
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
    }

    #endregion
}
