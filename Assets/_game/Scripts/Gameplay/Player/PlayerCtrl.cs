using UnityEngine;
using R3;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// PlayerCtrl manages in-game player attributes during gameplay.
/// Only handles HP and coins that reset each game session.
/// Gold management is handled by CurrencyManager.
/// </summary>
public class PlayerCtrl : SingletonMonoBehaviour<PlayerCtrl>
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // In-game reactive properties (reset each game)
    public bool IsDead { get; private set; } = false;
    public ReactiveProperty<int> Hp { get; private set; }
    public ReactiveProperty<int> Coin { get; private set; }

    // Configuration reference
    private PlayerConfigItem playerConfig;

    protected override void Awake()
    {
        base.Awake();
        // Subscribe to game events
        SubscribeToGameEvents();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnsubscribeFromGameEvents();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// Initialize player data from config
    /// </summary>
    public void Initialize()
    {
        try
        {
            // Get player config for initial values
            playerConfig = ConfigManager.instance.GetConfig<PlayerConfig>().GetFirst();
            if (playerConfig == null)
            {
                Debug.LogError("PlayerCtrl: Failed to load PlayerConfig");
                return;
            }

            // Initialize in-game reactive properties (reset each game)
            IsDead = false;
            Hp = new ReactiveProperty<int>(playerConfig.hp);
            Coin = new ReactiveProperty<int>(playerConfig.coin);

            if (enableDebugLogs)
            {
                Debug.Log($"PlayerCtrl: Initialized - HP: {Hp.Value}, Coin: {Coin.Value}");
            }

            GameEventMgr.GED.DispatcherEvent(GameEvent.OnPlayerInit, new PlayerInfo
            {
                HP = Hp,
                Coin = Coin
            });

        }
        catch (Exception ex)
        {
            Debug.LogError($"PlayerCtrl: Failed to initialize player data - {ex.Message}");
        }
    }

    #region In-Game Data Management

    /// <summary>
    /// Reset in-game data to initial values (called when starting a new game)
    /// </summary>
    public void ResetInGameData()
    {
        if (playerConfig != null)
        {
            Hp.Value = playerConfig.hp;
            Coin.Value = playerConfig.coin;

            if (enableDebugLogs)
            {
                Debug.Log($"PlayerCtrl: Reset in-game data - HP: {Hp.Value}, Coin: {Coin.Value}");
            }
        }
    }

    /// <summary>
    /// Get maximum HP from config
    /// </summary>
    /// <returns>Maximum HP value</returns>
    public int GetMaxHP()
    {
        return playerConfig?.hp ?? 0;
    }

    /// <summary>
    /// Get current HP value
    /// </summary>
    /// <returns>Current HP value</returns>
    public int GetCurrentHP()
    {
        return Hp.Value;
    }

    /// <summary>
    /// Get current coin value
    /// </summary>
    /// <returns>Current coin value</returns>
    public int GetCurrentCoin()
    {
        return Coin.Value;
    }

    #endregion

    #region HP Management

    /// <summary>
    /// Lose HP (e.g., when enemy reaches gate)
    /// </summary>
    /// <param name="amount">Amount of HP to lose</param>
    /// <returns>True if player died (HP <= 0)</returns>
    public bool LoseHP(int amount = 1)
    {
        if (Hp.Value <= 0) return true;

        int previousHP = Hp.Value;
        Hp.Value = Mathf.Max(0, Hp.Value - amount);
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Lost {amount} HP, current HP: {Hp.Value}");
        }

        if (Hp.Value <= 0)
        {
            IsDead = true;
            if (enableDebugLogs)
            {
                Debug.Log("PlayerCtrl: Player has died");
            }
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnPlayerDeath);
        }

        return Hp.Value <= 0;
    }

    /// <summary>
    /// Heal HP (e.g., from power-ups)
    /// </summary>
    /// <param name="amount">Amount of HP to heal</param>
    public void HealHP(int amount)
    {
        if (amount <= 0) return;

        int previousHP = Hp.Value;
        int maxHP = playerConfig.hp;
        Hp.Value = Mathf.Min(maxHP, Hp.Value + amount);
        
        int actualHealing = Hp.Value - previousHP;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Healed {actualHealing} HP, current HP: {Hp.Value}");
        }
    }

    /// <summary>
    /// Check if player is at full health
    /// </summary>
    /// <returns>True if HP is at maximum</returns>
    public bool IsAtFullHealth()
    {
        return Hp.Value >= playerConfig.hp;
    }

    /// <summary>
    /// Get HP percentage (0.0 to 1.0)
    /// </summary>
    /// <returns>HP as percentage</returns>
    public float GetHPPercentage()
    {
        return (float)Hp.Value / (float)playerConfig.hp;
    }

    #endregion

    #region Coin Management

    /// <summary>
    /// Consume in-game coins (e.g., for buying turrets)
    /// </summary>
    /// <param name="amount">Amount of coins to consume</param>
    /// <returns>True if transaction was successful</returns>
    public bool ConsumeCoin(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("PlayerCtrl: Cannot consume negative or zero coins");
            return false;
        }

        if (Coin.Value < amount)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"PlayerCtrl: Not enough coins. Needed: {amount}, Have: {Coin.Value}");
            }
            return false;
        }

        int previousCoins = Coin.Value;
        Coin.Value -= amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Consumed {amount} coins, remaining: {Coin.Value}");
        }
        return true;
    }

    /// <summary>
    /// Add in-game coins (e.g., from killing enemies)
    /// </summary>
    /// <param name="amount">Amount of coins to add</param>
    public void AddCoin(int amount)
    {
        if (amount <= 0) return;

        int previousCoins = Coin.Value;
        Coin.Value += amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Added {amount} coins, total: {Coin.Value}");
        }
    }

    /// <summary>
    /// Check if player has enough coins for a purchase
    /// </summary>
    /// <param name="amount">Amount to check</param>
    /// <returns>True if player has enough coins</returns>
    public bool HasEnoughCoin(int amount)
    {
        return Coin.Value >= amount;
    }

    #endregion

    #region Public Getters for UI

    /// <summary>
    /// Get in-game player info for UI display (without gold)
    /// </summary>
    public PlayerInfo GetPlayerInfo()
    {
        return new PlayerInfo
        {
            HP = Hp,
            Coin = Coin,
        };
    }

    #endregion

    #region Event Handling

    private void SubscribeToGameEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnGameStart, OnGameStart);
        GameEventMgr.GED.Register(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
        GameEventMgr.GED.Register(GameEvent.OnGameRetry, OnGameRetry);
        GameEventMgr.GED.Register(GameEvent.OnEnemyDead, OnEnemyDead);
        GameEventMgr.GED.Register(GameEvent.OnTurretSpawnCompleted, OnTurretSpawnCompleted);
    }

    private void UnsubscribeFromGameEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnGameStart, OnGameStart);
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
        GameEventMgr.GED.UnRegister(GameEvent.OnGameRetry, OnGameRetry);
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyDead, OnEnemyDead);
        GameEventMgr.GED.UnRegister(GameEvent.OnTurretSpawnCompleted, OnTurretSpawnCompleted);
    }

    private void OnGameStart(object data)
    {
        ResetInGameData();

        if (enableDebugLogs)
        {
            Debug.Log("PlayerCtrl: Game started - resetting in-game data");
        }
    }

    private void OnGameRetry(object data)
    {
        ResetInGameData();

        if (enableDebugLogs)
        {
            Debug.Log("PlayerCtrl: Game retry - resetting in-game data");
        }
    }

    private void OnEnemyReachHallGate(object data)
    {
        bool playerDied = LoseHP();
    }

    private void OnEnemyDead(object data)
    {
        var parseData = (EnemyInfo)data;
        if (parseData != null)
        {
            AddCoin(parseData.config.bonusCoin);
            if (enableDebugLogs)
            {
                Debug.Log($"PlayerCtrl: Enemy dead - added {parseData.config.bonusCoin} coins, total: {Coin.Value}");
            }
        }
    }

    private void OnTurretSpawnCompleted(object data)
    {
        var parseData = (TurretInfo)data;
        if (parseData != null)
        {
            if (HasEnoughCoin(parseData.config.cost))
            {
                ConsumeCoin(parseData.config.cost);
                if (enableDebugLogs)
                {
                    Debug.Log($"PlayerCtrl: Turret spawned - consumed {parseData.config.cost} coins, remaining: {Coin.Value}");
                }
            }
            else
            {
                Debug.LogWarning("PlayerCtrl: Not enough coins to spawn turret");
            }
        }
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// Add debug coins (for testing purposes)
    /// </summary>
    [ContextMenu("Add 100 Coins (Debug)")]
    public void AddDebugCoins()
    {
        AddCoin(100);
    }

    /// <summary>
    /// Take damage for testing
    /// </summary>
    [ContextMenu("Take 1 Damage (Debug)")]
    public void TakeDebugDamage()
    {
        LoseHP(1);
    }

    /// <summary>
    /// Heal to full HP for testing
    /// </summary>
    [ContextMenu("Heal to Full (Debug)")]
    public void HealToFull()
    {
        HealHP(playerConfig.hp);
    }

    #endregion

}

/// <summary>
/// Data structure for in-game player information (without gold)
/// </summary>
[System.Serializable]
public class PlayerInfo
{
    public ReactiveProperty<int> HP;
    public ReactiveProperty<int> Coin;
}
