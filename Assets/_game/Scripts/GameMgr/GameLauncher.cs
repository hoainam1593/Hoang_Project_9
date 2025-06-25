using System;
using UnityEngine;

//Main Application lifeCycle
public class GameLauncher : MonoBehaviour
{
    [SerializeField] private MapCtrl mapCtrl;
    private GameObject mapRoot;
    
    private Game game;
    
    private void Awake()
    {
        mapRoot = mapCtrl.gameObject;
    }

    private void Start()
    {
        //hide map
        mapRoot.SetActive(false);
        
        //show main ui
        UIManager.instance.OpenPanel<MainUIPanel>(UIPanel.MainUIPanel);
    }

    public void StartGame()
    {
        var mapName = "map1";
        mapRoot.SetActive(true);
        // mapCtrl.GenerateMap(mapName);
        
        UIManager.instance.ClosePanel(UIPanel.MainUIPanel);
        UIManager.instance.OpenPanel<BattleUIPanel>(UIPanel.BattleUIPanel);
        
        game.StartGame();   
    }

    public void ExitGame()
    {
        mapRoot.SetActive(false);
        
        UIManager.instance.ClosePanel(UIPanel.BattleUIPanel);
        UIManager.instance.OpenPanel<MainUIPanel>(UIPanel.MainUIPanel);
                
        game.ExitGame();
    }
}
