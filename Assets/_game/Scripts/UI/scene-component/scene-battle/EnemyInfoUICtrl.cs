using System.Security.Cryptography;
using TMPro;
using UnityEngine;

public class EnemyInfoUICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
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

    private void Start()
    {
        InitView();
    }

    private void InitView()
    {

    }


    #region SubscribeEvents / UnSubscribeEvents
    private void SubscribeEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnWaveStart, OnWaveStart);
        GameEventMgr.GED.Register(GameEvent.OnEnemySpawnCompleted, OnEnemySpawn);
    }

    private void UnSubscribeEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnWaveStart, OnWaveStart);
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemySpawnCompleted, OnEnemySpawn);
    }

    private void OnWaveStart(object data)
    {
        var crrWave = ((int waveId, int enemyCount))data;
        enemyWaveIdx = 0;
        enemyCount = crrWave.enemyCount;
        text.text = $"Enemy Count: {enemyWaveIdx}/{enemyCount}";
    }

    private void OnEnemySpawn(object data)
    {
        enemyWaveIdx++;
        text.text = $"Enemy Count: {enemyWaveIdx}/{enemyCount}";
    }
    #endregion SubscribeEvents / UnSubscribeEvents!!!
}
