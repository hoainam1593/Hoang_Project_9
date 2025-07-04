using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PopupTurretSelect : BasePopup
{
    [Space(10)]
    [SerializeField] private Button gunButton;
    [SerializeField] private TextMeshProUGUI textCost;
    [SerializeField] private Image mask;

    private MapCoordinate mapCoordinate;
    private Vector3 pos;
    private int turretCost;
    
    protected override void Start()
    {
        base.Start();

        OnStart();
    }

    private void OnStart()
    {        

        //Subscribes Event:
        gunButton.onClick.AddListener(() =>
        {
            SpawnTurretGun();
            ClosePopup();
        });
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);
        
        //UnSubscribes Event
        gunButton.onClick.RemoveAllListeners();
    }

    public void InitView(MapCoordinate mapCoordinate, Vector3 worldPos, Vector3 size)
    {
        InitData(mapCoordinate, worldPos);

        SetRectTransform(worldPos);

        UpdateCostAndButton();
    }

    private void SetRectTransform(Vector3 worldPos)
    {
        transform.position = worldPos;
        transform.localScale = Vector3.one * (1f / 100f); //100 here is ratio of UI space / world space
    }

    private void InitData(MapCoordinate mapCoordinate, Vector3 worldPos)
    {
        this.mapCoordinate = mapCoordinate;
        this.pos = worldPos;
        var turretConfig = ConfigManager.instance.GetConfig<TurretConfig>();
        var gunTurretItem = turretConfig.GetItem(TurretType.Gun, 1);
        turretCost = gunTurretItem.cost;
    }

    private void UpdateCostAndButton()
    {
        textCost.text = $"{turretCost}";
        int currentGold = PlayerCtrl.instance.GetCurrentCoin();
        gunButton.enabled = currentGold >= turretCost;
        mask.enabled = currentGold < turretCost;
    }

    private void SpawnTurretGun()
    {
        if (CurrencyManager.instance != null && turretCost > 0)
        {
            EntityManager.instance.SpawnTurret(mapCoordinate, pos, 0).Forget();

            int currentGold = PlayerCtrl.instance.GetCurrentCoin();
            CurrencyManager.instance.ConsumeGold(turretCost);
        }
    }
}

