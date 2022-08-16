using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using UnityEngine.EventSystems; 
using UnityEngine.Events; 
using UnityEngine.SceneManagement;

namespace MyLesson
{
    [RequireComponent(typeof(Image))]
    public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler   
    {
        public TabGroup tabGroup;
        public Image background; 
        public UnityEvent onTabSelected;
        public UnityEvent onTabDeselected; 
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Start selectedTab = " + TabManager.selectedTab);
            tabGroup.OnTabSelected(this);
            if (this.name  == "Mylessons")
            {
                SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.myLesson)); 
            }
            else if (this.name == "Library")
            {
                SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.home_user));
            }
            else
            {
                SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
                StartCoroutine(Helper.LoadAsynchronously(SceneConfig.createLesson_main));
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            tabGroup.OnTabEnter(this);
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            tabGroup.OnTabExit(this);
        }
        void Start()
        {
            background = GetComponent<Image>();
            tabGroup.Subscribe(this);
        }

        void Update()
        {

        }
        public void Select()
        {
            if (onTabSelected != null)
            {
                onTabSelected.Invoke(); 
            }
        }
        public void Deselect()
        {
            if (onTabDeselected != null)
            {
                onTabDeselected.Invoke(); 
            }
        }
    }
}
