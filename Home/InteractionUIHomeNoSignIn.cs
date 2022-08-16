using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUIHomeNoSignIn : MonoBehaviour
{
    [SerializeField]
    Animator statusAnimator; 
    public GameObject aboutXAPBtn; 
    public GameObject panelAboutXAP; 
    public GameObject btnExitPopUpXAP; 
    public GameObject btnClose; 
    public GameObject nextToSignInBtn; 
    public GameObject nextToSignUpBtn; 
    
    void Start()
    {
        InitUI(); 
        SetActions(); 
    }

    void InitUI()
    {
        nextToSignInBtn = GameObject.Find("Footer"); 
        nextToSignUpBtn = GameObject.Find("BtnSignUp"); 

    }

    void SetActions()
    {
        nextToSignInBtn.GetComponent<Button>().onClick.AddListener(NextToSignIn); 
        nextToSignUpBtn.GetComponent<Button>().onClick.AddListener(NextToSignUp); 
        aboutXAPBtn.GetComponent<Button>().onClick.AddListener(HandlerAboutXAPBtn); 
        btnExitPopUpXAP.GetComponent<Button>().onClick.AddListener(HandlerExitPopUpXAP);
        btnClose.GetComponent<Button>().onClick.AddListener(HandlerCloseXAP); 
    }

    void HandlerAboutXAPBtn()
    {
        statusAnimator.SetBool(AnimatorConfig.showLeftPanel, false); 
        panelAboutXAP.SetActive(true); 
    }

    void HandlerExitPopUpXAP()
    {
        panelAboutXAP.SetActive(false); 
    }

    void HandlerCloseXAP()
    {
        panelAboutXAP.SetActive(false); 
    }

    void NextToSignIn()
    {
        SceneManager.LoadScene(SceneConfig.signIn); 
    }

    void NextToSignUp()
    {
        BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.signUp);
        SceneManager.LoadScene(SceneConfig.signUp); 
    }
}
