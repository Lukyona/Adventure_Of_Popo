using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField] CinemachineFreeLook thisCamera;
    float zoomSpeed = 20.0f;

    bool isFixed = false;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if(isFixed) return;
         
        float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * zoomSpeed;
        float value = thisCamera.m_Lens.FieldOfView + distance;

        if(value >= 20f && value <= 65f)
            thisCamera.m_Lens.FieldOfView += distance;        
    }

    public void SetFixedState(bool value)
    {
        isFixed = value;
        if(value)
            thisCamera.GetComponent<CinemachineFreeLook>().enabled = false;
        else
            thisCamera.GetComponent<CinemachineFreeLook>().enabled = true;
    }

    public void SetXAxisValue(float value)
    {
        thisCamera.m_XAxis.Value = value;
    }

    public void SetYAxisValue(float value)
    {
        thisCamera.m_YAxis.Value = value;
    }

    public void SetFieldOfView(float value)
    {
        thisCamera.m_Lens.FieldOfView = value;
    }
}
