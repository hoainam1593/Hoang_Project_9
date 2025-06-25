using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour, IDispatcher
{
#if UNITY_EDITOR
    private const float ClickThreshold = 10f;
#else
    private const float ClickThreshold = 30f;
#endif
    private Vector3 crrPos;
    private Vector3 direction;
    private Vector3 startPos;

    private bool isDragging = false;
    
    
    private void Update()
    {
#if UNITY_EDITOR

        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            // Debug.Log("StartPos: " + Input.mousePosition);
            isDragging = true;
            startPos =  Input.mousePosition;
            crrPos =  Input.mousePosition;
            this.DispatcherEvent(GameEvent.OnDraggedStart, crrPos);

            // LogTest();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            crrPos = Input.mousePosition;
            
            // Debug.Log("EndPos: " + Input.mousePosition);
            isDragging = false;
            this.DispatcherEvent(GameEvent.OnDraggedEnd, crrPos);

            direction = crrPos - startPos;
            if (direction.magnitude < ClickThreshold && !EventSystem.current.IsPointerOverGameObject())
            {
                // if (!EventSystem.current.IsPointerOverGameObject())
                // {
                    this.DispatcherEvent(GameEvent.OnClick, crrPos);
                // }
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

    // public GraphicRaycaster raycaster;
    // public EventSystem eventSystem;
    // private void LogTest()
    // {
    //     List<RaycastResult> results = new List<RaycastResult>();
    //     PointerEventData eventData = new PointerEventData(eventSystem);
    //     raycaster.Raycast(eventData, results);
    // }
}