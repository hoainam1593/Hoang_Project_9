using System;
using UnityEngine;

public class InputProcessing : MonoBehaviour
{    
    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0))
        {
            CameraCtrl.instance.StartScroll(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            CameraCtrl.instance.CrollTo(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            CameraCtrl.instance.StopScroll();
        }
#endif
    }
}