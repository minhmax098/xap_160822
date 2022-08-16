using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; 
using UnityEngine.Video;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using EasyUI.Toast;
using System.Threading.Tasks;

namespace BuildLesson
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<AudioManager>();
                }
                return instance; 
            }
        }
       
        public GameObject panelListCreateLesson;
        private AudioSource audioData;
        public GameObject selectedObject;
        public bool IsPlayingAudio { get; set; }
        public bool IsDisplayAudio { get; set; }
        
        public Text timeCurrentAudio;
        public Text timeEndAudio;
        public GameObject sliderControlAudio;
        public Button btnControlAudio;
        public Button btnSaveRecord; 
        public Animator toggleListItemAnimator;
        public GameObject spinner;
        private bool isPlayingAudio = false;

        void Start()
        {
            spinner.SetActive(false);
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
            btnSaveRecord.onClick.AddListener(HandlerSaveRecord);
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
                // btnAudio.SetActive(false);
                SetPropertyAudio(false, true);
                SetPropertyComponentAudio(); // ham reset thuoc tinh audio
            }
            else 
            {
                if (audioData != null)
                {
                    audioData.Stop();
                }
                SetPropertyAudio(false, false);
                audioData = null;
                // btnAudio.SetActive(true);
            }
        }

        void HandlerSaveRecord()
        {
            ListItemsManager.startTime = 0f;
            Debug.Log("Enter save record: ");
            StartCoroutine(SaveRecordAudio());
            // SaveRecordAudio();
        }

        IEnumerator SaveRecordAudio()
        {
            Debug.Log("Save video: ");
            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            UnityWebRequest request;

            if (selectedObject.transform.GetChild(0).GetChild(1).GetComponent<Text>().text == "Add audio")
            {
                Debug.Log("Audio add lesson: "); 
                formData.Add(new MultipartFormDataSection("lessonId", Convert.ToString(SaveLesson.lessonId)));
                formData.Add(new MultipartFormFileSection("audio", Helper.FromAudioClip(audioData.clip), "abc.wav", "audio/wav"));
                request = UnityWebRequest.Post(APIUrlConfig.AddAudioLesson, formData);
                request.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
                yield return request.SendWebRequest();
            }
            else 
            {
                Debug.Log("Audio add LABEL "); 
                Debug.Log("Audio add TagHandler.Instance.labelIds " + TagHandler.Instance.labelIds.Count);
                Debug.Log("Audio add TagHandler.Instance.currentEditingIdx" + TagHandler.Instance.currentEditingIdx);
                Debug.Log("Audio add LABELID: " + Convert.ToString(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx]));
                    
                formData.Add(new MultipartFormDataSection("labelId", Convert.ToString(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx])));
                formData.Add(new MultipartFormFileSection("audio", Helper.FromAudioClip(audioData.clip), "dfg.wav", "audio/wav"));
                Debug.Log("Minh : " + formData );
                request = UnityWebRequest.Post(APIUrlConfig.AddAudioLabel, formData);
                request.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
                yield return request.SendWebRequest();
            }
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Audio add error " + request.error);
            }
            else
            {
                // spinner.SetActive(true);
                Debug.Log("Audio add complete: ");
                selectedObject.SetActive(false);
                panelListCreateLesson.SetActive(false);
                toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
                LoadDataListItemPanel.Instance.UpdateLessonInforPannel(SaveLesson.lessonId);
            }   
        }

        // public async Task SaveRecordAudio()
        // {
        //     try
        //     {
        //         List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //         UnityWebRequest request;

        //         if (selectedObject.transform.GetChild(0).GetChild(1).GetComponent<Text>().text == "Add audio")
        //         {
        //             Debug.Log("Audio add lesson: "); 
        //             formData.Add(new MultipartFormDataSection("lessonId", Convert.ToString(SaveLesson.lessonId)));
        //             formData.Add(new MultipartFormFileSection("audio", Helper.FromAudioClip(audioData.clip), "abc.wav", "audio/wav"));
        //             APIResponse<string> addAudioResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_ADD_AUDIO_LESSON, UnityWebRequest.kHttpVerbPOST, formData);
        //             if (addAudioResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
        //             {
        //                 selectedObject.SetActive(false);
        //                 panelListCreateLesson.SetActive(false);
        //                 toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
        //                 LoadDataListItemPanel.Instance.UpdateLessonInforPannel(SaveLesson.lessonId);
        //             }
        //             else
        //             {
        //                 Debug.Log("Audio add error: ");
        //             }
        //         }
        //         else
        //         {
        //             Debug.Log("Audio add LABEL "); 
        //             Debug.Log("Audio add TagHandler.Instance.labelIds " + TagHandler.Instance.labelIds.Count);
        //             Debug.Log("Audio add TagHandler.Instance.currentEditingIdx" + TagHandler.Instance.currentEditingIdx);
        //             Debug.Log("Audio add LABELID: " + Convert.ToString(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx]));
        //             formData.Add(new MultipartFormDataSection("labelId", Convert.ToString(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx])));
        //             formData.Add(new MultipartFormFileSection("audio", Helper.FromAudioClip(audioData.clip), "dfg.wav", "audio/wav"));
        //             Debug.Log("Minh : " + formData );
        //             APIResponse<string> addAudioLabelResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_ADD_AUDIO_LABEL, UnityWebRequest.kHttpVerbPOST, formData);
        //             if (addAudioLabelResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
        //             {
        //                 selectedObject.SetActive(false);
        //                 panelListCreateLesson.SetActive(false);
        //                 toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
        //                 LoadDataListItemPanel.Instance.UpdateLessonInforPannel(SaveLesson.lessonId);
        //             }
        //             else
        //             {
        //                 Debug.Log("Audio add error: ");
        //             }
        //         }
        //     }
        //     catch (Exception e)
        //     {

        //     }
        // }
    }
}
