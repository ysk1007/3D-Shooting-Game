using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void LateUpdate()
    {
        Camera cam = (Camera)FindAnyObjectByType(typeof(Camera));
        if (cam)
        {
            transform.LookAt(cam.transform.position);
        }
    }
}
