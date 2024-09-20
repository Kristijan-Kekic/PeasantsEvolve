using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class EnemyResourceGatherer : MonoBehaviour
{
    public LayerMask resourceLayer;
    public float gatherRange = 50f;
    public int maxPeasants = 5;
    private Transform enemyCommandPost;

    private List<GameObject> enemyPeasants = new List<GameObject>();
    private List<TreeResource> availableResources = new List<TreeResource>();

    private void Start()
    {
        BuildingInstance[] enemyBuildings = FindObjectsOfType<BuildingInstance>();
        foreach (BuildingInstance building in enemyBuildings)
        {
            if (building.CompareTag("EnemyCommandPost"))
            {
                enemyCommandPost = building.transform;
                break;
            }
        }
    }

    public void SetCommandPost(Transform commandPost)
    {
        enemyCommandPost = commandPost;
    }

    public void AddPeasant(GameObject newPeasant)
    {
        enemyPeasants.Add(newPeasant);
        AssignPeasantsToGatherResources();
    }

    private void AssignPeasantsToGatherResources()
    {
        if (enemyCommandPost == null)
        {
            Debug.LogWarning("No command post found for enemy peasants.");
            return;
        }

        FindNearbyResources();

        int peasantsAssigned = 0;
        foreach (GameObject peasant in enemyPeasants)
        {
            if (peasantsAssigned >= maxPeasants) break;

            ResourceGatherer gatherer = peasant.GetComponent<ResourceGatherer>();
            if (gatherer != null && availableResources.Count > 0)
            {
                NavMeshAgent navMeshAgent = gatherer.GetComponent<NavMeshAgent>();
                if (navMeshAgent == null)
                {
                    navMeshAgent = gatherer.gameObject.AddComponent<NavMeshAgent>();
                }

                TreeResource nearestTree = availableResources[0];
                gatherer.AssignCommandPost(enemyCommandPost);
                gatherer.GoToTree(nearestTree);
                peasantsAssigned++;
            }
        }
    }



    private void FindNearbyResources()
    {
        Collider[] hitColliders = Physics.OverlapSphere(enemyCommandPost.position, gatherRange, resourceLayer);
        availableResources.Clear();

        foreach (var hitCollider in hitColliders)
        {
            TreeResource resource = hitCollider.GetComponent<TreeResource>();
            if (resource != null && resource.woodAmount > 0)
            {
                availableResources.Add(resource);
            }
        }
    }
}
