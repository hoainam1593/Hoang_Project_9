using UnityEngine;
using TMPro;
using UnityEngine.UI;
using R3;

public class TurretUpgradeInfoUICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textAttack;
    [SerializeField] private TextMeshProUGUI textRange;
    [SerializeField] private TextMeshProUGUI textSpeed;
    [SerializeField] private TextMeshProUGUI textPrice;
    [SerializeField] private TextMeshProUGUI textButtonUpgrade;
    [SerializeField] private Button buttonUpgrade; // Single button for both buy and upgrade

    [SerializeField] private int turretId = 0;
    private CompositeDisposable disposables = new CompositeDisposable();

    TurretConfigItem defaultConfig;

    private void Start()
    {
        InitView();
        RegisterListeners();
        SubscribeToUpgradeManager();
        SubscribeToCurrencyManager();
    }

    private void OnDestroy()
    {
        RemoveListeners();
        UnSubscribeToUpgradeManager();
    }

    #region Task - UI Init/Display View

    public void InitView()
    {
        defaultConfig = ConfigManager.instance.GetConfig<TurretConfig>().GetItem(turretId);

        if (TurretUpgradeManager.instance == null)
        {
            Debug.LogError($"TurretUpgradeInfoUICtrl: TurretUpgradeManager instance not found");
            return;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (TurretUpgradeManager.instance == null || turretId == -1) return;

        var currentLevel = TurretUpgradeManager.instance.GetCurrentLevel(turretId);
        var maxLevel = TurretUpgradeManager.instance.GetMaxLevel(turretId);

        // Set turret name
        textName.text = $"Turret Lv{defaultConfig.level}\n[upgrade: Lv{currentLevel}]";

        if (TurretUpgradeManager.instance.IsLocked(turretId))
        {
            ShowLockedState();
        }
        else if (TurretUpgradeManager.instance.IsUnlocked(turretId))
        {
            ShowUnlockedState(defaultConfig);
        }
        else if (TurretUpgradeManager.instance.IsActive(turretId))
        {
            ShowActiveState(defaultConfig, currentLevel, maxLevel);
        }

        // Check if at max level
        if (TurretUpgradeManager.instance.CheckMaxUpgradeLevel(turretId))
        {
            ShowMaxLevelState();
        }
    }

    private void ShowLockedState()
    {
        textAttack.text = "Attack: Locked";
        textRange.text = "Range: Locked";
        textSpeed.text = "Speed: Locked";
        textPrice.text = "Price: -";
        buttonUpgrade.gameObject.SetActive(false);
    }

    private void ShowUnlockedState(TurretConfigItem defaultConfig)
    {
        // Show base stats
        textAttack.text = $"Attack: {defaultConfig.attack}";
        textRange.text = $"Range: {defaultConfig.range}";
        textSpeed.text = $"Speed: {defaultConfig.speed}";

        // Show next upgrade cost (level 1 - buying the turret)
        var nextUpgradeCost = TurretUpgradeManager.instance.GetNextUpgradeCost(turretId);
        textPrice.text = nextUpgradeCost > 0 ? $"Price: {nextUpgradeCost}" : "Price: -";

        buttonUpgrade.gameObject.SetActive(true);

        // Check if player has enough gold for the upgrade
        bool hasEnoughGold = CheckSufficientGold(nextUpgradeCost);
        buttonUpgrade.interactable = hasEnoughGold;
        textButtonUpgrade.text = "Buy";
    }

    private void ShowActiveState(TurretConfigItem defaultConfig, int currentLevel, int maxLevel)
    {
        // Use TurretUpgradeManager to get proper calculated stats
        var (totalAttack, totalRange, totalSpeed) = TurretUpgradeManager.instance.GetCurrentTotalStats(turretId);

        textAttack.text = $"Attack: {totalAttack}";
        textRange.text = $"Range: {totalRange}";
        textSpeed.text = $"Speed: {totalSpeed}";

        // Show next upgrade cost if not at max level
        int upgradeCost = 0;
        if (currentLevel < maxLevel)
        {
            upgradeCost = TurretUpgradeManager.instance.GetNextUpgradeCost(turretId);
            textPrice.text = upgradeCost > 0 ? $"Price: {upgradeCost}" : "Price: -";
        }

        buttonUpgrade.gameObject.SetActive(true);

        // Check if player has enough gold for the upgrade
        bool hasEnoughGold = CheckSufficientGold(upgradeCost);
        buttonUpgrade.interactable = hasEnoughGold;
        textButtonUpgrade.text = "Upgrade";
    }

    private void ShowMaxLevelState()
    {
        buttonUpgrade.gameObject.SetActive(true);
        buttonUpgrade.interactable = false;
        textPrice.text = "Max Level";
    }

    /// <summary>
    /// Check if player has sufficient gold for the upgrade cost
    /// </summary>
    /// <param name="cost">The cost of the upgrade</param>
    /// <returns>True if player has enough gold</returns>
    private bool CheckSufficientGold(int cost)
    {
        if (CurrencyManager.instance == null)
        {
            Debug.LogWarning("TurretUpgradeInfoUICtrl: CurrencyManager instance not found");
            return false;
        }

        if (cost <= 0) return true; // Free upgrades are always affordable

        int currentGold = CurrencyManager.instance.GetGold();
        return currentGold >= cost;
    }
    #endregion Task - UI Init/Display View

    #region Task - R3 EventHandler

    private void SubscribeToUpgradeManager()
    {
        if (TurretUpgradeManager.instance == null || turretId == -1) return;

        // Subscribe to this turret's upgrade level changes
        var upgradeLevelProperty = TurretUpgradeManager.instance.GetTurretUpgradeLevelProperty(turretId);
        if (upgradeLevelProperty != null)
        {
            upgradeLevelProperty.Subscribe(_ => UpdateDisplay()).AddTo(disposables);
        }
    }

    private void SubscribeToCurrencyManager()
    {
        if (CurrencyManager.instance == null) return;

        // Subscribe to gold changes to update button interactability
        CurrencyManager.instance.Gold.Subscribe(_ => UpdateDisplay()).AddTo(disposables);
    }

    private void UnSubscribeToUpgradeManager()
    {
        disposables?.Dispose();
    }

    #endregion Task - R3 EventHandler!!!

    #region Task - GameEvent Subscribes/Unsubscribes

    private void Subscribes()
    {
        //GameEventMgr
        GameEventMgr.GED.Register(GameEvent.OnTurretUpgradeMgrInit, OnTurretUpgradeMgrInit);
    }

    private void UnSubscribes()
    {
        //GameEventMgr
        GameEventMgr.GED.UnRegister(GameEvent.OnTurretUpgradeMgrInit, OnTurretUpgradeMgrInit);
    }

    private void OnTurretUpgradeMgrInit(object data)
    {
        InitView();
    }

    #endregion Task - GameEvent Subscribes/Unsubscribes!!!

    #region Button onClick events

    private void RegisterListeners()
    {
        buttonUpgrade.onClick.AddListener(OnUpgradeButtonClicked);
    }

    private void RemoveListeners()
    {
        buttonUpgrade.onClick.RemoveListener(OnUpgradeButtonClicked);
    }

    private void OnUpgradeButtonClicked()
    {
        if (TurretUpgradeManager.instance == null || turretId == -1) return;

        // Check if player has enough gold before attempting upgrade
        var upgradeCost = TurretUpgradeManager.instance.GetNextUpgradeCost(turretId);
        if (!CheckSufficientGold(upgradeCost))
        {
            Debug.LogWarning($"TurretUpgradeInfoUICtrl: Insufficient gold for turret {turretId} upgrade. Cost: {upgradeCost}, Available: {CurrencyManager.instance?.GetGold() ?? 0}");
            // TODO: Show "Not enough gold" popup or feedback to player
            return;
        }

        // Proceed with upgrade
        bool success = TurretUpgradeManager.instance.UpgradeTurret(turretId);
        if (success && upgradeCost > 0)
        {
            CurrencyManager.instance.ConsumeGold(upgradeCost);
        }
    }
    #endregion Button onClick events!!!
}
