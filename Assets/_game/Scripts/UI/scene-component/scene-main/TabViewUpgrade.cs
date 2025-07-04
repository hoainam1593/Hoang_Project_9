using R3;
using System.Collections.Generic;
using UnityEngine;

public class TabViewUpgrade : MonoBehaviour
{
    [SerializeField] private List<TurretUpgradeButton> buttons;
    [SerializeField] private TurretUpgradeInfoUICtrl upgradeInfoPanel;

    private ReactiveProperty<int> selectedTurretId = new ReactiveProperty<int>(0);

    TurretUpgradeModel turretUpgradeModel;

    private void Start()
    {
        turretUpgradeModel = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>();

        foreach (var button in buttons)
        {
            button.Init(selectedTurretId);
            button.SetCallback(SelectTurretButton);
        }

        selectedTurretId.Subscribe((turretId) =>
        {
            ChangeUpgradeInfo(turretId);
        });
    }

    private void SelectTurretButton(int turretId)
    {
        if (turretId < 0 || turretId >= buttons.Count)
        {
            Debug.LogError($"Invalid turretId: {turretId}");
            return;
        }

        if (turretId == selectedTurretId.Value)
        {
            return; // No change, do nothing
        }

        selectedTurretId.Value = turretId;
    }

    private void ChangeUpgradeInfo(int turretId)
    {
        upgradeInfoPanel.InitView(turretId);
    }

    public void Upgrade(int turretId)
    {
        var upgradeInfo = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>().GetItem(turretId);
        if (upgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeModel: No upgrade info found for turretId {turretId}");
            return;
        }

        // Check if turret is locked
        if (upgradeInfo.upgradeLv == -1)
        {
            Debug.LogError($"TurretUpgradeModel: Cannot upgrade locked turret {turretId}");
            return;
        }

        var turretUpgradeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>().GetItem(turretId);
        if (turretUpgradeConfig == null)
        {
            Debug.LogError($"TurretUpgradeModel: No turret upgrade config found for turretId {turretId}");
            return;
        }

        // Get the next upgrade level
        int nextLevel = upgradeInfo.upgradeLv + 1;
        var nextUpgradeStats = turretUpgradeConfig.GetBonusStats(nextLevel);
        if (nextUpgradeStats == null)
        {
            Debug.LogError($"TurretUpgradeModel: No upgrade stats found for turretId {turretId} level {nextLevel}");
            return;
        }

        // Check if we have reached max level
        if (nextLevel > turretUpgradeConfig.bonusStats.Count)
        {
            Debug.LogWarning($"TurretUpgradeModel: Turret {turretId} is already at max level");
            return;
        }

        // TODO: Check if player has enough resources to upgrade (cost check)
        // For now, just proceed with the upgrade

        // Increment the upgrade level
        upgradeInfo.upgradeLv = nextLevel;

        // Note: The bonus stats are applied when calculating display values in UI
        // The TurretUpgradeInfo doesn't store cumulative stats, just the level

        // Refresh the UI
        ChangeUpgradeInfo(turretId);

        // Refresh the button display
        foreach (var button in buttons)
        {
            if (button.TurretId == turretId)
            {
                // Force button to refresh its display
                button.gameObject.SetActive(false);
                button.gameObject.SetActive(true);
                break;
            }
        }
    }
}