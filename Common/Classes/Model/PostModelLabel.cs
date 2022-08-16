using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PostModelLabel
{
    public int lessonId;
    public int modelId;
    public string labelName;
    public Coordinate coordinates;
    public string level;
    public string subLevel;
}

[Serializable]
public class Coordinate
{
    public float x; 
    public float y; 
    public float z;
    public static Coordinate InitCoordinate(Vector3 coordinate) 
    { 
        Coordinate coordinateConf = new Coordinate();
        coordinateConf.x = coordinate.x; 
        coordinateConf.y = coordinate.y;
        coordinateConf.z = coordinate.z; 
        return coordinateConf;
    }
}

