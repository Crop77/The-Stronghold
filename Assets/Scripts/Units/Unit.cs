using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour

{
    public enum UnitKind
    {
        Melee,
        Archer,
        Tank,
        Siege
    }

    [Header("Unit Info")]
    public UnitKind unitKind = UnitKind.Melee;

    [Header("Separation")]
    public float separationRadius = 1.2f;    
    public float separationStrength = 0.8f;

    private float minX = -3.89f;
    private float maxX = 6.11f;
    private float minZ = -7.17f;
    private float maxZ = 2.83f;


    public static readonly List<Unit> AliveUnits = new();

    [Header("General Stats")]
    public int cost = 50;
    public int maxHP = 100;
    public int currentHP;

    [Header("Attack")]
    public float attackRange = 3f;     
    public int attackDamage = 10;
    public float attackCooldown = 1.0f;

    [Header("Types")]
    public DamageType damageType = DamageType.Melee;
    public ArmorType armorType = ArmorType.Light;


    [Header("Movement")]
    public bool isMelee = true;         
    public float moveSpeed = 2f;
    public float desiredFightDistance = 1.5f; 

    float _cd;

    void OnEnable()
    {
        {
            AliveUnits.Add(this);

            int baseHP = maxHP;
            int baseDamage = attackDamage;

            float hpMult = 1f;
            float dmgMult = 1f;

            if (GameDirector.I != null)
            {
                hpMult = GameDirector.I.GetUnitHpMultiplier(unitKind);
                dmgMult = GameDirector.I.GetUnitDamageMultiplier(unitKind);
            }

            maxHP = Mathf.RoundToInt(baseHP * hpMult);
            currentHP = maxHP;

            attackDamage = Mathf.RoundToInt(baseDamage * dmgMult);
        }

    }

    void OnDisable()
    {
        AliveUnits.Remove(this);
    }

    void Update()
    {
        Enemy target = Enemy.FindClosest(transform.position, 999f);
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.transform.position);

        if (isMelee)
        {
            if (dist > desiredFightDistance)
            {
                Vector3 dirToTarget = (target.transform.position - transform.position).normalized;

                Vector3 separation = Vector3.zero;
                int neighborCount = 0;

                foreach (var other in AliveUnits)
                {
                    if (other == null || other == this) continue;

                    Vector3 offset = transform.position - other.transform.position;
                    float sqrDist = offset.sqrMagnitude;
                    if (sqrDist < separationRadius * separationRadius && sqrDist > 0.001f)
                    {
                        separation += offset.normalized / Mathf.Max(sqrDist, 0.01f);
                        neighborCount++;
                    }
                }

                if (neighborCount > 0)
                    separation /= neighborCount;

                Vector3 moveDir = dirToTarget + separation * separationStrength;
                moveDir.y = 0f;   

                if (moveDir.sqrMagnitude > 0.001f)
                    moveDir.Normalize();

                Vector3 step = moveDir * moveSpeed * Time.deltaTime;

                Vector3 newPos = transform.position + step;

                newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
                newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

                transform.position = newPos;


            }
        }


        _cd -= Time.deltaTime;
        if (dist <= attackRange && _cd <= 0f)
        {
            int finalDamage = CombatCalculator.CalculateDamage(
                attackDamage,
                damageType,
                target.armorType
            );

            target.TakeDamage(finalDamage, KillSource.Unit);
            _cd = attackCooldown;
        }

    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }
}

