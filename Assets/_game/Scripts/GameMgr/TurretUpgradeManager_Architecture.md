# TurretUpgradeManager Architecture

## Overview
The `TurretUpgradeManager` is a singleton class that serves as a centralized module for all turret upgrade functionality. It uses R3 ReactiveProperty for automatic UI updates and provides a clean separation between business logic and UI components.

## Key Features

### 1. **Centralized Upgrade Logic**
- All upgrade operations (unlock, upgrade) are handled by the manager
- Validates upgrade conditions (locked state, max level, resources)
- Maintains data consistency between model and UI

### 2. **ReactiveProperty Integration**
- Each turret has its own `ReactiveProperty<int>` for upgrade level
- UI components automatically update when upgrade levels change
- Global observables for upgrade and unlock events

### 3. **Comprehensive Query API**
- `IsLocked(turretId)` - Check if turret is locked (-1)
- `IsUnlocked(turretId)` - Check if turret is unlocked but inactive (0)
- `IsActive(turretId)` - Check if turret is active (>0)
- `CheckMaxUpgradeLevel(turretId)` - Check if at maximum level
- `GetCurrentLevel(turretId)` - Get current upgrade level
- `GetMaxLevel(turretId)` - Get maximum possible level

### 4. **Stats Calculation**
- `GetCurrentTotalStats(turretId)` - Base stats + upgrade bonuses
- `GetStatsNextLevel(turretId)` - Stats for next upgrade level
- `GetNextUpgradeCost(turretId)` - Cost for next upgrade

## Architecture Benefits

### 1. **Separation of Concerns**
- **TurretUpgradeManager**: Business logic, data validation, state management
- **TurretUpgradeButton**: Display logic, user interaction
- **TurretUpgradeInfoUICtrl**: Detailed information display, upgrade actions
- **TabViewUpgrade**: UI navigation and selection

### 2. **Reactive UI Updates**
- UI components subscribe to ReactiveProperties
- Automatic updates when data changes
- No manual UI refresh needed

### 3. **Data Consistency**
- Single source of truth for upgrade data
- Centralized validation prevents invalid states
- Model and UI always in sync

### 4. **Extensibility**
- Easy to add new upgrade features
- Clean API for external systems
- Event system for cross-component communication

## Usage Examples

### Subscribe to Upgrade Level Changes
```csharp
var upgradeLevelProperty = TurretUpgradeManager.instance.GetTurretUpgradeLevelProperty(turretId);
upgradeLevelProperty.Subscribe(level => UpdateUI(level));
```

### Perform Upgrade Operations
```csharp
// Unlock a turret
bool success = TurretUpgradeManager.instance.UnlockTurret(turretId);

// Upgrade a turret
bool success = TurretUpgradeManager.instance.UpgradeTurret(turretId);
```

### Query Turret State
```csharp
if (TurretUpgradeManager.instance.IsLocked(turretId))
{
    // Show unlock options
}
else if (TurretUpgradeManager.instance.CheckMaxUpgradeLevel(turretId))
{
    // Show max level UI
}
```

### Get Stats Information
```csharp
var (attack, range, speed) = TurretUpgradeManager.instance.GetCurrentTotalStats(turretId);
var nextUpgradeCost = TurretUpgradeManager.instance.GetNextUpgradeCost(turretId);
```

## Migration from TabViewUpgrade

The old `TabViewUpgrade.Upgrade()` method has been deprecated in favor of:
- Direct button callbacks in `TurretUpgradeInfoUICtrl`
- Centralized logic in `TurretUpgradeManager`
- Reactive UI updates through subscriptions

This new architecture provides better maintainability, cleaner code separation, and more robust state management.