using UnityEngine;

public class SelectableUnit : MonoBehaviour
{
    private void Start()
    {
        SelectionManager.Instance.unitList.Add(this.gameObject);
    }

    private void OnDestroy()
    {
        if(this.gameObject != null)
            SelectionManager.Instance.unitList.Remove(this.gameObject);
    }
}
