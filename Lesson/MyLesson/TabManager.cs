using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyLesson
{
    [System.Serializable]
    public static class TabManager
    {
        public static TabButton selectedTab;
        public static void SetSelectedTab(TabButton _selectedTab)
        {
            Debug.Log("START SET TAB !!!! = " + _selectedTab);
            selectedTab = _selectedTab;
        }
    }
}
