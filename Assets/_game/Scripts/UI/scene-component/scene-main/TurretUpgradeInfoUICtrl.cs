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
        var upgradeInfo = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>().GetItem(turretId);
        var crrLevel = upgradeInfo.upgradeLv;

        var upgardeConfig = ConfigManager.instance.GetConfig<TurretUpgradeConfig>().GetItem(turretId);
        if (upgardeConfig == null)
        {
            Debug.LogError($"TurretUpgradeInfoUICtrl: No upgrade config found for turretId {turretId}");
            return;
        }
        var maxUpgradeLv = upgardeConfig.bonusStats.Count;
        var upgradeConfigItem = upgardeConfig.GetBonusStats(crrLevel);

        textName.text = $"Turret Lv{defaultConfig.level}";

        if (crrLevel <= 0)
        {
            //Locked
            textAttack.text = "Attack: -";
            textRange.text = "Range: -";
            textSpeed.text = "Speed: -";
            buttonUnlock.gameObject.SetActive(true);
            buttonUpgrade.gameObject.SetActive(false);
            textMaxLevel.gameObject.SetActive(false);
        }
        else
        {
            //get default turret config

            //get bonus stats
            var bonusStats = upgardeConfig.GetBonusStats(crrLevel);
            textAttack.text = $"Attack: {defaultConfig.attack + bonusStats.attack}";
            textRange.text = $"Range: {defaultConfig.range + bonusStats.range}";
            textSpeed.text = $"Speed: {defaultConfig.speed + bonusStats.speed}";

            //get price
            textPrice.text = $"Price: {upgradeConfigItem.cost}";

            buttonUnlock.gameObject.SetActive(false);
            buttonUpgrade.gameObject.SetActive(true);
            textMaxLevel.gameObject.SetActive(false);
        }

        if (crrLevel >= maxUpgradeLv)
        {
            buttonUnlock.gameObject.SetActive(false);
            buttonUpgrade.gameObject.SetActive(false);
            textMaxLevel.gameObject.SetActive(true);
        }
    }
}
