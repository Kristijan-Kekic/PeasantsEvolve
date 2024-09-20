using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{

    [SerializeField] GameObject panelPrefab;
    public List<GameObject> unitList;
    public List<GameObject> unitsSelected;

    public List<GameObject> playerUnits; 
    public List<GameObject> playerBuildings;
    public List<GameObject> enemyUnits;
    public List<GameObject> enemyBuildings;

    private static SelectionManager _instance;
    public static SelectionManager Instance { get { return _instance; } }

    private Dictionary<GameObject, GameObject> unitPanels = new Dictionary<GameObject, GameObject>();

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteSelectedUnits();
        }
    }

    public void ClickSelect(GameObject unitToAdd)
    {
        DeselectAll();
        AddUnitSelection(unitToAdd);
        unitsSelected.Add(unitToAdd);
        unitToAdd.GetComponent<UnitMovement>().enabled = true;
    }

    public void ShiftClickSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd))
        {
            unitsSelected.Add(unitToAdd);
            AddUnitSelection(unitToAdd);
            unitToAdd.GetComponent<UnitMovement>().enabled = true;
        }
        else
        {
            unitsSelected.Remove(unitToAdd);
            RemoveUnitSelection(unitToAdd);
        }
    }

    public void BoxSelect(GameObject unitToAdd)
    {
        if (!unitsSelected.Contains(unitToAdd))
        {
            unitsSelected.Add(unitToAdd);
            AddUnitSelection(unitToAdd);
            unitToAdd.GetComponent<UnitMovement>().enabled = true;
        }
    }

    public void DeselectAll()
    {
        foreach (var unit in unitsSelected)
        {
            if (unit != null)
            {
                unit.GetComponent<UnitMovement>().enabled = false;
                RemoveUnitSelection(unit);
            }
        }

        unitsSelected.Clear();

    }

    private void AddUnitSelection(GameObject unit)
    {
        if (!unitPanels.ContainsKey(unit))
        {
            GameObject newPanel = Instantiate(panelPrefab);
            unitPanels[unit] = newPanel;

            newPanel.transform.SetParent(unit.transform);

            // Position the panel relative to the unit
            newPanel.transform.localPosition = Vector3.zero;

            newPanel.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        }

        unitPanels[unit].SetActive(true);
    }

    private void RemoveUnitSelection(GameObject unit)
    {
        if (unitPanels.ContainsKey(unit))
        {
            unitPanels[unit].SetActive(false);
        }
    }

    private void DeleteSelectedUnits()
    {
        List<GameObject> unitsToDelete = new List<GameObject>();

        foreach (var unit in unitsSelected)
        {
            if (unit != null)
            {
                if (unitList.Contains(unit))
                {
                    unitsToDelete.Add(unit);
                }
            }
        }

        foreach (var unit in unitsToDelete)
        {
            PopulationManager.Instance.RemoveUnit(unit);

            unitList.Remove(unit);

            if (playerUnits.Contains(unit))
            {
                playerUnits.Remove(unit);
            }

            RemoveUnitSelection(unit);

            Destroy(unit);
        }

        unitsSelected.Clear();
    }

    public void CommandAttack(GameObject target)
    {
        foreach (GameObject unit in unitsSelected)
        {
            UnitStats unitStats = unit.GetComponent<UnitStats>();
            if (unitStats != null)
            {
                UnitStateMachine unitStateMachine = unit.GetComponent<UnitStateMachine>();
                if (unitStateMachine != null)
                {
                    unitStateMachine.SetTarget(target);
                }
            }
        }
    }

}
