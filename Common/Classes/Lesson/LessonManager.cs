using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public static class LessonManager
{
//  public static Lesson lesson { get; set; }
    public static int lessonId;
    public static void InitLesson(int _lessonId)
    { 
        lessonId = _lessonId;
    }
//  public static int lesson_id; 
//  public static void InitLesson(int _lesson_id)
//  {
//      lesson_id = _lesson_id;
//  }
}
