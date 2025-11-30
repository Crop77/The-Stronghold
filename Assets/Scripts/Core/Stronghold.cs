using UnityEngine;

public class Stronghold : MonoBehaviour
{
    [Header("Stats")]
    public int maxHP = 200;
    public int currentHP;

    [Header("Attack")]
    public int attackDamage = 15;
    public float attackRange = 5f; 
    public float attackCooldown = 1.0f;

    [Header("Types")]
    public DamageType damageType = DamageType.Siege;
    public ArmorType armorType = ArmorType.Fortified;

    float _cd;

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        _cd -= Time.deltaTime;
        if (_cd > 0f) return;

        Enemy target = Enemy.FindClosest(transform.position, attackRange);
        if (target == null) return;

        int finalDamage = CombatCalculator.CalculateDamage(
            attackDamage,
            damageType,
            target.armorType
        );

        target.TakeDamage(finalDamage, KillSource.Stronghold);

        _cd = attackCooldown;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            if (GameDirector.I != null)
                GameDirector.I.OnStrongholdFallen();
        }
    }
}
