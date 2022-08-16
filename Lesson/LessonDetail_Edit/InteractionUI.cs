using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using EasyUI.Toast;


namespace LessonDetail_Edit 
{
    public class InteractionUI : MonoBehaviour
    {
        private GameObject startLessonBtn;
        private GameObject startMeetingBtn;
        public GameObject backToHomeBtn;
        private GameObject buildLessonBtn;
        private GameObject lessonObjectivesBtn; 
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
        private string hightLightBtnColor = "#C8C8C8";
        
        void Start()
        {
            InitUI(); 
            SetActions();
        }
        void InitUI()
        {
            startLessonBtn = GameObject.Find("StartLessonBtn");
            if (PlayerPrefs.GetString("user_email") != "") 
            {
                startMeetingBtn = GameObject.Find("StartMeetingBtn"); 
            }
            buildLessonBtn = GameObject.Find("BtnBuildLesson");
            lessonObjectivesBtn = GameObject.Find("BtnLessonObjectives");
        }
        void SetActions()
        {
            backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToMyLesson);
            startLessonBtn.GetComponent<Button>().onClick.AddListener(StartExperience);
            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                startMeetingBtn.GetComponent<Button>().onClick.AddListener(StartMeeting);
            }
            buildLessonBtn.GetComponent<Button>().onClick.AddListener(BuildLesson);
            lessonObjectivesBtn.GetComponent<Button>().onClick.AddListener(UpdateLessonObjectives);
        }
        void BackToMyLesson()
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
        void StartExperience()
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.experience);
            SceneManager.LoadScene(SceneConfig.experience);
        }
        void StartMeeting()
        {
            try
            {
                BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.meetingStarting); 
                Exception exception = Network.CheckNetWorkToDisplayToast();
                if (exception != null) throw exception;
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.meetingStarting));
            }
            catch (Exception exception)
            {
                Toast.ShowCommonToast(exception.Message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
                return;
            }
        }
        void BuildLesson()
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.buildLesson); 
            StopAllCoroutines();
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.buildLesson));
        }
        void UpdateLessonObjectives()
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.update_lesson); 
            SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name); 
            SceneManager.LoadScene(SceneConfig.update_lesson);
        }
    }
}
