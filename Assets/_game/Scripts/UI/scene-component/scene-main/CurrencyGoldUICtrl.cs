using UnityEngine;
using R3;

/// <summary>
/// CurrencyGoldUICtrl handles the display of gold currency in the UI.
/// Gets data from CurrencyManager instead of PlayerCtrl.
/// </summary>
public class CurrencyGoldUICtrl : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI goldText;

    private void Start()
    {
        var goldProperty = CurrencyManager.instance?.Gold;

        // Initialize gold display with current value
        goldText.text = goldProperty.Value.ToString();

        // Subscribe to changes
        goldProperty.Subscribe(value =>
        {
            goldText.text = value.ToString();
        });
    }

    //private void Awake()
    //{
    //    Subscribes();
    //}

    //private void OnDestroy()
    //{
    //    UnSubscribes();
    //}

    #region Subscribes / Unsubscribes
    //private void Subscribes()
    //{
    //    GameEventMgr.GED.Register(GameEvent.OncurrencyManagerInit, OncurrencyManagerInit);
    //}

    //private void UnSubscribes()
    //{
    //    GameEventMgr.GED.UnRegister(GameEvent.OncurrencyManagerInit, OncurrencyManagerInit);
    //}


    //private void OncurrencyManagerInit(object data)
    //{
    //    var goldProperty = (ReactiveProperty<int>)data;
    //    Debug.Log("[CurrencyGoldUICtrl] OncurrencyManagerInit > gold: " + goldProperty.Value);

    //    // Initialize gold display with current value
    //    goldText.text = goldProperty.Value.ToString();

    //    // Subscribe to changes
    //    goldProperty.Subscribe(value => {
    //        goldText.text = value.ToString();
    //    });
    //}

    #endregion Susbscribes / Unsubscribes
}
