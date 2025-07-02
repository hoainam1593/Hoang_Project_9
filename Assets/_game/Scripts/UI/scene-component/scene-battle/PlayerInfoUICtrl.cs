using UnityEngine;
using TMPro;
using R3;

/// <summary>
/// PlayerInfoCtrl handles the UI display for player information during battle.
/// It gets data from PlayerCtrl and updates the UI accordingly.
/// </summary>
public class PlayerInfoUICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private TextMeshProUGUI textCoin;

    private InGamePlayerInfo playerInfo;

    void Awake()
    {
        SubscribeEvents();
    }

    private void Start()
    {
        InitializePlayerInfo();
    }

    private void OnDestroy()
    {
        UnSubscribeEvents();
    }

    /// <summary>
    /// Initialize player info by getting data from PlayerCtrl
    /// </summary>
    private void InitializePlayerInfo()
    {
        // Get in-game player info from PlayerCtrl
        if (PlayerCtrl.instance != null)
        {
            playerInfo = PlayerCtrl.instance.GetInGamePlayerInfo();
            
            // Set up UI subscriptions
            if (playerInfo != null)
            {
                playerInfo.HP.Subscribe(value =>
                {
                    textHp.text = $"{value}";
                });

                playerInfo.Coin.Subscribe(value =>
                {
                    textCoin.text = $"{value}";
                });
            }
            else
            {
                Debug.LogError("PlayerInfoCtrl: Failed to get player info from PlayerCtrl");
                // Fallback to config values
                InitializeFallbackPlayerInfo();
            }
        }
        else
        {
            Debug.LogError("PlayerInfoCtrl: PlayerCtrl instance not found");
            // Fallback to config values
            InitializeFallbackPlayerInfo();
        }
    }

    /// <summary>
    /// Fallback initialization using config values (for backward compatibility)
    /// </summary>
    private void InitializeFallbackPlayerInfo()
    {
        var playerConfig = ConfigManager.instance.GetConfig<PlayerConfig>().GetFirst();
        if (playerConfig != null)
        {
            textHp.text = playerConfig.hp.ToString();
            textCoin.text = playerConfig.coin.ToString();
            
            Debug.LogWarning("PlayerInfoCtrl: Using fallback initialization with config values");
        }
    }

    /// <summary>
    /// Reset player info display (called on retry)
    /// </summary>
    private void ResetPlayerInfo()
    {
        // PlayerCtrl will handle the data reset, UI will update automatically through reactive properties
        if (PlayerCtrl.instance != null)
        {
            // Data is reset in PlayerCtrl.OnGameRetry, UI updates automatically
            Debug.Log("PlayerInfoCtrl: Player info reset handled by PlayerCtrl");
        }
        else
        {
            // Fallback reset
            InitializeFallbackPlayerInfo();
        }
    }

    #region SubscribeEvents / UnSubscribeEvents
    private void SubscribeEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnGameRetry, OnGameRetry);
    }

    private void UnSubscribeEvents()
    {
        // Unsubscribe from events to prevent memory leaks
        GameEventMgr.GED.UnRegister(GameEvent.OnGameRetry, OnGameRetry);
    }

    /// <summary>
    /// Handle game retry event - reset player info display
    /// </summary>
    /// <param name="data">Event data (unused)</param>
    private void OnGameRetry(object data)
    {
        ResetPlayerInfo();
    }
    #endregion SubscribeEvents / UnSubscribeEvents!!!
}
