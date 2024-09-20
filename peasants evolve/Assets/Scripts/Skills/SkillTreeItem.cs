using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class SkillTreeItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int skillIndex;  // Index to reference the skill in SkillTreeManager
    private SkillTreeManager skillTreeManager;
    private TextMeshProUGUI skillDescriptionText;  // Reference to the text field in your SkillUIPanel
    private SkillTreeSkill skill;

    private void Start()
    {
        skillTreeManager = FindObjectOfType<SkillTreeManager>();  // Get the SkillTreeManager
        skillDescriptionText = GameObject.Find("SkillDescriptionText").GetComponent<TextMeshProUGUI>();  // Assuming you named the text element SkillDescriptionText

        if (skillTreeManager != null && skillIndex >= 0 && skillIndex < skillTreeManager.skillTree.Count)
        {
            skill = skillTreeManager.skillTree[skillIndex];  // Fetch the corresponding skill by index
        }
        else
        {
            Debug.LogError("SkillTreeManager or skillIndex is invalid");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (skill != null && skillDescriptionText != null)
        {
            // Update the text to show the skill's name and effect
            skillDescriptionText.text = $"Skill: {skill.skillName}\nEffect: {GetEffectDescription(skill)}";
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (skillDescriptionText != null)
        {
            // Clear the text when the pointer exits
            skillDescriptionText.text = "";
        }
    }

    // Helper function to get the skill's effect as a string
    private string GetEffectDescription(SkillTreeSkill skill)
    {
        switch (skill.effect)
        {
            case SkillTreeSkill.SkillEffect.UnitBuildTime:
                return $"Unit Decreased Build Time: {skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.BaseAttack:
                return $"Base Attack: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.MoveSpeed:
                return $"Move Speed: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.Range:
                return $"Range: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.BuildingHP:
                return $"Building HP: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.UnitDefense:
                return $"Unit Defense: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.UnitHP:
                return $"Unit HP: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.SightRange:
                return $"Sight Range: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.ExtraFarm:
                return $"Farm Production: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.ExtraQuarry:
                return $"Quarry Production: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.ExtraMine:
                return $"Mine Production: +{skill.effectValue}%";
            case SkillTreeSkill.SkillEffect.ExtraCommandPost:
                return $"Command Post Production: +{skill.effectValue}%";
            default:
                return "Unknown Effect";
        }
    }
}
