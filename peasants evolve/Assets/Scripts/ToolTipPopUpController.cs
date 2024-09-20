using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipPopupController : MonoBehaviour
{
    public GameObject tooltipPanel;  // The UI panel for the tooltip
    public TMP_Text descriptionText;  // Text field for description
    public TMP_Text additionalInfoText;  // Text field for cost or effect info

    private RectTransform tooltipRect;

    private void Start()
    {
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        HideTooltip();
    }

    // Show the tooltip at the mouse position with relevant text
    public void ShowTooltip(string description, string additionalInfo, Vector3 position)
    {
        descriptionText.text = description;
        additionalInfoText.text = additionalInfo;

        tooltipPanel.SetActive(true);
        SetTooltipPosition(position);
    }

    // Hide the tooltip
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    // Set the tooltip position near the mouse
    private void SetTooltipPosition(Vector3 mousePosition)
    {
        tooltipRect.position = mousePosition;
        // Adjust tooltip position if it's off-screen
        Vector3[] corners = new Vector3[4];
        tooltipRect.GetWorldCorners(corners);
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);

        // If tooltip goes off-screen, adjust its position
        if (!screenRect.Contains(corners[2]))  // bottom-right corner
        {
            tooltipRect.pivot = new Vector2(1, 0);  // adjust pivot to avoid going off screen
        }
    }
}
