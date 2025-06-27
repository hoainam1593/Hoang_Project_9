using UnityEngine;

public class EntityBase : MonoBehaviour, IEntity
{
    private const float Interval = 0.02f;
    
    private bool isSpawnCompleted = false;
    private float lateUpdateTime;
    private float updateTime;
    
    #region Spawn/DeSpawn
    
    
    public void OnSpawn(object data)
    {
        InitData(data);
        OnSpawnStart();
        OnSpawnComplete();
    }

    protected virtual void InitData(object data)
    {
        
    }
    
    protected virtual void OnSpawnStart()
    {
    }

    protected virtual void OnSpawnComplete()
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
        if (updateTime > Interval)
        {
            updateTime -= Interval;
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

        lateUpdateTime += Time.deltaTime;
        if (lateUpdateTime > Interval)
        {
            lateUpdateTime -= Interval;
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