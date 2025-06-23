using System;
using UnityEngine;

public class InputManager : MonoBehaviour
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
            CameraCtrl.instance.StartScroll(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log("EndPos: " + Input.mousePosition);
            isDragging = false;
            CameraCtrl.instance.StopScroll();

            crrPos = Input.mousePosition;
            direction = crrPos - startPos;
            if (direction.magnitude < 0.1f)
            {
                GameManager.instance.OnClick(crrPos);
            }
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            // Debug.Log("MousePos: " + Input.mousePosition);
            crrPos = Input.mousePosition;
            CameraCtrl.instance.ScrollTo(Input.mousePosition);
        }

#endif
    }
}