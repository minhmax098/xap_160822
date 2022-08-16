using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;

namespace BuildLesson
{
    public class AudioManager1 : MonoBehaviour
    {
        private static AudioManager1 instance;
        public static AudioManager1 Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager1>();
                }
                return instance; 
            }
        }
        private AudioSource audioData;
        public GameObject selectedObject;
        public bool IsPlayingAudio { get; set; }
        public bool IsDisplayAudio { get; set; }
        // UI
        public Text timeCurrentAudio;
        public Text timeEndAudio;
        public GameObject sliderControlAudio;
        public Button btnControlAudio; 
        private bool isPlayingAudio = false;
        
        void Start()
        {
            InitEvents();
            SetPropertyAudio(false, false);
        }

        void Update()
        {
           if (audioData != null)
            {
                timeCurrentAudio.text = Helper.FormatTime(audioData.time);
                sliderControlAudio.GetComponent<Slider>().value = audioData.time;
                if (!audioData.isPlaying)
                {
                    btnControlAudio.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE); 
                    isPlayingAudio = !isPlayingAudio;                   
                }
            }
        }

        void InitEvents()
        {
            btnControlAudio.onClick.AddListener(() => ControlAudio(!isPlayingAudio));
        }

        public void SetPropertyAudio(bool _IsPlayingAudio, bool _IsDisplayAudio)
        {
            IsPlayingAudio = _IsPlayingAudio;
            IsDisplayAudio = _IsDisplayAudio;
        }

        public void SetPropertyComponentAudio()
        {
            audioData = selectedObject.GetComponent<AudioSource>();
            if (audioData != null)
            {
                timeEndAudio.GetComponent<Text>().text = Helper.FormatTime(audioData.clip.length);
                btnControlAudio.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
                sliderControlAudio.GetComponent<Slider>().maxValue = audioData.clip.length;
            }
        }

        public void ControlAudio(bool _IsPlayingAudio)
        {
            Debug.Log("Control Audio Click");
            isPlayingAudio = !isPlayingAudio;
            IsPlayingAudio = _IsPlayingAudio; 
            if (audioData != null)
            {
                if (IsPlayingAudio)
                {
                    audioData.Play();
                    btnControlAudio.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PAUSE_IMAGE);
                }
                else
                {
                    audioData.Pause();
                    btnControlAudio.GetComponent<Image>().sprite = Resources.Load<Sprite>(PathConfig.AUDIO_PLAY_IMAGE);
                }
            }
        }

        public void DisplayAudio(bool _IsDisplayAudio)
        {
            IsDisplayAudio = _IsDisplayAudio;
            if (IsDisplayAudio)
            {
                SetPropertyAudio(false, true);
                SetPropertyComponentAudio();
            }
            else 
            {
                if (audioData != null)
                {
                    audioData.Stop();
                }
                SetPropertyAudio(false, false);
                audioData = null;
            }
        }
    }
}
