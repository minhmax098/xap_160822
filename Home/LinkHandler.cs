using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LinkHandler : MonoBehaviour
{
    private string tel = "tel://1900001";
    private string email = "mailto:kinhdoanhvts@viettel.com.vn";

    public void LinkToCallApp()
    {
        Debug.Log("Call to: " +  gameObject.GetComponent<TextMeshProUGUI>().text);
        Application.OpenURL(tel);
    }
    
    public void LinkToEmailApp()
    {
        Debug.Log("Mail to: " + gameObject.GetComponent<TextMeshProUGUI>().text);
        Application.OpenURL(email);
    
    }
}
