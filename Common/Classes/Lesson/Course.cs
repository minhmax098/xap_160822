using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Course
{
    public int id;
    public string name;
    public int author_id;
    public string description;
    public int number_less;
    public string thumbnail_file_path;
}

[System.Serializable]
public class Courses
{
    public Course[] courses;
}

