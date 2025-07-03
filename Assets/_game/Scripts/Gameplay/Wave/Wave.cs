using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

/// <summary>
/// Manages a single wave of enemies with spawn timing and completion tracking
/// </summary>
public class Wave
{
    public int WaveId { get; private set; }
    public WaveState State { get; private set; }
    public WaveConfigItem Config { get; private set; }
    public int SpawnedCount { get; private set; }
    public int AliveCount { get; private set; }
    public bool IsCompleted => State == WaveState.Completed;

    private List<int> spawnedEnemyIds = new List<int>();
    private bool isSpawning = false;

    public Wave(WaveConfigItem config)
    {
        Config = config;
        WaveId = config.id;
        State = WaveState.Waiting;
        SpawnedCount = 0;
        AliveCount = 0;
    }

    /// <summary>
    /// Start the wave spawning process
    /// </summary>
    public async UniTaskVoid StartWave(Vector3 spawnPosition)
    {
        if (State != WaveState.Waiting)
        {
            Debug.LogWarning($"[Wave] Cannot start wave {WaveId}, current state: {State}");
            return;
        }

        Debug.Log($"[Wave] Starting wave {WaveId} - {Config.enemyType} x{Config.num} (gap: {Config.gapTime}s)");
        
        ChangeState(WaveState.Spawning);
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnWaveStart, (WaveId, Config.num));
        
        await SpawnEnemies(spawnPosition);
    }

    /// <summary>
    /// Spawn enemies according to wave configuration
    /// </summary>
    private async UniTask SpawnEnemies(Vector3 spawnPosition)
    {
        isSpawning = true;
        spawnedEnemyIds.Clear();

        for (int i = 0; i < Config.num; i++)
        {
            if (!isSpawning) break; // Allow early termination

            // Get enemy ID from EnemyType
            int enemyId = (int)Config.enemyType;
            
            // Spawn enemy through EntityManager
            EntityManager.instance.SpawnEnemy(spawnPosition, enemyId).Forget();
            
            SpawnedCount++;
            AliveCount++;
            
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnWaveEnemySpawned, new { waveId = WaveId, enemyType = Config.enemyType, spawnedCount = SpawnedCount });
            
            Debug.Log($"[Wave] Spawned enemy {i + 1}/{Config.num} for wave {WaveId}");

            // Wait for gap time before next spawn (except for the last enemy)
            if (i < Config.num - 1)
            {
                await UniTask.WaitForSeconds(Config.gapTime);
            }
        }

        isSpawning = false;
        ChangeState(WaveState.Active);
        
        Debug.Log($"[Wave] Wave {WaveId} spawning completed. All {Config.num} enemies spawned.");
    }

    /// <summary>
    /// Called when an enemy from this wave is despawned/killed
    /// </summary>
    public void OnEnemyDespawned()
    {
        if (AliveCount > 0)
        {
            AliveCount--;
            Debug.Log($"[Wave] Enemy despawned from wave {WaveId}. Remaining: {AliveCount}");
            
            // Check if wave is completed
            if (AliveCount == 0 && State == WaveState.Active)
            {
                CompleteWave();
            }
        }
    }

    /// <summary>
    /// Force complete the wave
    /// </summary>
    public void CompleteWave()
    {
        if (State == WaveState.Completed) return;

        Debug.Log($"[Wave] Wave {WaveId} completed!");
        
        ChangeState(WaveState.Completed);
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnWaveComplete, WaveId);
    }

    /// <summary>
    /// Stop the wave (for cleanup purposes)
    /// </summary>
    public void StopWave()
    {
        isSpawning = false;
        spawnedEnemyIds.Clear();
        ChangeState(WaveState.Completed);
    }

    /// <summary>
    /// Reset wave to initial state
    /// </summary>
    public void ResetWave()
    {
        State = WaveState.Waiting;
        SpawnedCount = 0;
        AliveCount = 0;
        isSpawning = false;
        spawnedEnemyIds.Clear();
    }

    private void ChangeState(WaveState newState)
    {
        var oldState = State;
        State = newState;
        
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnWaveStateChanged, new { waveId = WaveId, oldState, newState });
        
        Debug.Log($"[Wave] Wave {WaveId} state changed: {oldState} -> {newState}");
    }
}