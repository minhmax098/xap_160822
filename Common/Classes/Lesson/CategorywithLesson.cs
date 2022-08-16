using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CategorywithLesson
{
    public int id;
    public string name;
    public int parent_id;
    public Lesson[] lessons;
}

public class CategorywithLessons
{
    public CategorywithLesson[] categorywithLessons;
}