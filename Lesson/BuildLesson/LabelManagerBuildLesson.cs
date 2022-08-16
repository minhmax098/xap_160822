using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.UI;
using EasyUI.Toast;
using System.Threading.Tasks;

namespace BuildLesson 
{
    public class LabelManagerBuildLesson : MonoBehaviour
    {
        private static LabelManagerBuildLesson instance; 
        public static LabelManagerBuildLesson Instance
        {
            get 
            {
                if (instance == null)
                {
                    // centerPosition = Helper.CalculateCentroid(ObjectManager.Instance.OriginObject);
                    instance = FindObjectOfType<LabelManagerBuildLesson>(); 
                }
                return instance; 
            }
        }
        private int calculatedSize = 10;
        public GameObject btnLabel;
        
        private List<Vector3> pointPositions = new List<Vector3>();
        public List<GameObject> listLabelObjects = new List<GameObject>(); 
        public List<GameObject> listLabelObjectsOnEditMode = new List<GameObject>(); 

        private bool isLabelOnEdit = false;
        private bool isShowingLabel = true;

        public bool IsLabelOnEdit { get; set;}
        
        public bool IsShowingLabel 
        {    
            get
            {
                return isShowingLabel;
            }
            set
            {
                Debug.Log("LabelManagerBuildLesson isShowingLabel call"); 
                isShowingLabel = value; 
                btnLabel.GetComponent<Image>().sprite = !isShowingLabel ? Resources.Load<Sprite>(PathConfig.LABEL_UNCLICK_IMAGE) : Resources.Load<Sprite>(PathConfig.LABEL_CLICKED_IMAGE);
            }
        }

        void Start()
        {
            InitUI();
        }

        void InitUI()
        {
            btnLabel = GameObject.Find("BtnLabel");
        }

        public void Update()
        {
            pointPositions.Add(transform.position);
        }
        
        private static Vector3 centerPosition;

        public void updateCenterPosition()
        {
            Debug.Log("After init object: " + ObjectManagerBuildLesson.Instance.OriginObject);
            centerPosition = Helper.CalculateCentroid(ObjectManagerBuildLesson.Instance.OriginObject);
        }

        public Bounds GetParentBound(GameObject parentObject, Vector3 center)
        {
            foreach (Transform child in parentObject.transform)
            {
                center += child.gameObject.GetComponent<Renderer>().bounds.center;
            }
            center /= parentObject.transform.childCount;
            Bounds bounds = new Bounds(center, Vector3.zero);
            foreach(Transform child in parentObject.transform)
            {
                bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
            }
            return bounds;
        }
        
        public async Task SaveCoordinate(int lessonId, int modelId, string labelName, Vector3 coordinates, string level, string subLevel)
        {
            try
            {
                PostModelLabel saveCoordinateRequest = new PostModelLabel();
                saveCoordinateRequest.lessonId = lessonId;
                saveCoordinateRequest.modelId = modelId;
                saveCoordinateRequest.labelName = labelName;
                saveCoordinateRequest.coordinates = Coordinate.InitCoordinate(coordinates);
                saveCoordinateRequest.level = level;
                saveCoordinateRequest.subLevel = subLevel;
                APIResponse<DataCoordinate> coordinateResponse = await UnityHttpClient.CallAPI<DataCoordinate>(APIUrlConfig.POST_CREATE_MODEL_LABEL, UnityWebRequest.kHttpVerbPOST, saveCoordinateRequest);
                if (coordinateResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    Debug.Log("save coordinate sucessfully: " + coordinateResponse.data.labelId);
                    TagHandler.Instance.AddLabelId(coordinateResponse.data.labelId);
                }
            }
            catch (Exception e)
            {   
                Debug.Log("save coordinate fail: " + e.Message);
            }
        }

        public string getIndexGivenGameObject(GameObject rootObject, GameObject targetObject)
        {
            var result = new System.Text.StringBuilder();
            while(targetObject != rootObject)
            {
                result.Insert(0, targetObject.transform.GetSiblingIndex().ToString());
                result.Insert(0, "-");
                targetObject = targetObject.transform.parent.gameObject;
            }
            result.Insert(0, "0");
            return result.ToString();
        }

        public void HandleLabelView(bool currentLabelStatus) 
        {
            IsShowingLabel = currentLabelStatus;
            ShowHideLabels(IsShowingLabel);
            TagHandler.Instance.ShowHideTags(IsShowingLabel);
        }

        private void ShowHideLabels(bool isShowing)
        {
            foreach(GameObject label in listLabelObjects)
            {
                label.SetActive(isShowingLabel);
            }    
        }
    }
}
