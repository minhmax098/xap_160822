using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 

public class InteractionUIPassReset : MonoBehaviour
{
    public Button nexttoSignInBtn; 
    public Button backtoResetPassBtn; 
    void Start()
    {
        InitScreen();
        SetActions(); 
    }

    void InitScreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }

    void SetActions()
    {
        nexttoSignInBtn.onClick.AddListener(NextToSignIn); 
        backtoResetPassBtn.onClick.AddListener(BackToResetPass); 
    }
   
    void NextToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.signIn); 
    }
    void BackToResetPass()
    {
        BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name); 
    }
}
