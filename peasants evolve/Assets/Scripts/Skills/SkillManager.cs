using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillManager : MonoBehaviour
{
    public List<Skill> skills = new List<Skill>();
    private ResourceManager resourceManager;
    private ResourceGatherer resourceGatherer;
    private BuildingPlacement buildingPlacement;
    private BuildingProgress buildingProgress;
    private UnitManager unitManager;

    private void Start()
    {
        // Automatically find and assign the components at the start
        if (resourceManager == null)
        {
            resourceManager = FindObjectOfType<ResourceManager>();
        }

        if (resourceGatherer == null)
        {
            resourceGatherer = FindObjectOfType<ResourceGatherer>();
        }

        if (buildingPlacement == null)
        {
            buildingPlacement = FindObjectOfType<BuildingPlacement>();
        }

        if (buildingProgress == null)
        {
            buildingProgress = FindObjectOfType<BuildingProgress>();
        }

        if (unitManager == null)
        {
            unitManager = FindObjectOfType<UnitManager>();
        }
    }
    public void UnlockSkill(int skillIndex)
    {
        Skill skill = skills[skillIndex];

        if (skill.isUnlocked || (skill.requiredSkillIndex != -1 && !skills[skill.requiredSkillIndex].isUnlocked))
        {
            return;
        }

        if (resourceManager.HasEnoughResources(skill.woodCost, skill.stoneCost, skill.goldCost, skill.foodCost, skill.moneyCost, skill.coalCost, skill.metalCost))
        {
            resourceManager.DeductResources(skill.woodCost, skill.stoneCost, skill.goldCost, skill.foodCost, skill.moneyCost, skill.coalCost, skill.metalCost);
            skill.isUnlocked = true;

            ApplySkillEffect(skill);

            if (skill.skillIcon != null)
            {
                Destroy(skill.skillIcon.gameObject);
            }
        }
        else
        {

        }
    }


    private void ApplySkillEffect(Skill skill)
    {
        switch (skill.effect)
        {
            case Skill.SkillEffect.FoodProduction:
                resourceManager.IncreaseFoodProduction(skill.effectValue);
                break;
            case Skill.SkillEffect.BuildingHP:
                buildingProgress.UpgradeHP(skill.effectValue);
                break;
            case Skill.SkillEffect.BuildSpeed:
                buildingProgress.IncreaseBuildSpeed(skill.effectValue);
                break;
            case Skill.SkillEffect.TreeGathering:
                resourceManager.IncreaseWorkerCapacity(skill.effectValue);
                break;
            case Skill.SkillEffect.MineProduction:
                resourceManager.IncreaseMineProduction(skill.effectValue);
                break;
            case Skill.SkillEffect.Education:
                resourceManager.IncreaseMoneyProduction(skill.effectValue);
                break;
            case Skill.SkillEffect.UnitAttack:
                unitManager.IncreaseUnitAttack(skill.effectValue);
                break;
            case Skill.SkillEffect.BridgeUnit:
                resourceManager.UnlockBridgeUnit();
                break;
        }
    }
}

