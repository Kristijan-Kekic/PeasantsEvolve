using UnityEngine;
using UnityEngine.UI;

public class SkillImage : MonoBehaviour
{
    public int skillIndex;  // The index of the skill in the SkillManager
    private SkillManager skillManager;

    private void Start()
    {
        skillManager = FindObjectOfType<SkillManager>();

        // Get the Button component and add a click listener
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnSkillImageClicked);
        }
        else
        {
            Debug.LogError("SkillImage needs a Button component!");
        }
    }

    private void OnSkillImageClicked()
    {
        skillManager.UnlockSkill(skillIndex);
    }
}
