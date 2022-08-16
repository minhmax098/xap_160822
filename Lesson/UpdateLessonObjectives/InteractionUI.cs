using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UpdateLessonObjectives
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        private GameObject backToLessonDetailEdit; 
        private GameObject cancelBtn;
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
            InitUI();
            SetActions();
        }
        void InitUI()
        {
            backToLessonDetailEdit = GameObject.Find("BackBtn");
            cancelBtn = GameObject.Find("CancelBtn");
        }
        void SetActions()
        {
            backToLessonDetailEdit.GetComponent<Button>().onClick.AddListener(BackToLessonDetailEdit);
            cancelBtn.GetComponent<Button>().onClick.AddListener(CancelButton);
        }
        void BackToLessonDetailEdit()
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
        void CancelButton()
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
    }
}
