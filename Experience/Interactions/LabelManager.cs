using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LabelManager : MonoBehaviour
{
    private static LabelManager instance;
    public static LabelManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LabelManager>();
            }
            return instance;
        }
    }

    private const float LONG_LINE_FACTOR = 6f;
    private string levelObject = "";

    // UI
    public Button btnLabel;
    private bool isShowingLabel;
    public bool IsShowingLabel
    {
        get
        {
            return isShowingLabel;
        }
        set
        {
            isShowingLabel = value;
            btnLabel.GetComponent<Image>().sprite = isShowingLabel ? Resources.Load<Sprite>(PathConfig.LABEL_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.LABEL_UNCLICK_IMAGE);
        }
    }

    public void HandleLabelView(bool currentLabelStatus)
    {
        IsShowingLabel = currentLabelStatus;
        if (IsShowingLabel)
        {
            btnLabel.interactable = false;
            CreateLabel();
            btnLabel.interactable = true;
        }
        else
        {
            btnLabel.interactable = false;
            ClearLabel();
            btnLabel.interactable = true;
        }
    }

    public bool CheckAvailableLabel(GameObject obj)
    {
        if (StaticLesson.ListLabel.Length <= 0)
            return false;

        string levelObject = Helper.GetLevelObjectInLevelParent(obj);
        foreach (Label item in StaticLesson.ListLabel)
        {
            if (item.level == levelObject)
                return true;
        }
        return false;
    }

    public void CreateLabel()
    {
        ClearLabel();
        levelObject = Helper.GetLevelObjectInLevelParent(ObjectManager.Instance.CurrentObject);
        foreach (Label itemInforLabel in StaticLesson.ListLabel)
        {
            if (itemInforLabel.level == levelObject)
            {
                GameObject labelObject = Instantiate(Resources.Load(PathConfig.MODEL_TAG_LABEL) as GameObject);
                
                labelObject.tag = TagConfig.LABEL_TAG;
                labelObject.transform.SetParent(ObjectManager.Instance.CurrentObject.transform, false);

                if (ObjectManager.Instance.OriginObject.transform.localScale.x > ObjectManager.Instance.OriginScale.x)
                    labelObject.transform.localScale = ExperienceConfig.ScaleOriginLabel / (1.3f * ObjectManager.Instance.OriginObject.transform.localScale.x / ObjectManager.Instance.OriginScale.x);
                else
                    labelObject.transform.localScale = 0.8f * ExperienceConfig.ScaleOriginLabel / (ObjectManager.Instance.OriginObject.transform.localScale.x / ObjectManager.Instance.OriginScale.x);
                SetLabel(labelObject, itemInforLabel);
            }
        }
    }

    public void ClearLabel()
    {
        foreach (LabelObjectInfo item in TagHandler.Instance.addedTags)
        {
            Destroy(item.labelObject);
        }
        TagHandler.Instance.DeleteTags();
    }

    public void SetLabel(GameObject labelObject, Label label)
    {
        // set infor for Line
        GameObject line = labelObject.transform.GetChild(0).gameObject;
        Vector3 pointClickInObject = new Vector3(label.coordinates.x, label.coordinates.y, label.coordinates.z);
        TagHandler.Instance.positionOriginLabel.Add(pointClickInObject);
        Vector3 targetLine = pointClickInObject * 2f;

        // foreach (Transform child in ObjectManager.Instance.CurrentObject.transform)
        // {
        //     if (Helper.CalculateBounds(child.gameObject).Contains(pointClickInObject * 0.6f))
        //         labelObject.transform.SetParent(child, false);
        // }
        
        // line.GetComponent<LineRenderer>().SetPosition(0, pointClickInObject * 0.6f);
        line.GetComponent<LineRenderer>().SetPosition(1, targetLine);

        // show label side by axis x
        int indexShowLabel = 0;
        if (label.coordinates.x >= 0)
            indexShowLabel = 2; // show label in right side
        else
            indexShowLabel = 1; // left side;

        LabelObjectInfo labelObjectInfo = new LabelObjectInfo();
        labelObjectInfo.labelObject = labelObject;
        labelObjectInfo.indexSideDisplay = indexShowLabel;
        TagHandler.Instance.AddTag(labelObjectInfo);

        SetContentLabel(labelObject.transform.GetChild(1), label, indexShowLabel, targetLine);
        SetContentLabel(labelObject.transform.GetChild(2), label, indexShowLabel, targetLine);
    }

    public void SetContentLabel(Transform labelSideObject, Label label, int indexShowLabel, Vector3 targetLine)
    {
        labelSideObject.localPosition = targetLine;
        Camera.main.WorldToScreenPoint(labelSideObject.position);
        labelSideObject.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = label.labelName;
        labelSideObject.gameObject.SetActive(labelSideObject.transform.GetSiblingIndex() == indexShowLabel);
    }
}