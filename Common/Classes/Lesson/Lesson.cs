using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Lesson
{
    public int lessonId; 
    public string lessonThumbnail; 
    public string lessonTitle; 
    public int viewed; 
    public int isFavorate; 
    
}

[System.Serializable]
public class Lessons
{
    public Lesson[] categorywithLessons;
}

[System.Serializable]
public class fourTypes
{
    public Lesson[] myFavorates;
    public Lesson[] mostViewed; 
    public Lesson[] recommendedLessons;
    public Lesson[] myLessons; 
}

[System.Serializable]
public class AllLessons
{
    public int code;
    public string message; 
    public fourTypes data; 
}


