using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class List3DModel
{
    public int modelId; 
    public string modelName; 
    public int modelFileId; 
    public int thumbnailFileId; 
    public string modelThumbnail; 
    public int total; 
}

[System.Serializable]
public class AllList3DModel
{
    public int code; 
    public string message; 
    public List3DModel[] data;
}

