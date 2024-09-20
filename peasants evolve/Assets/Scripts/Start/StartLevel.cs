using UnityEngine;

public class StartLevel : MonoBehaviour
{
    public GameObject playerUnitPrefab;
    public Transform playerSpawnPoint;
    public int startingPlayerUnits = 5;
    public float spawnSpacing = 2.0f;

    public GameObject enemyUnitPrefab;
    public Transform enemySpawnPoint;
    public int startingEnemyUnits = 5;

    void Start()
    {
        SpawnUnits(playerUnitPrefab, playerSpawnPoint, startingPlayerUnits, true);
    }

    void SpawnUnits(GameObject unitPrefab, Transform spawnPoint, int unitCount, bool isPlayerUnit)
    {
        for (int i = 0; i < unitCount; i++)
        {
            Vector3 spawnPosition = spawnPoint.position + new Vector3(i * spawnSpacing, 0, 0);
            GameObject unit = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);

            UnitMovement unitMovement = unit.GetComponent<UnitMovement>();
            if (unitMovement != null)
            {
                if (isPlayerUnit)
                {
                    unitMovement.enabled = false;
                    SelectionManager.Instance.unitList.Add(unit);
                    SelectionManager.Instance.playerUnits.Add(unit);
                }
                else
                {
                    unitMovement.enabled = true;
                    SelectionManager.Instance.enemyUnits.Add(unit);
                }
            }
        }
    }
}
