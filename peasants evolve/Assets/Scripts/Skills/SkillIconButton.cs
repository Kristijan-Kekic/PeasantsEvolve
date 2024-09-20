using UnityEngine;
using UnityEngine.UI;

public class SkillIconButton : MonoBehaviour
{
    public int skillIndex;  // The index of the skill in the skillTree
    private SkillTreeManager skillTreeManager;

    private void Start()
    {
        skillTreeManager = FindObjectOfType<SkillTreeManager>();

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSkillIconClicked);
        }
        else
        {
            Debug.LogError("SkillImage needs a Button component!");
        }
    }

    public void OnSkillIconClicked()
    {
        // When the icon is clicked, pass the index to unlock the skill
        skillTreeManager.UnlockSkill(skillIndex);
    }
}