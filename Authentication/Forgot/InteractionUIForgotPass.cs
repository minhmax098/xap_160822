using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUIForgotPass : MonoBehaviour
{
    public Button backBtn; 
    void Start()
    {
        SetActions(); 
    }
 
    void SetActions()
    {
        backBtn.onClick.AddListener(BackToSignIn); 
    }
    void BackToSignIn()
    {
        BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name); 
    }
}
