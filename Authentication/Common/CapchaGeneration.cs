using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class CapchaGeneration : MonoBehaviour
{
    public InputField generatedCapchaInputField; 
    public Button genCodeBtn; 
    private static CapchaGeneration instance;
    public static CapchaGeneration Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CapchaGeneration>();
            }
            return instance;
        }
    }
    void Start()
    {
        genCodeBtn.onClick.AddListener(() => GenCapchaCode(6));
    }
    public void GenCapchaCode(int length = 6)
    {
        generatedCapchaInputField.text = VerificationCode.SetDeleKey(length);
        PlayerPrefs.SetString(PlayerPrefConfig.capcha, generatedCapchaInputField.text);
    } 
}
