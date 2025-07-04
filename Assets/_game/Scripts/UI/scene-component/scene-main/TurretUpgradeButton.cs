using R3;
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
    [SerializeField] private Image selected_frame;
    [SerializeField] private Image mask;

    private Button button;
    private Action<int> onClickCallback;
    private ReactiveProperty<int> selectedTurretId;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public int TurretId => turretId;

    private void Start()
    {
        button.onClick.AddListener(() => onClickCallback?.Invoke(turretId));

        InitView();
    }

    private void InitView()
    {
        var turretUpgradeInfo = PlayerModelManager.instance.GetPlayerModel<TurretUpgradeModel>().GetItem(turretId);
        if (turretUpgradeInfo == null)
        {
            Debug.LogError($"TurretUpgradeButton: No upgrade info found for turretId {turretId}");
            return;
        }

        var upgradeLv = turretUpgradeInfo.upgradeLv;

        textLv.gameObject.SetActive(true);
        iconTurret.gameObject.SetActive(true);

        if (upgradeLv == -1)
        {
            // Locked state
            iconLocked.gameObject.SetActive(true);
            textLv.text = "Locked";
            // Make button grayscale and potentially non-interactable
            button.interactable = false;
        }
        else if (upgradeLv == 0)
        {
            // Unlocked but inactive
            iconLocked.gameObject.SetActive(false);
            textLv.text = "Unlocked";
            button.interactable = true;
        }
        else
        {
            // Active with upgrade level
            iconLocked.gameObject.SetActive(false);
            textLv.text = $"Lv.{upgradeLv}";
            button.interactable = true;
        }
    }

    public void Init(ReactiveProperty<int> selectedTurretId)
    {
        this.selectedTurretId = selectedTurretId;
        selectedTurretId.Subscribe((selectedId) =>
        {
            selected_frame.gameObject.SetActive(selectedId == turretId);
        });

        OnSelectTurret(selectedTurretId);
    }

    private void OnSelectTurret(ReactiveProperty<int> selectedTurretId)
    {
        if (selectedTurretId.Value == turretId)
        {
            selected_frame.gameObject.SetActive(true);
        }
        else
        {
            selected_frame.gameObject.SetActive(false);
        }
    }

    public void SetCallback(Action<int> callback)
    {
        onClickCallback = callback;
    }
}
