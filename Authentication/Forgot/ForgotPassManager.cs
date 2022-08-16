using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 
using System.Text;
using System.Text.RegularExpressions;
using EasyUI.Toast;
using AdvancedInputFieldPlugin;
using System;

public class ForgotPassManager : MonoBehaviour
{
    public GameObject waitingScreen; 
    public Button nextBtn; 
    public Button resetInputFieldBtn; 
    public AdvancedInputField emailInputField; 
    public GameObject emailNotificationText; 

    void Start()
    {
        InitScreen();
        InitUI();
        InitEvents();
    }
    
    void InitScreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }

    void InitUI()
    {
        HideAllNotifications();
        waitingScreen.SetActive(false);
    }

    void HideAllNotifications()
    {
        emailNotificationText.SetActive(false);
    }

    void InitEvents()
    {
        emailInputField.OnValueChanged.AddListener(CheckEmailFormat);
        nextBtn.onClick.AddListener(HandleForgotPass); 
        emailInputField.OnEndEdit.AddListener(OnEmailEndEdit);
    }

    public void OnEmailEndEdit(string arg0, EndEditReason reason)
    {
        if (reason == EndEditReason.KEYBOARD_DONE)
        {
            HandleForgotPass();
        }
    }

    private void HandleForgotPass()
    {
        string email = emailInputField.Text; 
        if (IsValidEmail(email))
        {
            CallAPIForgotPass(email);
        }
    }

    private void CheckEmailFormat(string data)
    {
        IsValidEmail(data);
    }

    private bool IsValidEmail(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            emailNotificationText.GetComponent<Text>().text = AuthenConfig.emptyValue;
            ChangeUIStatus(emailInputField, emailNotificationText, true);
            nextBtn.interactable = false;
            return false;
        }
        else if (!AuthenConfig.emailRgx.IsMatch(data))
        {
            emailNotificationText.GetComponent<Text>().text = AuthenConfig.invalidEmail;
            ChangeUIStatus(emailInputField, emailNotificationText, true);
            nextBtn.interactable = false;
            return false;
        }
        nextBtn.interactable = true;
        ChangeUIStatus(emailInputField, emailNotificationText, false);
        return true;
    }
    private void ChangeUIStatus(AdvancedInputField input, GameObject warning, bool status)
    {
        warning.SetActive(status); 
        if(status)
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
        }
        else
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        }
    }
    
    async void CallAPIForgotPass(string email)
    {
        try
        {
            FreezeUI(true);
            ForgotPassRequest forgotPassRequest = new ForgotPassRequest();
            forgotPassRequest.Init(email);
            APIResponse<string> userResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_FORGOT_PASS, UnityWebRequest.kHttpVerbPOST, forgotPassRequest);
            if (userResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                PlayerPrefs.SetString(PlayerPrefConfig.emailForResetPass, email);
                SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
                SceneManager.LoadScene(SceneConfig.resetpass);
            }
            else 
            {
                throw new Exception(userResponse.message);
            }
        }
        catch (Exception e)
        {
            FreezeUI(false);
        }
    }

    void FreezeUI(bool isFreezed)
    {
        nextBtn.interactable = !isFreezed;
        emailInputField.interactable = !isFreezed;
        resetInputFieldBtn.interactable = !isFreezed;
        waitingScreen.SetActive(isFreezed);
    }
}   
