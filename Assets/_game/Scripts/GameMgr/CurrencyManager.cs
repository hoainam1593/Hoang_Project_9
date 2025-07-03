using UnityEngine;
using R3;
using System;

/// <summary>
/// CurrencyManager handles persistent gold currency for upgrades and progression.
/// This is separate from in-game coins managed by PlayerCtrl.
/// </summary>
public class CurrencyManager : SingletonMonoBehaviour<CurrencyManager>
{
    [Header("Debug Settings")]
    [SerializeField] private bool enableDebugLogs = true;

    // Persistent gold for upgrades (saved between sessions)
    public ReactiveProperty<int> Gold { get; private set; }

    // Player model reference for persistence
    private CurrencyModel currencyModel;

    //private void Start()
    //{
    //    Initialize();
    //}

    /// <summary>
    /// Initialize currency manager with persistent data
    /// </summary>
    public void Initialize()
    {
        try
        {
            // Get persistent player model
            currencyModel = PlayerModelManager.instance.GetPlayerModel<CurrencyModel>();
            if (currencyModel == null)
            {
                Debug.LogError("CurrencyManager: Failed to load PlayerModel");
                return;
            }

            // Initialize gold reactive property
            Gold = new ReactiveProperty<int>(currencyModel.Gold);

            // Subscribe to gold changes to save data and dispatch events
            Gold.Subscribe(value => {
                currencyModel.Gold = value;
                SavePlayerModel();
            });

            GameEventMgr.GED.DispatcherEvent(GameEvent.OncurrencyManagerInit, Gold);
            Debug.Log($"CurrencyManager: Initialized with gold: {Gold.Value}");

            if (enableDebugLogs)
            {
                Debug.Log($"CurrencyManager: Initialized with gold: {Gold.Value}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"CurrencyManager: Failed to initialize - {ex.Message}");
        }
    }

    #region Gold Management

    /// <summary>
    /// Add gold (e.g., from battle rewards, achievements)
    /// </summary>
    /// <param name="amount">Amount of gold to add</param>
    public void AddGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("CurrencyManager: Cannot add negative or zero gold");
            return;
        }

        Gold.Value += amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CurrencyManager: Added {amount} gold, total: {Gold.Value}");
        }
    }

    /// <summary>
    /// Consume gold (e.g., for upgrades, purchases)
    /// </summary>
    /// <param name="amount">Amount of gold to consume</param>
    /// <returns>True if transaction was successful</returns>
    public bool ConsumeGold(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("CurrencyManager: Cannot consume negative or zero gold");
            return false;
        }

        if (Gold.Value < amount)
        {
            if (enableDebugLogs)
            {
                Debug.LogWarning($"CurrencyManager: Not enough gold. Needed: {amount}, Have: {Gold.Value}");
            }
            return false;
        }

        Gold.Value -= amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CurrencyManager: Consumed {amount} gold, remaining: {Gold.Value}");
        }

        return true;
    }

    public int GetGold()
    {
        return Gold.Value;
    }

    public void SetGold(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("CurrencyManager: Cannot set negative gold");
            return;
        }

        Gold.Value = amount;
        
        if (enableDebugLogs)
        {
            Debug.Log($"CurrencyManager: Set gold to: {Gold.Value}");
        }
    }

    #endregion

    #region Battle Rewards

    public void AddBattleReward(int baseReward, int waveBonus = 0, float performanceMultiplier = 1.0f)
    {
        int totalReward = Mathf.RoundToInt((baseReward + waveBonus) * performanceMultiplier);
        AddGold(totalReward);
        
        if (enableDebugLogs)
        {
            Debug.Log($"CurrencyManager: Battle reward - Base: {baseReward}, Bonus: {waveBonus}, " +
                     $"Multiplier: {performanceMultiplier:F2}, Total: {totalReward}");
        }
    }

    #endregion

    #region Utility

    private void SavePlayerModel()
    {
        currencyModel?.Save();
    }

    #endregion
}