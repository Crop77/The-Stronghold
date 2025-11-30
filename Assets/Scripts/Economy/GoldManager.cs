using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public int startGold = 200;
    public int CurrentGold { get; private set; }

    void Start()
    {
        CurrentGold = startGold;
    }

    public bool Spend(int amount)
    {
        if (CurrentGold < amount) return false;
        CurrentGold -= amount;
        return true;
    }

    public void Add(int amount)
    {
        CurrentGold += amount;
    }

    public void ResetGold()
    {
        CurrentGold = startGold;
    }
}
