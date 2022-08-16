using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using System;
using System.Net;
using UnityEngine.Networking; 
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using YoutubePlayer;
using EasyUI.Toast;
using System.Threading.Tasks;

namespace BuildLesson
{
    public class ListItemsManager : MonoBehaviour
    {
        private static ListItemsManager instance;
        public static ListItemsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<ListItemsManager>();
                }
                return instance;
            }
        }

        public GameObject spinner;
        public Button btnToggleListItem;
        public Animator toggleListItemAnimator;
        public GameObject groupIcon;

        // Panel List Create Lesson 
        public GameObject panelListCreateLesson;
        public Button btnAddAudio; 
        public Button btnAddVideo;
        public Button btnCancelListItem; 
        public GameObject panelAddAudio;
        public Button btnCancelAddAudio;
        public Button btnCancelAudio;

        // Panel Record 
        public GameObject panelRecord; 
        public Button btnCancelAddRecord;
        public Button btnCancelRecord;
        public Button btnRecord; 
        public Button btnUpload;
        public Button btnRedRecord;
        public GameObject timeIndicator;

        // Panel Save Record 
        public GameObject panelSaveRecord;
        public Button btnPlayBack;
        public Button btnCancelSaveRecordTL;
        public Button btnCancelSaveRecordBL;
        public Button btnRecording;

        // Panel Add Video 
        public GameObject panelAddVideo;   
        public Button btnCancelAddVideo;  // X Button 
        public Button btnCancelPasteVideo;  // Cancel Button
        public Button btnSaveAddVideo;
        
        public InputField pasteVideo;
        private Regex ytbRegex = new Regex(@"^((?:https?:)?\/\/)?((?:www|m)\.)?((?:youtube(-nocookie)?\.com|youtu.be))(\/(?:[\w\-]+\?v=|embed\/|v\/)?)([\w\-]+)(\S+)?$"); 
        private string jsonResponse;
        public Text title;
        private int calculatedSize = 25;
        public GameObject videoObj;

        // Panel Upload
        public GameObject panelUploadAudio;
        public Button btnCancelUploadAudio;
        public Button btnCancelUpload;
        public Button btnUploadAudio;

        private bool isRecordAudio = false;

        public static float startTime = 0f;
        private AudioSource audioSource;
        private bool isPlayingAudio = false;
        // add sampling rate 
        private static int samplingRate;

        void Awake()
        {
            // base.Awake();
            // Query the maximum frequency of the default microphone.
            int minSamplingRate; 
            Microphone.GetDeviceCaps(null, out minSamplingRate, out samplingRate);
        }

        void Start()
        {
            spinner.SetActive(false);
            Debug.Log("Record Pannel Start: ");
            foreach (var device in Microphone.devices)
            {
                Debug.Log("Record Pannel Name: " + device);
            }
            InitEvents();
            LoadDataListItemPanel.Instance.UpdateLessonInforPannel(LessonManager.lessonId); 
        }
        
        void Update()
        {
            if (panelRecord.activeSelf && isRecordAudio)
            {
                startTime += Time.deltaTime;
                DisplayTime(startTime, timeIndicator);
            }
            if (panelRecord.activeSelf && !isRecordAudio)
            {
                timeIndicator.GetComponent<Text>().text = "00:00";
            }
        }
    
        void InitEvents()
        {
            btnToggleListItem.onClick.AddListener(ToggleListItem);
            btnAddAudio.onClick.AddListener(HandlerAddAudio);
            btnCancelListItem.onClick.AddListener(CancelListItem); 
            // groupIcon = GetComponent<Image>();
            btnCancelAddAudio.onClick.AddListener(CancelAddAudio);
            btnCancelAudio.onClick.AddListener(CancelAudio);
            
            // Record
            btnCancelAddRecord.onClick.AddListener(CancelAddRecord);
            btnCancelRecord.onClick.AddListener(CancelRecord);
            btnRecord.onClick.AddListener(RecordLesson);
            btnUpload.onClick.AddListener(UploadAudioLesson);
            btnRedRecord.onClick.AddListener(HandlerRedRecord);

            // Playback Audio, Save Record
            btnPlayBack.onClick.AddListener(PlayStopAudio);
            btnCancelSaveRecordTL.onClick.AddListener(HandlerCancelSaveRecordTL);
            btnCancelSaveRecordBL.onClick.AddListener(HandlerCancelSaveRecordBL);
            btnRecording.onClick.AddListener(HandlerRecording);

            // Upload
            btnCancelUploadAudio.onClick.AddListener(CancelUploadAudio);
            btnCancelUpload.onClick.AddListener(CancelUpload);
            btnUploadAudio.onClick.AddListener(HandlerUploadAudio);
            
            // Add Video
            btnSaveAddVideo.interactable = false;
            btnAddVideo.onClick.AddListener(HandlerAddVideo);
            btnCancelAddVideo.onClick.AddListener(HandlerCancelAddVideo);
            btnCancelPasteVideo.onClick.AddListener(HandlerCancelPasteVideo);
            btnSaveAddVideo.onClick.AddListener(HandlerSaveAddVideo); 
            pasteVideo.onEndEdit.AddListener(delegate {LockInput(pasteVideo);});
        }

        // Checks if there is anything entered into the input field.
        void LockInput(InputField input)
        {
            if (input.text.Length > 0)
            {
                if (ytbRegex.IsMatch(input.text.Trim()))
                {
                    Debug.Log("Link is valid");
                    Debug.Log(input.text);
                    InfoLinkVideo info = GetVideoInfo(input.text.Trim());
                    Debug.Log("info: " + info.title);
                    title.gameObject.GetComponent<Text>().text = Helper.FormatString(info.title.ToLower(), calculatedSize);
                    VideoManagerBuildLesson.Instance.ShowVideo(input.text.Trim());
                    btnSaveAddVideo.interactable = true;
                }
                else
                {
                    Debug.Log("Pop up notify! Invalid link");
                    btnSaveAddVideo.interactable = false;
                    title.gameObject.GetComponent<Text>().text = "Video not available";
                }
            }
            else if (input.text.Length == 0)
            {
                Debug.Log("Pop up notity! Enter empty string");
                btnSaveAddVideo.interactable = false;
                title.gameObject.GetComponent<Text>().text = "Video not available";
            }
        }

        void HandlerSaveAddVideo()
        {
            Debug.Log("Enter save record: ");
            // Check add video label or video lesson 
            Debug.Log("ADD VIDEO: " + panelAddVideo.transform.GetChild(0).GetChild(1).GetComponent<Text>().text);
            if (panelAddVideo.transform.GetChild(0).GetChild(1).GetComponent<Text>().text == "Intro")
            {
                Debug.Log("ADD VIDEO: INTRO");
                SaveAddVideo(LessonManager.lessonId, pasteVideo.text);
            }
            else 
            {
                Debug.Log("ADD VIDEO: " + Convert.ToString(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx]));
                SaveAddVideoLabel(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx], pasteVideo.text);
            }
        }

        void ToggleListItem()
        {
            Debug.Log("log animation: " + !toggleListItemAnimator.GetBool(AnimatorConfig.isShowMeetingMemberList));
            toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, !toggleListItemAnimator.GetBool(AnimatorConfig.isShowMeetingMemberList)); 
            groupIcon.transform.Rotate(0.0f, 0.0f, 180.0f, Space.Self);
        }
        void CancelListItem()
        {
            toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, false);
        }
        void HandlerAddAudio()
        {
            Debug.Log("Add audio: ");

            panelAddAudio.SetActive(true);
            panelAddAudio.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Add audio";
        }
        void HandlerAddVideo()
        {
            panelAddVideo.SetActive(true);
            panelAddVideo.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Intro";
            panelListCreateLesson.SetActive(false);
        }
       
        void CancelAddAudio()
        {
            panelAddAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelAudio()
        {
            panelAddAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelAddRecord()
        {
            startTime = 0f;
            timeIndicator.GetComponent<Text>().text = "00:00";
            btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageRecordingIcon);
            panelRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelRecord()
        {
            startTime = 0f;
            timeIndicator.GetComponent<Text>().text = "00:00";
            btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageRecordingIcon);
            panelRecord.SetActive(false);
            isRecordAudio = false;
            panelListCreateLesson.SetActive(false);
        }
        void RecordLesson()
        {
            Debug.Log("Recorded: ");
            panelAddAudio.SetActive(false);
            panelRecord.SetActive(true);
            panelRecord.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Helper.ShortString(panelAddAudio.transform.GetChild(0).GetChild(1).GetComponent<Text>().text, 10);
        }
        void UploadAudioLesson()
        {
            Debug.Log("Uploaded:");
            panelAddAudio.SetActive(false);
        }
        void CancelUploadAudio()
        {
            panelUploadAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void CancelUpload()
        {
            panelUploadAudio.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerCancelAddVideo()
        {
            pasteVideo.text = "";
            title.gameObject.GetComponent<Text>().text = "";
            videoObj.GetComponent<Image>().sprite = null;
            videoObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            panelAddVideo.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerCancelPasteVideo()
        {
            pasteVideo.text = "";
            title.gameObject.GetComponent<Text>().text = "";
            videoObj.GetComponent<Image>().sprite = null;
            videoObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            panelAddVideo.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }
        void HandlerRecording()
        {
            Debug.Log("Recording again: ");
            panelSaveRecord.SetActive(false);
            panelRecord.SetActive(true);
            HandlerRedRecord();
        }

        // Enter save call API AddVideoLesson
        public async void SaveAddVideo(int lessonId, string video)
        {
            try
            {
                AddVideoRequest addVideoRequest = new AddVideoRequest();
                addVideoRequest.Init(lessonId, video);
                APIResponse<string> addVideoResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_ADD_VIDEO_LESSON, UnityWebRequest.kHttpVerbPOST, addVideoRequest);
                if (addVideoResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    // Invoke success action
                    panelAddVideo.SetActive(false);
                    panelListCreateLesson.SetActive(false);
                    pasteVideo.text = "";
                    title.gameObject.GetComponent<Text>().text = "";
                    videoObj.GetComponent<Image>().sprite = null;
                    videoObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                    spinner.SetActive(false);
                    toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
                    LoadDataListItemPanel.Instance.UpdateLessonInforPannel(lessonId);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        // Enter save call API AddVideoLabel
        public async void SaveAddVideoLabel(int labelId, string video)
        {
            try
            {
                AddVideoLabelRequest addVideoLabelRequest = new AddVideoLabelRequest();
                addVideoLabelRequest.Init(labelId, video);
                APIResponse<string> addVideoLabelResponse = await UnityHttpClient.CallAPI<string>(APIUrlConfig.POST_ADD_VIDEO_LABEL, UnityWebRequest.kHttpVerbPOST, addVideoLabelRequest);
                if (addVideoLabelResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    panelAddVideo.SetActive(false);
                    panelListCreateLesson.SetActive(false);
                    pasteVideo.text = "";
                    title.gameObject.GetComponent<Text>().text = "";
                    videoObj.GetComponent<Image>().sprite = null;
                    videoObj.transform.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
                    toggleListItemAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, true);
                    LoadDataListItemPanel.Instance.UpdateLessonInforPannel(LessonManager.lessonId);
                }
            }
            catch (Exception e)
            {
                
            }
        }

        public InfoLinkVideo GetVideoInfo(string link)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetLinkVideo, link)); 
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("jsonResponse");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<InfoLinkVideo>(jsonResponse); 
        }

        public string DecodeFromUtf8(string utf8String)
        {
            byte[] utf8Bytes = new byte[utf8String.Length];
            for (int i = 0; i < utf8String.Length; ++i)
            {
                utf8Bytes[i] = (byte)utf8String[i];
            }
            return Encoding.UTF8.GetString(utf8Bytes,0,utf8Bytes.Length);
        }

        void HandlerCancelSaveRecordTL()
        {
            panelSaveRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
            startTime = 0f;
            timeIndicator.GetComponent<Text>().text = "00:00";
            btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageRecordingIcon);
        }
        void HandlerCancelSaveRecordBL()
        {
            startTime = 0f;
            timeIndicator.GetComponent<Text>().text = "00:00";
            btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageRecordingIcon);
            panelSaveRecord.SetActive(false);
            panelListCreateLesson.SetActive(false);
        }

        void HandlerRedRecord()
        {
            isRecordAudio = !isRecordAudio;
            // Change UI 
            if (isRecordAudio)
            {
                btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageStopRecordingIcon);
                // Start recording
                audioSource = panelSaveRecord.GetComponent<AudioSource>();
                audioSource.clip = Microphone.Start(null, true, 3599, samplingRate); // Maximum record 1 hour
            }
            else 
            {
                startTime = 0f;
                btnRedRecord.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageRecordingIcon);
                panelRecord.SetActive(false);
                panelSaveRecord.SetActive(true);
                panelSaveRecord.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = Helper.ShortString(panelRecord.transform.GetChild(0).GetChild(1).GetComponent<Text>().text, 10);
                // Stop recording
                audioSource = EndRecording(audioSource);
                AudioManager.Instance.DisplayAudio(true);
            }
        }

        void DisplayTime(float timeToDisplay, GameObject time)
        {
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            time.GetComponent<Text>().text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        AudioSource EndRecording (AudioSource audS) 
        {
            // Capture the current clip data
            AudioClip recordedClip = audS.clip;
            var position = Microphone.GetPosition(null);  // Use default microphone 
            var soundData = new float[recordedClip.samples * recordedClip.channels];
            recordedClip.GetData (soundData, 0);
            // Create shortened array for the data that was used for recording
            var newData = new float[position * recordedClip.channels];
            // anonymous Microphone.End (null);
            // Copy the used samples to a new array
            for (int i = 0; i < newData.Length; i++) 
            {
                newData[i] = soundData[i];
            }
            // One does not simply shorten an AudioClip,
            // so we make a new one with the appropriate length
            var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
            newClip.SetData(newData, 0); // Give it the data from the old clip
            // Replace the old clip
            AudioClip.Destroy(recordedClip);
            audS.clip = newClip;   
            return audS;
        }

        void PlayStopAudio()
        {
            isPlayingAudio = !isPlayingAudio;
            AudioManager.Instance.ControlAudio(isPlayingAudio);
        }

        public void HandlerUploadAudio()
        {
            Debug.Log("Upload audio from local: ");
        }

        void NavigageToVideo(string videoUri)
        {
            Application.OpenURL(videoUri);
        }
    }
}
