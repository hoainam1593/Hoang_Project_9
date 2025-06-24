using System;
using UnityEngine;

public class InputManager : MonoBehaviour, IDispatcher
{
    private Vector3 crrPos;
    private Vector3 direction;
    private Vector3 startPos;

    private bool isDragging = false;
    
    private void Update()
    {
        
#if UNITY_EDITOR

        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            // Debug.Log("StartPos: " + Input.mousePosition);
            isDragging = true;
            startPos =  Input.mousePosition;
            crrPos =  Input.mousePosition;
            this.DispatcherEvent(GameEvent.OnDraggedStart, crrPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            crrPos = Input.mousePosition;
            
            // Debug.Log("EndPos: " + Input.mousePosition);
            isDragging = false;
            this.DispatcherEvent(GameEvent.OnDraggedEnd, crrPos);

            direction = crrPos - startPos;
            if (direction.magnitude < 0.1f)
            {
                this.DispatcherEvent(GameEvent.OnClick, crrPos);
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            // Debug.Log("MousePos: " + Input.mousePosition);
            crrPos = Input.mousePosition;
            this.DispatcherEvent(GameEvent.OnDragging, crrPos);
        }

#endif
    }
}