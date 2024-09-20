using UnityEngine;

public class BridgePlacement : MonoBehaviour
{
    public GameObject bridgePrefab;  // Bridge prefab to place
    private GameObject currentBridgePreview;
    private bool isPlacingBridge = false;

    private float bridgeLength = 2f; // Default bridge length
    public float minBridgeLength = 1f;
    public float maxBridgeLength = 4f;
    public float bridgeLengthStep = 0.5f; // Amount to increase/decrease bridge length with each step

    private void Update()
    {
        if (isPlacingBridge)
        {
            FollowCursorForBridge();

            // Adjust bridge length using Q and E keys
            if (Input.GetKeyDown(KeyCode.Q))
            {
                AdjustBridgeLength(-bridgeLengthStep);
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                AdjustBridgeLength(bridgeLengthStep);
            }

            // Place the bridge when left mouse button is clicked
            if (Input.GetMouseButtonDown(0) && currentBridgePreview != null)
            {
                PlaceBridge();
            }

            // Cancel bridge placement
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelBridgePlacement();
            }
        }
    }

    public void StartPlacingBridge(GameObject bridgePrefab)
    {
        isPlacingBridge = true;
        currentBridgePreview = Instantiate(bridgePrefab);
        currentBridgePreview.GetComponent<Collider>().enabled = false;  // Disable the collider to prevent collisions during placement
        SetBridgePreviewOpacity(0.5f);
    }

    private void FollowCursorForBridge()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Set bridge position based on where the cursor is on the ground
            currentBridgePreview.transform.position = hit.point;
        }
    }

    private void AdjustBridgeLength(float adjustment)
    {
        bridgeLength = Mathf.Clamp(bridgeLength + adjustment, minBridgeLength, maxBridgeLength);

        if (currentBridgePreview != null)
        {
            // Adjust the length of the bridge by scaling the Z-axis
            currentBridgePreview.transform.localScale = new Vector3(currentBridgePreview.transform.localScale.x,
                                                                   currentBridgePreview.transform.localScale.y,
                                                                   bridgeLength);
        }
    }

    private void PlaceBridge()
    {
        isPlacingBridge = false;

        // Finalize the placement and enable collider
        currentBridgePreview.GetComponent<Collider>().enabled = true;

        Debug.Log("Bridge placed with length: " + bridgeLength);
    }

    private void CancelBridgePlacement()
    {
        if (currentBridgePreview != null)
        {
            Destroy(currentBridgePreview);
        }
        isPlacingBridge = false;
    }

    private void SetBridgePreviewOpacity(float opacity)
    {
        // This method can set the opacity of the bridge preview for placement purposes
        Renderer[] renderers = currentBridgePreview.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.materials)
            {
                Color color = material.color;
                color.a = opacity;
                material.color = color;

                if (material.shader.name != "Transparent/Diffuse")
                {
                    material.shader = Shader.Find("Transparent/Diffuse");
                }
            }
        }
    }
}
