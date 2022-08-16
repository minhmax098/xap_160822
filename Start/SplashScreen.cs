using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Networking;
using EasyUI.Toast;
public class SplashScreen : MonoBehaviour
{
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        SetOrganCache();
    }

    void SetOrganCache()
    {
        string nextSceneName = string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefConfig.userToken)) ? SceneConfig.home_nosignin : SceneConfig.home_user;
        StartCoroutine(Helper.LoadAsynchronously(nextSceneName));
    }
}
