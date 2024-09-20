using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TreeGenerator : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public int numberOfTrees = 100;
    public Terrain terrain;
    public float minHeightVariation = 0.5f;
    public float maxSlope = 30f;
    private Vector3 terrainSize;

    public float maxLevel = 70f;
    public float waterLevel = 53f;

    // Start is called before the first frame update
    void Start()
    {
        if (terrain == null)
        {
            Debug.LogError("Terrain reference not set.");
            return;
        }

        terrainSize = terrain.terrainData.size; // Get the size of the terrain from the Terrain component
        GenerateTrees();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void GenerateTrees()
    {
        for (int i = 0; i < numberOfTrees; i++)
        {
            Vector3 position = FindSuitablePosition();
            if (position != Vector3.zero)
            {
                GameObject tree = Instantiate(treePrefabs[Random.Range(0, treePrefabs.Length)], position, Quaternion.identity);
                tree.layer = LayerMask.NameToLayer("Tree");

                Collider treeCollider = tree.GetComponent<Collider>();

                NavMeshObstacle navObstacle = tree.GetComponent<NavMeshObstacle>();
                if (navObstacle == null)
                {
                    navObstacle = tree.AddComponent<NavMeshObstacle>();
                    navObstacle.carving = true;

                    if (treeCollider is CapsuleCollider capsuleCollider)
                    {
                        navObstacle.shape = NavMeshObstacleShape.Capsule;
                        navObstacle.size = new Vector3(capsuleCollider.radius * 2, capsuleCollider.height, capsuleCollider.radius * 2);
                        navObstacle.center = capsuleCollider.center;
                    }
                }
            }
        }
    }


    Vector3 FindSuitablePosition()
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            Vector3 position = new Vector3(
                Random.Range(0, terrainSize.x),
                0,
                Random.Range(0, terrainSize.z)
            );
            position.y = terrain.SampleHeight(position) + terrain.GetPosition().y;

            if (IsSuitableForTree(position))
            {
                return position;
            }
        }
        return Vector3.zero;
    }

    bool IsSuitableForTree(Vector3 position)
    {
        float height = terrain.SampleHeight(position) + terrain.GetPosition().y;
        float slope = terrain.terrainData.GetSteepness(position.x / terrainSize.x, position.z / terrainSize.z);

        if (slope > maxSlope || height < waterLevel + 0.2f || height > maxLevel)
            return false;

        return true;
    }
}
