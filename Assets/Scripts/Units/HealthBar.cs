using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Enemy enemy;
    public Unit unit;
    public Stronghold stronghold;   

    Image _fill;

    void Awake()
    {
        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        if (enemy == null)
            enemy = GetComponentInParent<Enemy>();

        if (unit == null)
            unit = GetComponentInParent<Unit>();

        if (stronghold == null)
            stronghold = GetComponentInParent<Stronghold>();  

        if (slider != null && slider.fillRect != null)
            _fill = slider.fillRect.GetComponent<Image>();
    }

    void Update()
    {
        if (slider == null) return;

        float ratio = 1f;

        if (enemy != null)
        {
            if (enemy.maxHP > 0)
                ratio = Mathf.Clamp01((float)enemy.currentHP / enemy.maxHP);
        }
        else if (unit != null)
        {
            if (unit.maxHP > 0)
                ratio = Mathf.Clamp01((float)unit.currentHP / unit.maxHP);
        }
        else if (stronghold != null)   
        {
            if (stronghold.maxHP > 0)
                ratio = Mathf.Clamp01((float)stronghold.currentHP / stronghold.maxHP);
        }

        slider.value = ratio;

        if (_fill != null)
        {
            Color c;
            if (ratio > 0.5f)
            {
                float t = (ratio - 0.5f) / 0.5f;
                c = Color.Lerp(Color.yellow, Color.green, t);
            }
            else
            {
                float t = ratio / 0.5f;
                c = Color.Lerp(Color.red, Color.yellow, t);
            }
            _fill.color = c;
        }    
    }
}
