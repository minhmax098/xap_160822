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

public class ResetPassManager : MonoBehaviour
{
    public GameObject waitingScreen;
    public Button resetPassBtn; 
    public AdvancedInputField secretCodeInputField; 
    public AdvancedInputField passwordInputField; 
    public AdvancedInputField passwordConfirmationInputField; 

    public GameObject secretCodeNotificationText; 
    public GameObject passwordNotificationText; 
    public GameObject passwordConfirmationNotificationText; 
    public List<Button> resetInputFieldBtns;
    public Button togglePasswordBtn;
    public Button togglePasswordConfirmationPasswordBtn;
    public GameObject resetPassFormObject;
    private bool isFirstTimeInputPassConfirmation = true;
    public Image backgroundImage;
    public List<Sprite> backgroundSprrites;
    public GameObject logoObject;
    private float DELTA_TRANSFORM = 100;
    void Start()
    {
        InitScreen();
        InitUI();
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
        waitingScreen.SetActive(false);
    }

    void HideAllNotifications()
    {
        secretCodeNotificationText.SetActive(false);
        passwordNotificationText.GetComponent<Text>().text = "";
        passwordConfirmationNotificationText.SetActive(false);
    }

    void InitEvents()
    {
        secretCodeInputField.OnValueChanged.AddListener(CheckSecretCode);
        passwordInputField.OnValueChanged.AddListener(CheckPasswordStrength);
        passwordConfirmationInputField.OnValueChanged.AddListener(CheckPasswordMatching);
        secretCodeInputField.OnBeginEdit.AddListener(delegate { TransformResetPassInForm(true); });
        passwordInputField.OnBeginEdit.AddListener(delegate { TransformResetPassInForm(true);});
        passwordConfirmationInputField.OnBeginEdit.AddListener(delegate { TransformResetPassInForm(true); });
        resetPassBtn.onClick.AddListener(HandleResetPass);
        passwordConfirmationInputField.OnEndEdit.AddListener(OnPasswordConfirmationEndEdit);
    }

    public void OnPasswordConfirmationEndEdit(string arg0, EndEditReason reason)
    {
        if (reason == EndEditReason.KEYBOARD_DONE)
        {
            HandleResetPass();
        }
    }

    private void CheckSecretCode(string data)
    {
        IsValidSecretCode(data);
    }

    private bool IsValidSecretCode(string data)
    {
         if (string.IsNullOrEmpty(data))
        {
            secretCodeNotificationText.GetComponent<Text>().text = AuthenConfig.emptyValue;
            HightLightInputField(secretCodeInputField, secretCodeNotificationText, true);
            return false;
        }
        else if (data.Length != AuthenConfig.secretCodeLength)
        {
            secretCodeNotificationText.GetComponent<Text>().text = AuthenConfig.invalidSecretCode;
            HightLightInputField(secretCodeInputField, secretCodeNotificationText, true);
            return false;
        }
        
        HightLightInputField(secretCodeInputField, secretCodeNotificationText, false);
        return true;
    }

    private void CheckPasswordStrength(string data)
    {
        IsValidPassword(data);
    }

    private bool IsValidPassword(string data)
    {
        if (!isFirstTimeInputPassConfirmation)
        {
            IsValidPasswordConfirmation(passwordConfirmationInputField.Text);
        }
        if (string.IsNullOrEmpty(data))
        {
            passwordNotificationText.GetComponent<Text>().text = AuthenConfig.emptyValue;
            HightLightInputField(passwordInputField, passwordNotificationText, true);
            RefreshLayoutGroupsForLongNotifications(resetPassFormObject);
            return false;
        }
        else if (!AuthenConfig.passRgx.IsMatch(data))
        {
            passwordNotificationText.GetComponent<Text>().text = AuthenConfig.invalidPassword;
            HightLightInputField(passwordInputField, passwordNotificationText, true);
            RefreshLayoutGroupsForLongNotifications(resetPassFormObject);
            return false;
        }
        passwordNotificationText.GetComponent<Text>().text = "";
        passwordInputField.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        RefreshLayoutGroupsForLongNotifications(resetPassFormObject);
        return true;
    }

    public void RefreshLayoutGroupsForLongNotifications(GameObject root)
    {
        foreach (var layoutGroup in root.GetComponentsInChildren<LayoutGroup>())
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
        }
    }
    private void CheckPasswordMatching(string data)
    {
        isFirstTimeInputPassConfirmation = false;
        IsValidPasswordConfirmation(data);
    }

    private bool IsValidPasswordConfirmation(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            passwordConfirmationNotificationText.GetComponent<Text>().text = AuthenConfig.emptyValue;
            HightLightInputField(passwordConfirmationInputField, passwordConfirmationNotificationText, true);
            return false;
        }
        else if (!data.Equals(passwordInputField.Text))
        {
            passwordConfirmationNotificationText.GetComponent<Text>().text = AuthenConfig.invalidPasswordConfirmation;
            HightLightInputField(passwordConfirmationInputField, passwordConfirmationNotificationText, true);
            return false;
        }
        HightLightInputField(passwordConfirmationInputField, passwordConfirmationNotificationText, false);
        return true;
    }

    public void OnKeyboardHeightChanged(int keyboardHeight)
    {
        if (keyboardHeight == 0)
        {
            TransformResetPassInForm(false);
        }
    }
    
    void TransformResetPassInForm(bool isUp)
    {
        RectTransform rectTransform = resetPassFormObject.GetComponent<RectTransform>();
        if (isUp)
        {
            backgroundImage.sprite = backgroundSprrites[1];
            logoObject.SetActive(false);
            rectTransform.offsetMax =  new Vector2(rectTransform.offsetMax.x, DELTA_TRANSFORM);
        }
        else
        {
            backgroundImage.sprite = backgroundSprrites[0];
            logoObject.SetActive(true);
            rectTransform.offsetMax =  new Vector2(rectTransform.offsetMax.x, 0);
        }
    }
    void FreezeUI(bool isFreezed)
    {
        resetPassBtn.interactable = !isFreezed;
        togglePasswordBtn.interactable = !isFreezed;
        togglePasswordConfirmationPasswordBtn.interactable = !isFreezed;
        foreach (Button button in resetInputFieldBtns)
        {
            button.interactable = !isFreezed;
        }
        secretCodeInputField.interactable = !isFreezed;
        passwordInputField.interactable = !isFreezed;
        passwordConfirmationInputField.interactable = !isFreezed;
        waitingScreen.SetActive(isFreezed);
    }

    private void HandleResetPass()
    {
        string secretcode = secretCodeInputField.Text; 
        string newpass = passwordInputField.Text; 
        string passConfirmation = passwordConfirmationInputField.Text;
        bool isInvalidInfo = false;  
        if (!IsValidSecretCode(secretcode))
        {
            isInvalidInfo = true;   
        }
        if (!IsValidPassword(newpass))
        {
            isInvalidInfo = true;   
        }
        if (!IsValidPasswordConfirmation(passConfirmation))
        {
            isInvalidInfo = true;   
        }

        if (!isInvalidInfo)
        {
            CallAPIResetPass(PlayerPrefs.GetString(PlayerPrefConfig.emailForResetPass), secretcode, newpass, passConfirmation); 
        }
    }

    async void CallAPIResetPass(string email, string secretcode, string pass, string passConfirmation)
    {
        try
        {
            FreezeUI(true);
            TransformResetPassInForm(false);
            ResetPassRequest resetPassRequest = new ResetPassRequest();
            resetPassRequest.Init(email, Convert.ToInt32(secretcode), pass, passConfirmation);
            APIResponse<string> userResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_RESET_PASS, UnityWebRequest.kHttpVerbPOST, resetPassRequest);
            if (userResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.passresetdone));
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
    private void HightLightInputField(AdvancedInputField input, GameObject warning, bool status)
    {
        warning.SetActive(status);
        if (status)
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning); 
        }
        else
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        }
    }
}