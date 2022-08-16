using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeparateManager : MonoBehaviour
{
    private static SeparateManager instance;
    public static SeparateManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SeparateManager>();
            return instance;
        }
    }

    // constantly
    private const float RADIUS = 8f;
    private const float DISTANCE_FACTOR = 2f;

    // variable
    private int childCount;
    private Vector3 centerPosition;
    private Vector3 centerPosCurrentObject;
    private Vector3 centerPosChildObject;

    private Vector3 targetPosition;
    private float angle;
    public Button btnSeparate;
    private bool isSeparating;
    public bool IsSeparating
    {
        get
        {
            return isSeparating;
        }
        set
        {
            isSeparating = value;
            btnSeparate.GetComponent<Image>().sprite = isSeparating ? Resources.Load<Sprite>(PathConfig.SEPARATE_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.SEPARATE_UNCLICK_IMAGE);
        }
    }

    public void HandleSeparate(bool isSeparating)
    {
        IsSeparating = isSeparating;
        if (IsSeparating)
        {
            btnSeparate.interactable = false;
            SeparateOrganModel();
            btnSeparate.interactable = true;
        }
        else
        {
            btnSeparate.interactable = false;
            BackToPositionOrgan();
            btnSeparate.interactable = true;
        }
    }

    public void SeparateOrganModel()
    {
        childCount = ObjectManager.Instance.CurrentObject.transform.childCount;
        centerPosCurrentObject = ObjectManager.Instance.CurrentObject.transform.localPosition;
        int i = 0;
        foreach (Transform childTransform in ObjectManager.Instance.CurrentObject.transform)
        {
            if (childTransform.tag != TagConfig.LABEL_TAG)
            {
                centerPosChildObject = Helper.CalculateBounds(childTransform.gameObject).center;
                targetPosition = ComputeTargetPosition(centerPosCurrentObject, centerPosChildObject);
                StartCoroutine(MoveObjectWithLocalPosition(childTransform.gameObject, targetPosition));
                i++;
            }
        }
    }
    public Vector3 ComputeTargetPosition(Vector3 center, Vector3 currentPosition)
    {
        Vector3 dir = currentPosition - center;
        return dir.normalized * DISTANCE_FACTOR / ObjectManager.Instance.FactorScaleInitial;
    }

    public IEnumerator MoveObjectWithLocalPosition(GameObject moveObject, Vector3 targetPosition)
    {
        float timeSinceStarted = 0f;
        while (true)
        {
            timeSinceStarted += Time.deltaTime;
            moveObject.transform.localPosition = Vector3.Lerp(moveObject.transform.localPosition, targetPosition, timeSinceStarted);
            if (moveObject.transform.localPosition == targetPosition)
            {
                yield break;
            }
            yield return null;
        }
    }
    public void BackToPositionOrgan()
    {
        int childCount = ObjectManager.Instance.ListchildrenOfOriginPosition.Count;
        if (childCount < 1)
        {
            return;
        }
        for (int i = 0; i < childCount; i++)
        {   
            {
                targetPosition = ObjectManager.Instance.ListchildrenOfOriginPosition[i];
                StartCoroutine(MoveObjectWithLocalPosition(ObjectManager.Instance.CurrentObject.transform.GetChild(i).gameObject, targetPosition));
            }
        }
    }
}