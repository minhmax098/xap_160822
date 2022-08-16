using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using YoutubePlayer;


public class PopupManager : MonoBehaviour
{
    private static PopupManager instance;
    public static PopupManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PopupManager>();
            }
            return instance;
        }
    }

    public Button btnGuide;
    private bool isClickedGuideBoard;
    public bool IsClickedGuideBoard
    {
        get
        {
            return isClickedGuideBoard;
        }
        set
        {
            isClickedGuideBoard = value;
            btnGuide.GetComponent<Image>().sprite = isClickedGuideBoard ? Resources.Load<Sprite>(PathConfig.GUIDE_CLICKED_IMAGE) : Resources.Load<Sprite>(PathConfig.GUIDE_UNCLICK_IMAGE);
        }
    }

    public GameObject popupExit;
    public Text txtTitlePopupExitLesson;
    public Text txtContentPopupExitLesson;
    public Text txtBtnExitLesson;
    public GameObject loading;

    public bool IsClickedMenu { get; set; } = false;
    public bool IsClickedExitLesson { get; set; } = false;

    // UI
    public Button btnMenu;
    public GameObject listMediaBoard;
    public GameObject guideBoard;


    public void InitPopupMangaer(bool _IsClickedMenu, bool _IsClickedExitLesson, bool _IsClickedGuideBoard)
    {
        IsClickedMenu = _IsClickedMenu;
        IsClickedExitLesson = _IsClickedExitLesson;
        IsClickedGuideBoard = _IsClickedGuideBoard;
    }

    public void ShowListMedia(bool _IsClickedMenu)
    {
        IsClickedMenu = _IsClickedMenu;
        if (IsClickedMenu)
        {
            listMediaBoard.SetActive(true);
            VideoManager.Instance.IsDisplayVideo = false;
            VideoManager.Instance.DisplayVideo(VideoManager.Instance.IsDisplayVideo);
            AudioManager.Instance.IsDisplayAudio = false;
            AudioManager.Instance.ShowAudioCurrent(AudioManager.Instance.IsDisplayAudio);
            btnMenu.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.MENU_CLICKED_IMAGE);
        }
        else
        {
            listMediaBoard.SetActive(false);
            btnMenu.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.MENU_UNCLICK_IMAGE);
        }
    }
    public void ShowGuideBoard(bool currentGuideBoardStatus)
    {
        IsClickedGuideBoard = currentGuideBoardStatus;
        guideBoard.SetActive(IsClickedGuideBoard);
    }

    public void ShowPopupExitLesson(bool currentExitLessonBtnStatus)
    {
        IsClickedExitLesson = currentExitLessonBtnStatus;
        popupExit.SetActive(IsClickedExitLesson);
        if (loading.activeSelf) loading.SetActive(false);
    }

    public void SetContentForPopupExitLesson(string title, string content, string btnExit)
    {
        txtTitlePopupExitLesson.text = title;
        txtContentPopupExitLesson.text = content;
        txtBtnExitLesson.text = btnExit;
    }

    public bool IsHavingPopup()
    {
        return GameObject.FindGameObjectsWithTag(TagConfig.PANEL_TAG).Length > 0;
    }
}
