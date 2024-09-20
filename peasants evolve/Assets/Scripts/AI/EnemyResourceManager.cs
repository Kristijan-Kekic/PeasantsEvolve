using UnityEngine;

public class EnemyResourceManager : MonoBehaviour
{

    public static EnemyResourceManager Instance;


    public float foodCurrent = 1000f;
    public float stoneCurrent = 1000f;
    public float woodCurrent = 1000f;
    public float moneyCurrent = 1000f;
    public float coalCurrent = 1000f;
    public float goldCurrent = 1000f;
    public float metalCurrent = 1000f;

    public float foodProductionMultiplier = 1f;
    public float stoneProductionMultiplier = 1f;
    public float moneyProductionMultiplier = 1f;
    public float mineProductionMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Add resources methods for AI
    public void AddWood(int amount, bool production)
    {
        if (production)
            woodCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);
        else
            woodCurrent += amount;
    }

    public void AddFood(int amount, bool production)
    {
        if (production)
            foodCurrent += Mathf.RoundToInt(amount * foodProductionMultiplier);
        else
            foodCurrent += amount;
    }

    public void AddStone(int amount, bool production)
    {
        if (production)
            stoneCurrent += Mathf.RoundToInt(amount * stoneProductionMultiplier);
        else
            stoneCurrent += amount;
    }

    public void AddGold(int amount, bool production)
    {
        if (production)
            goldCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);
        else
            goldCurrent += amount;
    }

    public void AddCoal(int amount, bool production)
    {
        if (production)
            coalCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);
        else
            coalCurrent += amount;
    }

    public void AddMetal(int amount, bool production)
    {
        if (production)
            metalCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);
        else
            metalCurrent += amount;
    }

    public void AddMoney(int amount, bool production)
    {
        if (production)
            moneyCurrent += Mathf.RoundToInt(amount * moneyProductionMultiplier);
        else
            moneyCurrent += amount;
    }

    // Deduct resources methods for AI
    public bool HasEnoughResources(int wood, int stone, int gold, int food, int money, int coal, int metal)
    {
        return woodCurrent >= wood &&
               stoneCurrent >= stone &&
               goldCurrent >= gold &&
               foodCurrent >= food &&
               moneyCurrent >= money &&
               coalCurrent >= coal &&
               metalCurrent >= metal;
    }

    public void DeductResources(int wood, int stone, int gold, int food, int money, int coal, int metal)
    {
        woodCurrent -= wood;
        stoneCurrent -= stone;
        goldCurrent -= gold;
        foodCurrent -= food;
        moneyCurrent -= money;
        coalCurrent -= coal;
        metalCurrent -= metal;
    }
}
