using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillTreeToggle : MonoBehaviour
{
    public RectTransform skillTreePanel; // The skill tree panel (RectTransform)
    public Button toggleButton; // The button to toggle the panel
    public float slideSpeed; // Speed at which the panel slides
    public float raiseAmount; // Amount to raise the panel by

    private bool isSkillTreeVisible = false; // Tracks if the panel is visible
    private Vector2 hiddenPosition;
    private Vector2 visiblePosition;

    private void Start()
    {
        // Set the initial hidden position (off-screen)
        hiddenPosition = skillTreePanel.anchoredPosition;

        // Calculate the visible position by raising the panel by the specified amount
        visiblePosition = new Vector2(hiddenPosition.x, hiddenPosition.y + raiseAmount);

        // Attach the button click listener
        toggleButton.onClick.AddListener(ToggleSkillTree);
    }

    private void ToggleSkillTree()
    {
        isSkillTreeVisible = !isSkillTreeVisible; // Toggle the visibility state
        StopAllCoroutines(); // Stop any ongoing slide animations
        StartCoroutine(SlideSkillTree(isSkillTreeVisible ? visiblePosition : hiddenPosition)); // Slide to the target position
    }

    private IEnumerator SlideSkillTree(Vector2 targetPosition)
    {
        while (Vector2.Distance(skillTreePanel.anchoredPosition, targetPosition) > 0.1f)
        {
            skillTreePanel.anchoredPosition = Vector2.MoveTowards(skillTreePanel.anchoredPosition, targetPosition, slideSpeed * Time.deltaTime);
            yield return null;
        }
        skillTreePanel.anchoredPosition = targetPosition; // Snap to the final position to avoid overshoot
    }
}
