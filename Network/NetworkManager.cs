using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class NetworkManager : MonoBehaviour
{
    public Button btnTryAgain;
    // Start is called before the first frame update
    void Start()
    {
        InitScreen();
        InitEvent();
    }

    void InitScreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }

    void InitEvent()
    {
        btnTryAgain.onClick.AddListener(ReconnectNetwork);
    }

    void ReconnectNetwork()
    {
        Network.CheckNetWorkMoveScence();
    }
}
