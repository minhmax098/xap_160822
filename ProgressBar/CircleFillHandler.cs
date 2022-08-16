using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CircleFillHandler : MonoBehaviour
{
    [Range(0,100)]
    public float fillValue = 0; 
    public Image circleFillImage; 
    public RectTransform handlerEdgeImage; 
    public RectTransform fillHandler; 

    private float time = .0f;
    private float duration = 1f;
    void Start()
    {
        
    }

    void Update()
    {
        fillValue = 100 * (time - duration * (int)Mathf.Floor(time/duration));
        time += Time.deltaTime;
        FillCircleHandler(fillValue);
    }
    void FillCircleHandler(float value)
    {
        float fillAmount = (value / 100.0f); 
        circleFillImage.fillAmount = fillAmount;
        float angle = fillAmount * 360; 
        fillHandler.localEulerAngles = new Vector3(0, 0, -angle); 
        handlerEdgeImage.localEulerAngles = new Vector3(0, 0, angle);
    }
}
