using System;
using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ARPointerManager : MonoBehaviour
{
    public ARRaycastManager raycastManager;
    List<ARRaycastHit> hitList = new List<ARRaycastHit>();
    Vector2 centerOfScreen;
    public bool IsForcedHiddenPointer {get; set;} = false;


    private static ARPointerManager instance;
    public static ARPointerManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ARPointerManager>();
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_3D)
        {
            return;
        }
        if (IsForcedHiddenPointer)
        {
            ARUIManager.Instance.arPointer.SetActive(false);
            return;
        }
        if (ObjectManager.Instance.OriginObject == null)
        {
            return;
        }
        if (!ObjectManager.Instance.OriginObject.activeSelf)
        {
            DetectPointer();
        }
    }
    void DetectPointer()
    {
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            return;
        }
      
        centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
        raycastManager.Raycast(centerOfScreen, hitList, TrackableType.Planes);
        if (hitList.Count > 0)
        {
            ARUIManager.Instance.OnActivePointer(hitList[0].pose);
        }
        else
        {
            ARUIManager.Instance.OnInactivePointer();
        }
    }
}
