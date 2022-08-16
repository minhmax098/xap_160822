using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Meta
{
    public int page; 
    public int pageSize; 
    public int totalPage; 
    public int totalElements; 
}

[System.Serializable]
public class AllOrgans
{
    public int code; 
    public string message; 
    public Meta meta; 
    public Lesson[] data;  
}

[System.Serializable]
public class AllXRLibrary
{
    public int code; 
    public string message; 
    public Meta meta; 
    public Lesson[] data;
}

[System.Serializable]
public class All3DModel
{
    public int code; 
    public string message; 
    public Meta meta; 
    public List3DModel[] data;
}