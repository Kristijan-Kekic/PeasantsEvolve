using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Skill
{
    public string skillName;
    public string description;
    public bool isUnlocked;
    public int requiredSkillIndex;  // Index of the required skill, if any (-1 for no requirement)
    public Image skillIcon;

    public int woodCost;
    public int stoneCost;
    public int foodCost;
    public int moneyCost;
    public int coalCost;
    public int goldCost;
    public int metalCost;

    public enum SkillEffect { FoodProduction, BuildingHP, BuildSpeed, TreeGathering, MineProduction, Education, UnitAttack, BridgeUnit }
    public SkillEffect effect;
    public float effectValue; 
}
