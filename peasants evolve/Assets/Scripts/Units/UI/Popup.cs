using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public GameObject popupPanel;  // The panel that shows the info
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;

    private RectTransform popupRectTransform;

    private UnitManager unitManager;

    private void Start()
    {
        popupRectTransform = popupPanel.GetComponent<RectTransform>();
        unitManager = FindObjectOfType<UnitManager>();
    }

    // Show popup for unit
    public void ShowPopup(UnitStats unitStats, Vector3 position)
    {
        titleText.text = "Unit: " + unitStats.unitName;
        descriptionText.text = "Health: " + (unitStats.hp * unitManager.unitHpMultiplier).ToString() + "\nAttack: " + (unitStats.attack * unitManager.unitAttackMultiplier).ToString() + "\nDefense: " + (unitStats.defense * unitManager.unitDefenseMultiplier).ToString(); ;
        costText.text = $"Cost: Food {unitStats.foodCost}, Money {unitStats.moneyCost}, Metal {unitStats.metalCost}";

        // Position the popup near the mouse
        popupRectTransform.position = position + new Vector3(120f, 0f, 0f);
        popupPanel.SetActive(true);
    }

    // Show popup for skill
    public void ShowPopup(Skill skill, Vector3 position)
    {
        titleText.text = "Skill: " + skill.skillName;
        descriptionText.text = "Description: " + skill.description;
        costText.text = $"Cost: Food {skill.foodCost}, Stone {skill.stoneCost}, Wood {skill.woodCost}, Money {skill.moneyCost}, Coal {skill.coalCost}, Gold {skill.goldCost}, Metal {skill.metalCost}";

        // Position the popup near the mouse
        popupRectTransform.position = position + new Vector3(120f, 0f, 0f);
        popupPanel.SetActive(true);
    }

    // Hide the popup
    public void HidePopup()
    {
        popupPanel.SetActive(false);
    }
}
