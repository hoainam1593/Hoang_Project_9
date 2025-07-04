using UnityEngine;

public partial class EntityManager
{
    /// <summary>
    /// Upgrade turret at specified map coordinate with coin handling
    /// </summary>
    /// <param name="mapCoordinate">Map coordinate of the turret to upgrade</param>
    /// <returns>True if upgrade was successful</returns>
    public bool UpgradeTurret(MapCoordinate mapCoordinate)
    {
        // Get current turret
        if (!turretCtrls.ContainsKey(mapCoordinate))
        {
            Debug.LogWarning($"EntityManager.UpgradeTurret: No turret found at coordinate {mapCoordinate}");
            return false;
        }

        TurretCtrl currentTurretCtrl = turretCtrls[mapCoordinate];
        TurretConfigItem currentConfig = currentTurretCtrl.Config;
        
        if (currentConfig == null)
        {
            Debug.LogError("EntityManager.UpgradeTurret: Current turret config is null");
            return false;
        }

        // Get next level turret config
        var turretConfig = ConfigManager.instance.GetConfig<TurretConfig>();
        var nextLevelConfig = turretConfig.GetItem(currentConfig.type, currentConfig.level + 1);
        
        if (nextLevelConfig == null)
        {
            Debug.LogWarning($"EntityManager.UpgradeTurret: No next level available for turret type {currentConfig.type} level {currentConfig.level}");
            return false;
        }

        // Check if player has enough coins and consume them
        int upgradeCost = nextLevelConfig.cost;
        if (!PlayerCtrl.instance.HasEnoughCoin(upgradeCost))
        {
            Debug.LogWarning($"EntityManager.UpgradeTurret: Not enough coins for upgrade. Required: {upgradeCost}, Available: {PlayerCtrl.instance.GetCurrentCoin()}");
            return false;
        }

        // Consume coins for upgrade
        if (!PlayerCtrl.instance.ConsumeCoin(upgradeCost))
        {
            Debug.LogError("EntityManager.UpgradeTurret: Failed to consume coins for upgrade");
            return false;
        }

        // Get position for spawning new turret
        Vector3 turretPosition = currentTurretCtrl.transform.position;
        
        // Despawn current turret
        currentTurretCtrl.OnDespawn();
        DespawnEntity(EntityType.Turret, currentTurretCtrl.gameObject);
        turretCtrls.Remove(mapCoordinate);
        
        // Spawn upgraded turret
        SpawnTurret(mapCoordinate, turretPosition, nextLevelConfig.id).Forget();
        
        Debug.Log($"EntityManager.UpgradeTurret: Successfully upgraded turret at {mapCoordinate} from level {currentConfig.level} to level {nextLevelConfig.level}");
        return true;
    }

    /// <summary>
    /// Sell turret at specified map coordinate with coin rewards
    /// </summary>
    /// <param name="mapCoordinate">Map coordinate of the turret to sell</param>
    /// <returns>True if sell was successful</returns>
    public bool SellTurret(MapCoordinate mapCoordinate)
    {
        // Get current turret
        if (!turretCtrls.ContainsKey(mapCoordinate))
        {
            Debug.LogWarning($"EntityManager.SellTurret: No turret found at coordinate {mapCoordinate}");
            return false;
        }

        TurretCtrl currentTurretCtrl = turretCtrls[mapCoordinate];
        TurretConfigItem currentConfig = currentTurretCtrl.Config;
        
        if (currentConfig == null)
        {
            Debug.LogError("EntityManager.SellTurret: Current turret config is null");
            return false;
        }

        // Create turret info for events
        var turretInfo = new TurretInfo()
        {
            mapCoordinate = mapCoordinate,
            config = currentConfig,
        };

        // Dispatch turret despawn start event
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretDespawnStart, turretInfo);

        // Calculate sell price (total cost of this level + all lower levels * ratio)
        const float sellRatio = 0.75f;
        int sellPrice = CalculateTotalTurretCost(currentConfig.type, currentConfig.level);
        sellPrice = Mathf.RoundToInt(sellPrice * sellRatio);

        // Add coins to player
        PlayerCtrl.instance.AddCoin(sellPrice);
        
        // Despawn turret
        DespawnTurret(mapCoordinate);

        // Dispatch turret despawn complete event
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnTurretDespawnComplete, turretInfo);
        
        Debug.Log($"EntityManager.SellTurret: Successfully sold turret at {mapCoordinate} for {sellPrice} coins");
        return true;
    }

    /// <summary>
    /// Calculate total cost for a turret type up to specified level
    /// </summary>
    /// <param name="turretType">Type of turret</param>
    /// <param name="currentLevel">Current level of the turret</param>
    /// <returns>Total cost of all levels up to current level</returns>
    private int CalculateTotalTurretCost(TurretType turretType, int currentLevel)
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
}