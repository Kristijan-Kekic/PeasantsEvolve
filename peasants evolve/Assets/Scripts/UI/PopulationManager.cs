using UnityEngine;
using TMPro;

public class PopulationManager : MonoBehaviour
{
    public int maxPopulation = 0;
    private int currentPopulation = 0;

    public TextMeshProUGUI populationText; // Reference to the TextMeshProUGUI to display the population

    private static PopulationManager _instance;
    public static PopulationManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        UpdatePopulationUI();
    }

    public void AddUnit(GameObject unit, bool isStartUnit)
    {
        // If the unit is a starting unit, bypass population cap check
        if (isStartUnit || currentPopulation < maxPopulation)
        {
            currentPopulation++;
            UpdatePopulationUI();
        }
        else
        {
            Debug.Log("Cannot add more units, population cap reached!");
            // You can implement additional logic here to disable the ability to produce more units
        }
    }

    public void RemoveUnit(GameObject unit)
    {
        currentPopulation = Mathf.Max(0, currentPopulation - 1);
        UpdatePopulationUI();
    }

    private void UpdatePopulationUI()
    {
        populationText.text = $"Population: {currentPopulation}/{maxPopulation}";
    }

    public bool CanAddUnit()
    {
        return currentPopulation < maxPopulation;
    }

    public void AddBuildingPopulation(int additionalPopulation)
    {
        maxPopulation += additionalPopulation;
        UpdatePopulationUI();
    }
}
