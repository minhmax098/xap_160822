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

namespace List3DStore
{
    public class LoadScene : MonoBehaviour
    {
        private char[] charsToTrim = { '*', '.', ' '};
        public int calculatedSize = 26; 
        private string searchValueString; 
        private int offset = 0;
        private int limit = 12;
        public GameObject modelPanelObject;
        public GameObject modelObjectResource;
        public InputField searchInputField; 
        public GameObject resetSearchBoxBtn; 
        public GameObject loadMorePanel;
        public CustomScrollRect modelPanelScrollRect;
        public GameObject waitingScreen;
        private int type = -1;  // Initial value
        public GameObject noDataComponent;

        void OnEnable()
        {
            SearchOnInit();
            resetSearchBoxBtn.transform.GetComponent<Button>().onClick.AddListener(ResetSearchBox); 
            searchInputField.onValueChanged.AddListener(SearchModels); 
            loadMorePanel.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(LoadMoreModels);
        }

        void OnDisable()
        {
            resetSearchBoxBtn.transform.GetComponent<Button>().onClick.RemoveListener(ResetSearchBox); 
            searchInputField.onValueChanged.RemoveListener(SearchModels); 
            loadMorePanel.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveListener(LoadMoreModels);
            DestroyAllModels();
        }

        void Start()
        {
            InitScreen();
        }

        void SearchOnInit()
        {
            type = ModelTypeManager.Instance.GetCurrentModeType();
            string searchText = Regex.Replace(searchInputField.text, @"\s+", " ").ToLower().Trim(charsToTrim); 
            if (string.IsNullOrEmpty(searchText))
            {
                searchText = null; // make "" = null;
            }
            if (searchText != searchValueString)
            {
                searchValueString = searchText;
            }
            UpdateModelsData();
        }
        void InitScreen()
        {
            Screen.orientation = ScreenOrientation.Portrait; 
            StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
            StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
        }
        void ResetSearchBox()
        {
            searchInputField.text = "";
            searchInputField.Select();
        }

        void SearchModels(string value)
        {
            searchValueString = Regex.Replace(value, @"\s+", " ").ToLower().Trim(charsToTrim); 
            resetSearchBoxBtn.SetActive(!string.IsNullOrEmpty(searchValueString)); 
            if (string.IsNullOrEmpty(searchValueString))
            {
                UpdateModelsData();
            }
            else
            {
                StartCoroutine(CheckForSearchingModels(value));
            }
        }

        IEnumerator CheckForSearchingModels(string value)
        {
            yield return new WaitForSeconds(0.4f);
            if (value == searchValueString)
            {
                UpdateModelsData();
            }
        }

        void UpdateModelsData()
        {
            offset = 0;
            UpdateModelPanel(true);
        }

        void LoadMoreModels()
        {
            offset++;
            UpdateModelPanel(false);
        }

        void ProcessingLoadMoreBtn(bool isActive, bool isLoading)
        {
            if (loadMorePanel != null)
            {
                loadMorePanel.SetActive(isActive);
                loadMorePanel.transform.GetChild(0).gameObject.SetActive(!isLoading);
                loadMorePanel.transform.GetChild(1).gameObject.SetActive(isLoading);
            }
        }

        async void UpdateModelPanel(bool isRenewModelPanel)
        {
            try
            {
                if (string.IsNullOrEmpty(searchValueString))
                {
                    searchValueString = null;
                }
                if (isRenewModelPanel)
                {
                    DestroyAllModels();
                }
                ProcessingLoadMoreBtn(!isRenewModelPanel, true);
                waitingScreen.SetActive(true);
                noDataComponent.SetActive(false);

                string URL = String.Format(APIUrlConfig.GET_MODEL_LIST, searchValueString, offset, limit, type);
                APIResponse<List<List3DModel>> modelResponse = await UnityHttpClient.CallAPI<List<List3DModel>>(URL, UnityWebRequest.kHttpVerbGET);
                waitingScreen.SetActive(false);
                noDataComponent.SetActive(false); 
                 if (isRenewModelPanel)
                {
                    DestroyAllModels();
                }

                if (modelResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    offset = modelResponse.meta.page;
                    StartCoroutine(LoadModelsIntoUI(modelResponse.data, isRenewModelPanel));
                    modelPanelScrollRect.verticalNormalizedPosition = 0f;
                    ProcessingLoadMoreBtn(offset < modelResponse.meta.totalPage - 1, false);
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

        IEnumerator LoadModelsIntoUI(List<List3DModel> models, bool isRenewModelPanel)
        {
            if (isRenewModelPanel)
            {
                DestroyAllModels();
            }
            if (models.Count < 1)
            {
                noDataComponent.SetActive(true);   
                yield break;
            }
            noDataComponent.SetActive(false);   
            foreach (List3DModel model in models)
            {
                InstantiateModel(model, modelPanelObject.transform);
            }
        }

        void DestroyAllModels()
        {
            foreach (Transform child in modelPanelObject.transform) 
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        void InstantiateModel(List3DModel model, Transform parentTransform)
        {
            try
            {
                GameObject modelObject = Instantiate(modelObjectResource) as GameObject; 
                modelObject.transform.SetParent(parentTransform, false);

                string imageURL = APIUrlConfig.DOMAIN_SERVER + model.modelThumbnail;
                RawImage targetImage = modelObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<RawImage>();
                StartCoroutine(UnityHttpClient.LoadRawImageAsync(imageURL, targetImage, (isSuccess) => {
                    if (isSuccess)
                    {
                        modelObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    }
                }));
                modelObject.name = model.modelId.ToString(); 
                modelObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = Helper.FormatString(model.modelName, calculatedSize);
                modelObject.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(() => InteractionUI.Instance.onClickItemModel(model.modelId, model.modelName));        
            }
            catch (Exception e)
            {
                Debug.Log($"error InstantiateModel");
            }
        }
    }
}
