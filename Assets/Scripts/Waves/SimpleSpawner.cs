using System.Collections;
using UnityEngine;

public class SimpleWaveSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform target;
    public GameObject enemyPrefab;

    [Header("Wave Settings")]
    public int count = 10;
    public float interval = 0.5f;

    [Header("Stat Multipliers (set by GameDirector)")]
    public float hpMultiplier = 1f;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;


    public bool IsSpawning { get; private set; }

    public void SpawnWave()
    {
        if (IsSpawning) return;

        if (enemyPrefab == null || spawnPoint == null || target == null)
        {
            Debug.LogError("[SimpleWaveSpawner] Missing references.");
            return;
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        IsSpawning = true;
        Debug.Log($"[SimpleWaveSpawner] Spawning {count} enemies...");

        for (int i = 0; i < count; i++)
        {
            GameObject go = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            var e = go.GetComponent<Enemy>();
            if (e != null)
            {
                e.Init(target);

                if (hpMultiplier != 1f && e.maxHP > 0)
                {
                    e.maxHP = Mathf.RoundToInt(e.maxHP * hpMultiplier);
                    e.currentHP = e.maxHP;
                }

                if (damageMultiplier != 1f)
                {
                    e.damageToUnit = Mathf.RoundToInt(e.damageToUnit * damageMultiplier);
                    e.damageToStronghold = Mathf.RoundToInt(e.damageToStronghold * damageMultiplier);
                }

                if (speedMultiplier != 1f)
                {
                    e.moveSpeed *= speedMultiplier;
                }
            }
            else
            {
                Debug.LogError("[SimpleWaveSpawner] Enemy prefab has no Enemy component.");
            }

            yield return new WaitForSeconds(interval);
        }

        IsSpawning = false;
    }

}
