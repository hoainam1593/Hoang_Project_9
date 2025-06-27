using UnityEngine;

public class EntityBase : MonoBehaviour, IEntity
{
    private bool isSpawnCompleted = false;

    private float lateUpateTime;
    private float updateTime;
    private const float interval = 0.02f;
    
    #region Spawn/DeSpawn
    
    
    public virtual void OnSpawn(object data)
    {
        InitData(data);
        OnInitStart();
        OnInitComplete();
    }

    protected virtual void InitData(object data)
    {
        
    }
    
    protected virtual void OnInitStart()
    {
    }

    protected virtual void OnInitComplete()
    {
        isSpawnCompleted = true;
    }

    public virtual void OnDespawn()
    {
        isSpawnCompleted = false;
    }
    #endregion Spawn/DeSpawn!!!
    
    #region MainLoop
    
    private void Update()
    {
        // Debug.Log("[EntityBase] Update");
        if (!isSpawnCompleted)
        {
            return;
        }

        OnUpdate();
        
        updateTime += Time.deltaTime;
        if (updateTime > interval)
        {
            updateTime -= interval;
            UpdateEachInterval();
        }
    }

    protected virtual void OnUpdate()
    {
        // Debug.Log("[EntityBase] OnUpdate");
    }
    protected virtual void UpdateEachInterval()
    {
        // Debug.Log("[EntityBase] UpdateEachInterval");
    }

    private void LateUpdate()
    {
        // Debug.Log("[EntityBase] LateUpdate");
        if (!isSpawnCompleted)
        {
            return;
        }

        OnLateUpdate();

        lateUpateTime += Time.deltaTime;
        if (lateUpateTime > interval)
        {
            lateUpateTime -= interval;
            LateUpdateEachInterval();
        }
    }

    protected virtual void OnLateUpdate()
    {
        // Debug.Log("[EntityBase] OnLateUpdate");
        
    }
    protected virtual void LateUpdateEachInterval()
    {
        // Debug.Log("[EntityBase] LateUpdateEachInterval");
        
    }
    #endregion MainLoop!!
}