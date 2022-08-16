using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using EasyUI.Toast;

public class LoadingManager : MonoBehaviour
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public GameObject loadingPanel;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Singleton pattern: Create only one instance of LoadingManager class
    /// Note: All loading process in MEETING_STARTING and MEETING_JOINING will be handled in instance of LoadingManager class
    /// </summary>
    private static LoadingManager instance;
    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LoadingManager>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show loading panel with message
    /// </summary>
    /// <param name="message">Message will be displayed</param>
    public void ShowLoading(string message)
    {
        loadingPanel.SetActive(true);
        loadingPanel.transform.GetChild(1).GetComponent<Text>().text = message;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Hide loading panel
    /// </summary>
    public void HideLoading()
    {
        loadingPanel.SetActive(false);
    }
}