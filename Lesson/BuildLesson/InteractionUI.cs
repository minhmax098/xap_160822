using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System; 

namespace BuildLesson
{
    public class InteractionUI : MonoBehaviour
    {
        private static InteractionUI instance; 
        public static InteractionUI Instance
        {
            get
            {
                if (instance = null)
                {
                    instance = FindObjectOfType<InteractionUI>(); 
                }
                return instance;
            }
        }
        
        public Button btnShowExitPopup; 
        public GameObject popupExit; 
        public GameObject btnExitPopup; 
        public GameObject btnExitLesson; 
        public GameObject btnCancelExitLesson; 
        void OnEnable()
        {
            BackOrLeaveApp.onBackPressed += HandlerShowExitPopup;
        }
        
        void OnDisable()
        {
            BackOrLeaveApp.onBackPressed -= HandlerShowExitPopup;
        }
        void Start()
        {
            SetActions(); 
            InitLayoutScreen();
        }

        void InitLayoutScreen()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            StatusBarManager.statusBarState = StatusBarManager.States.Hidden;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        }

        void SetActions()
        {
            btnShowExitPopup.GetComponent<Button>().onClick.AddListener(HandlerShowExitPopup);
            btnExitPopup.GetComponent<Button>().onClick.AddListener(HandlerExitPopup);
            btnExitLesson.GetComponent<Button>().onClick.AddListener(HandlerExitLesson);
            btnCancelExitLesson.GetComponent<Button>().onClick.AddListener(HandlerCancelExitLesson);
        }

        void HandlerShowExitPopup()
        {
            popupExit.SetActive(true);
        }

        void HandlerExitPopup()
        {
            popupExit.SetActive(false);
        }

        void HandlerExitLesson()
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
        
        void HandlerCancelExitLesson()
        {
            popupExit.SetActive(false);
        }
    }
}
