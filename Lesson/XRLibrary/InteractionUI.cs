using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace XRLibrary
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen;
        public Button backToHomeBtn;
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
            waitingScreen.SetActive(true);
            LessonManager.InitLesson(lessonId); 
            string nextScene = string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefConfig.userToken)) ? SceneConfig.lesson_nosignin : SceneConfig.lesson;
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, nextScene);
            StartCoroutine(Helper.LoadAsynchronously(nextScene));
        }

        void Start()
        {
           InitEvents();  
        }
        
        void InitEvents()
        {
            backToHomeBtn.onClick.AddListener(BackToHome); 
        }

        void BackToHome()
        {
            waitingScreen.SetActive(true);
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
    }
}
