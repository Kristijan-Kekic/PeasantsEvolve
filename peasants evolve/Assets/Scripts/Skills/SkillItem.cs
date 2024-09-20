using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkillItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int skillIndex;  // Index to reference the skill in SkillManager
    private Popup popup;
    private SkillManager skillManager;
    private Coroutine showPopupCoroutine;
    private Coroutine hidePopupCoroutine;
    private Skill skill;

    private void Start()
    {
        popup = FindObjectOfType<Popup>();  // Reference the popup UI controller
        skillManager = FindObjectOfType<SkillManager>();  // Get the SkillManager attached to the empty GameObject

        if (skillManager != null && skillIndex >= 0 && skillIndex < skillManager.skills.Count)
        {
            skill = skillManager.skills[skillIndex];  // Fetch the corresponding skill by index
        }
        else
        {
            Debug.LogError("SkillManager or skillIndex is invalid");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hidePopupCoroutine != null)
        {
            StopCoroutine(hidePopupCoroutine);
            hidePopupCoroutine = null;
        }
        showPopupCoroutine = StartCoroutine(DelayedShowPopup());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (showPopupCoroutine != null)
        {
            StopCoroutine(showPopupCoroutine);
            showPopupCoroutine = null;
        }
        hidePopupCoroutine = StartCoroutine(DelayedHidePopup());
    }

    private IEnumerator DelayedShowPopup()
    {
        yield return new WaitForSeconds(0.3f);

        if (popup != null && skill != null)
        {
            Vector3 mousePosition = Input.mousePosition;
            popup.ShowPopup(skill, mousePosition);  // Show skill info
        }
    }

    private IEnumerator DelayedHidePopup()
    {
        yield return new WaitForSeconds(0.1f);
        while (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            yield return null;
        }
        popup.HidePopup();
    }
}

