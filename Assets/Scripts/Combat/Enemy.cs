using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public static readonly List<Enemy> Alive = new();

    [Header("Attack Type / Armor")]
    public DamageType damageType = DamageType.Melee;
    public ArmorType armorType = ArmorType.Light;

    [Header("Separation")]
    public float separationRadius = 1.2f;
    public float separationStrength = 0.8f;


    [Header("Stats")]
    public int maxHP = 50;
    public int currentHP;
    public float moveSpeed = 2f;

    [Header("Against Stronghold")]
    public int damageToStronghold = 10;
    public float strongholdAttackRange = 1f;

    [Header("Against Units")]
    public int damageToUnit = 10;
    public float unitDetectRange = 3f;      
    public float attackRangeVsUnit = 2f;    
    public float attackCooldown = 1.0f;

    Transform _strongholdTarget;
    Unit _currentTargetUnit;
    float _attackCd;

    void OnEnable()
    {
        Alive.Add(this);
    }

    void OnDisable()
    {
        Alive.Remove(this);
    }
    public void Init(Transform strongholdTarget)
    {
        _strongholdTarget = strongholdTarget;
        currentHP = maxHP;
    }

    void Update()
    {
        Unit unitTarget = FindClosestUnit(transform.position, unitDetectRange);

        if (unitTarget != null)
        {
            float distUnit = Vector3.Distance(transform.position, unitTarget.transform.position);

            if (distUnit <= attackRangeVsUnit)
            {
                _attackCd -= Time.deltaTime;
                if (_attackCd <= 0f)
                {
                    int finalDamage = CombatCalculator.CalculateDamage(
                        damageToUnit,
                        damageType,
                        unitTarget.armorType
                    );

                    unitTarget.TakeDamage(finalDamage);
                    _attackCd = attackCooldown;

                }
            }
            else
            {
                MoveWithSeparation(unitTarget.transform.position);
            }

            return;
        }

        if (_strongholdTarget == null) return;

        float d = Vector3.Distance(transform.position, _strongholdTarget.position);

        if (d > strongholdAttackRange)
        {
            MoveWithSeparation(_strongholdTarget.position);
            return;
        }

        _attackCd -= Time.deltaTime;
        if (_attackCd <= 0f)
        {
            Stronghold sh = _strongholdTarget.GetComponentInParent<Stronghold>();

            if (sh != null)
            {
                int finalDamage = CombatCalculator.CalculateDamage(
                    damageToStronghold,
                    damageType,
                    sh.armorType
                );

                sh.TakeDamage(finalDamage);
            }

            _attackCd = attackCooldown;
        }
    }



    void MoveTowards(Vector3 targetPos)
    {
        MoveWithSeparation(targetPos);
    }


    void MoveWithSeparation(Vector3 targetPos)
    {
        Vector3 dirToTarget = (targetPos - transform.position).normalized;

        Vector3 separation = Vector3.zero;
        int neighborCount = 0;

        foreach (var other in Alive)
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
        transform.position += step;
    }


    public void TakeDamage(int dmg, KillSource source)
    {
        currentHP -= dmg;
        if (currentHP <= 0)
        {
            Die(source);
        }
    }

    void Die(KillSource source)
    {
        if (GameDirector.I != null)
        {
            GameDirector.I.OnEnemyKilled(source);
        }
        Destroy(gameObject);
    }


    public static Enemy FindClosest(Vector3 pos, float range)
    {
        Enemy best = null;
        float bestD = float.MaxValue;

        foreach (var e in Alive)
        {
            if (e == null) continue;
            float d = Vector3.Distance(pos, e.transform.position);
            if (d < range && d < bestD)
            {
                bestD = d;
                best = e;
            }
        }

        return best;
    }

    static Unit FindClosestUnit(Vector3 pos, float range)
    {
        Unit best = null;
        float bestD = float.MaxValue;

        foreach (var u in Unit.AliveUnits)
        {
            if (u == null) continue;
            float d = Vector3.Distance(pos, u.transform.position);
            if (d < range && d < bestD)
            {
                bestD = d;
                best = u;
            }
        }

        return best;
    }
}
