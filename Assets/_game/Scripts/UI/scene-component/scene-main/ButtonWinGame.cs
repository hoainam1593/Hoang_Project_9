using UnityEngine;
using UnityEngine.UI;

public class ButtonWinGame : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            GameManager.instance?.Victory();
        });
    }
}
