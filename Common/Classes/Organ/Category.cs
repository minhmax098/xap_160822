using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Category
{
    public string id;
    public string name;
    public int parentId;
}

[System.Serializable]
public class Categories
{
    // public int resultCode;
    // public string resultMessage;
    public Category[] categories;
}



