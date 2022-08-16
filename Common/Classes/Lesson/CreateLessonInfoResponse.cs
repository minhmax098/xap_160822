using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreateLessonInfoResponse
{
    public int code; 
    public string message;
    public DataCreateLesson data;
}

[System.Serializable]
public class DataCreateLesson
{
    public int lessonId;
    // public string lessonTitle;
}
