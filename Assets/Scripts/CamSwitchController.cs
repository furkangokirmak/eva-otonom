using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSwitchController : MonoBehaviour
{
    public Camera MainCamera;
    public Camera DriverCam;
    public Camera DashCam;
    public Camera Thirdcam;

    public void ShowMainCamera()
    {
        MainCamera.enabled = true;
        DriverCam.enabled = false;
        DashCam.enabled = false;
        Thirdcam.enabled = false;
    }

    public void ShowDriverCamera()
    {
        MainCamera.enabled = false;
        DriverCam.enabled = true;
        DashCam.enabled = false;
        Thirdcam.enabled = false;
    }
    public void ShowDashCamera()
    {
        MainCamera.enabled = false;
        DriverCam.enabled = false;
        DashCam.enabled = true;
        Thirdcam.enabled = false;
    }
    public void ShowThirdCam()
    {
        MainCamera.enabled = false;
        DriverCam.enabled = false;
        DashCam.enabled = false;
        Thirdcam.enabled = true;

    }
}
