using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpawnEnemy : MonoBehaviour
{
    [SerializeField] private int enemyId;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            var root = MapCtrl.instance.GetTilemapPath().GetRoot();
            var pos = MapCtrl.instance.ConvertTilePosToCenterTileWorldPos(root);
            EntityManager.instance.SpawnEnemy(pos, enemyId);
        });
    }
}
