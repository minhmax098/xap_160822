using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System; 

namespace Home 
{
    public class InteractionUI : MonoBehaviour
    {
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
        public void onClickItemLesson(int lessonId)
        {
            LessonManager.InitLesson(lessonId);
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.listOrgan_edit);
            StopAllCoroutines();

            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                SceneNameManager.setPrevScene(SceneConfig.home_user);
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.lesson));  
            }
            else
            {
                SceneNameManager.setPrevScene(SceneConfig.home_nosignin);
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.lesson_nosignin));  
            }
        }
    }
}
