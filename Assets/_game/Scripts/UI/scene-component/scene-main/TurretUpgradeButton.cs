using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretUpgradeButton : TabButtonController
{
    [SerializeField] private int turretId;
    [SerializeField] private Image iconTurret;
    [SerializeField] private Image iconLocked;
    [SerializeField] private TextMeshProUGUI textLv;
    //[SerializeField] private Image selected_frame;
    //[SerializeField] private Image mask;

    private CompositeDisposable disposables = new CompositeDisposable();
    public int TurretId => turretId;

    private void Start()
    {
        //InitView();
        //Subscribes(); 
        SubscribeToUpgradeManager();
    }

    private void OnDestroy()
    {
        //UnSubscribes();
        UnSubscribeToUpgradeManager();
    }

    #region Task - Init/Display View
    private void InitView()
    {
        Debug.Log("InitView");
        if (TurretUpgradeManager.instance == null)
        {
            Debug.LogError($"TurretUpgradeButton: TurretUpgradeManager instance not found");
            return;
        }

        UpdateButtonDisplay();
    }

    private void UpdateButtonDisplay()
    {
        Debug.Log("[TurretUpgradeButton] UpdateButtonDisplay");
        if (TurretUpgradeManager.instance == null) return;

        var upgradeLv = TurretUpgradeManager.instance.GetCurrentLevel(turretId);
        Debug.Log($"[TurretUpgradeButton] UpdateButtonDisplay > turretId: {turretId} - upgradeLv: " + upgradeLv);

        textLv.gameObject.SetActive(true);
        iconTurret.gameObject.SetActive(true);

        if (TurretUpgradeManager.instance.IsLocked(turretId))
        {
            // Locked state
            iconLocked.gameObject.SetActive(true);
            textLv.text = "Locked";
            btnSelect.interactable = false;
        }
        else if (TurretUpgradeManager.instance.IsUnlocked(turretId))
        {
            // Unlocked but inactive
            iconLocked.gameObject.SetActive(false);
            var nextUpgradeCost = TurretUpgradeManager.instance.GetNextUpgradeCost(turretId);
            textLv.text = nextUpgradeCost > 0 ? $"Price: {nextUpgradeCost}" : "Price: -"; ;
            btnSelect.interactable = true;
        }
        else if (TurretUpgradeManager.instance.IsActive(turretId))
        {
            // Active with upgrade level
            iconLocked.gameObject.SetActive(false);
            textLv.text = $"Lv.{upgradeLv}";
            btnSelect.interactable = true;
        }
    }
    #endregion Task - Init/Display View

    #region Task - R3 Subscribes/Unsubscribes


    private void SubscribeToUpgradeManager()
    {
        if (TurretUpgradeManager.instance == null) return;

        // Subscribe to this turret's upgrade level changes
        var upgradeLevelProperty = TurretUpgradeManager.instance.GetTurretUpgradeLevelProperty(turretId);
        if (upgradeLevelProperty != null)
        {
            upgradeLevelProperty.Subscribe(_ => UpdateButtonDisplay()).AddTo(disposables);
        }
    }

    private void UnSubscribeToUpgradeManager()
    {
        disposables?.Dispose();
    }
    #endregion Task - R3 Subscribes/Unsubscribes!!!



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
}
