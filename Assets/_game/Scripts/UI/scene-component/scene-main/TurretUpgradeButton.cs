using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurretUpgradeButton : MonoBehaviour
{
    [SerializeField] private int turretId;
    [SerializeField] private Image iconTurret;
    [SerializeField] private Image iconLocked;
    [SerializeField] private TextMeshProUGUI textLv;
    private Button button;
    private Action<int> onClickCallback;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public int TurretId => turretId;

    private void Start()
    {
        button.onClick.AddListener(() => onClickCallback?.Invoke(turretId));

        //InitView
        var turretUpgradeInfo = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>().GetItem(turretId);
        if (turretUpgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeButton: No upgrade info found for turretId {turretId}");
            return;
        }

        var upgradeLv = turretUpgradeInfo.upgradeLv;

        textLv.gameObject.SetActive(true);
        iconTurret.gameObject.SetActive(true);

        if (upgradeLv <= 0)
        {
            iconLocked.gameObject.SetActive(true);
            textLv.text = "Locked";
        }
        else
        {
            iconLocked.gameObject.SetActive(false);
            textLv.text = $"Lv.{upgradeLv}";
        }
    }

    public void SetCallback(Action<int> callback)
    {
        onClickCallback = callback;
    }
}
