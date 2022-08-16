using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ModeManager : MonoBehaviour
{
    private static ModeManager instance;
    public static ModeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ModeManager>();
            }
            return instance;
        }
    }

    public enum MODE_EXPERIENCE
    {
        MODE_3D,
        MODE_AR,
    }

    // // UI
    // public Button btnAR;
    // public Button btnSwitch;

    public GameObject ARComponent;
    public GameObject UIComponentCommon;
    public GameObject _3DComponent;
    public Camera CameraAR;
    public Camera Camera3D;

    public MODE_EXPERIENCE Mode { get; set; }

    void Start()
    {
        Mode = MODE_EXPERIENCE.MODE_3D;
    }

    public void CheckModeInitToView(MODE_EXPERIENCE _Mode)
    {
        Mode = _Mode;
        if (Mode == MODE_EXPERIENCE.MODE_3D)
        {
            ViewInitScreenMode3D();
        }
        else if (Mode == MODE_EXPERIENCE.MODE_AR)
        {
            ViewInitScreenModeAR();
        }
    }

    public void ViewScreenWithMode(MODE_EXPERIENCE _mode)
    {
        Mode = _mode;
        if (Mode == MODE_EXPERIENCE.MODE_3D)
        {
            ViewInitScreenMode3D();
            ObjectManager.Instance.Instantiate3DObject();
        }
        else if (Mode == MODE_EXPERIENCE.MODE_AR)
        {
            ViewInitScreenModeAR();
            ObjectManager.Instance.OriginObject.SetActive(false);
        }

        // MediaManager.Instance.StopMedia();
    }
    public void ViewInitScreenMode3D()
    {
        ARComponent.SetActive(false);

        _3DComponent.SetActive(true);
        UIComponentCommon.SetActive(true);

        Camera3D.tag = StringConfig.TAG_CAMERA_MAIN;
    }

    public void ViewInitScreenModeAR()
    {
        // Reset UI AR
        ARUIManager.Instance.RefreshStatusControl();

        _3DComponent.SetActive(false);
        UIComponentCommon.SetActive(false);

        ARComponent.SetActive(true);

        CameraAR.tag = StringConfig.TAG_CAMERA_MAIN;
    }
}
