using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 
using UnityEngine.EventSystems; 

namespace List3DStore
{
    public class InteractionUI : MonoBehaviour
    {
        public GameObject waitingScreen; 
        public GameObject backToCreateLessonBtn; 
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
            InitEvents();
        }
       
        void InitEvents()
        {
            if (backToCreateLessonBtn != null)
            {
                backToCreateLessonBtn.GetComponent<Button>().onClick.AddListener(BackToCreateLesson); 
            }
        }
        void BackToCreateLesson()
        {
            StopAllCoroutines();
            waitingScreen.SetActive(true);
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
        public void onClickItemModel(int modelId, string modelName)
        {
            Debug.Log("On click item lesson: ");
            ModelStoreManager.InitModelStore(modelId, modelName);
            BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.createLesson);
            if (PlayerPrefs.GetString(PlayerPrefConfig.userToken) != "")
            {
                StopAllCoroutines();
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.createLesson)); 
            }
        }
    }
}
