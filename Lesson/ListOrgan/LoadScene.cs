using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using System.Text.RegularExpressions;
using UnityEngine.EventSystems; 
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Linq;
using EasyUI.Toast;

namespace ListOrgan 
{
    public class LoadScene : MonoBehaviour
    {
        private char[] charsToTrim = { '*', '.', ' '};
        public int calculatedSize = 35; 
        private string searchValueString; 
        private int offset = 0;
        private int limit = 8;
        public GameObject lessonPanelObject;
        public Text totalFoundLessonText;
        public GameObject lessonObjectResource;
        public InputField searchInputField; 
        public GameObject resetSearchBoxBtn; 
        public GameObject loadMorePanel;
        public CustomScrollRect lessonPanelScrollRect;
        public GameObject waitingScreen;
        public Text organHeaderText;
        void Start()
        {
            InitScreen();
            InitUI();
            InitEvents(); 
            UpdateLessonsData();
        }
        void InitScreen()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        }
        void InitEvents()
        {
            resetSearchBoxBtn.transform.GetComponent<Button>().onClick.AddListener(ResetSearchBox); 
            searchInputField.onValueChanged.AddListener(SearchLessons); 
            loadMorePanel.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(LoadMoreLessons);
        }

        void InitUI()
        {
            organHeaderText.text = OrganManager.organName;
        }
        void ResetSearchBox()
        {
            searchInputField.text = "";
            searchInputField.Select();
        }

        void SearchLessons(string value)
        {
            searchValueString = Regex.Replace(value, @"\s+", " ").ToLower().Trim(charsToTrim); 
            resetSearchBoxBtn.SetActive(!string.IsNullOrEmpty(searchValueString)); 
            if (string.IsNullOrEmpty(searchValueString))
            {
                UpdateLessonsData();
            }
            else
            {
                StartCoroutine(CheckForSearchingLessons(value));
            }
        }

        IEnumerator CheckForSearchingLessons(string value)
        {
            yield return new WaitForSeconds(0.4f);
            if (value == searchValueString)
            {
                UpdateLessonsData();
            }
        }

        void UpdateLessonsData()
        {
            offset = 0;
            UpdateLessonsPanel(true);
        }

        void LoadMoreLessons()
        {
            offset++;
            UpdateLessonsPanel(false);
        }

        void ProcessingLoadMoreBtn(bool isActive, bool isLoading)
        {
            loadMorePanel.transform.GetChild(0).gameObject.SetActive(!isLoading);
            loadMorePanel.transform.GetChild(1).gameObject.SetActive(isLoading);
            loadMorePanel.SetActive(isActive);
        }

        async void UpdateLessonsPanel(bool isRenewLessonPanel)
        {
            try
            {
                if (string.IsNullOrEmpty(searchValueString))
                {
                    searchValueString = null;
                }
                if (isRenewLessonPanel)
                {
                    DestroyAllLessons();
                }
                totalFoundLessonText.text = "";
                ProcessingLoadMoreBtn(!isRenewLessonPanel, true);
                waitingScreen.SetActive(true);
                string URL = String.Format(APIUrlConfig.GET_SEARCH_LESSONS_BY_ORGAN, searchValueString, offset, limit, OrganManager.organId);            
                APIResponse<List<Lesson>> lessonResponse = await UnityHttpClient.CallAPI<List<Lesson>>(URL, UnityWebRequest.kHttpVerbGET);
                waitingScreen.SetActive(false);
                if (isRenewLessonPanel)
                {
                    DestroyAllLessons();
                }
                totalFoundLessonText.text = "";

                if (lessonResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    offset = lessonResponse.meta.page;
                    StartCoroutine(LoadLessonsIntoUI(lessonResponse.data, lessonResponse.meta.totalElements, isRenewLessonPanel));
                    lessonPanelScrollRect.verticalNormalizedPosition = 0f;
                    ProcessingLoadMoreBtn(offset < lessonResponse.meta.totalPage - 1, false);
                }
                else
                {
                    ProcessingLoadMoreBtn(false, false);
                }                
            } 
            catch (Exception e)
            {
                ProcessingLoadMoreBtn(false, false);
                Debug.Log(e.Message);
            }
        }

        IEnumerator LoadLessonsIntoUI(List<Lesson> lessons, int totalFoundLesson, bool isRenewLessonPanel)
        {
            if (isRenewLessonPanel)
            {
                DestroyAllLessons();
            }
            totalFoundLessonText.text = (lessons.Count) > 0 ? totalFoundLesson.ToString() + ItemConfig.foundLessonNumberIntro : ItemConfig.noDataNotification;
            if (lessons.Count < 1)
            {
                yield break;
            }
            foreach (Lesson lesson in lessons)
            {
                InstantiateLesson(lesson, lessonPanelObject.transform);
            }
        }

        void DestroyAllLessons()
        {
            foreach (Transform child in lessonPanelObject.transform) 
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        void InstantiateLesson(Lesson lesson, Transform parentTransform)
        {
            try
            {
                GameObject lessonObject = Instantiate(lessonObjectResource) as GameObject; 
                lessonObject.transform.SetParent(parentTransform, false);

                string imageURL = APIUrlConfig.DOMAIN_SERVER + lesson.lessonThumbnail;
                RawImage targetImage = lessonObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
                StartCoroutine(UnityHttpClient.LoadRawImageAsync(imageURL, targetImage, (isSuccess) => {
                    if (isSuccess)
                    {
                        lessonObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    }
                }));

                lessonObject.name = lesson.lessonId.ToString(); 
                lessonObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Helper.FormatString(lesson.lessonTitle, calculatedSize);
                lessonObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => InteractionUI.Instance.onClickItemLesson(lesson.lessonId));            
            }
            catch (Exception e)
            {
                Debug.Log($"error InstantiateLesson {e.Message}");
            }
        }
    }
}
