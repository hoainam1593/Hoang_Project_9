using UnityEngine;
using TMPro;
using R3;

/// <summary>
/// PlayerInfoUICtrl handles the UI display for player information during battle.
/// It gets in-game data (HP, coin) from PlayerCtrl. Gold is handled separately by CurrencyGoldUICtrl.
/// </summary>
public class PlayerInfoUICtrl : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHp;
    [SerializeField] private TextMeshProUGUI textCoin;
    private PlayerInfo playerInfo;

    void Awake()
    {
        SubscribeEvents();
    }

    private void OnDestroy()
    {
        UnSubscribeEvents();
    }

    /// <summary>
    /// Initialize player info by getting in-game data from PlayerCtrl
    /// </summary>
    private void InitView(PlayerInfo playerInfo)
    {
        // Set up UI subscriptions for in-game attributes
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
            Debug.LogError("PlayerInfoUICtrl: Failed to get player info from PlayerCtrl");
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
            
            Debug.LogWarning("PlayerInfoUICtrl: Using fallback initialization with config values");
        }
        else
        {
            textHp.text = "0";
            textCoin.text = "0";
        }
    }

    #region SubscribeEvents / UnSubscribeEvents

    private void SubscribeEvents()
    {
        GameEventMgr.GED.Register(GameEvent.OnPlayerInit, OnPlayerInit);
    }

    private void UnSubscribeEvents()
    {
        GameEventMgr.GED.UnRegister(GameEvent.OnPlayerInit, OnPlayerInit);
    }

    private void OnPlayerInit(object data)
    {
        var parseData = (PlayerInfo)data;
        if (parseData != null)
        {
            InitView(parseData);
        }
    }

    #endregion
}
