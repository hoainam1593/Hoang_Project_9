using UnityEngine.UI;
using UnityEngine;

public class PopupTurretSelect : BasePopup
{
    [Space(10)]
    [SerializeField] private Button gunButton;

    private MapCoordinate mapCoordinate;
    private Vector3 pos;
    
    protected override void Start()
    {
        base.Start();
        OnStart();
    }

    private void OnStart()
    {
        // Debug.Log("OnStart");
        //Subscribes Event:
        gunButton.onClick.AddListener(() =>
        {
            SpawnTurretGun();
            ClosePopup();
        });        
    }

    public override void OnClosePopup(bool isRunAnim = true)
    {
        base.OnClosePopup(isRunAnim);
        
        //UnSubscribes Event
        gunButton.onClick.RemoveAllListeners();
    }

    public void InitView(MapCoordinate mapCoordinate, Vector3 worldPos, Vector3 size)
    {
        this.mapCoordinate = mapCoordinate;
        this.pos = worldPos;
        
        transform.position = worldPos;
        transform.localScale = size;
    }
    
    
    private void SpawnTurretGun()
    {
        // Debug.Log("SpawnTurretGun");
        EntityManager.instance.SpawnTurret(mapCoordinate, pos, 0);
    }
}

