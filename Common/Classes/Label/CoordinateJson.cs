using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CoordinateJson
{
    public int code;
    public string message;
    public DataCoordinate data;
}

[System.Serializable]
public class DataCoordinate
{
    public int labelId;
}
