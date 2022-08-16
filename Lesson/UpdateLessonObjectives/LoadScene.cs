using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using EasyUI.Toast;
using System.Threading.Tasks;

namespace UpdateLessonObjectives
{
    public class LoadScene : MonoBehaviour
    {
        public GameObject spinner;
        public LessonDetail[] myData;
        public LessonDetail currentLesson; 
        public GameObject bodyObject; 
        public Button updateBtn;
        private ListOrgans listOrgans;
        // public GameObject dropdownObj; 
        private Dropdown dropdown;
        private List<Dropdown.OptionData> option_ = new List<Dropdown.OptionData>();
        private int indexItemDropdown = 0;

        void Start()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
            
            updateBtn.onClick.AddListener(UpdateLessonObjective);
            myData = LoadData.Instance.GetLessonByID(LessonManager.lessonId.ToString()).data;
            // currentLesson = Array.Find(myData, lesson => lesson.lessonId == LessonManager.lessonId);
            currentLesson = myData[0];
            StartCoroutine(LoadCurrentLesson(currentLesson));
            spinner.SetActive(false);
            // dropdown = dropdownObj.GetComponent<Dropdown>();
            UpdateDropDown();
        }

        IEnumerator LoadCurrentLesson(LessonDetail currentLesson)
        {
            string imageUri = String.Format(APIUrlConfig.LoadLesson, currentLesson.lessonThumbnail);
            bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonTitle; 
            bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text = currentLesson.lessonObjectives; 
            ModelStoreManager.InitModelStore(currentLesson.modelId, ModelStoreManager.modelName);
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUri);
            yield return request.SendWebRequest(); 
            if (request.isNetworkError || request.isHttpError)
            {

            }
            if (request.isDone)
            {
                Texture2D tex = ((DownloadHandlerTexture) request.downloadHandler).texture;
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
                // bodyObject.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = sprite;
            }
        }

        void UpdateDropDown()
        {
            listOrgans = LoadData.Instance.getListOrgans();
            if (listOrgans.data.Length > 0)
            {
                foreach (ListOrganLesson organ in listOrgans.data)
                {
                    option_.Add(new Dropdown.OptionData(organ.organsName));
                }
                dropdown.AddOptions(option_);
            }
            else
            {
                indexItemDropdown = 0;
                dropdown.value = indexItemDropdown;
            }
            ClickItemDropdown(dropdown);
            dropdown.onValueChanged.AddListener(delegate { ClickItemDropdown(dropdown); });
        }

        public void ClickItemDropdown(Dropdown dropdown)
        {
            Debug.Log("minhlh17: " + dropdown.value);
            indexItemDropdown = dropdown.value;
        }

        public IEnumerator WaitForAPIResponse(UnityWebRequest webRequest)
        {
            spinner.SetActive(true);
            Debug.Log("Calling API");
            while(!webRequest.isDone)
            {
                yield return null;
            }
        }

        void UpdateLessonObjective()
        {
            // Check form valid
            Debug.Log("form submit: ");
            Debug.Log("Index choose: " + dropdown.value);
            // Reference the Real index by the API 
            Debug.Log("Real index: " + listOrgans.data[dropdown.value].organsId); 
            Debug.Log("Lesson name modified: " + bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text);
            Debug.Log("Lesson obj mod: " + bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text);
            // Debug.Log($"organName {bodyObject.transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text}");
            Debug.Log("Organ name: " + bodyObject.transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetComponent<Text>().text);
            
            PublicLesson newLesson = new PublicLesson();
            newLesson.modelId = ModelStoreManager.modelId;
            newLesson.lessonTitle = bodyObject.transform.GetChild(0).GetChild(1).GetComponent<InputField>().text;
            newLesson.organId = listOrgans.data[dropdown.value].organsId;
            Debug.Log("new lesson: "+ newLesson.organId);
            newLesson.lessonObjectives = bodyObject.transform.GetChild(2).GetChild(1).GetComponent<InputField>().text;
            newLesson.publicLesson = bodyObject.transform.GetChild(3).GetChild(0).GetComponent<Toggle>().isOn ? 1 : 0;
            spinner.SetActive(false);
            Debug.Log("lhminh17 before new lesson: " + newLesson.modelId);
            Debug.Log("lhminh17 new lesson: " + newLesson.lessonTitle);
            Submit(LessonManager.lessonId, newLesson);
        }

        public async void Submit(int lessonId, PublicLesson newLesson)
        {
            try
            {
                string url = String.Format(APIUrlConfig.PATCH_UPDATE_LESSON_INFO, lessonId);
                Debug.Log("lhminh17 url: " + url);
                Debug.Log("lhminh17 lessonTitle: " + newLesson.lessonTitle);
                APIResponse<string> updateLessonResponse = await UnityHttpClient.CallAPI<string>(url, APIUrlConfig.PATCH_METHOD, newLesson);
                if (updateLessonResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    // SceneManager.LoadScene(SceneConfig.lesson_edit);
                    StartCoroutine(Helper.LoadAsynchronously(SceneConfig.lesson_edit));
                    Debug.Log("TEST SUBMIT submit done: ");
                }
                Debug.Log($"lhminh17 {updateLessonResponse.code}");
            }
            catch (Exception e)
            {
                Debug.Log("TEST SUBMIT FAIL " + e.Message);
            }
        }
    }
}
