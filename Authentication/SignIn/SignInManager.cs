using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Text;
using EasyUI.Toast;
using AdvancedInputFieldPlugin;
using System;

public class SignInManager : MonoBehaviour
{
    public AdvancedInputField userNameInputField;
    public AdvancedInputField capchaInputField;
    public AdvancedInputField passwordInputField;

    public GameObject userNameWarningText;
    public GameObject passwordWarningText;
    public GameObject invalidCapchaWarningText;

    public Button signInBtn;
    public GameObject waitingScreen;
    public GameObject capchaComponent;
    private int failureNumber;
    public List<Button> resetInputFieldBtns;
    public Button togglePasswordFieldBtn;
    public Button capchaGenerationBtn;
    public Button forgotPasswordBtn;
    public Image backgroundImage;
    public List<Sprite> backgroundSprrites;
    public GameObject logoObject;
    void Start()
    {
        InitScreen();
        InitUI();
        HideAllNotifications();
        InitEvents();
    }

    void OnEnable()
    {
        NativeKeyboardManager.AddKeyboardHeightChangedListener(OnKeyboardHeightChanged); 
    }

    void OnDisable()
    {
        NativeKeyboardManager.RemoveKeyboardHeightChangedListener(OnKeyboardHeightChanged); 
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
        capchaComponent.SetActive(false);
    }

    void InitEvents()
    {
        userNameInputField.OnValueChanged.AddListener(CheckUserName);
        passwordInputField.OnValueChanged.AddListener(CheckPassword);
        signInBtn.onClick.AddListener(HandleSignIn);
        capchaInputField.OnValueChanged.AddListener(HandleCaptcha);
        capchaInputField.OnBeginEdit.AddListener(delegate { TransformSignInForm(true); });
        userNameInputField.OnBeginEdit.AddListener(delegate { TransformSignInForm(true); });
        passwordInputField.OnBeginEdit.AddListener(delegate { TransformSignInForm(true); });
        passwordInputField.OnEndEdit.AddListener(OnPasswordEndEdit);
        capchaInputField.OnEndEdit.AddListener(OnCapchaEndEdit);
    }

    public void OnPasswordEndEdit(string arg0, EndEditReason reason)
    {
        if (reason == EndEditReason.KEYBOARD_DONE)
        {
            HandleSignIn();
        }
    }
     public void OnCapchaEndEdit(string arg0, EndEditReason reason)
    {
        if (reason == EndEditReason.KEYBOARD_DONE)
        {
            HandleSignIn();
        }
    }

    public void OnKeyboardHeightChanged(int keyboardHeight)
    {
        if (keyboardHeight == 0)
        {
            TransformSignInForm(false);
        }
    }

    void HideAllNotifications()
    {
        userNameWarningText.SetActive(false);
        passwordWarningText.SetActive(false);
        invalidCapchaWarningText.SetActive(false);
    }

    void HandleCaptcha(string data)
    {
        invalidCapchaWarningText.SetActive(false);
    }

    void CheckUserName(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            UpdateInputFieldStatus(userNameInputField, userNameWarningText, false);
        }
    }

    void CheckPassword(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            UpdateInputFieldStatus(passwordInputField, passwordWarningText, false);
        }
    }

    private void HandleSignIn()
    {
        HideAllNotifications();

        string email = userNameInputField.Text;
        string pass = passwordInputField.Text;
        bool isInvalidInfo = false;
        
        if (string.IsNullOrEmpty(email))
        {
            UpdateInputFieldStatus(userNameInputField, userNameWarningText, true);
            isInvalidInfo = true;
        }

        if (string.IsNullOrEmpty(pass))
        {
            UpdateInputFieldStatus(passwordInputField, passwordWarningText, true);
            isInvalidInfo = true;
        }

        if (failureNumber >= 5)
        {
            string capchaCode = capchaInputField.Text;
            if (string.IsNullOrEmpty(capchaCode))
            {
                invalidCapchaWarningText.GetComponent<Text>().text = AuthenConfig.emptyValue;
                UpdateInputFieldStatus(capchaInputField, invalidCapchaWarningText, true, true);
                isInvalidInfo = true;
            }
            else if (!capchaCode.Equals(PlayerPrefs.GetString(PlayerPrefConfig.capcha)))
            {
                invalidCapchaWarningText.GetComponent<Text>().text = AuthenConfig.invalidCapcha;
                UpdateInputFieldStatus(capchaInputField, invalidCapchaWarningText, true, true);
                isInvalidInfo = true;
                CapchaGeneration.Instance.GenCapchaCode();
            }
        }
        if (!isInvalidInfo)
        {
            TransformSignInForm(false);
            CallAPISignIn(email, pass);
        }
    }

    private void UpdateInputFieldStatus(AdvancedInputField input, GameObject warning, bool status, bool isCapcha = false)
    {
        warning.SetActive(status);
        if (status)
        {
            if (isCapcha)
            {
                input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageCapchaWarning);
            }
            else
            {
                input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
            }
        }
        else
        {
            if (isCapcha)
            {
                input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
            }
            else
            {
                input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
            }
        }
    }

    async void CallAPISignIn(string email, string password)
    {
        try
        {
            FreezeUI(true);
            SignInRequest signInRequest = new SignInRequest();
            signInRequest.Init(email, password);
            APIResponse<List<Data>> userResponse = await UnityHttpClient.CallAPI<List<Data>>(APIUrlConfig.POST_SIGN_IN, UnityWebRequest.kHttpVerbPOST, signInRequest);
            if (userResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                PlayerPrefs.SetString(PlayerPrefConfig.userEmail, userResponse.data[0].email);
                PlayerPrefs.SetString(PlayerPrefConfig.userName, userResponse.data[0].fullName);
                
                Debug.Log("USER TOKEN: " + userResponse.data[0].token);
                PlayerPrefs.SetString(PlayerPrefConfig.userToken, userResponse.data[0].token);
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.home_user));
            }
            else 
            {
                throw new Exception(userResponse.message);
            }
        }
        catch (Exception e)
        {
            failureNumber++;
            CapchaGeneration.Instance.GenCapchaCode();
            capchaInputField.SetText("", true);
            if (failureNumber >= 5)
            {
                passwordInputField.NextInputField = capchaInputField;
                capchaComponent.SetActive(true);
            }
            FreezeUI(false);
        }
    }

    void FreezeUI(bool isFreezed)
    {
        signInBtn.interactable = !isFreezed;
        togglePasswordFieldBtn.interactable = !isFreezed;
        capchaGenerationBtn.interactable = !isFreezed;
        foreach (Button button in resetInputFieldBtns)
        {
            button.interactable = !isFreezed;
        }
        forgotPasswordBtn.interactable = !isFreezed;
        userNameInputField.interactable = !isFreezed;
        passwordInputField.interactable = !isFreezed;
        capchaInputField.interactable = !isFreezed;
        waitingScreen.SetActive(isFreezed);
    }

    void TransformSignInForm(bool isUp)
    {
        if (isUp)
        {
            backgroundImage.sprite = backgroundSprrites[1];
            logoObject.SetActive(false);

        }
        else
        {
            backgroundImage.sprite = backgroundSprrites[0];
            logoObject.SetActive(true);
        }
    }
}
