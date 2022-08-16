using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUIResetPass : MonoBehaviour
{
    public Button backtoForgotBtn; 
    void Start()
    {
        SetActions(); 
    }
    void SetActions()
    {
        backtoForgotBtn.onClick.AddListener(BackToForgot); 
    }
    void BackToForgot()
    {
        BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);  
    }
}
