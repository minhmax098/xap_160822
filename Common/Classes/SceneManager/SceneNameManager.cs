using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneNameManager : MonoBehaviour
{
    public static string prevScene;
    public static void setPrevScene(string _prevScene)
    {
        prevScene = _prevScene;
    }
}
