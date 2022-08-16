using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUISignUp : MonoBehaviour
{
    public GameObject waitingScreen; 
    public GameObject switchToHomeNoSignIn;
    public GameObject switchToSignInBtn; 
    void Start()
    {
        InitUI(); 
        SetActions(); 
    }
    void InitUI()
    {
        waitingScreen.SetActive(false); 
    }   
    void SetActions()
    {
        waitingScreen.SetActive(false); 
        switchToHomeNoSignIn.GetComponent<Button>().onClick.AddListener(SwitchToHomeNoSignIn); 
        switchToSignInBtn.GetComponent<Button>().onClick.AddListener(SwitchToSignIn); 
    }
    void SwitchToHomeNoSignIn()
    {
        BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);  
    }
    void SwitchToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.signIn); 
    }
}
