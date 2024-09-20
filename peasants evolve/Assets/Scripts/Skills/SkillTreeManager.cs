using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillTreeSkill
{
    public string skillName;
    public bool isUnlocked = false;
    public GameObject skillIcon;  // Icon in UI
    public SkillEffect effect;
    public float effectValue;
    public int pointsRequired = 1;  // Points required to unlock the skill

    public enum SkillEffect
    {
        UnitBuildTime,
        BaseAttack,
        MoveSpeed,
        Range,
        BuildingHP,
        UnitDefense,
        UnitHP,
        SightRange,
        ExtraFarm,
        ExtraQuarry,
        ExtraMine,
        ExtraCommandPost
    }
}

public class SkillTreeManager : MonoBehaviour
{
    public int totalSkillPoints = 6;  // Total possible skill points
    public int usableSkillPoints = 0;  // Points the player can currently use
    public List<SkillTreeSkill> skillTree = new List<SkillTreeSkill>();  // List of skills in the skill tree

    public GameObject farmBuildingPrefab;  // Reference to the farm building prefab
    public GameObject quarryBuildingPrefab;  // Reference to the quarry building prefab
    public GameObject mineBuildingPrefab;  // Reference to the mine building prefab
    public GameObject commandPostBuildingPrefab;  // Reference to the command post building prefab

    private BuildingManager buildingManager;
    private UnitManager unitManager;
    private ResourceManager resourceManager;

    // UI Reference to display skill points
    public TextMeshProUGUI skillPointsText;

    private void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
        unitManager = FindObjectOfType<UnitManager>();
        buildingManager.OnBuildingCompleted += AwardSkillPoint;
        resourceManager = FindObjectOfType<ResourceManager>();

        UpdateSkillPointsUI();
    }

    private void AwardSkillPoint(string buildingName)
    {
        if (usableSkillPoints < totalSkillPoints)
        {
            usableSkillPoints++;
            UpdateSkillPointsUI();
        }
    }

    public void UnlockSkill(int skillIndex)
    {
        SkillTreeSkill skill = skillTree[skillIndex];

        if (usableSkillPoints >= skill.pointsRequired && !skill.isUnlocked)
        {
            usableSkillPoints -= skill.pointsRequired;
            skill.isUnlocked = true;
            ApplySkillEffect(skill);
            UpdateSkillPointsUI();

            if (skill.skillIcon != null)
            {
                Image iconImage = skill.skillIcon.GetComponent<Image>();

                if (iconImage != null)
                {
                    Color currentColor = iconImage.color;
                    currentColor.a = Mathf.Clamp(currentColor.a - 0.2f, 0f, 1f);
                    iconImage.color = currentColor;
                }
            }
        }
        else
        {

        }
    }


    // Method to update the skill points UI
    private void UpdateSkillPointsUI()
    {
        int usedSkillPoints = totalSkillPoints - usableSkillPoints;  // Calculate used skill points
        skillPointsText.text = $"{usedSkillPoints}/{totalSkillPoints} ({usableSkillPoints} usable)";
    }

    private void ApplySkillEffect(SkillTreeSkill skill)
    {
        switch (skill.effect)
        {
            // Attack Category
            case SkillTreeSkill.SkillEffect.UnitBuildTime:
                unitManager.DecreaseUnitProductionTime(skill.effectValue);
                break;
            case SkillTreeSkill.SkillEffect.BaseAttack:
                unitManager.IncreaseUnitAttack(skill.effectValue);
                break;
            case SkillTreeSkill.SkillEffect.MoveSpeed:
                unitManager.IncreaseUnitMoveSpeed(skill.effectValue);
                break;
            case SkillTreeSkill.SkillEffect.Range:
                unitManager.IncreaseUnitRange(skill.effectValue);
                break;

            // Defense Category
            case SkillTreeSkill.SkillEffect.BuildingHP:
                buildingManager.UpgradeBuildingHP(skill.effectValue);
                break;
            case SkillTreeSkill.SkillEffect.UnitDefense:
                unitManager.IncreaseUnitDefense(skill.effectValue);
                break;
            case SkillTreeSkill.SkillEffect.UnitHP:
                unitManager.IncreaseUnitHP(skill.effectValue);
                break;
            case SkillTreeSkill.SkillEffect.SightRange:
                unitManager.IncreaseUnitSightRange(skill.effectValue);
                break;

            case SkillTreeSkill.SkillEffect.ExtraFarm:
                resourceManager.IncreaseFoodProduction(skill.effectValue);
                Debug.Log("Farm production increased by: " + skill.effectValue + "%");
                break;
            case SkillTreeSkill.SkillEffect.ExtraQuarry:
                resourceManager.IncreaseStoneProduction(skill.effectValue);
                Debug.Log("Quarry production increased by: " + skill.effectValue + "%");
                break;
            case SkillTreeSkill.SkillEffect.ExtraMine:
                resourceManager.IncreaseMineProduction(skill.effectValue);
                Debug.Log("Mine production increased by: " + skill.effectValue + "%");
                break;
            case SkillTreeSkill.SkillEffect.ExtraCommandPost:
                resourceManager.IncreaseMoneyProduction(skill.effectValue);  // Assuming command posts affect money production
                Debug.Log("Money production increased by: " + skill.effectValue + "%");
                break;
        }
    }
}
