using UnityEngine;
using TMPro;
using R3;

public class PlayerInfoCtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private TextMeshProUGUI textCoin;

    private ReactiveProperty<int> hp;
    private ReactiveProperty<int> coin;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerConfig = ConfigManager.instance.GetConfig<PlayerConfig>().GetFirst();
        hp = new ReactiveProperty<int>(playerConfig.hp);
        coin = new ReactiveProperty<int>(playerConfig.coin);

        hp.Subscribe((value) =>
        {
            textHp.text = $"{value}";
        });

        coin.Subscribe((value) =>
        {
            textCoin.text = $"{value}";
        });

        //SubscribeEvents
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeEvents();
    }


    #region SubscribeEvents / UnSubscribeEvents
    private void SubscribeEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
    }

    private void UnSubscribeEvents()
    {
        // Unsubscribe from events to prevent memory leaks
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
    }

    private void OnEnemyReachHallGate(object data)
    {
        hp.Value -= 1;
    }
    #endregion SubscribeEvents / UnSubscribeEvents!!!
}
