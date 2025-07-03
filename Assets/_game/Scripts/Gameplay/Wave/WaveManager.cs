using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Linq;

/// <summary>
/// Central wave manager that controls wave spawning for the entire game
/// Ensures only one wave is active at a time and manages wave progression
/// </summary>
public class WaveManager : SingletonMonoBehaviour<WaveManager>
{
    [Header("Wave Manager Settings")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float delayBetweenWaves = 3f;
    [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;

    private MapWaveConfig mapWaveConfig;
    private List<Wave> currentMapWaves = new List<Wave>();
    private int currentWaveIndex = -1;
    private Wave currentWave;
    private bool isWaveSystemActive = false;
    private int currentMapId = -1;

    public bool IsWaveActive => currentWave != null && !currentWave.IsCompleted;
    public bool IsMapCompleted => currentWaveIndex >= currentMapWaves.Count - 1 && (currentWave?.IsCompleted ?? true);
    public int CurrentWaveIndex => currentWaveIndex;
    public int TotalWaves => currentMapWaves.Count;
    public Wave CurrentWave => currentWave;

    protected override void Awake()
    {
        base.Awake();
        SubscribeToEvents();
    }

    #region Public API

    /// <summary>
    /// Initialize wave system for a specific map
    /// </summary>
    public void Initialize(int mapId)
    {
        try
        {
            if (enableDebugLogs) Debug.Log($"[WaveManager] Initializing map {mapId}");

            currentMapId = mapId;
            mapWaveConfig = ConfigManager.instance.GetConfig<MapWaveConfig>();
            foreach (var mapWave in mapWaveConfig.listConfigItems)
            {
                if (mapWave.mapId == mapId)
                {
                    if (enableDebugLogs) Debug.Log($"[WaveManager] > InitializeMap > MapWaveConfig: {mapWave}");
                    break;
                }
            }

            var waveConfigs = mapWaveConfig.GetItem(mapId).waves;
            if (waveConfigs == null || waveConfigs.Count == 0)
            {
                Debug.LogError($"[WaveManager] No waves found for map {mapId}");
                return;
            }

            // Create wave objects from configuration
            currentMapWaves.Clear();
            foreach (var waveConfig in waveConfigs)
            {
                if (enableDebugLogs) Debug.Log("[WaveManager] > InitializeMap > WaveConfig: " + waveConfig);
                var wave = new Wave(waveConfig);
                currentMapWaves.Add(wave);
            }

            currentWaveIndex = -1;
            currentWave = null;
            isWaveSystemActive = false;

            GameEventMgr.GED.DispatcherEvent(GameEvent.OnWaveManagerInit, currentMapWaves.Count);
            if (enableDebugLogs) Debug.Log($"[WaveManager] Map {mapId} initialized with {currentMapWaves.Count} waves");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[WaveManager] Failed to initialize map {mapId}: {ex.Message}");
        }
    }

    /// <summary>
    /// Start the wave system for the current map
    /// </summary>
    public async UniTaskVoid StartWaveSystem()
    {
        if (currentMapWaves.Count == 0)
        {
            Debug.LogError("[WaveManager] Cannot start wave system: No map initialized");
            return;
        }

        if (isWaveSystemActive)
        {
            Debug.LogWarning("[WaveManager] Wave system is already active");
            return;
        }

        if (enableDebugLogs) Debug.Log("[WaveManager] Starting wave system");
        
        isWaveSystemActive = true;
        await ProcessWaves();
    }

    /// <summary>
    /// Start the next wave if available
    /// </summary>
    public async UniTask<int> StartNextWave()
    {
        if (!isWaveSystemActive)
        {
            Debug.LogWarning("[WaveManager] Wave system is not active");
            return -1;
        }

        if (IsWaveActive)
        {
            Debug.LogWarning("[WaveManager] Cannot start next wave: Current wave is still active");
            return -1;
        }

        currentWaveIndex++;
        
        if (currentWaveIndex >= currentMapWaves.Count)
        {
            await CompleteAllWaves();
            return -1;
        }

        currentWave = currentMapWaves[currentWaveIndex];
        
        if (enableDebugLogs) Debug.Log($"[WaveManager] Starting wave {currentWaveIndex + 1}/{currentMapWaves.Count}");
        
        currentWave.StartWave(defaultSpawnPosition).Forget();
        return 1;
    }

    /// <summary>
    /// Stop the current wave system
    /// </summary>
    public void StopWaveSystem()
    {
        if (enableDebugLogs) Debug.Log("[WaveManager] Stopping wave system");
        
        isWaveSystemActive = false;
        
        // Stop current wave if active
        currentWave?.StopWave();
        currentWave = null;
        currentWaveIndex = -1;
    }

    /// <summary>
    /// Reset the wave system
    /// </summary>
    public void ResetWaveSystem()
    {
        StopWaveSystem();
        
        foreach (var wave in currentMapWaves)
        {
            wave.ResetWave();
        }
        
        if (enableDebugLogs) Debug.Log("[WaveManager] Wave system reset");
    }

    /// <summary>
    /// Set the spawn position for waves
    /// </summary>
    public void SetSpawnPosition(Vector3 position)
    {
        defaultSpawnPosition = position;
        if (enableDebugLogs) Debug.Log($"[WaveManager] Spawn position set to {position}");
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Process all waves in sequence
    /// </summary>
    private async UniTask ProcessWaves()
    {
        while (isWaveSystemActive && currentWaveIndex < currentMapWaves.Count - 1)
        {
            await StartNextWave();
            
            // Wait for current wave to complete
            await UniTask.WaitUntil(() => currentWave?.IsCompleted ?? true);
            
            if (enableDebugLogs) Debug.Log($"[WaveManager] Wave {currentWaveIndex + 1} completed. Waiting {delayBetweenWaves}s before next wave...");
            
            // Delay between waves
            if (currentWaveIndex < currentMapWaves.Count - 1)
            {
                await UniTask.WaitForSeconds(delayBetweenWaves);
            }
        }
        
        if (isWaveSystemActive)
        {
            await CompleteAllWaves();
        }
    }

    /// <summary>
    /// Complete all waves and finish the map
    /// </summary>
    private async UniTask CompleteAllWaves()
    {
        if (enableDebugLogs) Debug.Log("[WaveManager] All waves completed for map!");
        
        isWaveSystemActive = false;
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnAllWavesComplete, currentMapId);
        
        // Small delay before triggering victory
        await UniTask.WaitForSeconds(1f);
        
        // Check if this should trigger victory condition
        GameManager.instance?.Victory();
    }

    #endregion

    #region Event Handling

    /// <summary>
    /// Subscribe to relevant game events
    /// </summary>
    private void SubscribeToEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnEnemyDespawnCompleted, OnEnemyDespawned);
        
        if (enableDebugLogs) Debug.Log("[WaveManager] Subscribed to game events");
    }

    /// <summary>
    /// Unsubscribe from game events
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyDespawnCompleted, OnEnemyDespawned);
        
        if (enableDebugLogs) Debug.Log("[WaveManager] Unsubscribed from game events");
    }

    /// <summary>
    /// Handle enemy despawn events to track wave completion
    /// </summary>
    private void OnEnemyDespawned(object data)
    {
        if (currentWave != null && !currentWave.IsCompleted)
        {
            currentWave.OnEnemyDespawned();
        }
    }

    protected override void OnDestroy()
    {
        UnsubscribeFromEvents();
        base.OnDestroy();
    }

    #endregion

    #region Debug & Utility

    /// <summary>
    /// Get current wave information for UI display
    /// </summary>
    public string GetWaveInfo()
    {
        if (currentMapWaves.Count == 0)
            return "No map loaded";
            
        return $"Wave {currentWaveIndex + 1}/{currentMapWaves.Count}";
    }

    /// <summary>
    /// Get detailed wave status
    /// </summary>
    public string GetDetailedWaveStatus()
    {
        if (currentWave == null)
            return "No active wave";
            
        return $"Wave {currentWave.WaveId}: {currentWave.State} - Spawned: {currentWave.SpawnedCount}/{currentWave.Config.num}, Alive: {currentWave.AliveCount}";
    }

    #endregion
}