using System.Collections;
using UnityEngine;

public class ResourceProducer : MonoBehaviour
{
    public enum ResourceType { Food, Stone, Gold, Metal, Coal, Money }

    public ResourceType resourceProduced;      
    public int productionAmount;               
    public float productionInterval;           

    private ResourceManager resourceManager;
    private EnemyResourceManager enemyResourceManager;
    private bool isProducing = false;
    private BuildingProgress buildingProgress;

    public bool isAIControlled = false;

    private void Start()
    {
        if (isAIControlled)
        {
            enemyResourceManager = EnemyResourceManager.Instance;
        }
        else
        {
            resourceManager = FindObjectOfType<ResourceManager>();
        }

        buildingProgress = GetComponent<BuildingProgress>();

        if (buildingProgress == null || buildingProgress.IsCompleted())
        {
            StartProducing();
        }
        else
        {
            buildingProgress.OnBuildingComplete += StartProducing;
        }
    }

    public void StartProducing()
    {
        if (!isProducing && (buildingProgress == null || buildingProgress.IsCompleted()))
        {
            isProducing = true;
            StartCoroutine(ProduceResources());
        }
    }
    private IEnumerator ProduceResources()
    {
        while (true)
        {
            yield return new WaitForSeconds(productionInterval);

            if (isAIControlled && enemyResourceManager != null)
            {
                ProduceForEnemy();
            }
            else if (resourceManager != null)
            {
                ProduceForPlayer();
            }
        }
    }

    private void ProduceForPlayer()
    {
        switch (resourceProduced)
        {
            case ResourceType.Food:
                resourceManager.AddFood(productionAmount, true);
                break;
            case ResourceType.Stone:
                resourceManager.AddStone(productionAmount, true);
                break;
            case ResourceType.Gold:
                resourceManager.AddGold(productionAmount, true);
                break;
            case ResourceType.Metal:
                resourceManager.AddMetal(productionAmount, true);
                break;
            case ResourceType.Coal:
                resourceManager.AddCoal(productionAmount, true);
                break;
            case ResourceType.Money:
                resourceManager.AddMoney(productionAmount, true);
                break;
        }
    }

    private void ProduceForEnemy()
    {
        switch (resourceProduced)
        {
            case ResourceType.Food:
                enemyResourceManager.AddFood(productionAmount, true);
                break;
            case ResourceType.Stone:
                enemyResourceManager.AddStone(productionAmount, true);
                break;
            case ResourceType.Gold:
                enemyResourceManager.AddGold(productionAmount, true);
                break;
            case ResourceType.Metal:
                enemyResourceManager.AddMetal(productionAmount, true);
                break;
            case ResourceType.Coal:
                enemyResourceManager.AddCoal(productionAmount, true);
                break;
            case ResourceType.Money:
                enemyResourceManager.AddMoney(productionAmount, true);
                break;
        }
    }
}
