using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;

namespace YoutubePlayer
{
    public class VideoManagerBuildLesson : MonoBehaviour
    {
        private static VideoManagerBuildLesson instance;
        public static VideoManagerBuildLesson Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<VideoManagerBuildLesson>();
                }
                return instance;
            }
        }

        public GameObject mainViewVideo;
        YoutubePlayer youtubePlayer;
        VideoPlayer videoPlayer;
        public Button btnControlVideo;
        bool _IsPlayingVideo { get; set;} = false;

        public bool IsPlayingVideo { get; set; } = false;
        void Start()
        {
            InitEvent();
        }

        void InitEvent()
        {
            btnControlVideo.onClick.AddListener(HandleControlVideo);
        }

        void HandleControlVideo()
        {
            Debug.Log("button click");
            IsPlayingVideo = !IsPlayingVideo;
            ControlVideo(IsPlayingVideo);
        }

        void Awake()
        {
            videoPlayer = mainViewVideo.GetComponent<VideoPlayer>();
            videoPlayer.prepareCompleted += VideoPlayerPreparedCompleted;
            youtubePlayer = mainViewVideo.GetComponent<YoutubePlayer>();
        }

        void Update()
        {
            if (videoPlayer != null)
            {
                // sliderControlVideo.GetComponent<Slider>().value = (float)videoPlayer.time;
                // sliderControlVideo.GetComponent<Slider>().onValueChanged.AddListener((value) => { 
                //     videoPlayer.time = value;
                // });
                // StartCoroutine(SetValueForSlider());
            }
        }

        public void ShowVideo(string videoUrl)
        {
            Debug.Log("Todo Video url : " + videoUrl);
            ResetVideo();
            btnControlVideo.interactable = false;
            youtubePlayer.youtubeUrl = videoUrl;
            Debug.Log("Todo youtube url " + youtubePlayer.youtubeUrl);
            Prepare();
        }

        void VideoPlayerPreparedCompleted(VideoPlayer source)
        {
            btnControlVideo.interactable = source.isPrepared;
        }

        public async void Prepare()
        {
            Debug.Log("Loading video...");
            try
            {
                await youtubePlayer.PrepareVideoAsync();
                Debug.Log("Loading video success!");
            }
            catch(Exception e)
            {
                videoPlayer = null;
                Debug.Log("ERROR video " + e);
            }
        }

        public void ControlVideo(bool _IsPlayingVideo)
        {        
            IsPlayingVideo = _IsPlayingVideo;
            if (videoPlayer != null)
            {
                if (IsPlayingVideo)
                {
                    PlayVideo();
                    btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
                }
                else
                {
                    PauseVideo();
                    btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_START_PLAY_IMAGE);
                }
            }
        }

        public void PlayVideo()
        {
            videoPlayer.Play();
        }

        public void PauseVideo()
        {
            videoPlayer.Pause();
        }

        public void ResetVideo()
        {
            if (videoPlayer != null)
                videoPlayer.Stop();
            // videoPlayer = null;
            btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_START_PLAY_IMAGE);
            // MediaManager.Instance.DeleteAllIconTickOnItemMedia();
        }

        public void ExitVideo()
        {
            ResetVideo();
        }

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPreparedCompleted;
        }
    }
}
