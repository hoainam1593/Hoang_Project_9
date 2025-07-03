using R3;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

        upgradeInfo.upgradeLv++;

        var turretUpgradeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>().GetItem(turretId).GetBonusStats(upgradeInfo.upgradeLv);
        if (turretUpgradeConfig == null)
        {
            Debug.LogError($"TurretUpgradeModel: No turret upgrade config found for turretId {turretId}");
            return;
        }

        upgradeInfo.attack += turretUpgradeConfig.attack;
        upgradeInfo.range += turretUpgradeConfig.range;
        upgradeInfo.speed += turretUpgradeConfig.speed;
    }
}