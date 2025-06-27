
using UnityEngine;

public class EnemySoldierCtrl : EnemyCtrl
{
    [SerializeField] private GameObject soldierBody;
    [SerializeField] private GameObject soldierHead;
    [SerializeField] private GameObject soldierDied;
    
    protected override void OnInitStart()
    {
        //Set Animation state
        soldierDied.SetActive(false);
    }
}