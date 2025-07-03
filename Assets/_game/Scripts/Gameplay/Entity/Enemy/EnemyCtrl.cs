using Amazon.S3.Model;
using R3;
using System.Text;
using UnityEngine;

public class EnemyCtrl : EntityBase, IDamagable
{
    private const int EnemyKey = 50;
    private const int EnemyLimit = 1000;
    private const float FixedSpeed = 5f;
    
    private static int _uid = -1;

    private int id;
    public int Uid { get; private set; }
    public bool IsDead { get; private set; }
    public float MaxHp { get; private set; }
    public ReactiveProperty<float> CrrHp { get; private set; }
    public ReactiveProperty<Vector3> Position { get; private set; }
    private float speed;  //Unit per second
    private string name;

    // Current path tracking - CONVERTED TO PathEntry
    private PathEntry target = null;
    private Vector3 targetPos;
    private Vector3 direction;
    private bool isMoving;
    private const float DefaultForward = 90f;

    // Curved corner movement variables - CONVERTED TO PathEntry
    private PathEntry currentPathEntry = null;
    private PathEntry nextPathEntry = null;
    private PathEntry prePathEntry = null;
    private bool isInCorner = false;
    private bool cornerTriggered = false; // Flag to prevent multiple corner triggers
    private Vector3 cornerStartPos;
    private Vector3 cornerExitPos;
    private Vector3 targetCurvePos;
    private float cornerMovingProgress = 0f;
    [SerializeField] private float curveIntensity = 0.3f; // How curved the corners should be (0-1)
    [SerializeField] private float cornerDetectionDistance = 0.3f; // Distance from tile center to start corner detection
    
    private static int GenerateUid()
    {
        _uid++;
        return EnemyKey * EnemyLimit + _uid;
    }

    private void OnDrawGizmos()
    {
        if (isMoving)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(targetPos, 0.1f);

            // Draw corner visualization
            if (isInCorner)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(cornerStartPos, 0.08f);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(cornerExitPos, 0.08f);
                
                // Draw curve path
                Gizmos.color = Color.cyan;
                Vector3 lastPos = cornerStartPos;
                for (float t = 0.1f; t <= 1f; t += 0.1f)
                {
                    Vector3 curvePos = PathFinding.instance.GetCurvedPosition(cornerStartPos, cornerExitPos, 
                        targetPos, t, curveIntensity);
                    Gizmos.DrawLine(lastPos, curvePos);
                    lastPos = curvePos;
                }
            }

            // Draw lookahead visualization with proper null checks
            if (IsValidPathEntry(nextPathEntry))
            {
                Gizmos.color = Color.blue;
                Vector3 nextPos = PathFinding.instance.GetWorldPos(nextPathEntry.mapCoordinate);
                Gizmos.DrawSphere(nextPos, 0.06f);
            }
            
            if (IsValidPathEntry(prePathEntry))
            {
                Gizmos.color = Color.magenta;
                Vector3 nextNextPos = PathFinding.instance.GetWorldPos(prePathEntry.mapCoordinate);
                Gizmos.DrawSphere(nextNextPos, 0.06f);
            }
        }
    }

    private bool IsValidPathEntry(PathEntry entry)
    {
        return entry != null && entry.mapCoordinate != null && entry.mapCoordinate != MapCoordinate.oneNegative;
    }


    #region Task - Spawn / Despawn
    
    protected override void InitData(object data)
    {
        Uid = GenerateUid();
        
        if (data == null)
        {
            return;
        }

        id = (int)data;
        var config = ConfigManager.instance.GetConfig<EnemyConfig>().GetItem(id);
        IsDead = true;
        MaxHp = config.hp;
        CrrHp = new ReactiveProperty<float>(config.hp);
        Position = new ReactiveProperty<Vector3>(transform.position);
        speed = config.speed / FixedSpeed;
        name = config.prefabName;
    }

    protected override void OnSpawnStart()
    {
        base.OnSpawnStart();
        gameObject.name = $"{name}[{Uid}]";
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    protected override void OnSpawnComplete()
    {
        base.OnSpawnComplete();
        StartMove();
    }
    
    #endregion Task - Spawn / Despawn!!!
    
    #region Task - MainLoop
    
    protected override void OnLateUpdate()
    {
        if (!IsDead)
        {
            return;
        }
        
        if (isMoving)
        {
            Moving();
            Rotate();
        }
    }
        

    protected override void UpdateEachInterval()
    {
        if (IsDead)
        {
            UpdateTarget();
        }
    }

    #endregion Task - MainLoop!!!
    
    #region Task - UpdateTarget
    private void UpdateTarget()
    {
        if (target.isCorner && CheckForCornerStart())
        {
            StartMoveIntoCorner();
            return;
        }

        // Check if we're in corner movement mode
        if (isInCorner)
        {
            // Check if corner movement is complete
            if (CheckForCornerCompleted())
            {
               
                MoveOutOfCorner();

                NextStep();
            }
            return;
        }

        if (IsReachTarget())
        {
            if (IsEndOfPath())
            {
                EndMove();
                return;
            }

            NextStep();
        }
    }

    private void NextStep()
    {
        GetNewTarget();
        UpdateLookahead();
        CheckForCornerAheadOptimized();
    }


    private void GetFirstTarget()
    {
        currentPathEntry = PathFinding.instance.GetStartEntry();
        nextPathEntry = PathFinding.instance.GetNextEntry(currentPathEntry.mapCoordinate);
        target = currentPathEntry;

        NextStep();
    }

    private bool IsReachTarget()
    {
        var distance = targetPos - transform.position;
        return (Vector3.SqrMagnitude(distance) < Mathf.Epsilon);
    }

    private bool IsEndOfPath()
    {
        return PathFinding.instance.IsEnd(target.mapCoordinate);
    }

    private void GetNewTarget()
    {
        // NEW LOGIC: Get next PathEntry instead of MapCoordinate
        target = nextPathEntry;
        if (IsValidPathEntry(target))
        {
            targetPos = PathFinding.instance.GetWorldPos(target.mapCoordinate);
            direction = targetPos - transform.position;
            direction.z = 0;
        }
    }

    #endregion Task - UpdateTarget!!!

    #region Task - UpdateTarget: Corner Detection


    // Cached corner detection results
    private Vector3 cachedCornerStart;
    private Vector3 cachedCornerExit;

    private void StartMoveIntoCorner()
    {
        Debug.Log($"Enemy[{Uid}] > StartMoveIntoCorner");
        // Trigger corner movement (only once per target)
        isInCorner = true;
        cornerTriggered = true;
        cornerStartPos = cachedCornerStart;
        cornerExitPos = cachedCornerExit;
        cornerMovingProgress = 0.01f;
        targetCurvePos = PathFinding.instance.GetCurvedPosition(cornerStartPos, cornerExitPos, targetPos, cornerMovingProgress, curveIntensity);
    }

    private void MoveOutOfCorner()
    {
        Debug.Log($"Enemy[{Uid}] > MoveOutOfCorner");
        isInCorner = false;
        cornerTriggered = false; // Reset corner trigger for next corner
    }

    private void CheckForCornerAheadOptimized()
    {
        // Only check for corner if we have valid target entry
        if (!IsValidPathEntry(target) || !target.isCorner)
        {
            return;
        }

        bool hasCorner = target.isCorner;

        Debug.Log($"[EnemyCtrl {Uid}] CheckForCornerAheadOptimized: target={target} hasCorner={hasCorner}");

        if (hasCorner)
        {
            if (IsValidPathEntry(prePathEntry) && IsValidPathEntry(currentPathEntry) && IsValidPathEntry(nextPathEntry))
            {
                PathFinding.instance.GetCornerPositions(target, out cachedCornerStart, out cachedCornerExit);

                // Update instance variables for visualization
                cornerStartPos = cachedCornerStart;
                cornerExitPos = cachedCornerExit;

                //DebugCorner();
            }
            else
            {
                Debug.LogWarning($"[EnemyCtrl {Uid}] Corner detected but missing path entries for calculation");
            }
        }
        else
        {
            Debug.Log($"[EnemyCtrl {Uid}] No corner detected - continuing straight path");
        }
    }

    private void UpdateLookahead()
    {
        prePathEntry = currentPathEntry;
        currentPathEntry = nextPathEntry;
        nextPathEntry = PathFinding.instance.GetNextEntry(target.mapCoordinate);
    }

    private bool CheckForCornerStart()
    {
        //Debug.Log($"[EnemyCtrl {Uid}] CheckForCornerStart > target: {target} > cornerTriggered: {cornerTriggered} > isCorner: {target.isCorner}");

        if (cornerTriggered || !target.isCorner)
        {
            return false;
        }

        // Check if we've reached or passed the corner entry position
        float distanceToEntry = Vector3.Distance(transform.position, cachedCornerStart);
        Debug.Log($"[EnemyCtrl {Uid}] CheckForCornerStart > distance from:{transform.position} to {cachedCornerStart} is: {distanceToEntry}");

        // Use a smaller threshold since we want to trigger exactly at the edge
        return (distanceToEntry <= 0.1f);
    }

    private bool CheckForCornerCompleted()
    {
        return Vector3.Distance(transform.position, cornerExitPos) < 0.1f;
    }

    #endregion Task - UpdateTarget: Corner Detection!!!

    #region Task - Move & Rotate

    private void StartMove()
    {
        GetFirstTarget();
        if (IsValidPathEntry(target))
        {
            Rotate();
            isMoving = true;
        }
    }

    private void Moving()
    {
        Vector3 newPos;
        
        if (isInCorner)
        {
            // Use curved movement for corners
            newPos = MovingInCorner();
        }
        else
        {
            // Use straight line movement
            newPos = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        }
        
        transform.position = newPos;
        Position.Value = newPos;
    }

    private Vector3 MovingInCorner()
    {
        cornerMovingProgress = GetCornerProgress();

        // Get curved position
        targetCurvePos = PathFinding.instance.GetCurvedPosition(cornerStartPos, cornerExitPos, targetPos, cornerMovingProgress, curveIntensity);

        // Move towards the curved target position
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetCurvePos, speed * Time.deltaTime);

        // If we're very close to the curve target, move directly towards exit
        if (Vector3.Distance(transform.position, targetCurvePos) < 0.05f)
        {
            newPos = Vector3.MoveTowards(transform.position, cornerExitPos, speed * Time.deltaTime);
        }

        return newPos;
    }

    private float GetCornerProgress()
    {
        // Calculate progress along the corner curve
        var currentPos = transform.position;
        float totalCornerDistance = Vector3.Distance(cornerStartPos, cornerExitPos);
        float currentDistance = Vector3.Distance(cornerStartPos, currentPos);
        float t = Mathf.Clamp01(currentDistance / totalCornerDistance);
        return t;
    }

    private void EndMove()
    {
        isMoving = false;
        EntityManager.instance.DespawnEnemy(Uid);
        GameEventMgr.GED.DispatcherEvent(GameEvent.OnEnemyReachHallGate, Uid);
    }
    
    private void Rotate()
    {
        //Vector3 rotationDirection = direction;
        
        //// If in corner, rotate towards the curve direction
        //if (isInCorner & cornerMovingProgress > 0.1f)
        //{
        //    rotationDirection = (targetCurvePos - transform.position).normalized;
        //}
        
        //float angle = Mathf.Atan2(rotationDirection.y, rotationDirection.x) * Mathf.Rad2Deg - DefaultForward;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    #endregion Task - Move & Rotate!!!
    
    #region Task - IDamageable

    public void TakeDamage(float dmg)
    {
        float crrHp = CrrHp.Value - dmg;
        if (crrHp <= 0)
        {
            CrrHp.Value = 0;
            IsDead = false;
            OnDead();
        }
        else
        {
            CrrHp.Value = crrHp;
        }
    }

    private void OnDead()
    {
        Debug.Log($"[EnemyCtrl] OnDead > Uid: {Uid}, Id: {id}");
        EntityManager.instance.DespawnEnemy(this.Uid);
    }
    #endregion Task - IDamageable!!!




    #region Debug/Log
    private void DebugCorner()
    {
        var strBuilder = new StringBuilder();

        // Debug log all position values when IsCornerAhead is called
        strBuilder.AppendLine($"[EnemyCtrl {Uid}] CheckForCornerAheadOptimized");
        strBuilder.AppendLine($"PathEntry: {currentPathEntry} => {target} => {nextPathEntry} => {prePathEntry}");

        // Convert coordinates to world positions for better understanding
        var currentWorldPos = IsValidPathEntry(currentPathEntry) ? PathFinding.instance.GetWorldPos(currentPathEntry.mapCoordinate) : Vector3.zero;
        var targetWorldPos = IsValidPathEntry(target) ? PathFinding.instance.GetWorldPos(target.mapCoordinate) : Vector3.zero;
        var nextWorldPos = IsValidPathEntry(nextPathEntry) ? PathFinding.instance.GetWorldPos(nextPathEntry.mapCoordinate) : Vector3.zero;
        var preWorldPos = IsValidPathEntry(prePathEntry) ? PathFinding.instance.GetWorldPos(prePathEntry.mapCoordinate) : Vector3.zero;

        strBuilder.AppendLine($"World Pos: {Vector3ToPos(preWorldPos)} => {Vector3ToPos(currentWorldPos)} => {Vector3ToPos(targetWorldPos)} => {Vector3ToPos(nextWorldPos)} real Pos: {transform.position}");
        strBuilder.AppendLine($"CORNER DETECTED >> start: {Vector3ToPos(cachedCornerStart)} => exit:{Vector3ToPos(cachedCornerExit)}");
        strBuilder.AppendLine($"Target IsCorner: {target?.isCorner}");
        strBuilder.AppendLine($"[EnemyCtrl {Uid}] === END CORNER DETECTION DEBUG ===");

        Debug.Log(strBuilder.ToString());
    }

    private string Vector3ToPos(Vector3 pos)
    {
        return $"({pos.x:F1}, {pos.y:F1})";
    }

    #endregion Debug/Log!!!
}
