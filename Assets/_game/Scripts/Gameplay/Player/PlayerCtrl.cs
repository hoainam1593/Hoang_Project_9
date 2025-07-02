using UnityEngine;
using R3;
using System;

/// <summary>
/// PlayerCtrl manages player data during gameplay using reactive properties.
/// It controls in-game resources like HP, coins, and gold for upgrades.
/// </summary>
public class PlayerCtrl : SingletonMonoBehaviour<PlayerCtrl>
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // In-game reactive properties (reset each game)
    private ReactiveProperty<int> inGameHP;
    private ReactiveProperty<int> inGameCoin;
    
    // Persistent reactive properties (from PlayerModel)
    private ReactiveProperty<int> gold; // Renamed from persistentGold - used for all upgrades in main scene

    // Configuration and models
    private PlayerConfigItem playerConfig;
    private PlayerModel playerModel;

    /// <summary>
    /// Initialize player data from config and persistent model
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

            // Get persistent player model
            playerModel = PlayerModelManager.instance.GetPlayerModel<PlayerModel>();
            if (playerModel == null)
            {
                Debug.LogError("PlayerCtrl: Failed to load PlayerModel");
                return;
            }

            // Initialize in-game reactive properties (reset each game)
            inGameHP = new ReactiveProperty<int>(playerConfig.hp);
            inGameCoin = new ReactiveProperty<int>(playerConfig.coin);

            // Initialize persistent gold for upgrades
            gold = new ReactiveProperty<int>(playerModel.Gold);

            // Subscribe to gold changes to save data
            gold.Subscribe(value => {
                playerModel.Gold = value;
                //Dispatcher Event goldChange
                SavePlayerModel();
            });

            if (enableDebugLogs)
            {
                Debug.Log($"PlayerCtrl: Initialized - Gold: {gold.Value}, HP: {inGameHP.Value}, Coin: {inGameCoin.Value}");
            }

            // Subscribe to game events
            SubscribeToGameEvents();
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
            inGameHP.Value = playerConfig.hp;
            inGameCoin.Value = playerConfig.coin;

            if (enableDebugLogs)
            {
                Debug.Log($"PlayerCtrl: Reset in-game data - HP: {inGameHP.Value}, Coin: {inGameCoin.Value}");
            }
        }
    }

    /// <summary>
    /// Lose HP (e.g., when enemy reaches gate)
    /// </summary>
    /// <param name="amount">Amount of HP to lose</param>
    /// <returns>True if player died (HP <= 0)</returns>
    public bool LoseHP(int amount = 1)
    {
        if (inGameHP.Value <= 0) return true;

        inGameHP.Value = Mathf.Max(0, inGameHP.Value - amount);
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Lost {amount} HP, current HP: {inGameHP.Value}");
        }

        return inGameHP.Value <= 0;
    }

    /// <summary>
    /// Heal HP (e.g., from power-ups)
    /// </summary>
    /// <param name="amount">Amount of HP to heal</param>
    public void HealHP(int amount)
    {
        int maxHP = playerConfig.hp;
        inGameHP.Value = Mathf.Min(maxHP, inGameHP.Value + amount);
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Healed {amount} HP, current HP: {inGameHP.Value}");
        }
    }

    /// <summary>
    /// Consume in-game coins (e.g., for buying turrets)
    /// </summary>
    /// <param name="amount">Amount of coins to consume</param>
    /// <returns>True if transaction was successful</returns>
    public bool ConsumeCoin(int amount)
    {
        if (inGameCoin.Value < amount)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"PlayerCtrl: Not enough coins. Needed: {amount}, Have: {inGameCoin.Value}");
            }
            return false;
        }

        inGameCoin.Value -= amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Consumed {amount} coins, remaining: {inGameCoin.Value}");
        }

        return true;
    }

    /// <summary>
    /// Add in-game coins (e.g., from killing enemies)
    /// </summary>
    /// <param name="amount">Amount of coins to add</param>
    public void AddCoin(int amount)
    {
        inGameCoin.Value += amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Added {amount} coins, total: {inGameCoin.Value}");
        }
    }

    #endregion

    #region Gold Management (for upgrades)

    /// <summary>
    /// Add gold (used for upgrades in main scene)
    /// </summary>
    /// <param name="amount">Amount of gold to add</param>
    public void AddGold(int amount)
    {
        gold.Value += amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Added {amount} gold, total: {gold.Value}");
        }
    }

    /// <summary>
    /// Consume gold (e.g., for upgrades in main scene)
    /// </summary>
    /// <param name="amount">Amount of gold to consume</param>
    /// <returns>True if transaction was successful</returns>
    public bool ConsumeGold(int amount)
    {
        if (gold.Value < amount)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"PlayerCtrl: Not enough gold. Needed: {amount}, Have: {gold.Value}");
            }
            return false;
        }

        gold.Value -= amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Consumed {amount} gold, remaining: {gold.Value}");
        }

        return true;
    }

    /// <summary>
    /// Update gold with bonus at end of battle
    /// </summary>
    /// <param name="bonusAmount">Amount of bonus gold to add</param>
    public void UpdateGoldWithBonus(int bonusAmount)
    {
        AddGold(bonusAmount);
        
        if (enableDebugLogs)
        {
            Debug.Log($"PlayerCtrl: Updated gold with battle bonus: {bonusAmount}, total: {gold.Value}");
        }
    }

    /// <summary>
    /// Get current gold amount
    /// </summary>
    public int GetGold() => gold.Value;

    #endregion

    #region Public Getters for UI

    /// <summary>
    /// Get in-game player info for UI display
    /// </summary>
    public InGamePlayerInfo GetPlayerInfo()
    {
        return new InGamePlayerInfo
        {
            HP = inGameHP,
            Coin = inGameCoin,
            Gold = gold,
        };
    }


    #endregion

    #region Event Handling

    private void SubscribeToGameEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnGameStart, OnGameStart);
        GameEventMgr.GED.Register(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
    }

    private void UnsubscribeFromGameEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnGameStart, OnGameStart);
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
    }

    private void OnGameStart(object data)
    {
        ResetInGameData();

        if (enableDebugLogs)
        {
            Debug.Log("PlayerCtrl: Game started - resetting in-game data");
        }
    }

    private void OnEnemyReachHallGate(object data)
    {
        bool playerDied = LoseHP();
        if (playerDied)
        {
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnPlayerDeath);
        }
    }

    #endregion

    #region Utility

    private void SavePlayerModel()
    {
        PlayerModelManager.instance.SaveModel(playerModel);
    }

    protected override void OnDestroy()
    {
        UnsubscribeFromGameEvents();
        base.OnDestroy();
    }

    #endregion
}

/// <summary>
/// Data structure for in-game player information (simplified)
/// </summary>
[System.Serializable]
public class InGamePlayerInfo
{
    public ReactiveProperty<int> HP;
    public ReactiveProperty<int> Coin;
    public ReactiveProperty<int> Gold;
}