using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TagHandler : MonoBehaviour
{
    private static TagHandler instance;
    public static TagHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TagHandler>();
            }
            return instance;
        }
    }

    public List<LabelObjectInfo> addedTags = new List<LabelObjectInfo>();
    public List<Vector3> positionOriginLabel = new List<Vector3>();

    void Update()
    {
        if (LabelManager.Instance.IsShowingLabel)
        {
            OnMove();
        }
    }

    public void AddTag(LabelObjectInfo labelObjectInfor)
    {
        addedTags.Add(labelObjectInfor);
    }

    public void DeleteTags()
    {
        addedTags.Clear();
        positionOriginLabel.Clear();
    }

    public void OnMove()
    {
        foreach (LabelObjectInfo item in addedTags)
        {
            if (item.labelObject != null)
            {
                DenoteTag(item);
                MoveTag(item);
            }
        }
    }

    public void DenoteTag(LabelObjectInfo labelObjectInfo)
    {
        if (labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).GetChild(0).transform.position.x > 0f)
        {
            labelObjectInfo.indexSideDisplay = 2;
            labelObjectInfo.labelObject.transform.GetChild(2).localScale = labelObjectInfo.labelObject.transform.GetChild(1).localScale;
            labelObjectInfo.labelObject.transform.GetChild(1).gameObject.SetActive(false);
            labelObjectInfo.labelObject.transform.GetChild(2).gameObject.SetActive(true);
        }
        else
        {
            labelObjectInfo.indexSideDisplay = 1;
            labelObjectInfo.labelObject.transform.GetChild(1).localScale = labelObjectInfo.labelObject.transform.GetChild(2).localScale;
            labelObjectInfo.labelObject.transform.GetChild(2).gameObject.SetActive(false);
            labelObjectInfo.labelObject.transform.GetChild(1).gameObject.SetActive(true);
        }
        labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).localPosition = labelObjectInfo.labelObject.transform.GetChild(0).GetComponent<LineRenderer>().GetPosition(1);

        if (labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).GetChild(0).transform.position.z > 1f)
            labelObjectInfo.labelObject.transform.gameObject.SetActive(false);
        else
            labelObjectInfo.labelObject.transform.gameObject.SetActive(true);
    }

    public void MoveTag(LabelObjectInfo labelObjectInfo)
    {
        labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).transform.LookAt(
                labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).position + Camera.main.transform.rotation * Vector3.forward, 
                Camera.main.transform.rotation * Vector3.up);
        labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).GetChild(0).transform.LookAt(
                labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).GetChild(0).position + Camera.main.transform.rotation * Vector3.forward, 
                Camera.main.transform.rotation * Vector3.up);
        labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).GetChild(0).GetChild(0).transform.LookAt(
                labelObjectInfo.labelObject.transform.GetChild(labelObjectInfo.indexSideDisplay).GetChild(0).GetChild(0).position + Camera.main.transform.rotation * Vector3.forward, 
                Camera.main.transform.rotation * Vector3.up);
    }
}
