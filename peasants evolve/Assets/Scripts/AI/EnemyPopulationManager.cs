using UnityEngine;
using TMPro;

public class EnemyPopulationManager : MonoBehaviour
{
    public int maxPopulation = 0;
    private int currentPopulation = 0;

    public TextMeshProUGUI populationText; // Reference to the TextMeshProUGUI to display the population

    private static EnemyPopulationManager _instance;
    public static EnemyPopulationManager Instance { get { return _instance; } }

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

    }

    public void AddUnit(GameObject unit, bool isStartUnit)
    {
        // If the unit is a starting unit, bypass population cap check
        if (isStartUnit || currentPopulation < maxPopulation)
        {
            currentPopulation++;
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
    }

    public bool CanAddUnit()
    {
        return currentPopulation < maxPopulation;
    }

    public void AddBuildingPopulation(int additionalPopulation)
    {
        maxPopulation += additionalPopulation;
    }
}
