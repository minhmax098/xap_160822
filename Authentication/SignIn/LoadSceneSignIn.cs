using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System;
using UnityEngine.SceneManagement; 

public class LoadSceneSignIn : MonoBehaviour
{
    public Button signInBtn; 
    public InputField userNameInput; 
    public InputField passwordInput; 
    public GameObject EmailWarning; 
    public InputField Email; 
    public GameObject PassWarning; 
    public GameObject capcha; 
    private int invalidCount; 
    public GameObject UserPassWarning; 

    Account[] users = { 
        new Account()
        {
            username = "Nacriema", 
            email = "nacriema@gmail.com", 
            password = "nacriema457"
        }, 
        new Account()
        {
            username = "Le Hoc Minh", 
            email = "hocminh1998@gmail.com", 
            password = "hocminh098l8n"
        }, 
        new Account()
        {
            username = "Nguyen Nguyen", 
            email = "nthainguyen@gmail.com", 
            password = "nguyen123qwe$%^"
        }
    }; 
    Account getUser(string email, string password)
    {
        Account acc = Array.Find(users, user => user.email == email && user.password == password); 
        return acc; 
    }
    void Start()
    {
        invalidCount = 0; 
        signInBtn = transform.GetComponent<Button>(); 
        signInBtn.onClick.AddListener(UserAuthorization); 
        EmailWarning.SetActive(false); 
        PassWarning.SetActive(false); 
        capcha.SetActive(false); 
        // Warning; username or password is incorrect
        UserPassWarning.SetActive(false); 
    }
    void UserAuthorization()
    {
        string email = userNameInput.text; 
        string pass = passwordInput.text; 
        if (email == "")
        {
            EmailWarning.SetActive(true);
            Email.selectionColor = Color.red; 
        }
        if (pass == "")
        {
            PassWarning.SetActive(true); 
        }
        if (email != "")
        {
            EmailWarning.SetActive(false);
        }
        if (pass != "")
        {
            PassWarning.SetActive(false);
        }

        Account result = getUser(email, pass); 
        if(result != null)
        {
            // save player info to Player Ref, render next scene
            PlayerPrefs.SetString("user_email", email); 
            PlayerPrefs.SetString("user_name", result.username); 
            SceneManager.LoadScene(SceneConfig.home_user); 
        }
        else 
        {
            // Display notification
            Debug.Log("Invalid input"); 
            if(email != "" && pass != "")
            {
                Debug.Log("Invalid user"); 
                UserPassWarning.SetActive(true); 
                invalidCount += 1; 
                if (invalidCount == 5)
                {
                    capcha.SetActive(true); 
                }
            }
        }
    }
}
