using UnityEngine;
using TMPro;
using R3;

public class PlayerInfoCtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private TextMeshProUGUI textCoin;

    private ReactiveProperty<int> hp;
    private ReactiveProperty<int> coin;
    private PlayerConfigItem initialPlayerConfig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePlayerInfo();
    }

    private void OnDestroy()
    {
        UnSubscribeEvents();
    }

    /// <summary>
    /// Initialize player info with config values and set up subscriptions
    /// </summary>
    private void InitializePlayerInfo()
    {
        // Get and cache the initial player config
        initialPlayerConfig = ConfigManager.instance.GetConfig<PlayerConfig>().GetFirst();
        
        // Initialize reactive properties with config values
        if (hp == null)
        {
            hp = new ReactiveProperty<int>(initialPlayerConfig.hp);
            coin = new ReactiveProperty<int>(initialPlayerConfig.coin);
        }
        else
        {
            // Reset values to initial config values
            hp.Value = initialPlayerConfig.hp;
            coin.Value = initialPlayerConfig.coin;
        }

        // Set up UI subscriptions
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

    /// <summary>
    /// Reset player info to initial values (called on retry)
    /// </summary>
    private void ResetPlayerInfo()
    {
        if (initialPlayerConfig != null && hp != null && coin != null)
        {
            hp.Value = initialPlayerConfig.hp;
            coin.Value = initialPlayerConfig.coin;
        }
    }

    #region SubscribeEvents / UnSubscribeEvents
    private void SubscribeEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
        GameEventMgr.GED.Register(GameEvent.OnGameRetry, OnGameRetry);
    }

    private void UnSubscribeEvents()
    {
        // Unsubscribe from events to prevent memory leaks
        GameEventMgr.GED.UnRegister(GameEvent.OnEnemyReachHallGate, OnEnemyReachHallGate);
        GameEventMgr.GED.UnRegister(GameEvent.OnGameRetry, OnGameRetry);
    }

    /// <summary>
    /// Handle enemy reaching hall gate - reduce HP
    /// </summary>
    /// <param name="data">Event data (unused)</param>
    private void OnEnemyReachHallGate(object data)
    {
        if (hp.Value <= 0)
        {
            return; // No need to process if hp is already zero or less
        }
        hp.Value -= 1;
        if (hp.Value <= 0)
        {
            GameEventMgr.GED.DispatcherEvent(GameEvent.OnPlayerDeath);
        }
    }

    /// <summary>
    /// Handle game retry event - reset player info to initial values
    /// </summary>
    /// <param name="data">Event data (unused)</param>
    private void OnGameRetry(object data)
    {
        ResetPlayerInfo();
    }
    #endregion SubscribeEvents / UnSubscribeEvents!!!
}
