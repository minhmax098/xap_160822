using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace CreateLesson_Main
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        public GameObject storeBtn; 
        public GameObject importBtn; 
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
            SetActions(); 
            InitUI();
            SetActions(); 
        }

        void InitUI()
        {
            storeBtn = GameObject.Find("3DStore");
            importBtn = GameObject.Find("ImportModel");
        }
        void SetActions()
        {
            storeBtn.GetComponent<Button>().onClick.AddListener(StoreModel);
            importBtn.GetComponent<Button>().onClick.AddListener(HandleBtnImportModel3D);
        }
        void StoreModel()
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.storeModel);
            StartCoroutine(Helper.LoadAsynchronously(SceneConfig.storeModel));
        }

        void HandleBtnImportModel3D()
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.uploadModel);
            SceneManager.LoadScene(SceneConfig.uploadModel);
        }
    }
}
