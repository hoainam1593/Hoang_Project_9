using System;
using UnityEngine;

public class InputProcessing : MonoBehaviour
{

    private Vector3 previousPos;
    private Vector3 crrPos;
    private Vector3 direction;

    private bool isDragging = false;
    
    private void Update()
    {
        
#if UNITY_EDITOR

        if (!isDragging && Input.GetMouseButtonDown(0))
        {
            // Debug.Log("StartPos: " + Input.mousePosition);
            isDragging = true;
            previousPos =  Input.mousePosition;
            CameraCtrl.instance.StartScroll(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log("EndPos: " + Input.mousePosition);
            isDragging = false;
            previousPos = Vector3Int.zero;
            CameraCtrl.instance.StopScroll();
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            crrPos = Input.mousePosition;
            direction = crrPos - previousPos;
            // Debug.Log("MousePos: " + Input.mousePosition + " > direction: " + direction);
            CameraCtrl.instance.ScrollTo(Input.mousePosition);
        }

#endif
    }
}