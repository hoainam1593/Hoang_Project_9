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

    public void UpdateMapSize(float width, float height, Vector3 pointBottomLeft)
    {
        viewWidth = width;
        viewHeight = height;
        mapLeftBottomPos = pointBottomLeft;
        
        ZoomFixedSize(width, height);
        SetStartPosition();
    }
    
    #region Task Zoom Camera
    
        private void ZoomFixedSize(float width, float height)
        {
            
            if (mainCam.aspect > height / width)
            {
                //Fixed Horizontal
                lockSide = LockScroll.Horizontal;
                
                camWidth = width;
                camHeight = camWidth / mainCam.aspect;
                mainCam.orthographicSize = camHeight / 2f;
            }
            else
            {
                //Fixed Vertical
                lockSide = LockScroll.Vertical;
                
                camHeight = height;
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
        public void StartScroll(Vector3 touchPoint)
        {
            onTouched = true;
            touchStart = mainCam.ScreenToWorldPoint(touchPoint);
            touchStart = FixPositionLockSide(touchStart);
            targetPosition = touchStart;
        }

        public void StopScroll()
        {
            onTouched = false;
            direction = Vector3.zero;
            targetPosition = Vector3.zero;
        }

        public void ScrollTo(Vector3 touchPoint)
        {
            var currentTouch = mainCam.ScreenToWorldPoint(touchPoint);
            direction = currentTouch - touchStart;
            direction = FixPositionLockSide(direction);
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
            
            if (position.x - halfWidth < mapLeftBottomPos.x)
            {
                position.x = mapLeftBottomPos.x + halfWidth;
            }
            
            if (position.x + halfWidth > mapLeftBottomPos.x + viewWidth)
            {
                position.x = mapLeftBottomPos.x + viewWidth - halfWidth;
            }

            if (position.y - halfHeight < mapLeftBottomPos.y)
            {
                position.y = mapLeftBottomPos.y + halfHeight;
            }

            if (position.y + halfHeight > mapLeftBottomPos.y + viewHeight)
            {
                position.y = mapLeftBottomPos.y + viewHeight - halfHeight;
            }

            return position;
        }
    
    #endregion Task Scroll Camera!
    
}