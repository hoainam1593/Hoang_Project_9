using System;
using UnityEngine;

public partial class CameraCtrl : SingletonMonoBehaviour<CameraCtrl>
{
    private Camera mainCam;
    private float viewWidth;
    private float viewHeight;
    private Vector3 mapLeftBottomPos;

    private LockScroll lockSide;

    private Vector3 touchStart;
    private bool onTouched;

    private Vector3 direction;
    private Vector3 targetPosition;
    private Vector3 velocity = Vector3.zero;
    private const float smoothTime = 0.1f;
    private const float dragSpeed = 0.1f;

    private float camWidth;
    private float camHeight;

    enum LockScroll
    {
        Vertical,
        Horizontal
    }

    protected override void Awake()
    {
        base.Awake();
        mainCam = Camera.main;
        Subscribes();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnSubscribes();
    }

    private void LateUpdate()
    {
        if (onTouched && direction.magnitude > 0.04f)
        {
            targetPosition -= direction * dragSpeed;
            targetPosition = FixPositionInView(targetPosition);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }
    }
    
    #region IRegister methods:

        public void Subscribes()
        {
            GameEventMgr.GED.Register(GameEvent.OnDraggedStart, StartScroll);
            GameEventMgr.GED.Register(GameEvent.OnDragging, ScrollTo);
            GameEventMgr.GED.Register(GameEvent.OnDraggedEnd, StopScroll);
            GameEventMgr.GED.Register(GameEvent.OnMapSizeUpdate, OnMapSizeUpdate);
        }

        public void UnSubscribes()
        {
            GameEventMgr.GED.UnRegister(GameEvent.OnDraggedStart, StartScroll);
            GameEventMgr.GED.UnRegister(GameEvent.OnDragging, ScrollTo);
            GameEventMgr.GED.UnRegister(GameEvent.OnDraggedEnd, StopScroll);
            GameEventMgr.GED.UnRegister(GameEvent.OnMapSizeUpdate, OnMapSizeUpdate);
        }

    #endregion IRegister methods!!

    public void OnMapSizeUpdate(object data)
    {
        var sizeData = (GEventData.MapSizeData)data;
        viewWidth = sizeData.Width;
        viewHeight = sizeData.Height;
        mapLeftBottomPos = sizeData.BottomLeftPoint;
        
        ZoomFixedSize(viewWidth, viewHeight);
        SetStartPosition();
    }
    
    #region Task Zoom Camera
    
        private void ZoomFixedSize(float width, float height)
        {
            // Debug.Log($"ZoomFixedSize > camera Ratio: {mainCam.aspect} - map Ratio: {width/height}");
            
            if (mainCam.aspect > width / height)
            {
                // Debug.Log("Fixed Horizontal");
                //Fixed Horizontal
                lockSide = LockScroll.Horizontal;
                
                camWidth = width;
                camHeight = camWidth / mainCam.aspect;
                mainCam.orthographicSize = camHeight / 2f;
            }
            else
            {
                // Debug.Log("Fixed Vertical");
                //Fixed Vertical
                lockSide = LockScroll.Vertical;
                
                camHeight = height;
                camWidth = camHeight * mainCam.aspect;
                mainCam.orthographicSize = camHeight / 2f;
            }
        }

        private void SetStartPosition()
        {
            var startPosition = FixPositionInView(mainCam.transform.position);
            mainCam.transform.position = startPosition;
        }
    
    #endregion Task Zoom Camera!!
    
    
    #region Task Scroll Camera
    
        //Public Method:
        public void StartScroll(object data)
        {
            Vector3 touchPoint = (Vector3)data;
            // Debug.Log("CameraCtrl Start Scroll: " +  touchPoint);
            onTouched = true;
            touchStart = mainCam.ScreenToWorldPoint(touchPoint);
            touchStart = FixPositionLockSide(touchStart);
            targetPosition = touchStart;
        }

        public void StopScroll(object data)
        {
            // Debug.Log("CameraCtrl Stop Scroll");
            onTouched = false;
            direction = Vector3.zero;
            targetPosition = Vector3.zero;
        }

        public void ScrollTo(object data)
        {
            Vector3 touchPoint = (Vector3)data;
            var currentTouch = mainCam.ScreenToWorldPoint(touchPoint);
            direction = currentTouch - touchStart;
            direction = FixPositionLockSide(direction);
            targetPosition = currentTouch;
            // Debug.Log("CameraCtrl Start Scroll: " +  touchPoint + " > direction: " + direction);
        }

        //Calculate position
        private Vector3 FixPositionLockSide(Vector3 vector)
        {
            if (lockSide == LockScroll.Horizontal)
            {
                vector.x = 0;
            }
            else
            {
                vector.y = 0;
            }

            return vector;
        }
        
        private Vector3 FixPositionInView(Vector3 position)
        {
            var halfWidth = camWidth / 2;
            var halfHeight = camHeight / 2;
            
            if (position.x - halfWidth < mapLeftBottomPos.x)  //limit left
            {
                position.x = mapLeftBottomPos.x + halfWidth;
            }
            
            if (position.x + halfWidth > mapLeftBottomPos.x + viewWidth) // limit right
            {
                position.x = mapLeftBottomPos.x + viewWidth - halfWidth;
            }

            if (position.y - halfHeight < mapLeftBottomPos.y) // limit bottom
            {
                position.y = mapLeftBottomPos.y + halfHeight;
            }

            if (position.y + halfHeight > mapLeftBottomPos.y + viewHeight) // limit top
            {
                position.y = mapLeftBottomPos.y + viewHeight - halfHeight;
            }

            return position;
        }
    
    #endregion Task Scroll Camera!
    
}