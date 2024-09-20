using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private TMP_Text food;
    [SerializeField] private TMP_Text stone;
    [SerializeField] private TMP_Text wood;
    [SerializeField] private TMP_Text money;
    [SerializeField] private TMP_Text coal;
    [SerializeField] private TMP_Text gold;
    [SerializeField] private TMP_Text metal;


    public float foodInit = 1000f;
    public float stoneInit = 1000f;
    public float woodInit = 1000f;
    public float moneyInit = 1000f;
    public float coalInit = 1000f;
    public float goldInit = 1000f;
    public float metalInit = 1000f;

    private float foodCurrent;
    private float stoneCurrent;
    private float woodCurrent;
    private float moneyCurrent;
    private float coalCurrent;
    private float goldCurrent;
    private float metalCurrent;

    public float foodProductionMultiplier = 1f;
    public float stoneProductionMultiplier = 1f;
    public float moneyProductionMultiplier = 1f;
    public float mineProductionMultiplier = 1f;
    public float workerCapacityMultiplier = 1f;

    [SerializeField] private GameObject bridgeUnitIcon;
    [SerializeField] private GameObject bridgeUnitSlot;

    // Start is called before the first frame update
    public void Awake()
    {
        foodCurrent = foodInit;
        stoneCurrent = stoneInit;
        woodCurrent = woodInit;
        moneyCurrent = moneyInit;
        coalCurrent = coalInit;
        goldCurrent = goldInit;
        metalCurrent = metalInit;

    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        food.text = foodCurrent.ToString();
        stone.text = stoneCurrent.ToString();
        wood.text = woodCurrent.ToString();
        money.text = moneyCurrent.ToString();
        coal.text = coalCurrent.ToString();
        gold.text = goldCurrent.ToString();
        metal.text = metalCurrent.ToString();
    }

    public float GetFoodAmount() { return foodCurrent; }
    public float GetStoneAmount() { return stoneCurrent; }
    public float GetWoodAmount() { return woodCurrent; }
    public float GetGoldAmount() { return goldCurrent; }
    public float GetCoalAmount() { return coalCurrent; }
    public float GetMetalAmount() { return metalCurrent; }
    public float GetMoneyAmount() { return moneyCurrent; }

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

    public void DeductWood(int amount) { woodCurrent -= amount; }
    public void DeductStone(int amount) { stoneCurrent -= amount; }
    public void DeductGold(int amount) { goldCurrent -= amount; }
    public void DeductCoal(int amount) { coalCurrent -= amount; }
    public void DeductMetal(int amount) { metalCurrent -= amount; }
    public void DeductMoney(int amount) { moneyCurrent -= amount; }

    public void AddWood(int amount, bool production)
    {
        if (production)
            woodCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);  // Apply mine production multiplier
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
            stoneCurrent += Mathf.RoundToInt(amount * stoneProductionMultiplier);  // Apply mine production multiplier
        else
            stoneCurrent += amount;
    }

    public void AddGold(int amount, bool production)
    {
        if (production)
            goldCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);  // Apply mine production multiplier
        else
            goldCurrent += amount;

    }

    public void AddCoal(int amount, bool production)
    {
        if (production)
            coalCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);  // Apply mine production multiplier
        else
            coalCurrent += amount;

    }

    public void AddMetal(int amount, bool production)
    {
        if (production)
            metalCurrent += Mathf.RoundToInt(amount * mineProductionMultiplier);  // Apply mine production multiplier
        else
            metalCurrent += amount;

    }

    public void AddMoney(int amount, bool production)
    {
        if (production)
            moneyCurrent += Mathf.RoundToInt(amount * moneyProductionMultiplier);  // You might want a different multiplier for money if it comes from trade, not mining
        else
            moneyCurrent += amount;

    }

    public void IncreaseFoodProduction(float percentage)
    {
        foodProductionMultiplier += percentage / 100f;
    }

    public void IncreaseStoneProduction(float percentage)
    {
        stoneProductionMultiplier += percentage / 100f;
    }

    public void IncreaseMineProduction(float percentage)
    {
        mineProductionMultiplier += percentage / 100f;
    }

    public void IncreaseMoneyProduction(float percentage)
    {
        moneyProductionMultiplier += percentage / 100f;
    }

    public void UnlockBridgeUnit()
    {
        // Unlock the bridge unit by making its icon visible
        if (bridgeUnitIcon != null)
        {
            bridgeUnitIcon.SetActive(true);
            bridgeUnitSlot.SetActive(true);
            Debug.Log("Bridge unit unlocked and icon is now visible.");
        }
        else
        {

        }
    }

    public void IncreaseWorkerCapacity(float percentage)
    {
        workerCapacityMultiplier += percentage / 100f;
        Debug.Log("Worker capacity increased by: " + percentage + "%");
    }
}
