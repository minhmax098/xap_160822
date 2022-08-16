using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.Networking; 
using System.Text; 
using System.Text.RegularExpressions;
using EasyUI.Toast;

public class ResetScript : MonoBehaviour
{
    public GameObject waitingScreen;
    public Slider slider; 
    public Button resetPassBtn; 
    public InputField secretcodeInput; 
    public GameObject secretCodeWarning; 
    public InputField newpassInput; 
    public GameObject newPassWarning; 
    public GameObject newPassWarning2; 
    public InputField confirmpassInput; 
    public GameObject confirmPassWarning; 
    public GameObject confirmPassWarning2; 
    // public GameObject resetPassWarning; 

    private string email;
    private Regex rgx;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;

        resetPassBtn = transform.GetComponent<Button>();
        resetPassBtn.onClick.AddListener(ValidateResetPass); 
        secretCodeWarning.SetActive(false); 
        newPassWarning.SetActive(false); 
        newPassWarning2.SetActive(false);
        confirmPassWarning.SetActive(false);
        confirmPassWarning2.SetActive(false);
        waitingScreen.SetActive(false);
        email = PlayerPrefs.GetString("user_email_for_reset");
        Debug.Log("test mail in previous scene");
        rgx = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$");
        newpassInput.onValueChanged.AddListener(CheckPasswordStrength);
        secretcodeInput.onValueChanged.AddListener(CheckSecretCodeInput);
        confirmpassInput.onValueChanged.AddListener(CheckPasswordMatch);
    }
    void CheckSecretCodeInput(string data)
    {
        if (data == "")
        {
            changeUIStatus(secretcodeInput, secretCodeWarning, true);
        }
        else
        {
            changeUIStatus(secretcodeInput, secretCodeWarning, false); 
            secretCodeWarning.SetActive(false);
        }
    }
    public void CheckPasswordMatch(string data)
    {
        Debug.Log(newpassInput.text);
        if (data == "")
        {
            changeUIStatus(confirmpassInput, confirmPassWarning, false);
            confirmPassWarning2.SetActive(false);
        }
        else if (!data.Equals(newpassInput.text))
        {
            changeUIStatus(confirmpassInput, confirmPassWarning2, true); 
            confirmPassWarning.SetActive(false);            
        }
        else
        {
            changeUIStatus(confirmpassInput, confirmPassWarning2,false); 
            confirmPassWarning.SetActive(false);
        }
    }

    public void CheckPasswordStrength(string data){
        if (data == "")
        {
            changeUIStatus(newpassInput, newPassWarning, false);
            changeUIStatus(newpassInput, newPassWarning2, false); 
        }
        else if (!rgx.IsMatch(data))
        {
            newPassWarning.SetActive(false);
            newPassWarning2.SetActive(true);
            changeUIStatus(newpassInput, newPassWarning, false);
            changeUIStatus(newpassInput, newPassWarning2, true); 
        }
        else
        {
            newPassWarning.SetActive(false);
            newPassWarning2.SetActive(false);
            changeUIStatus(newpassInput, newPassWarning, false);
            changeUIStatus(newpassInput, newPassWarning2, false);  
        }
    }

    private void ValidateResetPass()
    {
        string secretcode = secretcodeInput.text; 
        string newpass = newpassInput.text; 
        string confirmpass = confirmpassInput.text;
        bool check = false;  
        if (secretcode == "")
        {
            changeUIStatus(secretcodeInput, secretCodeWarning, true); 
            check = true; 
        }
        if (newpass == "")
        {
            changeUIStatus(newpassInput, newPassWarning, true);
            newPassWarning2.SetActive(false); 
            check = true; 
        }
        if (confirmpass == "")
        {
            confirmPassWarning2.SetActive(false); 
            changeUIStatus(confirmpassInput, confirmPassWarning, true);
            check = true; 
        }
        if (secretcode != "")
        {
            secretCodeWarning.SetActive(false); 
        }
        if (newpass != "")
        {
            newPassWarning.SetActive(false);  
        }
        if (confirmpass != "")
        {
            confirmPassWarning.SetActive(false); 
        }
        if (!check && !newPassWarning2.activeSelf && !confirmPassWarning2.activeSelf)
        {
            StartCoroutine(CallResetPass(secretcode, newpass, confirmpass)); 
        }
    }
    private void changeUIStatus(InputField input, GameObject warning, bool status)
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
    public IEnumerator LoadAsynchronously (string sceneName)
    {
      AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
      waitingScreen.SetActive(true);
      while (!operation.isDone)
      {
        yield return new WaitForSeconds(.1f); 
      }
    }
    public IEnumerator WaitForAPIResponse(UnityWebRequest request)
    {
      waitingScreen.SetActive(true); 
      Debug.Log("calling API: "); 
      while(!request.isDone)
      {
        yield return new WaitForSeconds(.1f);
      }
    }

    public IEnumerator CallResetPass(string Secretcode, string Newpass, string Confirmpass)
    {
        string logindataJsonString = "{\"email\": \"" + email + "\", \"secretCode\": \"" + Secretcode + "\", \"newPassword\": \"" + Newpass + "\",\"confirmPassword\": \"" + Confirmpass + "\"}";
        Debug.Log(logindataJsonString);
        var request = new UnityWebRequest(APIUrlConfig.ResetPass, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(logindataJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        StartCoroutine(WaitForAPIResponse(request));
        yield return request.SendWebRequest(); 
        waitingScreen.SetActive(false); 
        if (request.error != null)
        {
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            UserDetail userDetail = JsonUtility.FromJson<UserDetail>(response);
            Debug.Log(response);
            Debug.Log(userDetail.message);
            // code != 200
            Debug.Log("Error: " + request.error);
            if (request.responseCode == 400){
                // Debug.Log("The account is not correct, please check again!");
                Toast.Show(userDetail.message); // need change later
            }
        }
        else
        {   
            // code = 200 
            Debug.Log("All OK");
            Debug.Log("Status Code: " + request.responseCode);
            string response = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            UserDetail userDetail = JsonUtility.FromJson<UserDetail>(response);
            if(userDetail.code == 200)
            {
                SceneManager.LoadScene(SceneConfig.passresetdone);
            }
        }
    }
}
