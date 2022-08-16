using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using UnityEngine.Networking;
using System.Threading.Tasks;
using EasyUI.Toast;

namespace LessonDetail_Edit
{
    public class LoadScene : MonoBehaviour
    {
        public LessonDetail[] myData; 
        public LessonDetail currentLesson; 
        public GameObject bodyObject; 
        public GameObject lessonTitle; 
        public int calculatedSize = 15;
        public Button startMeetingBtn;
        public Button startExperienceBtn;

        void Start()
        {
            InitScreen();
            InitUI();
            LoadLessonDetail(LessonManager.lessonId);
        }

        void InitUI()
        {
            if (startMeetingBtn != null)
            {
                startMeetingBtn.interactable = false;
            }
            if (startExperienceBtn != null)
            {
                startExperienceBtn.interactable = false;
            }
        }

        void InitScreen()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        }

        async void LoadLessonDetail(int lessonId)
        {
            try
            {
                APIResponse<LessonDetail[]> lessonDetailResponse = await UnityHttpClient.CallAPI<LessonDetail[]>(String.Format(APIUrlConfig.GET_LESSON_BY_ID, lessonId), UnityWebRequest.kHttpVerbGET);
                if (lessonDetailResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    StaticLesson.SetValueForStaticLesson(lessonDetailResponse.data[0]);
                    if (startMeetingBtn != null)
                    {
                        startMeetingBtn.interactable = true;
                    }
                    if (startExperienceBtn != null)
                    {
                        startExperienceBtn.interactable = true;
                    }
                    LoadDataIntoUI();
                }
                else
                {
                    throw new Exception(lessonDetailResponse.message);
                }
            }
            catch (Exception ex)
            {
                Toast.ShowCommonToast(ex.Message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
            }
        }

        void LoadDataIntoUI()
        {
            try
            {
                string imageURL = APIUrlConfig.DOMAIN_SERVER + StaticLesson.LessonThumbnail;
                RawImage targetImage = bodyObject.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
                StartCoroutine(UnityHttpClient.LoadRawImageAsync(imageURL, targetImage, (isSuccess) => {
                    if (isSuccess)
                    {
                        bodyObject.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                    }
                }));

                lessonTitle.gameObject.GetComponent<Text>().text = Helper.FormatString(StaticLesson.LessonTitle.ToLower(), calculatedSize);
                bodyObject.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>().text = Helper.FormatString(StaticLesson.AuthorName, calculatedSize);
                bodyObject.transform.GetChild(2).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = DateTime.Parse(StaticLesson.CreatedDate).ToString("dd/MM/yyyy HH:mm:ss");
                bodyObject.transform.GetChild(2).GetChild(1).GetChild(0).GetComponent<Text>().text = "#" + StaticLesson.LessonId.ToString();
                bodyObject.transform.GetChild(2).GetChild(1).GetChild(1).GetComponent<Text>().text = StaticLesson.Viewed.ToString() + " Views";
            }
            catch(Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}
