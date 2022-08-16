using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace List3DStore
{
    [System.Serializable]
    public static class TabManager
    {
        public static TabButton selectedTab; 
        public static void SetSelectedTab(TabButton _selectedTab)
        {
            Debug.Log("Start set tab: " + _selectedTab);
            selectedTab = _selectedTab;
        }
    }
}
