using System;
using UnityEngine.UI;
using UnityEngine;

public class ButtonHome : MonoBehaviour
{
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        
        button.onClick.AddListener(() =>
        {
            GameLauncher.instance.ExitGame();
        });
    }
}
