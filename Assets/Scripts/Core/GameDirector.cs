using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unit;

public class GameDirector : MonoBehaviour
{
    public static GameDirector I { get; private set; }

    [Header("Refs")]
    public SimpleWaveSpawner spawner;
    public GoldManager goldManager;
    public Stronghold stronghold;

    [Header("Upgrade UI")]
    public UpgradeCostLabel meleeUpgradeLabel;
    public UpgradeCostLabel archerUpgradeLabel;


    [System.Serializable]
    public class WaveConfig
    {
        public string waveName = "Wave";
        public GameObject enemyPrefab;
        public int enemyCount = 10;
        public float spawnInterval = 0.5f;
        public bool isBoss = false;

        [Header("Stat Multipliers (optional)")]
        public float hpMultiplier = 1f;
        public float damageMultiplier = 1f;
        public float speedMultiplier = 1f;
    }

    [Header("UI")]
    public TMPro.TMP_Text waveInfoText;

    [Header("UI Panels")]
    public GameObject startMenuPanel;
    public GameObject gameplayUIPanel;
    public GameObject victoryPanel;
    public GameObject pauseMenuPanel;
    public GameObject defeatPanel;

    bool isPaused = false;

    [Header("Waves Config")]
    public WaveConfig[] waves;

    [Header("Unit Upgrades")]
    public float meleeDamageMultiplier = 1f;
    public float meleeHpMultiplier = 1f;

    public float archerDamageMultiplier = 1f;
    public float archerHpMultiplier = 1f;


    [Header("Unit Upgrades - Levels & Costs")]
    public int meleeUpgradeLevel = 0;
    public int meleeUpgradeBaseCost = 50;

    public int archerUpgradeLevel = 0;
    public int archerUpgradeBaseCost = 60;


    [Header("Waves")]
    public int maxWaves = 10;
    public int currentWave = 0;
    public bool waveInProgress = false;

    [Header("Gold Rewards")]
    public int baseWaveReward = 100;    
    public int waveRewardStep = 20;       
    public int killGold = 10;             
    public float strongholdKillMultiplier = 0.5f; 

    int _unitKillsThisWave;
    int _strongholdKillsThisWave;

    public void StartGame()
    {
        Time.timeScale = 1f;

        if (startMenuPanel != null)
            startMenuPanel.SetActive(false);

        if (gameplayUIPanel != null)
            gameplayUIPanel.SetActive(true);

        if (AudioManager.I != null)
            AudioManager.I.PlayBuildPhase();

        Debug.Log("[GameDirector] Game started.");
    }



    public float GetUnitDamageMultiplier(UnitKind kind)
    {
        switch (kind)
        {
            case UnitKind.Melee: return meleeDamageMultiplier;
            case UnitKind.Archer: return archerDamageMultiplier;
            default: return 1f;
        }
    }

    public float GetUnitHpMultiplier(UnitKind kind)
    {
        switch (kind)
        {
            case UnitKind.Melee: return meleeHpMultiplier;
            case UnitKind.Archer: return archerHpMultiplier;
            default: return 1f;
        }
    }


    void Awake()
    {
        
            UpdateNextWaveInfo();

        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
    }

    void Start()
    {
        {
            Time.timeScale = 0f;

            if (gameplayUIPanel != null) gameplayUIPanel.SetActive(false);
            if (victoryPanel != null) victoryPanel.SetActive(false);
        }

        if (AudioManager.I != null)
        {
            AudioManager.I.PlayMenuMusic();
        }
    }


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (startMenuPanel != null && startMenuPanel.activeSelf) { }
            else if (victoryPanel != null && victoryPanel.activeSelf) { }
            else
            {
                TogglePauseMenu();
            }
        }

        if (waveInProgress && !spawner.IsSpawning && Enemy.Alive.Count == 0)
        {
            EndCurrentWave();
        }
    }

    public void QuitGame()
    {
        Debug.Log("[GameDirector] QuitGame called.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void TogglePauseMenu()
    {
        if (pauseMenuPanel == null)
            return;

        isPaused = !isPaused;

        pauseMenuPanel.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;

            if (AudioManager.I != null)
                AudioManager.I.PlayMenuMusic();
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;

            if (AudioManager.I != null)
            {
                if (currentWave >= waves.Length)
                {
                    return;
                }

                if (waveInProgress)
                {
                    var cfg = waves[currentWave - 1];
                    if (cfg.isBoss)
                        AudioManager.I.PlayBossTheme();
                    else
                        AudioManager.I.PlayCombatPhase();
                }
                else
                {
                    AudioManager.I.PlayBuildPhase();
                }
            }
        }
    }

    public void UpgradeMeleeDamage()
    {
        int cost = meleeUpgradeBaseCost + meleeUpgradeLevel * 25;

        if (!goldManager.Spend(cost))
            return;

        meleeUpgradeLevel++;
        meleeDamageMultiplier += 0.25f;

        meleeUpgradeLabel.Refresh();
    }


    public void UpgradeArcherDamage()
    {
        int cost = archerUpgradeBaseCost + archerUpgradeLevel * 25;

        if (!goldManager.Spend(cost))
            return;

        archerUpgradeLevel++;
        archerDamageMultiplier += 0.25f;

        archerUpgradeLabel.Refresh();
    }



    void UpdateNextWaveInfo()
    {
        if (waveInfoText == null)
            return;

        if (waves == null || waves.Length == 0)
        {
            waveInfoText.text = "No waves configured.";
            return;
        }

        int nextIndex = currentWave; 

        if (nextIndex >= waves.Length)
        {
            waveInfoText.text = $"All {waves.Length} waves completed.";
            return;
        }

        WaveConfig cfg = waves[nextIndex];

        string waveLabel = $"Wave {nextIndex + 1}: {cfg.waveName}";

        string enemyTypeInfo = "";
        if (cfg.enemyPrefab != null)
        {
            var enemy = cfg.enemyPrefab.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemyTypeInfo = $"{enemy.damageType} damage, {enemy.armorType} armor";
            }
            else
            {
                enemyTypeInfo = "Enemy stats unavailable";
            }
        }
        else
        {
            enemyTypeInfo = "No enemy prefab assigned";
        }

        int roundReward = baseWaveReward + nextIndex * waveRewardStep;

        int unitKillGold = killGold;
        int strongholdKillGold = Mathf.RoundToInt(killGold * strongholdKillMultiplier);

        string rewardInfo =
            $"Reward: {roundReward} for round, {unitKillGold} per kill " +
            $"({strongholdKillGold} if Stronghold gets the kill).";

        waveInfoText.text = $"{waveLabel}\n{enemyTypeInfo}\n{rewardInfo}";
    }


    void ResetAllUnits()
    {
        foreach (var spot in PlacementSpot.AllSpots)
        {
            if (spot == null) continue;
            spot.ResetUnitInstance();
        }
    }

    public void StartNextWave()
    {
        if (waveInProgress)
        {
            Debug.Log("[GameDirector] Wave already in progress.");
            return;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogError("[GameDirector] No waves configured!");
            return;
        }

        if (currentWave >= waves.Length)
        {
            Debug.Log("[GameDirector] All waves completed (no more configs).");
            return;
        }

        currentWave++;
        int waveIndex = currentWave - 1;
        WaveConfig cfg = waves[waveIndex];

        if (cfg.enemyPrefab == null)
        {
            Debug.LogError($"[GameDirector] Wave {currentWave} has no enemy prefab assigned!");
            return;
        }

        spawner.enemyPrefab = cfg.enemyPrefab;
        spawner.count = cfg.enemyCount;
        spawner.interval = cfg.spawnInterval;

        spawner.hpMultiplier = cfg.hpMultiplier;
        spawner.damageMultiplier = cfg.damageMultiplier;
        spawner.speedMultiplier = cfg.speedMultiplier;

        waveInProgress = true;
        _unitKillsThisWave = 0;
        _strongholdKillsThisWave = 0;

        Debug.Log($"[GameDirector] Starting Wave {currentWave}: {cfg.waveName} " +
                  $"(Enemy={cfg.enemyPrefab.name}, Count={cfg.enemyCount})");

        if (AudioManager.I != null)
        {
            if (cfg.isBoss)
                AudioManager.I.PlayBossTheme();
            else
                AudioManager.I.PlayCombatPhase();
        }

        spawner.SpawnWave();
    }


    void EndCurrentWave()
    {
        waveInProgress = false;

        int waveReward = baseWaveReward + (currentWave - 1) * waveRewardStep;
        goldManager.Add(waveReward);

        int unitGold = _unitKillsThisWave * killGold;
        int strongholdGold = Mathf.RoundToInt(_strongholdKillsThisWave * killGold * strongholdKillMultiplier);

        Debug.Log(
            $"[GameDirector] Wave {currentWave} ended. " +
            $"Base: {waveReward}, " +
            $"Unit kills: {_unitKillsThisWave} (+{unitGold}), " +
            $"Stronghold kills: {_strongholdKillsThisWave} (+{strongholdGold}). " +
            $"Total gold now = {goldManager.CurrentGold}"
        );

        ResetAllUnits();
        UpdateNextWaveInfo();

        if (AudioManager.I != null)
        {
            if (currentWave < waves.Length)
            {
                AudioManager.I.PlayBuildPhase();
            }

        }
        if (currentWave >= waves.Length)
        {
            Debug.Log("[GameDirector] ALL WAVES COMPLETED — VICTORY!");

            gameplayUIPanel.SetActive(false);


            victoryPanel.SetActive(true);

            Time.timeScale = 0f;

            AudioManager.I.StopMusic();

            return;
        }

    }

public void ReturnToMenu()
{
    Debug.Log("[GameDirector] Returning to main menu (full scene reload).");

    Time.timeScale = 1f;
    isPaused = false;
    waveInProgress = false;

    Scene current = SceneManager.GetActiveScene();
    SceneManager.LoadScene(current.name);
}


void ResetGameState()
    {
        currentWave = 0;
        _unitKillsThisWave = 0;
        _strongholdKillsThisWave = 0;

        ResetAllUnits();
        goldManager.ResetGold();
        UpdateNextWaveInfo();
    }

    public void OnEnemyKilled(KillSource source)
    {
        if (!waveInProgress) return; 

        if (source == KillSource.Unit)
        {
            _unitKillsThisWave++;
            goldManager.Add(killGold);
        }
        else if (source == KillSource.Stronghold)
        {
            _strongholdKillsThisWave++;
            int g = Mathf.RoundToInt(killGold * strongholdKillMultiplier);
            goldManager.Add(g);
        }
    }

    public void OnStrongholdFallen()
    {
        Debug.Log("[GameDirector] DEFEAT — Stronghold has fallen!");

        waveInProgress = false;

        Time.timeScale = 0f;

        if (gameplayUIPanel != null) gameplayUIPanel.SetActive(false);
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);

        if (defeatPanel != null) defeatPanel.SetActive(true);

        if (AudioManager.I != null)
            AudioManager.I.PlayDefeatMusic();
    }

}
