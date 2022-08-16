using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using System; 
using UnityEngine.SceneManagement;
using TMPro; 
using UnityEngine.Networking;
using System.Text;
using EasyUI.Toast;
using System.Threading.Tasks;

namespace BuildLesson
{
    public class TouchHandler : MonoBehaviour
    {
        private static TouchHandler instance; 
        public static TouchHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TouchHandler>();
                }
                return instance; 
            }
        }
        public static event Action onResetStatusFeature; 
        public static event Action<GameObject> onSelectChildObject; 

        public GameObject UIComponent;
        const float ROTATION_RATE = 0.08f;
        const float LONG_TOUCH_THRESHOLD = 1f; 
        const float ROTATION_SPEED = 0.5f; 
        float touchDuration = 0.0f; 
        Touch touch; 
        Touch touchZero; 
        Touch touchOne; 
        float originDelta; 
        Vector3 originScale;

        Vector3 originLabelScale = new Vector3(1f, 1f, 1f);
        Vector3 originLabelTagScale = new Vector3(7f, 1f, 1f);
        Vector3 originLineScale = new Vector3(1f, 1f, 1f); 
        Vector3 originScaleSelected;
        bool isMovingByLongTouch = false; 
        bool isLongTouch = false;
        float currentDelta;
        float scaleFactor;

        private GameObject currentSelectedObject; 
        private GameObject recentSelectedObject;
        private Vector3 centerPosition;
        private Vector3 mOffset; 
        private float mZCoord;
        private string currentSelectedLabelOrganName;
        private Vector3 hitPoint;
        private Vector2 hitPoint2D;

        // Panel DeleteTag
        public GameObject panelPopUpDeleteLabel;
        public Button btnExitPopupDeleteLabel; 
        public Button btnDeleteLabel; 
        public Button btnCancelDeleteLabel;

        // Panel AddActivities: addVideo, addAudio
        public GameObject panelAddActivities;
        public Button btnAddAudioLabel;
        public Button btnAddVideoLabel;
        public Button btnCancelAddActivities;

        private int calculatedSize = 10;
        GameObject labelEditObject;

        void Start()
        {
            InitUILabel();
        }

        void InitUILabel()
        {
            labelEditObject = Instantiate(Resources.Load(PathConfig.MODEL_TAG_EDIT) as GameObject);
            // Add functional to the buttons of labelEditObject
            labelEditObject.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => 
            {
                InputField inpField = labelEditObject.transform.GetChild(0).gameObject.GetComponent<InputField>();
                inpField.ActivateInputField();
                inpField.Select();
                inpField.onEndEdit.AddListener(delegate{OnEndEditLabel(inpField);});
            });
            // Button DeleteLabel
            labelEditObject.transform.GetChild(4).gameObject.GetComponent<Button>().onClick.AddListener(HandlerDeleteTag);
            btnDeleteLabel.onClick.AddListener(() => onClickYes(TagHandler.Instance.labelIds[TagHandler.Instance.currentEditingIdx])); 
            btnExitPopupDeleteLabel.onClick.AddListener(ExitPopupDeleteLabel);
            btnCancelDeleteLabel.onClick.AddListener(ExitPopupDeleteLabel);
            // Button AddLabel: addAudio, addVideo for Label
            labelEditObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => StartCoroutine(HandlerAddTag()));
            btnCancelAddActivities.onClick.AddListener(CancelAddActivities);
        }

        public void Update()
        {
            Debug.Log("Touch Handler Check: currentSelectedObject " + currentSelectedObject + "; IsLabelOnEdit: " + LabelManagerBuildLesson.Instance.IsLabelOnEdit);
        }
        
        public void HandleTouchInteraction()    
        {
            if (ObjectManagerBuildLesson.Instance.CurrentObject == null)
            {
                return;
            }
            if (Input.touchCount == 1)
            {
                touch = Input.GetTouch(0); 
                if (touch.tapCount == 1)
                {
                    HandleSingleTouch(touch);
                }
                else if (touch.tapCount == 2)
                {
                    touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Ended)
                    {
                        HandleDoupleTouch(touch);
                    }
                }
            }
            // Add HandleSimultaneousThreeTouch
            if (Input.touchCount == 3)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved && Input.GetTouch(2).phase == TouchPhase.Moved)
                {
                    HandleSimultaneousThreeTouch(Input.GetTouch(1));
                }
            }
            // Add HandleSimultaneousTouch
            else if (Input.touchCount == 2)
            {
                touchZero = Input.GetTouch(0);
                touchOne = Input.GetTouch(1);
                HandleSimultaneousTouch(touchOne, touchZero);
            }
        }

        private void HandleSingleTouch(Touch touch)
        {
            switch (touch.phase)
            {
                case TouchPhase.Began: 
                {
                    if (LabelManagerBuildLesson.Instance.IsLabelOnEdit)
                    {   
                        Debug.Log("Show normal label");
                        Debug.Log("Check: " + currentSelectedObject);
                        if (currentSelectedObject != null)
                        {
                            Debug.Log("message: ");
                            LabelManagerBuildLesson.Instance.IsLabelOnEdit = !LabelManagerBuildLesson.Instance.IsLabelOnEdit;
                            TagHandler.Instance.labelEditTag.SetActive(false);
                            TagHandler.Instance.ShowHideCurrentLabel(true);
                        }
                    }

                    if (!LabelManagerBuildLesson.Instance.IsLabelOnEdit)
                    {
                        Debug.Log("Hit: IsLabelOnEdit" + LabelManagerBuildLesson.Instance.IsLabelOnEdit);
                        // Construct a ray from the current touch coordinates
                        Ray ray = Camera.main.ScreenPointToRay(touch.position);
                        RaycastHit hit; 
                        if (Physics.Raycast(ray, out hit))
                        {
                            Debug.Log("Hit: " + hit.transform.gameObject.name);
                            if (hit.transform.gameObject.tag == TagConfig.labelModel)
                            {
                                Debug.Log("Hit the Normal label: ");
                                // Get the text inside the hit object 
                                string hitLabelText = hit.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>().text;
                                TagHandler.Instance.labelEditTag.transform.GetChild(0).GetComponent<InputField>().text = hitLabelText;
                                TagHandler.Instance.updateCurrentEditingIdx(hitLabelText);
                                TagHandler.Instance.labelEditTag.SetActive(true);
                                TagHandler.Instance.ShowHideCurrentLabel(false);
                                LabelManagerBuildLesson.Instance.IsLabelOnEdit = !LabelManagerBuildLesson.Instance.IsLabelOnEdit;
                            }
                        }
                    } 
                    isMovingByLongTouch = true;
                    break; 
                }
                case TouchPhase.Stationary: 
                {
                    Debug.Log("Stationary state");
                    Debug.Log("isMovingByLongTouch: " + isMovingByLongTouch + "!isLongTouch" + !isLongTouch);
                    if (isMovingByLongTouch && !isLongTouch)
                    {
                        touchDuration += Time.deltaTime;
                        if (touchDuration > LONG_TOUCH_THRESHOLD)
                        {
                            OnLongTouchInvoke();
                            Debug.Log("OnLongTouchInvoke");
                        }
                    }
                    break;
                }
                case TouchPhase.Moved:
                {
                    Debug.Log("Touch phase move: ");
                    if (isLongTouch)
                    {
                        // Drag(touch, currentSelectedObject);
                    }
                    else
                    {
                        Rotate(touch);
                    }
                    break;
                }
                case TouchPhase.Ended: 
                {
                    ResetLongTouch(); 
                    break;
                }
                case TouchPhase.Canceled: 
                {
                    ResetLongTouch(); 
                    break;
                }
            }
        }

        private void Rotate(Touch touch)
        {
            ObjectManagerBuildLesson.Instance.OriginObject.transform.Rotate(touch.deltaPosition.y * ROTATION_RATE, -touch.deltaPosition.x * ROTATION_RATE, 0, Space.World);
        }

        void OnLongTouchInvoke()
        {
            var rs = GetChildOrganOnTouchByTag(touch.position);
            currentSelectedObject = rs.Item1; 
            recentSelectedObject = rs.Item1;
            hitPoint = rs.Item2;
            isMovingByLongTouch = currentSelectedObject != null; 
            if (currentSelectedObject != null)
            {
                Debug.Log("Current selected object: " + currentSelectedObject.name);
                // Hide all 3D labels
                TagHandler.Instance.ResetEditLabelIndex();
                TagHandler.Instance.ShowHideAllLabels(false);
                // Project 3D point into the camera image plane
                hitPoint2D = Camera.main.WorldToScreenPoint(hitPoint);
                // Add label Icon 
                GameObject addLabelIcon = Instantiate(Resources.Load(PathConfig.MODEL_ADD_LABEL) as GameObject);
                addLabelIcon.transform.parent = UIComponent.transform;
                addLabelIcon.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
                addLabelIcon.transform.position = hitPoint2D;
                addLabelIcon.transform.GetComponent<Button>().onClick.AddListener(() => DisplayLabel2D(addLabelIcon, hitPoint2D));
            }
            isLongTouch = true;
        }

        void DisplayLabel2D(GameObject destroyObj, Vector2 hitPoint2D)
        {
            // Create label 2D
            GameObject label2D = Instantiate(Resources.Load(PathConfig.MODEL_TAG_CONFIG) as GameObject);
            label2D.tag = "Tag2D";
            label2D.transform.parent = UIComponent.transform;
            label2D.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
            label2D.transform.position = hitPoint2D;
            label2D.transform.GetChild(0).gameObject.GetComponent<InputField>().onValueChanged.AddListener(ValidateCreatedLabel);
            label2D.transform.GetChild(0).gameObject.GetComponent<InputField>().ActivateInputField();
            label2D.transform.GetChild(0).gameObject.GetComponent<InputField>().Select();
            label2D.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
            label2D.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(() => OnCreatedLabel(label2D));
            label2D.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => CancelCreatedLabel(label2D));
            Destroy(destroyObj);  

            void ValidateCreatedLabel(string data)
            {
                IsValidCreatedLabel(data);
            }

            bool IsValidCreatedLabel(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    Debug.Log("Empty data: ");
                    label2D.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;
                    return false;
                }
                else
                {
                    label2D.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
                    return true;
                }  
            }      
        }
       
        void ResetLongTouch()
        {
            touchDuration = 0f;
            isLongTouch = false;
            isMovingByLongTouch = false;
        }

        private void Drag(Touch touch, GameObject obj)
        {
            if (obj != null)
            {
                obj.transform.position = Helper.GetTouchPositionAsWorldPoint(touch) + mOffset;
            }
        }

        IEnumerator HightLightObject()
        {
            originScaleSelected = currentSelectedObject.transform.localScale;
            currentSelectedObject.transform.localScale = originScaleSelected * 1.5f;
            yield return new WaitForSeconds(0.12f);
            currentSelectedObject.transform.localScale = originScaleSelected;
        }

        private (GameObject, Vector3) GetChildOrganOnTouchByTag(Vector3 position)
        {
            Ray ray = Camera.main.ScreenPointToRay(position);
            RaycastHit hit; 
            Debug.Log("Get Child: ");
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform.root.gameObject.tag == TagConfig.ORGAN_TAG)
                {
                    Debug.Log("Get Child later: ");
                    if (hit.collider.transform.parent == ObjectManagerBuildLesson.Instance.CurrentObject.transform)
                    {
                        return (hit.collider.gameObject, hit.point);
                    }
                }
            }
            return (null, new Vector3(0f, 0f, 0f));
        }

        private void HandleDoupleTouch(Touch touch)
        {
            GameObject selectedObject = Helper.GetChildOrganOnTouchByTag2(touch.position);
            if (selectedObject == null || selectedObject == ObjectManagerBuildLesson.Instance.OriginObject || ObjectManagerBuildLesson.Instance.CurrentObject.transform.childCount < 1)
            {
                return;
            }
            onSelectChildObject?.Invoke(selectedObject);
        }

        void OnCreatedLabel(GameObject destroyedObj)
        {
            LabelManagerBuildLesson.Instance.IsLabelOnEdit = !LabelManagerBuildLesson.Instance.IsLabelOnEdit;            
            GameObject labelObject = Instantiate(Resources.Load(PathConfig.MODEL_TAG) as GameObject); // Normal label
            labelObject.transform.localScale *=  ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x * ObjectManagerBuildLesson.Instance.OriginScale.x;                        
            labelObject.transform.SetParent(recentSelectedObject.transform, false);            
            centerPosition = Helper.CalculateCentroid(ObjectManagerBuildLesson.Instance.OriginObject);            
            currentSelectedLabelOrganName = destroyedObj.transform.GetChild(0).gameObject.GetComponent<InputField>().text;
            SetLabel(currentSelectedLabelOrganName, hitPoint, recentSelectedObject, ObjectManagerBuildLesson.Instance.OriginObject, centerPosition, labelObject, labelEditObject);            
            Debug.Log("Hit point 3D in world pos: " + hitPoint.x + ", " + hitPoint.y + ", " + hitPoint.z);            
            Destroy(destroyedObj);
            // Save label info to the server, prepare data            
            string level = LabelManagerBuildLesson.Instance.getIndexGivenGameObject(ObjectManagerBuildLesson.Instance.OriginObject, recentSelectedObject); 
            string subLevel = "0-1";           
            LabelManagerBuildLesson.Instance.SaveCoordinate(LessonManager.lessonId, ModelStoreManager.modelId, currentSelectedLabelOrganName, hitPoint, level, subLevel);
        }

        void CancelCreatedLabel(GameObject destroyObj)
        {
            Destroy(destroyObj);
        }

        void OnEndEditLabel(InputField input)
        {
            if (input.text.Length > 0) 
            {
                Debug.Log("Text has been entered: " + input.text);
                // Change the text inside the corresponding counter part
                TagHandler.Instance.addedTags[TagHandler.Instance.currentEditingIdx].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshPro>().text = Helper.ShortString(input.text, 10);
                // Save the tag by calling API
            }
        }

        private void SetLabel(string name, Vector3 hitpoint, GameObject currentObject, GameObject parentObject, Vector3 rootPosition, GameObject label, GameObject editLabel)
        {
            Debug.Log("set label: ");
            GameObject sphere = label.transform.GetChild(2).gameObject;
            var spereRenderer = sphere.GetComponent<Renderer>(); 
            spereRenderer.material.SetColor("_Color", Color.red);

            Debug.Log("Current selected object: " + currentObject.name);
            Debug.Log("OriginScale: " + ObjectManagerBuildLesson.Instance.OriginScale.x);
            Debug.Log("localScale: " + ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x);
            
            // Consider, may be fixed the local scale of sphere is 10, then localScale *= localScale * ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x
            sphere.transform.localScale = new Vector3 (10f, 10f, 10f) * ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x;
            sphere.transform.position = hitPoint; // Global variable

            GameObject line = label.transform.GetChild(0).gameObject; 
            GameObject labelName = label.transform.GetChild(1).gameObject;  
            labelName.transform.GetChild(0).GetComponent<TextMeshPro>().text = Helper.FormatString(name, calculatedSize);           
            Bounds parentBounds = Helper.GetParentBound(parentObject, rootPosition);
            Bounds objectBounds = currentObject.GetComponent<Renderer>().bounds;

            // Vector3 dir = hitPoint - rootPosition; 
            Vector3 dir = parentObject.transform.InverseTransformPoint(hitPoint) - parentObject.transform.InverseTransformPoint(rootPosition);    
            Debug.Log("Parent bounds magnitude : " + parentBounds.max.magnitude);
            Debug.Log("Originscale: " + ObjectManagerBuildLesson.Instance.OriginScale.x);
            Debug.Log("Localscale X: " + ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x);
            labelName.transform.localPosition = parentBounds.max.magnitude * dir.normalized / ObjectManagerBuildLesson.Instance.OriginScale.x;

            line.GetComponent<LineRenderer>().useWorldSpace = false;
            line.GetComponent<LineRenderer>().widthMultiplier = 0.25f * parentObject.transform.localScale.x;  // 0.2 -> 0.05 then 0.02 -> 0.005
            line.GetComponent<LineRenderer>().SetVertexCount(2);
            line.GetComponent<LineRenderer>().SetPosition(0, label.transform.InverseTransformPoint(hitPoint));
            line.GetComponent<LineRenderer>().SetPosition(1, label.transform.InverseTransformPoint(labelName.transform.position));
            line.GetComponent<LineRenderer>().SetColors(Color.black, Color.black);
            line.GetComponent<LineRenderer>().SetWidth(0.03f, 0.03f);

            // Set position and the text inside editLabel
            editLabel.transform.GetChild(0).GetComponent<InputField>().text = name;
            Vector2 endPoint = Camera.main.WorldToScreenPoint(labelName.transform.position);
            editLabel.transform.parent = UIComponent.transform;
            editLabel.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
            editLabel.transform.position = endPoint;
            // Update TagHandler, add normal Label into the TagHander 
            TagHandler.Instance.AddTag(label);
            TagHandler.Instance.updateCurrentEditingIdx(Helper.FormatString(name, calculatedSize));
            TagHandler.Instance.positionOriginLabel.Add(labelName.transform.localPosition);

            // Also update the counterpart
            if (TagHandler.Instance.labelEditTag == null) 
            {
                TagHandler.Instance.labelEditTag = editLabel;
            }  
            TagHandler.Instance.ShowHideCurrentLabel(false);
        }

        // API call to delete label by Id
        void onClickYes(int labelId)
        {
            Debug.Log("Delete label Id: " + labelId);
            // StartCoroutine(DeleteLabel(labelId));
            DeleteLabel(labelId);
            TagHandler.Instance.deleteCurrentLabel();
            TagHandler.Instance.labelEditTag.SetActive(false);
            panelPopUpDeleteLabel.SetActive(false);
        }

        // Handle DeleteTag
        void HandlerDeleteTag()
        {
            panelPopUpDeleteLabel.SetActive(true);
        }

        void ExitPopupDeleteLabel()
        {
            panelPopUpDeleteLabel.SetActive(false);
        }

        // Handle AddTag: addAudio, addVideo
        IEnumerator HandlerAddTag()
        {   
            panelAddActivities.SetActive(true);
            btnAddAudioLabel.onClick.AddListener(AddAudioLabel);
            btnAddVideoLabel.onClick.AddListener(AddVideoLabel);
            Debug.Log("Handler Add Tag: ");
            yield return null;
        }
        void AddAudioLabel()
        {
            panelAddActivities.SetActive(false);
            ListItemsManager.Instance.panelAddAudio.SetActive(true);
            ListItemsManager.Instance.panelAddAudio.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = TagHandler.Instance.labelEditTag.transform.GetChild(0).GetComponent<InputField>().text;
        }
        void AddVideoLabel()
        {
            panelAddActivities.SetActive(false);
            ListItemsManager.Instance.panelAddVideo.SetActive(true);
            ListItemsManager.Instance.panelAddVideo.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = TagHandler.Instance.labelEditTag.transform.GetChild(0).GetComponent<InputField>().text;
        }
        void CancelAddActivities()
        {
            panelAddActivities.SetActive(false);
        }

        // Zoom in, zoom out
        private void HandleSimultaneousTouch(Touch touchZero, Touch touchOne)
        {
            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                originDelta = Vector2.Distance(touchZero.position, touchOne.position);
                originScale = ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale;
            }
            else if (touchZero.phase == TouchPhase.Moved || touchOne.phase == TouchPhase.Moved)
            {
                currentDelta = Vector2.Distance(touchZero.position, touchOne.position);
                scaleFactor = currentDelta / originDelta;
                ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale = originScale * scaleFactor;
                // Adjust label size: 
                TagHandler.Instance.AdjustTag(scaleFactor);
                Debug.Log("Scale factor: " + scaleFactor);
            }
        }

        private void HandleSimultaneousThreeTouch(Touch touch)
        {
            Drag(touch, ObjectManagerBuildLesson.Instance.OriginObject);
        }
        
        // IEnumerator DeleteLabel(int labelId)
        // {
        //     Debug.Log("Trigger delete label: ");
        //     UnityWebRequest webRequest = UnityWebRequest.Delete(String.Format(APIUrlConfig.DeleteLabel, labelId));
        //     webRequest.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));
        //     yield return webRequest.SendWebRequest();
        //     if (webRequest.isNetworkError || webRequest.isHttpError)
        //     {
        //         Debug.Log("An error has occur");
        //         Debug.Log(webRequest.error);
        //     }
        //     else
        //     {
        //         if (webRequest.isDone)
        //         {
        //             Debug.Log("Label delete!");
        //         }
        //     }
        // }
        public async void DeleteLabel(int labelId)
        {
            try
            {

            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}

