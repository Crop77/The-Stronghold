using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public GoldManager goldManager;

    [Header("Melee")]
    public GameObject meleePrefab;
    public int meleeCost = 50;

    [Header("Archer")]
    public GameObject archerPrefab;
    public int archerCost = 60;

    [Header("Tank")]
    public GameObject tankPrefab;
    public int tankCost = 80;

    [Header("Siege")]
    public GameObject siegePrefab;
    public int siegeCost = 100;

    GameObject _selectedPrefab;
    int _selectedCost;

    public void SelectMelee()
    {
        _selectedPrefab = meleePrefab;
        _selectedCost = meleeCost;
        Debug.Log("[BuildManager] Selected Melee Unit");
    }

    public void SelectArcher()
    {
        _selectedPrefab = archerPrefab;
        _selectedCost = archerCost;
        Debug.Log("[BuildManager] Selected Archer Unit");
    }

    public void SelectTank()
    {
        _selectedPrefab = tankPrefab;
        _selectedCost = tankCost;
        Debug.Log("[BuildManager] Selected Tank Unit");
    }

    public void SelectSiege()
    {
        _selectedPrefab = siegePrefab;
        _selectedCost = siegeCost;
        Debug.Log("[BuildManager] Selected Siege Unit");
    }

    public void TryBuildAt(PlacementSpot spot)
    {
        if (_selectedPrefab == null)
        {
            Debug.Log("[BuildManager] No unit selected.");
            return;
        }

        if (!goldManager.Spend(_selectedCost))
        {
            Debug.Log("[BuildManager] Not enough gold to build this unit.");
            return;
        }

        spot.PlaceUnit(_selectedPrefab);
    }
}

