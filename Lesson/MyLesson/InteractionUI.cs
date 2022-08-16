using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

public class InteractionUI : MonoBehaviour
{
    public GameObject waitingScreen; 
    private static InteractionUI instance; 
    public static InteractionUI Instance
    { 
        get 
        { 
            if (instance == null)
            {
                instance = FindObjectOfType<InteractionUI>();
            }
            return instance;
        }
    }
    void Start()
    {
        
    }
  
    public void onClickItemLesson(int lessonId)
    {
        StopAllCoroutines();
        LessonManager.InitLesson(lessonId);
        SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name); 
        if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
        {
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.lesson_edit)); 
        }
    }   
}
