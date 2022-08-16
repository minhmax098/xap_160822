using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SaveLesson
{
    public static int lessonId; 
    public static string lessonTitle;
    public static int modelId;
    
    public static void SaveLessonId(int _lessonId)
    {
        lessonId = _lessonId;
    }
    public static void SaveLessonTitle(string _lessonTitle)
    {
        lessonTitle = _lessonTitle;
    }
    public static void SaveModelId(int _modelId)
    {
        modelId = _modelId;
    }
}
