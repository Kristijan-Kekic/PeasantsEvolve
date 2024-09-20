using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandPostUnitProduction : MonoBehaviour
{
    public GameObject unitPrefab;   // The unit to produce (peasant)
    public Transform spawnPoint;    // The point where units will spawn
    public float productionInterval = 5f;  // Time between unit production
    public int maxUnitsToProduce = 5;  // Total number of units to produce (5 peasants)

    private int unitsProduced = 0;  // Keep track of the units produced
    private bool isProducing = true;  // Set to true to start production
    private Transform commandPost;

    private EnemyResourceGatherer enemyResourceGatherer;
    private List<GameObject> producedUnits = new List<GameObject>();  // List to store produced peasants

    private void Start()
    {
        // Start unit production loop
        if (spawnPoint != null)
        {
            commandPost = this.transform;  // Assuming this script is attached to the CommandPost
            enemyResourceGatherer = FindObjectOfType<EnemyResourceGatherer>();  // Find the enemy resource gatherer script
            StartCoroutine(ProduceUnits());
        }
    }

    IEnumerator ProduceUnits()
    {
        while (unitsProduced < maxUnitsToProduce)  // Stop when all peasants are produced
        {
            // Wait for the production interval before producing the next unit
            yield return new WaitForSeconds(productionInterval);

            if (isProducing)
            {
                // Check if there is enough population space
                if (!EnemyPopulationManager.Instance.CanAddUnit())
                {
                    Debug.LogWarning("Cannot produce unit, population limit reached.");
                    continue;  // Wait for next interval
                }

                // Find a valid spawn position
                Vector3 spawnPosition = FindValidSpawnPosition(spawnPoint.position, unitPrefab);
                if (spawnPosition != Vector3.zero)
                {
                    // Instantiate a new unit at the valid spawn position
                    GameObject newUnit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

                    // Assign the CommandPost to the new unit's ResourceGatherer or similar script
                    ResourceGatherer gatherer = newUnit.GetComponent<ResourceGatherer>();
                    if (gatherer != null)
                    {
                        gatherer.AssignCommandPost(commandPost);  // Assign the CommandPost
                        producedUnits.Add(newUnit);  // Store the produced unit for later assignment
                    }

                    // Add unit to population manager
                    EnemyPopulationManager.Instance.AddUnit(newUnit, false);

                    Debug.Log("Produced a new unit and assigned CommandPost.");
                    unitsProduced++;  // Increment the count of units produced
                }
                else
                {
                    Debug.LogWarning("No valid spawn position found for the new unit.");
                }
            }
        }
        // Once all units are produced, assign them to gather resources
        SendAllPeasantsToGather();

        // Stop production after producing all peasants
        isProducing = false;
        Debug.Log("Finished producing all units.");
    }

    private void SendAllPeasantsToGather()
    {
        if (enemyResourceGatherer != null)
        {
            foreach (GameObject peasant in producedUnits)
            {
                enemyResourceGatherer.AddPeasant(peasant);  // Add the peasants to the resource gatherer
            }
            producedUnits.Clear();  // Clear the list after assigning tasks
        }
    }

    // Method to find a valid spawn position if the spawn point is occupied
    private Vector3 FindValidSpawnPosition(Vector3 initialPosition, GameObject unitPrefab)
    {
        float checkRadius = 0.5f;
        float initialSpawnRadius = 10f;
        float radiusIncrement = 1f;
        int maxAttemptsPerRadius = 10;
        int maxRadiusExpansions = 5;

        float currentSpawnRadius = initialSpawnRadius;

        for (int expansion = 0; expansion < maxRadiusExpansions; expansion++)
        {
            for (int i = 0; i < maxAttemptsPerRadius; i++)
            {
                Vector3 randomPosition = GetRandomPositionAround(initialPosition, currentSpawnRadius);
                if (!IsPositionOccupied(randomPosition, checkRadius))
                {
                    Debug.Log($"Found a free position at {randomPosition} after {i + 1} attempts with a radius of {currentSpawnRadius}.");
                    return randomPosition;
                }
            }

            // Increase the spawn radius after maxAttemptsPerRadius attempts
            currentSpawnRadius += radiusIncrement;
            Debug.Log($"Expanding spawn radius to {currentSpawnRadius}.");
        }

        Debug.LogWarning("No valid spawn position found after maximum radius expansions.");
        return Vector3.zero;  // If no valid position is found, return Vector3.zero
    }

    // Method to check if a position is occupied by a unit or other objects
    private bool IsPositionOccupied(Vector3 position, float radius)
    {
        // Specify the layer of units to avoid detecting non-unit objects
        LayerMask LayerMask = LayerMask.GetMask("EnemyTroops", "EnemyBuilding");

        // Check if there are any colliders within the specified radius on the "EnemyTroops" layer
        Collider[] colliders = Physics.OverlapSphere(position, radius, LayerMask);
        return colliders.Length > 0;
    }

    // Method to get a random position around a point within a radius
    private Vector3 GetRandomPositionAround(Vector3 originalPosition, float radius)
    {
        // Generate a random position around the original position within a circular area
        float randomAngle = Random.Range(0f, 360f);
        float randomDistance = Random.Range(0.5f, radius);  // Adjust the minimum and maximum distance

        Vector3 offset = new Vector3(Mathf.Sin(randomAngle) * randomDistance, 0f, Mathf.Cos(randomAngle) * randomDistance);
        return originalPosition + offset;
    }
}
