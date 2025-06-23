using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private MapCtrl mapCtrl;
    
#if UNITY_EDITOR
    public void OnClick(Vector3 viewPos)
    {
        var tile = mapCtrl.GetTile(viewPos);
        Debug.Log("OnClick Tile: " + tile);
        if (tile == TileEnum.Ground)
        {
            ShowTurretSelectorUI();
        }
    }

    private void ShowTurretSelectorUI()
    {
        
    }
    
#endif
    
    
}
