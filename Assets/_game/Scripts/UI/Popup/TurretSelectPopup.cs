using UnityEngine.UI;
using UnityEngine;

public class TurretSelectPopup : BasePopup
{
    [Space(10)]
    [SerializeField] private Button gunButton;
    
    protected override void Start()
    {
        base.Start();
        OnStart();
    }

    private void OnStart()
    {
        Debug.Log("OnStart");
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

    public void InitView(Vector3 worldPos, Vector3 size)
    {
        transform.position = worldPos;
        transform.localScale = size;
    }
    
    
    private void SpawnTurretGun()
    {
        Debug.Log("SpawnTurretGun");
        // EntityManager.instance.SpawnTurret("")
    }
}
