using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Video;
using YoutubePlayer;

public class MediaManager : MonoBehaviour
{
    private static MediaManager instance;
    public static MediaManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<MediaManager>();
            return instance;
        }
    }

    public const int MAX_NUMBER_LETTERS = 30;

    // UI
    public GameObject contentListMedia;
    public GameObject txtNameOrgan;
    public GameObject nodata;

    public enum MediaType
    {
        VIDEO,
        AUDIO
    }

    void OnEnable()
    {
        ObjectManager.onReadyModel += OnReadyModel;
    }

    void OnDisable()
    {
        ObjectManager.onReadyModel -= OnReadyModel;
    }

    public void SetNameObjectInListMedia()
    {
        txtNameOrgan.GetComponent<TMPro.TextMeshProUGUI>().text = ObjectManager.Instance.OriginObject.name;
    }
    
    void OnReadyModel()
    {
        SetNameObjectInListMedia();
    }

    public void ListMediaOnPanel()
    {
        if (StaticLesson.Video != "")
        {
            nodata.SetActive(false);
            AssignValueToItemMedia(MediaType.VIDEO,
                                    StaticLesson.LessonTitle,
                                    StaticLesson.Video,
                                    StaticLesson.Audio);
        }

        if (StaticLesson.Audio != "")
        {
            nodata.SetActive(false);
            AssignValueToItemMedia(MediaType.AUDIO,
                                    StaticLesson.LessonTitle,
                                    StaticLesson.Video,
                                    StaticLesson.Audio);
        }
        // For sub-organ
        if (StaticLesson.ListLabel.Length > 0)
        {
            for (int i = 0; i < StaticLesson.ListLabel.Length; i++)
            {
                if (StaticLesson.ListLabel[i].videoLabel != "")
                {
                    nodata.SetActive(false);
                    AssignValueToItemMedia(MediaType.VIDEO,
                                    StaticLesson.ListLabel[i].labelName,
                                    StaticLesson.ListLabel[i].videoLabel,
                                    StaticLesson.ListLabel[i].audioLabel);
                }

                if (StaticLesson.ListLabel[i].audioLabel != "")
                {
                    nodata.SetActive(false);
                    AssignValueToItemMedia(MediaType.AUDIO,
                                    StaticLesson.ListLabel[i].labelName,
                                    StaticLesson.ListLabel[i].videoLabel,
                                    StaticLesson.ListLabel[i].audioLabel);
                }
            }
        }
    }

    public void AssignValueToItemMedia(MediaType typeMedia, string nameMedia, string videoURL, string audioURL)
    {
        GameObject itemMedia = Instantiate(Resources.Load(PathConfig.MODEL_ITEM_MEDIA) as GameObject);

        // Data
        MediaOrganItem dataItemVideo = itemMedia.GetComponent<MediaOrganItem>();
        dataItemVideo.SetValueMediaOrganItem(nameMedia, videoURL, audioURL);

        // UI
        itemMedia.transform.SetParent(contentListMedia.transform, false);

        if (typeMedia == MediaType.VIDEO)
        {
            itemMedia.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.VIDEO_ICON);
            itemMedia.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = StringConfig.videoPreText + nameMedia;
            itemMedia.transform.GetComponent<Button>().onClick.AddListener(delegate { VideoManager.Instance.ShowVideoByClickItemMedia(dataItemVideo, itemMedia); });
        }
        else if (typeMedia == MediaType.AUDIO)
        {
            itemMedia.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = StringConfig.audioPreText + nameMedia;
            itemMedia.transform.GetComponent<Button>().onClick.AddListener(delegate { AudioManager.Instance.ShowAudioByClickItemMedia(dataItemVideo, itemMedia); });
        }
    }


    public void DeleteAllIconTickOnItemMedia()
    {
        GameObject[] objectsWithTagIconTick = GameObject.FindGameObjectsWithTag(TagConfig.ICON_TICK);
        Debug.Log(objectsWithTagIconTick.Length);
        foreach (GameObject subObject in objectsWithTagIconTick)
        {
            subObject.SetActive(false);
        }
    }
}
