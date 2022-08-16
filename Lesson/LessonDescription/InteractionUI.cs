using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using EasyUI.Toast;

namespace LessonDescription
{
    public class InteractionUI : MonoBehaviour
    {
        // public GameObject waitingScreen;
        private GameObject startLessonBtn; 
        private GameObject startMeetingBtn; 
        public GameObject backToHomeBtn;
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
        void Start()
        {
            InitUI(); 
            SetActions(); 
        }

        void Update()
        {
            
        }
        void InitUI()
        {
            startLessonBtn = GameObject.Find("StartLessonBtn");
            if (PlayerPrefs.GetString("user_email") != "")
            {
                startMeetingBtn = GameObject.Find("StartMeetingBtn");
            } 
        }
        void SetActions()
        {
            if (backToHomeBtn != null)
            {
                backToHomeBtn.GetComponent<Button>().onClick.AddListener(BackToRenalSystem); 
            }
            startLessonBtn.GetComponent<Button>().onClick.AddListener(StartExperience);
            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                startMeetingBtn.GetComponent<Button>().onClick.AddListener(StartMeeting);
            } 
        }
        void BackToRenalSystem()
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
        void StartExperience()
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.experience);
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.experience));
        }
        void StartMeeting()
        {
            try
            {
                BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.experience);
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
    }
}


