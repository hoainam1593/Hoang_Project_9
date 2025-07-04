//using R3;
//using System.Collections.Generic;
//using UnityEngine;

//public class TabViewUpgrade : MonoBehaviour
//{
//    [SerializeField] private List<TurretUpgradeButton> buttons;
//    [SerializeField] private TurretUpgradeInfoUICtrl upgradeInfoPanel;

//    private ReactiveProperty<int> selectedTurretId = new ReactiveProperty<int>(0);
//    private CompositeDisposable disposables = new CompositeDisposable();

//    private void Start()
//    {
//        InitializeButtons();
//        SubscribeToSelectionChanges();
        
//        // Initialize with first turret selected
//        if (buttons.Count > 0)
//        {
//            SelectTurretButton(0);
//        }
//    }

//    private void InitializeButtons()
//    {
//        foreach (var button in buttons)
//        {
//            button.Init(selectedTurretId);
//            button.SetCallback(SelectTurretButton);
//        }
//    }

//    private void SubscribeToSelectionChanges()
//    {
//        selectedTurretId.Subscribe((turretId) =>
//        {
//            ChangeUpgradeInfo(turretId);
//        }).AddTo(disposables);
//    }

//    private void SelectTurretButton(int turretId)
//    {
//        if (turretId < 0 || turretId >= buttons.Count)
//        {
//            Debug.LogError($"TabViewUpgrade: Invalid turretId: {turretId}");
//            return;
//        }

//        if (turretId == selectedTurretId.Value)
//        {
//            return; // No change, do nothing
//        }

//        selectedTurretId.Value = turretId;
//    }

//    private void ChangeUpgradeInfo(int turretId)
//    {
//        upgradeInfoPanel.InitView(turretId);
//    }

//    // This method is kept for compatibility but no longer used since upgrade logic is in TurretUpgradeManager
//    [System.Obsolete("Use TurretUpgradeManager.UpgradeTurret() instead")]
//    public void Upgrade(int turretId)
//    {
//        if (TurretUpgradeManager.instance != null)
//        {
//            TurretUpgradeManager.instance.UpgradeTurret(turretId);
//        }
//        else
//        {
//            Debug.LogError("TabViewUpgrade: TurretUpgradeManager instance not found");
//        }
//    }

//    // This method is kept for compatibility but no longer used since unlock logic is in TurretUpgradeManager  
//    [System.Obsolete("Use TurretUpgradeManager.UnlockTurret() instead")]
//    public void Unlock(int turretId)
//    {
//        if (TurretUpgradeManager.instance != null)
//        {
//            TurretUpgradeManager.instance.UnlockTurret(turretId);
//        }
//        else
//        {
//            Debug.LogError("TabViewUpgrade: TurretUpgradeManager instance not found");
//        }
//    }

//    private void OnDestroy()
//    {
//        disposables?.Dispose();
//    }
//}