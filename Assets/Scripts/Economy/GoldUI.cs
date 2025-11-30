using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    public GoldManager goldManager;
    public TextMeshProUGUI goldText;

    void Update()
    {
        if (goldManager == null || goldText == null) return;
        goldText.text = $"Gold: {goldManager.CurrentGold}";
    }
}
