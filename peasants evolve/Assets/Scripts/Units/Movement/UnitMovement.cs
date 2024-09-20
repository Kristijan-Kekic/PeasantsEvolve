using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private Camera myCam;
    private NavMeshAgent myAgent;
    private ResourceGatherer resourceGatherer;

    public LayerMask groundLayer;
    public LayerMask treeLayer;
    public LayerMask buildingLayer;
    private Animator animator;

    public bool isWorker;

    void Start()
    {
        myCam = Camera.main;
        myAgent = GetComponent<NavMeshAgent>();
        myAgent.stoppingDistance = 1f; // Adjust as needed
        myAgent.autoBraking = true;
        resourceGatherer = GetComponent<ResourceGatherer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleRightClick();
        CheckIfDestinationReached();
        UpdateAnimations();
    }

    private void HandleRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = myCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (isWorker)
                resourceGatherer.StopCurrentTask();

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, treeLayer))
            {
                TreeResource tree = hit.collider.GetComponent<TreeResource>();
                if (tree != null)
                {
                    resourceGatherer.GoToTree(tree);
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, buildingLayer))
            {
                BuildingProgress building = hit.collider.GetComponent<BuildingProgress>();
                if (building != null)
                {
                    resourceGatherer.GoToBuilding(building);
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                myAgent.isStopped = false;
                myAgent.ResetPath();
                myAgent.SetDestination(hit.point);
                animator.SetBool("isWalking", true);
                animator.SetBool("isAttacking", false);
            }
            else
            {
                Debug.Log("Ground not hit.");
                myAgent.isStopped = true;
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                animator.SetBool("isAttacking", false);
            }
        }
    }

    private void CheckIfDestinationReached()
    {
        if (!myAgent.pathPending)
        {
            if (myAgent.remainingDistance <= myAgent.stoppingDistance)
            {
                if (!myAgent.hasPath || myAgent.velocity.sqrMagnitude < 0.01f)
                {
                    myAgent.isStopped = true;

                    animator.SetBool("isWalking", false);
                    animator.SetBool("isIdle", true);
                }
            }
        }
    }

    private void UpdateAnimations()
    {
        if (myAgent.velocity.sqrMagnitude < 0.1f)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
    }
}
