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

public class SignUpManager : MonoBehaviour
{
    public GameObject waitingScreen;
    public Button signUpBtn;
    public AdvancedInputField emailInputField;
    public AdvancedInputField fullnameInputField;
    public AdvancedInputField passwordInputField;
    public AdvancedInputField passwordConfirmationInputField;
    public GameObject emailNotificationText;
    public GameObject fullnameNotificationText;
    public GameObject passwordNotificationText;
    public GameObject passwordConfirmationNotificationText;
    public List<Button> resetInputFieldBtns;
    public Button togglePasswordBtn;
    public Button togglePasswordConfirmationPasswordBtn;
    public GameObject signUpFormObject;
    private bool isFirstTimeInputPassConfirmation = true;
    public Image backgroundImage;
    public List<Sprite> backgroundSprrites;
    public GameObject logoObject;
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
        emailNotificationText.SetActive(false);
        fullnameNotificationText.SetActive(false);
        passwordNotificationText.GetComponent<Text>().text = "";
        passwordConfirmationNotificationText.SetActive(false);
    }

    void InitEvents()
    {
        emailInputField.OnValueChanged.AddListener(CheckEmailFormat);
        fullnameInputField.OnValueChanged.AddListener(CheckFullnameFormat);
        passwordInputField.OnValueChanged.AddListener(CheckPasswordStrength);
        passwordConfirmationInputField.OnValueChanged.AddListener(CheckPasswordMatching);
        signUpBtn.onClick.AddListener(HandleSignUp);
        emailInputField.OnBeginEdit.AddListener(OnEmailInputFieldBeginEdit);
        fullnameInputField.OnBeginEdit.AddListener(OnFullNameInputFieldBeginEdit);
        passwordInputField.OnBeginEdit.AddListener(OnPasswordInputFieldBeginEdit);
        passwordConfirmationInputField.OnBeginEdit.AddListener(OnPasswordConfirmationInputFieldBeginEdit);
        passwordConfirmationInputField.OnEndEdit.AddListener(OnPasswordConfirmationEndEdit);
    }

    public void OnPasswordConfirmationEndEdit(string arg0, EndEditReason reason)
    {
        if (reason == EndEditReason.KEYBOARD_DONE)
        {
            HandleSignUp();
        }
    }

    public void OnEmailInputFieldBeginEdit(BeginEditReason reason)
    {
        TransformSignUpForm(true);
    }

    public void OnFullNameInputFieldBeginEdit( BeginEditReason reason)
    {
        TransformSignUpForm(true);
    }

    public void OnPasswordInputFieldBeginEdit(BeginEditReason reason)
    {
        TransformSignUpForm(true);
        emailInputField.transform.parent.gameObject.SetActive(false);
    }

    public void OnPasswordConfirmationInputFieldBeginEdit(BeginEditReason reason)
    {
        TransformSignUpForm(true);
        emailInputField.transform.parent.gameObject.SetActive(false);
    }
    public void OnKeyboardHeightChanged(int keyboardHeight)
    {
        if (keyboardHeight == 0)
        {
            if (emailInputField.Selected || fullnameInputField.Selected || passwordInputField.Selected || passwordConfirmationInputField.Selected)
            {
                return;
            }
            TransformSignUpForm(false);
        }
    }
    void TransformSignUpForm(bool isUp)
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
            emailInputField.transform.parent.gameObject.SetActive(true);
            fullnameInputField.transform.parent.gameObject.SetActive(true);
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
            HightLightInputField(emailInputField, emailNotificationText, true);
            return false;
        }
        else if (!AuthenConfig.emailRgx.IsMatch(data))
        {
            emailNotificationText.GetComponent<Text>().text = AuthenConfig.invalidEmail;
            HightLightInputField(emailInputField, emailNotificationText, true);
            return false;
        }
        HightLightInputField(emailInputField, emailNotificationText, false);
        return true;
    }

    private void CheckFullnameFormat(string data)
    {
        IsValidFullname(data);
    }

    private bool IsValidFullname(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            fullnameNotificationText.GetComponent<Text>().text = AuthenConfig.emptyValue;
            HightLightInputField(fullnameInputField, fullnameNotificationText, true);
            return false;
        }
        
        HightLightInputField(fullnameInputField, fullnameNotificationText, false);
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
            RefreshLayoutGroupsForLongNotifications(signUpFormObject);
            return false;
        }
        else if (!AuthenConfig.passRgx.IsMatch(data))
        {
            passwordNotificationText.GetComponent<Text>().text = AuthenConfig.invalidPassword;
            HightLightInputField(passwordInputField, passwordNotificationText, true);
            RefreshLayoutGroupsForLongNotifications(signUpFormObject);
            return false;
        }
        passwordNotificationText.GetComponent<Text>().text = "";
        passwordInputField.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        RefreshLayoutGroupsForLongNotifications(signUpFormObject);
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

    void FreezeUI(bool isFreezed)
    {
        signUpBtn.interactable = !isFreezed;
        togglePasswordBtn.interactable = !isFreezed;
        togglePasswordConfirmationPasswordBtn.interactable = !isFreezed;
        foreach (Button button in resetInputFieldBtns)
        {
            button.interactable = !isFreezed;
        }
        emailInputField.interactable = !isFreezed;
        fullnameInputField.interactable = !isFreezed;
        passwordInputField.interactable = !isFreezed;
        passwordConfirmationInputField.interactable = !isFreezed;
        waitingScreen.SetActive(isFreezed);
    }

    private void HandleSignUp()
    {
        string email = emailInputField.Text;
        string fullName = fullnameInputField.Text;
        string password = passwordInputField.Text;
        string passwordConfirmation = passwordConfirmationInputField.Text;
        bool isInvalidInfo = false;

        if (!IsValidEmail(email))
        {
            isInvalidInfo = true;
        }
        if (!IsValidFullname(fullName))
        {
            isInvalidInfo = true;
        }
        if (!IsValidPassword(password))
        {
            isInvalidInfo = true;
        }
        if (!IsValidPasswordConfirmation(passwordConfirmation))
        {
            isInvalidInfo = true;
        }

        if (!isInvalidInfo)
        {
            CallAPISignUp(email, fullName, password, passwordConfirmation);
        }
    }

    async void CallAPISignUp(string email, string fullName, string password, string passwordConfirmation)
    {
        try
        {
            FreezeUI(true);
            TransformSignUpForm(false);
            SignUpRequest signUpRequest = new SignUpRequest();
            signUpRequest.Init(email, fullName, password, passwordConfirmation);
            APIResponse<string> userResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_SIGN_UP, UnityWebRequest.kHttpVerbPOST, signUpRequest);
            if (userResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.signIn));
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