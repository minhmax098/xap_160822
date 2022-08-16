using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace List3DStore
{
    public class TabGroup : MonoBehaviour
    {
        public List<TabButton> tabButtons; 
        // add sprite 
        public Sprite tabIdle; 
        public Sprite tabHover; 
        public Sprite tabActive; 
        // public TabButton selectedTab = TabManager.selectedTab; 
        public List<GameObject> objectsToSwap; 
        void Start()
        {
            Debug.Log("Starting TabGroup: ");
            Debug.Log("TabManager.selectedTab start = " + TabManager.selectedTab);
        }
        public void Subscribe(TabButton button)
        {
            if (tabButtons == null)
            {
                tabButtons = new List<TabButton>();
            }
            tabButtons.Add(button);
        }
        public void OnTabEnter(TabButton button)
        {
            ResetTabs(); 
            if (TabManager.selectedTab == null || button != TabManager.selectedTab)
            {
                button.background.sprite = tabHover; 
            }
        }
        public void OnTabExit(TabButton button)
        {
            ResetTabs();
        }
        public void OnTabSelected(TabButton button)
        {
            //
            if (TabManager.selectedTab != null)
            {
                TabManager.selectedTab.Deselect(); 
            }
            // selectedTab = button;
            TabManager.SetSelectedTab(button);
            // selectedTab.Select(); 
            // TabManager.selectedTab.Select();
            ResetTabs();
            button.background.sprite = tabActive;
            int index = button.transform.GetSiblingIndex(); 
            for (int i=0; i<objectsToSwap.Count; i++)
            {
                if (i == index)
                {
                    objectsToSwap[i].SetActive(true); 
                }
                else 
                {
                    objectsToSwap[i].SetActive(false);
                }
            }
        }
        public void ResetTabs()
        {
            foreach(TabButton button in tabButtons)
            {
                if(TabManager.selectedTab != null && button == TabManager.selectedTab)
                {
                    continue;
                }
                button.background.sprite = tabIdle;
            }
        }
    }
}
