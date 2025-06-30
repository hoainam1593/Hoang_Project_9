
using UnityEngine;
using System.Collections.Generic;
using R3;
using Cysharp.Threading.Tasks;

public class HealthBarManager : SingletonMonoBehaviour<HealthBarManager>
{
    private Dictionary<int, HealthBarCtrl> healthBars = new Dictionary<int, HealthBarCtrl>();
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private ObjectPool pool;

    private const string HealthBarPrefabFolder = "";

    public async UniTaskVoid CreateHealthBar(int enemyUid, ReactiveProperty<Vector3> pos, float maxHp, ReactiveProperty<float> crrHp)
    {
        if (healthBars.ContainsKey(enemyUid))
        {
            return;
        }
        
        var go = await pool.Spawn("EnemyHealthBar");
        var healthBarCtrl = go.GetOrAddComponent<HealthBarCtrl>();
        healthBarCtrl.Init(pos, maxHp, crrHp);

        if (!healthBars.ContainsKey(enemyUid))
        {
            healthBars.Add(enemyUid, healthBarCtrl);
        }
    }

    public void DespawnHealthBar(int enemyUid)
    {
        if (!healthBars.ContainsKey(enemyUid))
        {
            return;
        }
        healthBars[enemyUid].OnDespawn();
        healthBars.Remove(enemyUid);
    }
}
