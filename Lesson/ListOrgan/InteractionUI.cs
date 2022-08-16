using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; 

namespace ListOrgan 
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        private GameObject backToHomeBtn; 
        private string emailCheck;
        private static InteractionUI instance; 
        public static InteractionUI Instance
        {
            get 
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<InteractionUI>(); 
                }
                return instance;
            }
        }

        public void onClickItemLesson (int lessonId)
        {
            LessonManager.InitLesson(lessonId);
            string nextScene = string.IsNullOrEmpty(PlayerPrefs.GetString(PlayerPrefConfig.userToken)) ? SceneConfig.lesson_nosignin : SceneConfig.lesson;
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, nextScene);
            StartCoroutine(Helper.LoadAsynchronously(nextScene));
        }   
        void Start()
        {
            InitUI(); 
            SetActions(); 
        }
        void InitUI()
        {
            waitingScreen.SetActive(false);
            backToHomeBtn = GameObject.Find("BackBtn"); 
        }
        void SetActions()
        {
            backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToHome); 
        }
        void BackToHome()
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        } 
        
    }
}
