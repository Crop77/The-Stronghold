using UnityEngine;
using TMPro;

public class UnitBuildCostLabel : MonoBehaviour
{
    public TMP_Text label;
    public Unit unitPrefab;
    public string prefix = "";

    void Awake()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();

        UpdateCost();
    }

    public void UpdateCost()
    {
        if (label == null || unitPrefab == null) return;

        label.text = $"{prefix}{unitPrefab.cost}";
    }
}
