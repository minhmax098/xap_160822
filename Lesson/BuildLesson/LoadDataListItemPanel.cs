using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Text;
using System.IO;
using UnityEngine.UI;
using EasyUI.Toast;
using System.Threading.Tasks;

namespace BuildLesson 
{
    public class LoadDataListItemPanel : MonoBehaviour
    {
        private static LoadDataListItemPanel instance;
        public static LoadDataListItemPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<LoadDataListItemPanel>();
                }
                return instance; 
            }
        }
        public GameObject lessonInfoPanel;
        private int calculatedSize = 30;
        public GameObject showListItem;
        public GameObject txtShowListItem;

        // panel PopUpDeleteActions
        public GameObject panelPopUpDeleteActions;
        public Button btnExitPopupDeleteActions;
        public Button btnDeleteActions;
        public Button btnCancelDeleteActions; 

        // panel PopUpDeleteActionsVideo
        public GameObject panelPopUpDeleteActionsVideo;
        public Button btnExitPopupDeleteActionsVideo;
        public Button btnDeleteActionsVideo;
        public Button btnCancelDeleteActionsVideo; 

        void Update()
        {
            btnExitPopupDeleteActions.onClick.AddListener(HandlerExitPopupDeleteActions);
            btnCancelDeleteActions.onClick.AddListener(HandlerCancelDeleteActions);
            btnExitPopupDeleteActionsVideo.onClick.AddListener(HandlerExitPopupDeleteActionsVideo);
            btnCancelDeleteActionsVideo.onClick.AddListener(HandlerCancelDeleteActionsVideo);
        }
        
        void HandlerExitPopupDeleteActions()
        {
            panelPopUpDeleteActions.SetActive(false);
        }
        void HandlerCancelDeleteActions()
        {
            panelPopUpDeleteActions.SetActive(false);
        }
        void HandlerExitPopupDeleteActionsVideo()
        {
            panelPopUpDeleteActionsVideo.SetActive(false);
        }
        void HandlerCancelDeleteActionsVideo()
        {
            panelPopUpDeleteActionsVideo.SetActive(false); 
        }

        // Update the Pannel 
        public async Task UpdateLessonInforPannel(int lessonId)
        {
            Debug.Log("Update lesson info pannel ...");
            try
            {
                Debug.Log("Update lesson info pannel lesson id: " + lessonId);
                APIResponse<List<LessonDetail>> lessonDetailResponse = await UnityHttpClient.CallAPI<List<LessonDetail>>(String.Format(APIUrlConfig.GET_LESSON_BY_ID, lessonId), UnityWebRequest.kHttpVerbGET); 
                if (lessonDetailResponse.code  == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    // Use static class to store 
                    StaticLesson.SetValueForStaticLesson(lessonDetailResponse.data[0]);
                    Debug.Log("Update lesson info pannel Get lesson info: " + StaticLesson.LessonTitle);
                    Debug.Log("Update lesson info pannel Get lesson info: " + StaticLesson.Audio); 
                    // Refresh the item 
                    foreach (Transform child in lessonInfoPanel.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    // Load all infomation from the api result 
                    if (StaticLesson.Audio != "")
                    {
                        txtShowListItem.SetActive(false);
                        loadAudio(StaticLesson.Audio, lessonId);
                    }
                    if (StaticLesson.Video != "")
                    {
                        txtShowListItem.SetActive(false);
                        StartCoroutine(loadVideo(StaticLesson.Video, lessonId));
                    } 
                    Debug.Log("Update lesson info pannel List label length: " + StaticLesson.ListLabel.Length);
                    if (StaticLesson.ListLabel.Length > 0) 
                    {
                        foreach (Label label in StaticLesson.ListLabel)
                        {
                            txtShowListItem.SetActive(false);
                            Debug.Log("Update lesson info pannel label audio " + label.audioLabel);
                            if (label.audioLabel != "")
                            {
                                txtShowListItem.SetActive(false);
                                loadAudio(label.audioLabel, lessonId, label.labelName);
                            }
                            Debug.Log("Update lesson info pannel label video " + label.videoLabel);
                            if (label.videoLabel != "")
                            {
                                txtShowListItem.SetActive(false);
                                StartCoroutine(loadVideo(label.videoLabel, lessonId, label.labelName));
                            }
                            // if (label.audioLabel == "" && label.videoLabel == "")
                            // {
                            //     // txtShowListItem.SetActive(true);
                            // }
                        }
                    }
                    if (StaticLesson.ListLabel.Length.ToString() == "")
                    {
                        txtShowListItem.SetActive(true);
                    }
                    if (StaticLesson.Audio == "" && StaticLesson.Video == "")
                    {
                        txtShowListItem.SetActive(true);
                        // showListItem.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = $"No item";
                    }
                }
                else 
                {
                    Debug.Log("Update lesson info pannel Throw Exception");
                    throw new Exception(lessonDetailResponse.message);
                }
            }
            catch (Exception e)
            {

                Debug.Log($"Update lesson info pannel Update lesson info panel failed: {e.Message}");
            }
        }

        private async Task loadAudio(string audioUrl, int lessonId, string title = "Intro")
        {
            GameObject audioComp = Instantiate(Resources.Load(PathConfig.ADD_AUDIO) as GameObject);
            if (title == "Intro")
            {
                audioComp.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteAudio(lessonId, title));
            }
            else 
            {
                audioComp.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteAudioLabel(lessonId, TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx], title));
            }
            audioComp.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = String.Format("Audio: {0}", Helper.ShortString(title, 15));
            Debug.Log("minh debug: "+ audioComp.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text);
            audioComp.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = String.Format("Audio: {0}", Helper.ShortString(title, 15));

            audioComp.transform.parent = lessonInfoPanel.transform;     
            audioComp.transform.localScale = new Vector3(1f, 1f, 1f);  

            AudioClip audioData = await UnityHttpClient.GetAudioClip(audioUrl); 
            audioComp.GetComponent<AudioSource>().clip = audioData;
            // audioComp.GetComponent<AudioSource>().Play();
            AudioManager1.Instance.DisplayAudio(true);
        }

        // private async Task loadVideo(string videoUrl, int lessonId)
        // {
        //     GameObject videoComp = Instantiate(Resources.Load(PathConfig.ADD_VIDEO) as GameObject); 
        //     videoComp.transform.GetChild(0).GetChild(3).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteVideo(lessonId));
        //     videoComp.transform.parent = lessonInfoPanel.transform;
        //     videoComp.transform.localScale = new Vector3(1f, 1f, 1f);
        //     InfoLinkVideo dataResp = JsonUtility.FromJson<InfoLinkVideo>(webRequest.downloadHandler.text); 
        //     videoComp.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = Helper.FormatString(dataResp.title.ToLower(), calculatedSize);
        //     LoadVideoThumbnail(videoComp, dataResp.thumbnail_url, videoUrl);
        // }

        private IEnumerator loadVideo(string videoUrl, int lessonId, string title = "Intro")
        {
            GameObject videoComp = Instantiate(Resources.Load(PathConfig.ADD_VIDEO) as GameObject); 
            videoComp.transform.GetChild(0).GetChild(2).GetComponent<Button>().onClick.AddListener(() => HandlerDeleteVideo(lessonId, title));
            videoComp.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = String.Format("Video: {0}", Helper.ShortString(title, 15));
            Debug.Log("minh debug: " + videoComp.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text);
            videoComp.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = String.Format("Video: {0}", Helper.ShortString(title, 15));
            Debug.Log("Load video url: " + String.Format(APIUrlConfig.LoadLesson, videoUrl));
            UnityWebRequest webRequest = UnityWebRequest.Get(String.Format(APIUrlConfig.GetLinkVideo, videoUrl));
            videoComp.transform.parent = lessonInfoPanel.transform;
            videoComp.transform.localScale = new Vector3(1f, 1f, 1f);
            yield return webRequest.SendWebRequest();
            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log("An error has occur");
                Debug.Log(webRequest.error);
            }
            else
            {
                // Check when response is received 
                if (webRequest.isDone)
                {
                    // videoComp.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Helper.ShortString(title, 15);
                    Debug.Log("Update video: " + webRequest.downloadHandler.text);
                    InfoLinkVideo dataResp = JsonUtility.FromJson<InfoLinkVideo>(webRequest.downloadHandler.text); 
                    videoComp.transform.GetChild(1).GetChild(0).GetChild(2).GetComponent<Text>().text = Helper.FormatString(dataResp.title.ToLower(), calculatedSize);
                    StartCoroutine(LoadVideoThumbnail(videoComp, dataResp.thumbnail_url, videoUrl));
                }
            }
        }

        IEnumerator LoadVideoThumbnail(GameObject videoObj, string imageUri, string videoUri)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {

            }
            if (request.isDone)
            {
                Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                // videoObj.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
                // videoObj.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(() => NavigageToVideo(videoUri));
            }
        }

        void NavigageToVideo(string videoUri)
        {
            Application.OpenURL(videoUri);
        }

        void HandlerDeleteAudio(int lessonId, string lessonTitle)
        {
            panelPopUpDeleteActions.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = Helper.ShortString(lessonTitle, 10) + "?";
            panelPopUpDeleteActions.SetActive(true);
            btnDeleteActions = GameObject.Find("BtnDeleteActions").GetComponent<Button>();
            btnDeleteActions.onClick.AddListener(() => DeleteAudioLesson(lessonId));
        }

        void HandlerDeleteAudioLabel(int lessonId, int labelId, string labelTitle)
        {
            panelPopUpDeleteActions.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = Helper.ShortString(labelTitle, 10) + "?";
            panelPopUpDeleteActions.SetActive(true);
            btnDeleteActions = GameObject.Find("BtnDeleteActions").GetComponent<Button>();
            btnDeleteActions.onClick.AddListener(() => DeleteAudioLabel(lessonId, labelId));
        }

        void HandlerDeleteVideo(int lessonId, string lessonTitle)
        {
            panelPopUpDeleteActionsVideo.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = Helper.ShortString(lessonTitle, 10) + "?";
            panelPopUpDeleteActionsVideo.SetActive(true);
            btnDeleteActionsVideo = GameObject.Find("BtnDeleteActionsVideo").GetComponent<Button>();
            btnDeleteActionsVideo.onClick.AddListener(() => DeleteVideoLesson(lessonId));
        }

        void HandlerDeleteVideoLabel(int lessonId, int labelId, string labelTitle)
        {
            panelPopUpDeleteActions.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<Text>().text = Helper.ShortString(labelTitle, 10) + "?";
            panelPopUpDeleteActions.SetActive(true);
            btnDeleteActions = GameObject.Find("BtnDeleteActions").GetComponent<Button>();
            btnDeleteActionsVideo.onClick.AddListener(() => DeleteVideoLabel(lessonId, labelId));
        }

        public async void DeleteAudioLesson(int lessonId)
        {
            try
            {
                panelPopUpDeleteActions.SetActive(false);
                DeleteAudioLessonRequest deleteAudioLessonRequest = new DeleteAudioLessonRequest();
                string url = String.Format(APIUrlConfig.DELETE_AUDIO_LESSON, lessonId);
                APIResponse<string> deleteAudioLessonResponse = await UnityHttpClient.CallAPI<string>(url, UnityWebRequest.kHttpVerbDELETE, deleteAudioLessonRequest);
                if (deleteAudioLessonResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    Debug.Log("Delete audio: ");
                    UpdateLessonInforPannel(lessonId);
                    if (StaticLesson.Audio == "")
                    {
                        // showListItem.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = $"No item";
                        txtShowListItem.SetActive(true);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Delete audio lesson failed: " + e.Message);
            }
        }

        public async void DeleteAudioLabel (int lessonId, int labelId)
        {
            try
            {
                panelPopUpDeleteActions.SetActive(false);
                DeleteAudioLabelRequest deleteAudioLabelRequest = new DeleteAudioLabelRequest();
                string url = String.Format(APIUrlConfig.DELETE_AUDIO_LABEL, labelId);
                APIResponse<string> deleteAudioLabelResponse = await UnityHttpClient.CallAPI<string>(url, UnityWebRequest.kHttpVerbDELETE, deleteAudioLabelRequest);
                if (deleteAudioLabelResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    Debug.Log("Delete audio: ");
                    UpdateLessonInforPannel(lessonId);
                    // if (StaticLesson.ListLabel.Audio == "")
                    // {
                    //     // showListItem.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text = $"No item";
                    //     txtShowListItem.SetActive(true);
                    // }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Delete audio label failed: " + e.Message);
            }
        }

        public async void DeleteVideoLesson(int lessonId)
        {
            try
            {
                DeleteVideoLessonRequest deleteVideoLessonRequest = new DeleteVideoLessonRequest();
                string url = String.Format(APIUrlConfig.DELETE_VIDEO_LESSON, lessonId);
                APIResponse<string> deleteVideoLessonResponse = await UnityHttpClient.CallAPI<string>(url, UnityWebRequest.kHttpVerbDELETE, deleteVideoLessonRequest);
                if (deleteVideoLessonResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    Debug.Log("Delete video lesson: ");
                    UpdateLessonInforPannel(lessonId);
                    panelPopUpDeleteActionsVideo.SetActive(false);
                    if (StaticLesson.Video == "")
                    {
                        txtShowListItem.SetActive(true);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Delete video lesson failed: " + e.Message);
            }
        }

        public async void DeleteVideoLabel(int lessonId, int labelId)
        {
            try
            {
                DeleteVideoLabelRequest deleteVideoLabelRequest = new DeleteVideoLabelRequest();
                string url = String.Format(APIUrlConfig.DELETE_VIDEO_LABEL, labelId);
                APIResponse<string> deleteVideoLabelResponse = await UnityHttpClient.CallAPI<string>(url, UnityWebRequest.kHttpVerbDELETE, deleteVideoLabelRequest);
                if (deleteVideoLabelResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    Debug.Log("Delete video label: ");
                    UpdateLessonInforPannel(lessonId);
                    // panelPopUpDeleteActionsVideo.SetActive(false);
                    // if (StaticLesson.Video == "")
                    // {
                    //     txtShowListItem.SetActive(true);
                    // }
                }
            }
            catch (Exception e)
            {
                Debug.Log("Delete video label failed: " + e.Message);
            }
        }
    }
}

