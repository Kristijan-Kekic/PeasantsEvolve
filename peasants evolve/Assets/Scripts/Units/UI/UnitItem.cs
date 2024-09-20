using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UnitItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UnitStats unitStats;  // The data for this specific unit
    private Popup popup;
    private Coroutine showPopupCoroutine;
    private Coroutine hidePopupCoroutine;

    private void Start()
    {
        popup = FindObjectOfType<Popup>();  // Reference the popup UI controller
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Stop any ongoing hide coroutine to avoid premature hiding
        if (hidePopupCoroutine != null)
        {
            StopCoroutine(hidePopupCoroutine);
            hidePopupCoroutine = null;
        }

        // Start the show popup coroutine
        showPopupCoroutine = StartCoroutine(DelayedShowPopup());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Stop the show coroutine if it's still running
        if (showPopupCoroutine != null)
        {
            StopCoroutine(showPopupCoroutine);
            showPopupCoroutine = null;
        }

        // Start the hide popup coroutine with a slight delay
        hidePopupCoroutine = StartCoroutine(DelayedHidePopup());
    }

    private IEnumerator DelayedShowPopup()
    {
        yield return new WaitForSeconds(0.3f);  // Delay before showing the popup

        if (popup != null && unitStats != null)
        {
            Vector3 mousePosition = Input.mousePosition;  // Get mouse position
            popup.ShowPopup(unitStats, mousePosition);  // Show the popup with unit information
        }
    }

    private IEnumerator DelayedHidePopup()
    {
        yield return new WaitForSeconds(0.1f);  // Small delay before hiding the popup

        // Manually check if the mouse is still over the icon before hiding
        while (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
        {
            yield return null;  // Keep waiting while the mouse is still over the icon
        }

        // Hide the popup once the mouse is no longer over the icon
        if (popup != null)
        {
            popup.HidePopup();
        }
    }
}
