using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class TouchModel : MonoBehaviour
{
    private static TouchModel instance;
    public static TouchModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TouchModel>();
            }
            return instance;
        }
    }

    public static event Action onResetStatusFeature;
    public static event Action<GameObject> onSelectChildObject;

    const float ROTATION_RATE = 0.08f;
    const float LONG_TOUCH_THRESHOLD = 1f;
    const float ROTATION_SPEED = 0.5f;
    const float ALLOWED_DIFFERENCE = 0.00001f;
    float touchDuration = 0.0f;
    Touch touch;
    Touch touchZero;
    Touch touchOne;
    float originDelta;
    Vector3 originScale;

    Vector3 originLabelScale = new Vector3(1f, 1f, 1f);
    Vector3 originLabelTagScale = new Vector3(7f, 1f, 1f);
    Vector3 originLineScale = new Vector3(1, 1, 1);

    float currentDelta;
    float scaleFactor;
    Vector2 originPosition;
    bool isMovingByLongTouch = false;
    bool isLongTouch = false;
    Vector3 originScaleSelected;
    GameObject currentSelectedObject;
    private Vector3 mOffset;
    private float mZCoord;
    public Camera cam;


    public bool IsPointerOverUIObject(string targetUI)
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach(RaycastResult result in results)
        {
            if(result.gameObject.name == targetUI) return true;
        }
        return false;
    }

    public void TouchEventTest()
    {
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);
            Debug.Log("Touch position: " + touch.position);
            Debug.Log("Touch tap: " + touch.tapCount);
            
        }
    }

    public void HandleTouchInteraction()
    {
        if (ObjectModel.Instance.CurrentObject == null)
        {
            return;
        }
        else if (Input.touchCount == 2 && IsPointerOverUIObject("ImgRenderModel"))
        {
            touchZero = Input.GetTouch(0);
            touchOne = Input.GetTouch(1);
            HandleSimultaneousTouch(touchZero, touchOne);
        }
        else if (Input.touchCount == 1 && IsPointerOverUIObject("ImgRenderModel"))
        {
            touch = Input.GetTouch(0);
            if (touch.tapCount == 1)
            {
                HandleSingleTouch(touch);
            }
            else if (touch.tapCount == 2)
            {
                touch = Input.touches[0];
                if (touch.phase == TouchPhase.Ended)
                {
                    HandleDoupleTouch(touch);
                }
            }
        }
    }
    private void HandleSingleTouch(Touch touch)
    {
        switch (touch.phase)
        {
            case TouchPhase.Began:
                {
                    currentSelectedObject = Helper.GetChildOrganOnTouchByTag(touch.position);
                    if (currentSelectedObject != null)
                    {
                        Debug.Log("Touch pharse begin: " + currentSelectedObject.name);
                    }
                    isMovingByLongTouch = currentSelectedObject != null;
                    if (currentSelectedObject != null)
                    {
                        mZCoord = Camera.main.WorldToScreenPoint(currentSelectedObject.transform.position).z;
                        mOffset = currentSelectedObject.transform.position - Helper.GetTouchPositionAsWorldPoint(touch);
                    }
                    break;
                }

            case TouchPhase.Moved:
                {
                    if (isLongTouch)
                    {
                        Drag(touch, currentSelectedObject);
                    }
                    else
                    {
                        Rotate(touch);
                    }
                    break;
                }
            case TouchPhase.Ended:
                {
                    ResetLongTouch();
                    break;
                }

            case TouchPhase.Canceled:
                {
                    ResetLongTouch();
                    break;
                }
        }
    }

    void ResetLongTouch()
    {
        touchDuration = 0f;
        isLongTouch = false;
        isMovingByLongTouch = false;
        currentSelectedObject = null;
    }

    void OnLongTouchInvoke()
    {
        StartCoroutine(HightLightObject());
        isLongTouch = true;
    }

    IEnumerator HightLightObject()
    {
        originScaleSelected = currentSelectedObject.transform.localScale;
        currentSelectedObject.transform.localScale = originScaleSelected * 1.5f;
        yield return new WaitForSeconds(0.12f);
        currentSelectedObject.transform.localScale = originScaleSelected;
    }
    private void Rotate(Touch touch)
    {
        ObjectModel.Instance.OriginObject.transform.Rotate(touch.deltaPosition.y * ROTATION_RATE, -touch.deltaPosition.x * ROTATION_RATE, 0, Space.World);
    }

    private void Drag(Touch touch, GameObject obj)
    {
        if (obj != null)
        {
            obj.transform.position = Helper.GetTouchPositionAsWorldPoint(touch) + mOffset;
        }
    }

    private void HandleSimultaneousTouch(Touch touchZero, Touch touchOne)
    {
        if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
        {
            originDelta = Vector2.Distance(touchZero.position, touchOne.position);
            originScale = ObjectModel.Instance.OriginObject.transform.localScale;
        }
        else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
        {
            currentDelta = Vector2.Distance(touchZero.position, touchOne.position);
            scaleFactor = currentDelta / originDelta;

            if ((scaleFactor <= 1f && ObjectModel.Instance.OriginObject.transform.localScale.x / ObjectModel.Instance.OriginScale.x < 0.05f)
                ||
                (scaleFactor > 1f && ObjectModel.Instance.OriginObject.transform.localScale.x / ObjectModel.Instance.OriginScale.x > 2.5f))
            {
                return;
            }
            ObjectModel.Instance.OriginObject.transform.localScale = originScale * scaleFactor;
        }

    }

    private void HandleDoupleTouch(Touch touch)
    {
        GameObject selectedObject = Helper.GetChildOrganOnTouchByTag(touch.position);

        if (selectedObject == null || selectedObject == ObjectModel.Instance.OriginObject || ObjectModel.Instance.CurrentObject.transform.childCount < 1)
        {
            return;
        }
        onSelectChildObject?.Invoke(selectedObject);
    }

    private void HandleSimultaneousThreeTouch(Touch touch)
    {
        Drag(touch, ObjectModel.Instance.OriginObject);
    }
}
