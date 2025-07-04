using R3;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager class that controls all turret upgrade functionality as a separate module.
/// Uses ReactiveProperty for UI binding and provides centralized upgrade logic.
/// </summary>
public class TurretUpgradeManager : SingletonMonoBehaviour<TurretUpgradeManager>
{
    // ReactiveProperties for UI binding - each turret has its own upgrade level observable
    private Dictionary<int, ReactiveProperty<int>> turretUpgradeLevels = new Dictionary<int, ReactiveProperty<int>>();

    public ReactiveProperty<int> GetTurretUpgradeLevelProperty(int turretId)
    {
        if (!turretUpgradeLevels.ContainsKey(turretId))
        {
            Debug.LogWarning($"TurretUpgradeManager: No upgrade level property found for turretId {turretId}");
            return null;
        }
        return turretUpgradeLevels[turretId];
    }


    #region Get/Set Config Model

    private TurretUpgradeModel _turretUpgradeModel;
    private TurretUpgradeConfig _turretUpgradeConfig;
    private TurretConfig _turretConfig;

    private TurretUpgradeModel turretUpgradeModel
    {
        get
        {
            if (_turretUpgradeModel == null)
            {
                _turretUpgradeModel = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>();
            }
            return _turretUpgradeModel;
        }
    }
    private TurretUpgradeConfig turretUpgradeConfig
    {
        get
        {
            if (_turretUpgradeConfig == null)
            {
                _turretUpgradeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>();
            }
            return _turretUpgradeConfig;
        }
    }
    private TurretConfig turretConfig
    {
        get
        {
            if (_turretConfig == null)
            {
                _turretConfig = ConfigManager.instance.GetConfig<TurretConfig>();
            }
            return _turretConfig;
        }
    }

    #endregion Get/Set Config Model!!!

    #region R3 Observable
    // Global events for UI to subscribe to
    private Subject<int> onTurretUnlocked = new Subject<int>();
    private Subject<int> onTurretBought = new Subject<int>();
    private Subject<int> onTurretUpgraded = new Subject<int>();

    /// <summary>
    /// Observable for when any turret gets upgraded (emits turretId)
    /// </summary>
    public Observable<int> OnTurretUpgraded => onTurretUpgraded.AsObservable();
    public Observable<int> OnTurretUnlocked => onTurretUnlocked.AsObservable();
    public Observable<int> OnTurretBought => onTurretBought.AsObservable();
    #endregion



    #region Initialization
    protected override void Awake()
    {
        base.Awake();

        Initialize();
    }

    private void Initialize()
    {
        // Get model and config references
        _turretUpgradeModel = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>();
        _turretUpgradeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>();
        _turretConfig = ConfigManager.instance.GetConfig<TurretConfig>();

        if (turretUpgradeModel == null)
        {
            Debug.LogError("TurretUpgradeManager: Failed to get TurretUpgradeModel");
            return;
        }

        if (turretUpgradeConfig == null)
        {
            Debug.LogError("TurretUpgradeManager: Failed to get TurretUpgradeConfig");
            return;
        }

        if (turretConfig == null)
        {
            Debug.LogError("TurretUpgradeManager: Failed to get TurretConfig");
            return;
        }

        // Initialize ReactiveProperties for all turrets
        InitializeReactiveProperties();

        Debug.Log("IsLocked called for turretId: " + 0 + " is: " + IsLocked(0));
        if (IsLocked(0))
        {
            UnlockTurret(0); // Ensure the first turret is unlocked by default
        }
        //if (IsUnlocked(0) && !IsActive(0))
        //{
        //    UpgradeTurret(0); // Upgrade the first turret to level 1
        //}


        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretUpgradeMgrInit);
    }

    private void InitializeReactiveProperties()
    {
        foreach (var upgradeInfo in turretUpgradeModel.upgradeInfos)
        {
            int turretId = upgradeInfo.Key;
            int currentLevel = upgradeInfo.Value.upgradeLv;
            
            // Create ReactiveProperty for this turret's upgrade level
            turretUpgradeLevels[turretId] = new ReactiveProperty<int>(currentLevel);
        }

        OnTurretUpgraded.Subscribe(turretId =>
        {
            Debug.Log($"TurretUpgradeManager: OnTurretUpgraded onNext turretId: {turretId}");
            OnTurretUpgrade(turretId);
        }).AddTo(this);

        Debug.Log($"TurretUpgradeManager: Initialized {turretUpgradeLevels.Count} turret upgrade level properties");
    }
    #endregion

    #region Core Upgrade Functions

    /// <summary>
    /// Unlock a turret (upgrade from level -1 to 0)
    /// </summary>
    /// <param name="turretId">ID of the turret to unlock</param>
    /// <returns>True if unlock was successful</returns>
    public bool UnlockTurret(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        if (upgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeManager: No upgrade info found for turretId {turretId}");
            return false;
        }

        if (upgradeInfo.upgradeLv != -1)
        {
            Debug.LogWarning($"TurretUpgradeManager: Turret {turretId} is already unlocked (level {upgradeInfo.upgradeLv})");
            return false;
        }

        // TODO: Add cost checking and resource deduction here
        // For now, just unlock without cost

        upgradeInfo.upgradeLv = 0;
        
        // Update ReactiveProperty
        if (turretUpgradeLevels.ContainsKey(turretId))
        {
            turretUpgradeLevels[turretId].Value = 0;
        }

        turretUpgradeModel.Save();
        // Notify observers
        onTurretUnlocked.OnNext(turretId);

        Debug.Log($"TurretUpgradeManager: UnlockTurret {turretId}");
        return true;
    }

    public bool BuyTurret(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        if (upgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeManager: No upgrade info found for turretId {turretId}");
            return false;
        }


        if (IsLocked(turretId))
        {
            Debug.LogError($"TurretUpgradeManager: Cannot upgrade locked turret {turretId}");
            return false;
        }

        if (upgradeInfo.upgradeLv != 0)
        {
            Debug.LogError("Buy failed");
            return false;
        }
        else
        {
            UpgradeTurret(turretId); // Automatically upgrade to level 1 when buying
            onTurretBought.OnNext(turretId);
            Debug.Log($"TurretUpgradeManager: BuyTurret {turretId} Success");
            return true;
        }
    }

    /// <summary>
    /// Upgrade a turret by one level
    /// </summary>
    /// <param name="turretId">ID of the turret to upgrade</param>
    /// <returns>True if upgrade was successful</returns>
    public bool UpgradeTurret(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        if (upgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeManager: No upgrade info found for turretId {turretId}");
            return false;
        }

        if (IsLocked(turretId))
        {
            Debug.LogError($"TurretUpgradeManager: Cannot upgrade locked turret {turretId}");
            return false;
        }

        if (CheckMaxUpgradeLevel(turretId))
        {
            Debug.LogWarning($"TurretUpgradeManager: Turret {turretId} is already at max level");
            return false;
        }

        int nextLevel = upgradeInfo.upgradeLv + 1;
        var nextUpgradeStats = GetUpgradeStatsForLevel(turretId, nextLevel);
        if (nextUpgradeStats == null)
        {
            Debug.LogError($"TurretUpgradeManager: No upgrade stats found for turretId {turretId} level {nextLevel}");
            return false;
        }

            // TODO: Check if player has enough resources (nextUpgradeStats.cost)
            // For now, just upgrade without cost checking

        upgradeInfo.attack += nextUpgradeStats.attack;
        upgradeInfo.range += nextUpgradeStats.range;
        upgradeInfo.speed += nextUpgradeStats.speed;
        upgradeInfo.upgradeLv = nextLevel;

        // Update ReactiveProperty
        if (turretUpgradeLevels.ContainsKey(turretId))
        {
            turretUpgradeLevels[turretId].Value = nextLevel;
        }

        turretUpgradeModel.Save(); // Save the model after upgrade

        // Notify observers
        onTurretUpgraded.OnNext(turretId);
        Debug.Log($"TurretUpgradeManager: UpgradeTurret {turretId} to level {nextLevel}");
        return true;
    }

    private void OnTurretUpgrade(int turretId)
    {
        var nextLevelTurretId = GetNextTurretLevelId(turretId);
        if (nextLevelTurretId == -1)
        {
            //Don't have new level of this turret type
            return;
        }
        
        //Debug.Log($"[TurretUpgradeManager] > OnTurretUpgrade: Turret {turretId} > nextLevelTurretId: {nextLevelTurretId}");
        if (!IsUnlocked(nextLevelTurretId))
        {
            // Automatically unlock the next turret if it exists
            UnlockTurret(nextLevelTurretId);
        }

    }
    #endregion

    #region Query Functions

    /// <summary>
    /// Check if a turret is locked (level -1)
    /// </summary>
    public bool IsLocked(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        return upgradeInfo?.upgradeLv == -1;
    }

    /// <summary>
    /// Check if a turret is unlocked but inactive (level 0)
    /// </summary>
    public bool IsUnlocked(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        return upgradeInfo?.upgradeLv == 0;
    }

    /// <summary>
    /// Check if a turret is active (level > 0)
    /// </summary>
    public bool IsActive(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        return upgradeInfo?.upgradeLv > 0;
    }

    /// <summary>
    /// Check if a turret is at maximum upgrade level
    /// </summary>
    public bool CheckMaxUpgradeLevel(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        if (upgradeInfo == null) return false;

        var upgradeConfigItem = turretUpgradeConfig.GetItem(turretId);
        if (upgradeConfigItem == null) return false;

        return upgradeInfo.upgradeLv >= upgradeConfigItem.bonusStats.Count;
    }

    /// <summary>
    /// Get current upgrade level of a turret
    /// </summary>
    public int GetCurrentLevel(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        return upgradeInfo?.upgradeLv ?? -1;
    }

    /// <summary>
    /// Get maximum possible upgrade level for a turret
    /// </summary>
    public int GetMaxLevel(int turretId)
    {
        var upgradeConfigItem = turretUpgradeConfig.GetItem(turretId);
        return upgradeConfigItem?.bonusStats.Count ?? 0;
    }

    public int GetNextTurretLevelId(int turretId)
    {
        var turretCfg = turretConfig.GetItem(turretId);
        int nextLevel = turretCfg.level + 1;
        TurretType turretType = turretCfg.type;
        return turretConfig.GetItem(turretType, nextLevel)?.id ?? -1;
    }
    #endregion

    #region Stats Calculation

    /// <summary>
    /// Get stats for the next upgrade level
    /// </summary>
    public TurretUpgradeStats GetStatsNextLevel(int turretId)
    {
        var upgradeInfo = turretUpgradeModel.GetItem(turretId);
        if (upgradeInfo == null) return null;

        int nextLevel = upgradeInfo.upgradeLv + 1;
        return GetUpgradeStatsForLevel(turretId, nextLevel);
    }

    /// <summary>
    /// Get upgrade stats for a specific level
    /// </summary>
    public TurretUpgradeStats GetUpgradeStatsForLevel(int turretId, int level)
    {
        var upgradeConfigItem = turretUpgradeConfig.GetItem(turretId);
        return upgradeConfigItem?.GetBonusStats(level);
    }

    /// <summary>
    /// Get cost for next upgrade
    /// </summary>
    public int GetNextUpgradeCost(int turretId)
    {
        var nextStats = GetStatsNextLevel(turretId);
        return nextStats?.cost ?? 0;
    }
    #endregion

    #region Cleanup
    protected override void OnDestroy()
    {
        // Dispose all ReactiveProperties
        foreach (var property in turretUpgradeLevels.Values)
        {
            property?.Dispose();
        }
        turretUpgradeLevels.Clear();

        // Dispose subjects
        onTurretUpgraded?.Dispose();
        onTurretUnlocked?.Dispose();
        onTurretBought?.Dispose();

        base.OnDestroy();
    }
    #endregion
}