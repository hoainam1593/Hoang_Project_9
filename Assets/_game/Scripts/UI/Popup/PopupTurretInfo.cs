using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PopupTurretInfo : BasePopup
{
    [Space(10)]
    [SerializeField] private Button buttonUpgrade;
    [SerializeField] private Button buttonSell;
    [SerializeField] private TextMeshProUGUI textUpgradeCost;
    [SerializeField] private TextMeshProUGUI textSellPrice;
    [SerializeField] private Image mask;

    private MapCoordinate mapCoordinate;
    private Vector3 pos;
    private TurretCtrl currentTurretCtrl;
    private TurretConfigItem currentTurretConfig;
    private int upgradeCost;
    private int sellPrice;
    private const float sellRatio = 0.75f;
    
    protected override void Start()
    {
        base.Start();
        OnStart();
    }

    private void OnStart()
    {
        // Debug.Log("OnStart");
        //Subscribes Event:
        buttonUpgrade.onClick.AddListener(() =>
        {
            UpgradeTurret();
            ClosePopup();
        });

        buttonSell.onClick.AddListener(() =>
        {
            SellTurret();
            ClosePopup();
        });        
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);
        
        //UnSubscribes Event
        buttonUpgrade.onClick.RemoveAllListeners();
        buttonSell.onClick.RemoveAllListeners();
    }

    public void InitView(MapCoordinate mapCoordinate, Vector3 worldPos, Vector3 size)
    {
        InitData(mapCoordinate, worldPos);
        SetRectTransform(worldPos);
        UpdateCostAndButtons();
    }

    private void InitData(MapCoordinate mapCoordinate, Vector3 worldPos)
    {
        this.mapCoordinate = mapCoordinate;
        this.pos = worldPos;
        
        // Get current turret from EntityManager
        currentTurretCtrl = EntityManager.instance.GetTurretCtrl(mapCoordinate);
        if (currentTurretCtrl != null)
        {
            currentTurretConfig = currentTurretCtrl.Config;
        }
    }

    private void SetRectTransform(Vector3 worldPos)
    {
        transform.position = worldPos;
        transform.localScale = Vector3.one * (1f / 100f); //100 here is ratio of UI space / world space
    }

    private void UpdateCostAndButtons()
    {
        if (currentTurretConfig == null)
        {
            Debug.LogError("PopupTurretInfo: Current turret config is null");
            return;
        }

        // Calculate upgrade cost
        var turretConfig = ConfigManager.instance.GetConfig<TurretConfig>();
        var nextLevelConfig = turretConfig.GetItem(currentTurretConfig.type, currentTurretConfig.level + 1);
        
        if (nextLevelConfig != null)
        {
            upgradeCost = nextLevelConfig.cost;
            textUpgradeCost.text = $"{upgradeCost}";
            
            // Check if player has enough coins for upgrade
            int currentCoin = PlayerCtrl.instance.GetCurrentCoin();
            buttonUpgrade.interactable = currentCoin >= upgradeCost;
            mask.enabled = currentCoin < upgradeCost;
        }
        else
        {
            // Max level reached
            textUpgradeCost.text = "MAX";
            buttonUpgrade.interactable = false;
            mask.enabled = false;
        }

        // Calculate sell price (total cost of this level + all lower levels * ratio)
        sellPrice = CalculateTotalCost(currentTurretConfig.type, currentTurretConfig.level);
        sellPrice = Mathf.RoundToInt(sellPrice * sellRatio);
        textSellPrice.text = $"{sellPrice}";
    }

    private int CalculateTotalCost(TurretType turretType, int currentLevel)
    {
        int totalCost = 0;
        var turretConfig = ConfigManager.instance.GetConfig<TurretConfig>();
        
        for (int level = 1; level <= currentLevel; level++)
        {
            var levelConfig = turretConfig.GetItem(turretType, level);
            if (levelConfig != null)
            {
                totalCost += levelConfig.cost;
            }
        }
        
        return totalCost;
    }
    
    private void UpgradeTurret()
    {
        // EntityManager now handles all coin logic internally
        EntityManager.instance.UpgradeTurret(mapCoordinate);
    }

    private void SellTurret()
    {
        // EntityManager now handles all coin logic and despawning internally
        EntityManager.instance.SellTurret(mapCoordinate);
    }
}

