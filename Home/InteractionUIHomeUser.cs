using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class InteractionUIHomeUser : MonoBehaviour
{
    [SerializeField]
    Animator statusAnimator;
    public GameObject signOutBtn; 
    public GameObject panelSignOut;
    public GameObject btnExitPopUp; 
    public GameObject btnSignOut;
    public GameObject btnCancel; 

    public GameObject aboutXAPBtn; 
    public GameObject panelAboutXAP; 
    public GameObject btnExitPopUpXAP; 
    public GameObject btnClose; 
    
    void Start()
    {
        SetActions(); 
    }

    void SetActions()
    {
        signOutBtn.GetComponent<Button>().onClick.AddListener(HandlerSignOutBtn);
        btnExitPopUp.GetComponent<Button>().onClick.AddListener(HandlerExitPopUpSignOut);
        btnSignOut.GetComponent<Button>().onClick.AddListener(HandlerBtnSignOut); 
        btnCancel.GetComponent<Button>().onClick.AddListener(HandlerBtnCancel); 

        aboutXAPBtn.GetComponent<Button>().onClick.AddListener(HandlerAboutXAPBtn);
        btnExitPopUpXAP.GetComponent<Button>().onClick.AddListener(HandlerExitPopUpXAP);
        btnClose.GetComponent<Button>().onClick.AddListener(HandlerCloseXAP);
    }

    void HandlerSignOutBtn()
    {
        statusAnimator.SetBool(AnimatorConfig.showLeftPanel, false); 
        panelSignOut.SetActive(true); 
    }

    void HandlerExitPopUpSignOut()
    {
        panelSignOut.SetActive(false); 
    }

    void HandlerBtnSignOut()
    {
        Helper.Logout();
    }

    void HandlerBtnCancel()
    {
        panelSignOut.SetActive(false); 
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
}
