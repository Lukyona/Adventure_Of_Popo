using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public CinemachineFreeLook thisCamera;
    float zoomSpeed = 20.0f;

    private void Update()
    {
        float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * zoomSpeed;

        if (thisCamera.m_Lens.FieldOfView < 20f)
        {
            thisCamera.m_Lens.FieldOfView = 20f;
        }
        else if (thisCamera.m_Lens.FieldOfView > 65f)
        {
            thisCamera.m_Lens.FieldOfView = 65f;
        }
        thisCamera.m_Lens.FieldOfView += distance;
    }
}
