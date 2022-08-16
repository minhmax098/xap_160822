using System.Collections;
using System.Collections.Generic;
using EasyUI.Toast;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public static class Network
{
    static bool IsDisconnecting = false;

    public static Exception CheckNetWorkToDisplayToast()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
            return new Exception(NetworkString.errorNetwork);
        return null;
    }


    public static void CheckNetWorkMoveScence()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (SceneManager.GetActiveScene().name != SceneConfig.network)
                SceneManager.LoadScene(SceneConfig.network);
            else
                Toast.ShowCommonToast(NetworkString.reconnectFailed, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
        }
        else
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            if (SceneManager.GetActiveScene().name == SceneConfig.network)
                SceneManager.LoadScene(SceneNameManager.prevScene);
        }
    }

    public static Exception CheckNetWorkMoveScenceForAPI()
    {
        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            SceneManager.LoadScene(SceneConfig.network);
            return new Exception(NetworkString.errorNetwork);
        }
        return null;
    }
}
