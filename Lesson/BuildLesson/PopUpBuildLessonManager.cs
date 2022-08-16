using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace BuildLesson
{
    public class PopUpBuildLessonManager : MonoBehaviour
    {
        private static PopUpBuildLessonManager instance; 
        public static PopUpBuildLessonManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<PopUpBuildLessonManager>();
                }
                return instance;
            }
        }
        
        // UI: Add Audio, Add Video
        public GameObject listCreateLesson;
        public bool IsClickedAdd { get; set; } = false;

        public void InitPopUpBuildLessonManager(bool _IsClickedAdd)
        {
            IsClickedAdd = _IsClickedAdd;
        }

        public void ShowListAdd(bool _IsClickedAdd)
        {
            IsClickedAdd = _IsClickedAdd;
            if (IsClickedAdd)
            {
                listCreateLesson.SetActive(true);
            }
            else
            {
                listCreateLesson.SetActive(false);
            }
        }
    }
}
