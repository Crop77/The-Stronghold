using TMPro;
using UnityEngine;

public class UpgradeCostLabel : MonoBehaviour
{
    public TMP_Text label;
    public string prefix = "";

    public enum UpgradeType { Melee, Archer }
    public UpgradeType type;

    void Start()
    {
        if (label == null)
            label = GetComponentInChildren<TMP_Text>();
        Refresh();
    }

    void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (label == null || GameDirector.I == null)
            return;

        int cost = 0;

        switch (type)
        {
            case UpgradeType.Melee:
                cost = GameDirector.I.meleeUpgradeBaseCost + GameDirector.I.meleeUpgradeLevel * 25;
                break;
            case UpgradeType.Archer:
                cost = GameDirector.I.archerUpgradeBaseCost + GameDirector.I.archerUpgradeLevel * 30;
                break;
        }

        label.text = $"{prefix}{cost}";
    }
}

