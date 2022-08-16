using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using EasyUI.Toast;


namespace YoutubePlayer
{
    public class VideoManager : MonoBehaviour
    {

        private static VideoManager instance;
        public static VideoManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<VideoManager>();
                }
                return instance;
            }
        }

        public GameObject videoPlayerZoom;
        YoutubePlayer youtubePlayer;
        VideoPlayer videoPlayer;
        public Button btnControlVideo;
        public Button btnControlVideoFull;
        public Button btnExitVideo;
        public GameObject sliderControlVideo;
        public GameObject sliderControlVideoFull;
        public GameObject zoomScreen;
        public GameObject fullScreen;
        public GameObject panelLoading;
        public GameObject panelVideo;
        public bool IsPlayingVideo { get; set; } = false;
        public bool IsDisplayVideo { get; set; } = false;
        public bool IsShowingFullScreen { get; set; }

        private void Awake()
        {
            videoPlayer = videoPlayerZoom.GetComponent<VideoPlayer>();
            videoPlayer.prepareCompleted += VideoPlayerPreparedCompleted;
            youtubePlayer = videoPlayerZoom.GetComponent<YoutubePlayer>();
        }

        void Update()
        {
            if (videoPlayer != null)
            {
                StartCoroutine(SetValueForSlider(sliderControlVideo));
                StartCoroutine(SetValueForSlider(sliderControlVideoFull));
            }
        }

        IEnumerator SetValueForSlider(GameObject slider)
        {
            slider.GetComponent<Slider>().onValueChanged.AddListener((value) =>
            {
                videoPlayer.time = value;
            });

            yield return new WaitForSeconds(2);

            slider.GetComponent<Slider>().value = (float)videoPlayer.time;
        }

        public void ShowVideoByClickItemMedia(MediaOrganItem dataItemVideo, GameObject itemMedia)
        {
            if (videoPlayer != null)
                ResetVideo();

            // Hide list media
            PopupManager.Instance.IsClickedMenu = false;
            PopupManager.Instance.ShowListMedia(PopupManager.Instance.IsClickedMenu);

            // Set UI Video
            IsDisplayVideo = true;
            panelVideo.SetActive(true);
            panelLoading.SetActive(true);
            btnControlVideo.interactable = false;

            // Show icon tick on item selected
            MediaManager.Instance.DeleteAllIconTickOnItemMedia();
            itemMedia.transform.GetChild(2).gameObject.SetActive(true);
            youtubePlayer.youtubeUrl = dataItemVideo.VideoURL;
            GetVideo(itemMedia);
        }

        public void DisplayVideo(bool _IsDisplayVideo)
        {
            IsDisplayVideo = _IsDisplayVideo;
            if (videoPlayer != null)
            {
                if (IsDisplayVideo)
                {
                    panelVideo.SetActive(true);
                }
                else
                {
                    PauseVideo();
                    IsPlayingVideo = false;
                    ControlVideo(IsPlayingVideo);

                    // Set UI
                    panelVideo.SetActive(false);
                }
            }
        }

        void VideoPlayerPreparedCompleted(VideoPlayer source)
        {
            btnControlVideo.interactable = source.isPrepared;
            btnExitVideo.interactable = source.isPrepared;
            panelLoading.SetActive(!source.isPrepared);
            sliderControlVideo.SetActive(source.isPrepared);
            sliderControlVideo.GetComponent<Slider>().maxValue = (float)videoPlayer.length;
        }

        public async void GetVideo(GameObject itemMedia)
        {
            Debug.Log("Loading video...");
            try
            {
                Exception exception = Network.CheckNetWorkToDisplayToast();
                if (exception != null)
                    throw exception;

                await youtubePlayer.PrepareVideoAsync();
                IsPlayingVideo = true;
                btnControlVideo.interactable = true;
                ControlVideo(IsPlayingVideo);
                Debug.Log("Loading video success!");
            }
            catch (Exception exception)
            {
                SetUIErrorNetwork(itemMedia);
                Toast.ShowCommonToast(exception.Message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
            }
        }

        void SetUIErrorNetwork(GameObject itemMedia)
        {
            IsDisplayVideo = false;
            DisplayVideo(IsDisplayVideo);
            PopupManager.Instance.IsClickedMenu = true;
            PopupManager.Instance.ShowListMedia(PopupManager.Instance.IsClickedMenu);

            MediaManager.Instance.DeleteAllIconTickOnItemMedia();
            itemMedia.transform.GetChild(2).gameObject.SetActive(true);
        }

        public void ControlVideo(bool _IsPlayingAudio)
        {
            IsPlayingVideo = _IsPlayingAudio;
            if (videoPlayer != null)
            {
                if (IsPlayingVideo)
                {
                    PlayVideo();
                    btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
                    btnControlVideoFull.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
                }
                else
                {
                    PauseVideo();
                    btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
                    btnControlVideoFull.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
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
            videoPlayer.Stop();
            btnControlVideo.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
            MediaManager.Instance.DeleteAllIconTickOnItemMedia();
        }
        public void ChangeVideoView(bool _IsShowingFullScreen)
        {
            IsShowingFullScreen = _IsShowingFullScreen;
            if (videoPlayer != null)
            {
                if (IsShowingFullScreen)
                {
                    fullScreen.SetActive(true);
                    zoomScreen.SetActive(false);
                    videoPlayer.targetTexture = Resources.Load<RenderTexture>("Textures/FullScreen");
                }
                else
                {
                    fullScreen.SetActive(false);
                    zoomScreen.SetActive(true);
                    videoPlayer.targetTexture = Resources.Load<RenderTexture>("Textures/VideoZoom");
                }
            }

        }

        public void ExitVideo()
        {
            ResetVideo();
            panelVideo.SetActive(false);
        }

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPreparedCompleted;
        }
    }
}
