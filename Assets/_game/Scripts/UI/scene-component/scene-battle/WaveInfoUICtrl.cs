using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class WaveInfoUICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private int waveIdx;
    private int waveCount;
    private int enemyCount;
    private int enemyWaveIdx;

    private void Awake()
    {
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeEvents();
    }

    private void UpdateText()
    {
        text.text = $"Wave: {waveIdx}/{waveCount} - Enemy Count: {enemyWaveIdx}/{enemyCount}";
    }


    #region SubscribeEvents / UnSubscribeEvents
    private void SubscribeEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnWaveManagerInit, OnWaveManagerInit);
        GameEventMgr.GED.Register(GameEvent.OnWaveStart, OnWaveStart);
        GameEventMgr.GED.Register(GameEvent.OnEnemySpawnCompleted, OnEnemySpawn);
    }

    private void UnSubscribeEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnWaveManagerInit, OnWaveManagerInit);
        GameEventMgr.GED.UnRegister(GameEvent.OnWaveStart, OnWaveStart);
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemySpawnCompleted, OnEnemySpawn);
    }

    private void OnWaveManagerInit(object data)
    {
        int waveCount = (int)data;
        this.waveCount = waveCount;
        this.waveIdx = 0;

        UpdateText();
    }

    private void OnWaveStart(object data)
    {
        var crrWave = ((int waveId, int enemyCount))data;
        enemyWaveIdx = 0;
        enemyCount = crrWave.enemyCount;
        waveIdx = crrWave.waveId;

        UpdateText();
    }

    private void OnEnemySpawn(object data)
    {
        enemyWaveIdx++;
        UpdateText();
    }
    #endregion SubscribeEvents / UnSubscribeEvents!!!
}
