using UnityEngine;
using R3;

public class CurrencyGoldUICtrl : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI goldText;

    private void Start()
    {
        var playerInfo = PlayerCtrl.instance.GetPlayerInfo();

        goldText.text = playerInfo.Gold.ToString();

        playerInfo.Gold.Subscribe(
            onNext: (value) => {
                goldText.text = value.ToString();
            }
        );
    }
}
