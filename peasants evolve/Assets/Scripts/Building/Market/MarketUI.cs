using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class MarketUI : MonoBehaviour
{
    public GameObject marketPanel;  // The panel to enable/disable
    public TMP_Dropdown sellDropdown;  // Dropdown for selecting the resource to sell
    public TMP_Dropdown buyDropdown;   // Dropdown for selecting the resource to buy
    public TMP_InputField sellAmountInput;  // Input field for the amount to sell
    public TMP_Text buyAmountText;  // Text field for the amount to buy (auto-calculated)
    public Button tradeButton;
    public TextMeshProUGUI buildingHpText;

    private GameObject currentBuilding;
    public GameObject selectionIndicatorPrefab;
    private GameObject currentIndicator;

    private ResourceManager resourceManager;  // Reference to ResourceManager to get current resources

    // Define exchange rates between resources (sell -> buy)
    private Dictionary<string, Dictionary<string, float>> exchangeRates;

    private void Start()
    {
        resourceManager = FindObjectOfType<ResourceManager>();

        PopulateDropdowns();
        InitializeExchangeRates();

        tradeButton.onClick.AddListener(OnTradeButtonClicked);

        sellAmountInput.onValueChanged.AddListener(OnSellAmountChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.B))
        {
            HideMarketPanel();
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.gameObject == currentBuilding)
                {
                    return;
                }
                else
                {
                    HideMarketPanel();
                }
            }
            else
            {
                HideMarketPanel();
            }
        }

        if (currentBuilding != null)
        {
            UpdateBuildingHpDisplay();
        }
    }

    private void InitializeExchangeRates()
    {
        exchangeRates = new Dictionary<string, Dictionary<string, float>>()
    {
        { "Wood", new Dictionary<string, float>
            {
                { "Stone", 0.5f },  // 1 Wood -> 0.5 Stone
                { "Gold", 0.2f },   // 1 Wood -> 0.2 Gold
                { "Coal", 0.4f },   // 1 Wood -> 0.4 Coal
                { "Metal", 0.4f },  // 1 Wood -> 0.4 Metal
                { "Money", 0.5f },  // 1 Wood -> 0.5 Money
                { "Food", 0.8f }    // 1 Wood -> 0.8 Food
            }
        },
        { "Stone", new Dictionary<string, float>
            {
                { "Wood", 2f },     // 1 Stone -> 2 Wood (inverse of 0.5 Wood -> 1 Stone)
                { "Gold", 0.4f },   // 1 Stone -> 0.4 Gold (derived from 1 Wood -> 0.2 Gold)
                { "Coal", 0.8f },   // 1 Stone -> 0.8 Coal (derived from 1 Wood -> 0.4 Coal)
                { "Metal", 0.8f },  // 1 Stone -> 0.8 Metal
                { "Money", 1f },    // 1 Stone -> 1 Money
                { "Food", 1.6f }    // 1 Stone -> 1.6 Food (inverse of 1 Wood -> 0.8 Food)
            }
        },
        { "Gold", new Dictionary<string, float>
            {
                { "Wood", 5f },     // 1 Gold -> 5 Wood
                { "Stone", 2.5f },  // 1 Gold -> 2.5 Stone (derived from inverse of 0.4 Stone -> Gold)
                { "Coal", 2f },     // 1 Gold -> 2 Coal
                { "Metal", 2.25f }, // 1 Gold -> 2.25 Metal
                { "Money", 1.25f }, // 1 Gold -> 1.25 Money
                { "Food", 4f }      // 1 Gold -> 4 Food (inverse of 0.25 Wood -> Food)
            }
        },
        { "Coal", new Dictionary<string, float>
            {
                { "Wood", 2.5f },   // 1 Coal -> 2.5 Wood
                { "Stone", 1.25f }, // 1 Coal -> 1.25 Stone
                { "Gold", 0.5f },   // 1 Coal -> 0.5 Gold
                { "Metal", 1f },    // 1 Coal -> 1 Metal
                { "Money", 1.25f }, // 1 Coal -> 1.25 Money
                { "Food", 2f }      // 1 Coal -> 2 Food
            }
        },
        { "Metal", new Dictionary<string, float>
            {
                { "Wood", 2.5f },   // 1 Metal -> 2.5 Wood
                { "Stone", 1.25f }, // 1 Metal -> 1.25 Stone
                { "Gold", 0.45f },  // 1 Metal -> 0.45 Gold
                { "Coal", 1f },     // 1 Metal -> 1 Coal
                { "Money", 1.2f },  // 1 Metal -> 1.2 Money
                { "Food", 2.2f }    // 1 Metal -> 2.2 Food
            }
        },
        { "Money", new Dictionary<string, float>
            {
                { "Wood", 2f },     // 1 Money -> 2 Wood
                { "Stone", 1f },    // 1 Money -> 1 Stone
                { "Gold", 0.8f },   // 1 Money -> 0.8 Gold
                { "Coal", 0.8f },   // 1 Money -> 0.8 Coal
                { "Metal", 0.9f },  // 1 Money -> 0.9 Metal
                { "Food", 1.6f }    // 1 Money -> 1.6 Food
            }
        },
        { "Food", new Dictionary<string, float>
            {
                { "Wood", 1.25f },  // 1 Food -> 1.25 Wood
                { "Stone", 0.625f },// 1 Food -> 0.625 Stone
                { "Gold", 0.25f },  // 1 Food -> 0.25 Gold
                { "Coal", 0.5f },   // 1 Food -> 0.5 Coal
                { "Metal", 0.5f },  // 1 Food -> 0.5 Metal
                { "Money", 0.625f } // 1 Food -> 0.625 Money
            }
        }
    };
    }


    // Opens the market panel
    public void ShowMarketPanel(GameObject building)
    {
        BuildingProgress buildingProgress = building.GetComponent<BuildingProgress>();

        if (buildingProgress != null && !buildingProgress.IsCompleted())
        {
            Debug.Log("Building is not fully constructed yet.");
            return;
        }

        marketPanel.SetActive(true);
        currentBuilding = building;

        Collider buildingCollider = currentBuilding.GetComponent<Collider>();

        // Instantiate the selection indicator and adjust its size and position
        if (currentIndicator == null && selectionIndicatorPrefab != null && buildingCollider != null)
        {
            // Instantiate the selection indicator as a child of the current building
            currentIndicator = Instantiate(selectionIndicatorPrefab, currentBuilding.transform);

            // Set the size of the indicator to match the building's collider bounds
            Vector3 colliderSize = buildingCollider.bounds.size;
            currentIndicator.transform.localScale = new Vector3(colliderSize.x / 1000, 1, colliderSize.z / 1000);

            // Position the indicator at the bottom of the building's collider and center it
            Vector3 colliderBottomCenter = buildingCollider.bounds.center;
            colliderBottomCenter.y = buildingCollider.bounds.min.y + 0.01f; // Slightly above the ground to avoid z-fighting

            currentIndicator.transform.position = colliderBottomCenter;
            currentIndicator.transform.rotation = currentBuilding.transform.rotation;

            currentIndicator.transform.rotation *= Quaternion.Euler(90, 0, 0);

            Debug.Log("Selection indicator instantiated and adjusted for Unit Production.");
        }
    }

    // Hides the market panel
    public void HideMarketPanel()
    {
        marketPanel.SetActive(false);

        if (currentIndicator != null)
        {
            Destroy(currentIndicator);
            currentIndicator = null;
            Debug.Log("Selection indicator destroyed for Market.");
        }
    }

    // Populate the dropdown options for resources
    private void PopulateDropdowns()
    {
        string[] resourceOptions = { "Wood", "Stone", "Gold", "Coal", "Metal", "Money" };

        sellDropdown.ClearOptions();
        buyDropdown.ClearOptions();

        sellDropdown.AddOptions(new List<string>(resourceOptions));
        buyDropdown.AddOptions(new List<string>(resourceOptions));
    }

    // Called when the trade button is clicked
    private void OnTradeButtonClicked()
    {
        // Get selected sell and buy resources from the dropdowns
        string sellResource = sellDropdown.options[sellDropdown.value].text;
        string buyResource = buyDropdown.options[buyDropdown.value].text;

        // Parse the sell amount input field
        int sellAmount = int.Parse(sellAmountInput.text);
        int buyAmount = int.Parse(buyAmountText.text);  // This is auto-calculated

        // Perform the trade if possible
        if (PerformTrade(sellResource, buyResource, sellAmount, buyAmount))
        {
            Debug.Log($"Traded {sellAmount} {sellResource} for {buyAmount} {buyResource}");
        }
        else
        {
            Debug.LogError("Trade failed. Check resource availability.");
        }
    }

    private bool PerformTrade(string sellResource, string buyResource, int sellAmount, int buyAmount)
    {
        if (HasEnoughResources(sellResource, sellAmount))
        {
            DeductResource(sellResource, sellAmount);
            AddResource(buyResource, buyAmount);
        }
        return false;
    }

    // Check if the player has enough resources to trade
    private bool HasEnoughResources(string resource, int amount)
    {
        switch (resource)
        {
            case "Wood": return resourceManager.GetWoodAmount() >= amount;
            case "Stone": return resourceManager.GetStoneAmount() >= amount;
            case "Gold": return resourceManager.GetGoldAmount() >= amount;
            case "Coal": return resourceManager.GetCoalAmount() >= amount;
            case "Metal": return resourceManager.GetMetalAmount() >= amount;
            case "Money": return resourceManager.GetMoneyAmount() >= amount;
            default: return false;
        }
    }

    // Deduct resources from the player's inventory
    private void DeductResource(string resource, int amount)
    {
        switch (resource)
        {
            case "Wood": resourceManager.DeductWood(amount); break;
            case "Stone": resourceManager.DeductStone(amount); break;
            case "Gold": resourceManager.DeductGold(amount); break;
            case "Coal": resourceManager.DeductCoal(amount); break;
            case "Metal": resourceManager.DeductMetal(amount); break;
            case "Money": resourceManager.DeductMoney(amount); break;
        }
    }

    // Add resources to the player's inventory
    private void AddResource(string resource, int amount)
    {
        switch (resource)
        {
            case "Wood": resourceManager.AddWood(amount, false); break;
            case "Stone": resourceManager.AddStone(amount, false); break;
            case "Gold": resourceManager.AddGold(amount, false); break;
            case "Coal": resourceManager.AddCoal(amount, false); break;
            case "Metal": resourceManager.AddMetal(amount, false); break;
            case "Money": resourceManager.AddMoney(amount, false); break;
        }
    }

    // Update buy amount when the sell amount changes
    private void OnSellAmountChanged(string sellAmountStr)
    {
        // Get the selected resources
        string sellResource = sellDropdown.options[sellDropdown.value].text;
        string buyResource = buyDropdown.options[buyDropdown.value].text;

        // Parse sell amount input
        if (int.TryParse(sellAmountStr, out int sellAmount) && sellAmount > 0)
        {
            // Calculate the buy amount based on the exchange rate
            float exchangeRate = GetExchangeRate(sellResource, buyResource);
            int buyAmount = Mathf.FloorToInt(sellAmount * exchangeRate);

            // Update the buy amount text
            buyAmountText.text = buyAmount.ToString();
        }
        else
        {
            buyAmountText.text = "0";  // Reset if input is invalid
        }
    }

    // Get exchange rate between two resources
    private float GetExchangeRate(string sellResource, string buyResource)
    {
        if (exchangeRates.ContainsKey(sellResource) && exchangeRates[sellResource].ContainsKey(buyResource))
        {
            return exchangeRates[sellResource][buyResource];
        }

        // Default exchange rate if not found
        return 1.0f;
    }

    private void UpdateBuildingHpDisplay()
    {
        // Get the BuildingProgress component from the selected building
        BuildingProgress buildingProgress = currentBuilding.GetComponent<BuildingProgress>();

        if (buildingProgress != null)
        {
            // Update the text to show current HP / total HP
            buildingHpText.text = $"Building HP: {buildingProgress.currentBuildPoints}/{buildingProgress.totalBuildPoints}";
        }
        else
        {
            // If no BuildingProgress component found, show default or hide the text
            buildingHpText.text = "Building HP: N/A";
        }
    }
}
