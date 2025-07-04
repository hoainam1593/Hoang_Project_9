using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TurretUpgradeInfoUICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textName;
    [SerializeField] private TextMeshProUGUI textAttack;
    [SerializeField] private TextMeshProUGUI textRange;
    [SerializeField] private TextMeshProUGUI textSpeed;
    [SerializeField] private TextMeshProUGUI textPrice;
    [SerializeField] private TextMeshProUGUI textMaxLevel;
    [SerializeField] private Button buttonUnlock;
    [SerializeField] private Button buttonUpgrade;

    public void InitView(int turretId)
    {
        var defaultConfig = ConfigManager.instance.GetConfig<TurretConfig>().GetItem(turretId);
        if (defaultConfig == null)
        {
            Debug.LogError($"TurretUpgradeInfoUICtrl: No turret config found for turretId {turretId}");
            return;
        }

        var upgradeInfo = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>().GetItem(turretId);
        if (upgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeInfoUICtrl: No upgrade info found for turretId {turretId}");
            return;
        }

        var crrLevel = upgradeInfo.upgradeLv;
        var upgradeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>().GetItem(turretId);
        if (upgradeConfig == null)
        {
            Debug.LogError($"TurretUpgradeInfoUICtrl: No upgrade config found for turretId {turretId}");
            return;
        }

        var maxUpgradeLv = upgradeConfig.bonusStats.Count;

        // Set turret name
        textName.text = $"{defaultConfig.prefabName} Lv{defaultConfig.level}";

        if (crrLevel == -1)
        {
            // Locked state
            textAttack.text = "Attack: Locked";
            textRange.text = "Range: Locked";
            textSpeed.text = "Speed: Locked";
            textPrice.text = "Price: -";
            buttonUnlock.gameObject.SetActive(true);
            buttonUpgrade.gameObject.SetActive(false);
            textMaxLevel.gameObject.SetActive(false);
        }
        else if (crrLevel == 0)
        {
            // Unlocked but inactive - show base stats
            textAttack.text = $"Attack: {defaultConfig.attack}";
            textRange.text = $"Range: {defaultConfig.range}";
            textSpeed.text = $"Speed: {defaultConfig.speed}";
            
            // Show next upgrade cost (level 1)
            var nextUpgradeStats = upgradeConfig.GetBonusStats(1);
            if (nextUpgradeStats != null)
            {
                textPrice.text = $"Price: {nextUpgradeStats.cost}";
            }
            else
            {
                textPrice.text = "Price: -";
            }

            buttonUnlock.gameObject.SetActive(false);
            buttonUpgrade.gameObject.SetActive(true);
            textMaxLevel.gameObject.SetActive(false);
        }
        else
        {
            // Active with upgrade level - show base stats + bonus stats
            var bonusStats = upgradeConfig.GetBonusStats(crrLevel);
            if (bonusStats != null)
            {
                textAttack.text = $"Attack: {defaultConfig.attack + bonusStats.attack}";
                textRange.text = $"Range: {defaultConfig.range + bonusStats.range}";
                textSpeed.text = $"Speed: {defaultConfig.speed + bonusStats.speed}";
            }
            else
            {
                // Fallback to base stats if bonus stats not found
                textAttack.text = $"Attack: {defaultConfig.attack}";
                textRange.text = $"Range: {defaultConfig.range}";
                textSpeed.text = $"Speed: {defaultConfig.speed}";
            }

            // Show next upgrade cost if not at max level
            if (crrLevel < maxUpgradeLv)
            {
                var nextUpgradeStats = upgradeConfig.GetBonusStats(crrLevel + 1);
                if (nextUpgradeStats != null)
                {
                    textPrice.text = $"Price: {nextUpgradeStats.cost}";
                }
                else
                {
                    textPrice.text = "Price: -";
                }
            }

            buttonUnlock.gameObject.SetActive(false);
            buttonUpgrade.gameObject.SetActive(true);
            textMaxLevel.gameObject.SetActive(false);
        }

        // Check if at max level
        if (crrLevel >= maxUpgradeLv && crrLevel > 0)
        {
            buttonUnlock.gameObject.SetActive(false);
            buttonUpgrade.gameObject.SetActive(false);
            textMaxLevel.gameObject.SetActive(true);
            textPrice.text = "Max Level";
        }
    }
}
