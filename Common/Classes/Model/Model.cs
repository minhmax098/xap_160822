using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Model
{
    public int id;
    public string name;
    public string model_file_path;
    public string thumbnail_file_path;
    public bool is_trial_available; 
} 

[System.Serializable]
public class Models
{
    public Model[] models;
}

