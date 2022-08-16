using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GetLessonDetail
{
    public string lessonTitle;
    public string lessonThumbnail;
    public int viewd; 
    public string lessonObjectives; 
    public int lessonId; 
    public string size; 
    public int createdBy; 
    public string authorAvatar; 
    public string authorName; 
    public string createdDate; 
    public int organId; 
    public int modelId; 
    public string modelField; 
    public string video; 
    public string audio; 
    public List<ListLabel> listLabel; 
}

[System.Serializable]
public class AllGetLessonDetail
{
    public int code; 
    public string messsage; 
    public GetLessonDetail[] data; 
}

[System.Serializable]
public class ListLabel
{
    public int labelId; 
    public string labelName; 
    public string coordinate; 
    public string labelIndex; 
    public int level; 
    public int parentId; 
    public string videoLabel; 
    public string audioLabel;
}
