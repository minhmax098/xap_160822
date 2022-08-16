using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ListOrgans
{
    public int code;
    public string message;
    public ListOrganLesson[] data;  
}
[System.Serializable]
public class ListOrganLesson // need change name of class correctly
{
    public int organsId;
    public string organsName;
    public int isActive;
    public string organsThumbnailId;
}

