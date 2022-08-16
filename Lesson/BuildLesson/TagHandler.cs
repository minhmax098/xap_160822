using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Networking;

namespace BuildLesson
{
    public class TagHandler : MonoBehaviour
    {
        // Tag Handler use to hanlder: - All NormalTag and One 2DTag(with the index)
        private static TagHandler instance;
        public static TagHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TagHandler>();
                }
                return instance;
            }
        }
        public List<GameObject> addedTags = new List<GameObject>();
        public List<int> labelIds = new List<int>();
        public GameObject labelEditTag;
        public int currentEditingIdx = -1; 
        private Vector2 rootLabel2D;
        private Vector3 originLabelScale;
        public List<Vector3> positionOriginLabel = new List<Vector3>();

        void Update()
        {
            OnMove();
        }

        async void LoadLessonDetail(int lessonId)
        {
            try
            {
                APIResponse<LessonDetail[]> lessonDetailResponse = await UnityHttpClient.CallAPI<LessonDetail[]>(String.Format(APIUrlConfig.GET_LESSON_BY_ID, lessonId), UnityWebRequest.kHttpVerbGET);
                if (lessonDetailResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
                {
                    StaticLesson.SetValueForStaticLesson(lessonDetailResponse.data[0]);
                }
                else
                {
                    throw new Exception(lessonDetailResponse.message);
                }
            }
            catch (Exception ex)
            {
                
            }
        }

        public void GetTagFromServer(int lessonId, GameObject rootObject)
        {
            LoadLessonDetail(lessonId);
            // From StaticLesson, get the label info and generate the labels, fill into TagHandler
            // AddedTags, LabelIds, positionOriginLabel
            Debug.Log("Loading labels from server " + lessonId);
            Vector3 centerPosition = Helper.CalculateCentroid(ObjectManagerBuildLesson.Instance.OriginObject);
            Debug.Log("Loading labels from server COORDINATE " + centerPosition.x);
            foreach(Label lb in StaticLesson.ListLabel)
            {
                Debug.Log("Loading labels from server " + lb.labelName);
                // Call AddLabels
                Debug.Log("Loading labels from server " + getGameObjectByLevel(lb.level, rootObject).name);
                AddLabels(lb, centerPosition, rootObject);
            }
        }

        private void AddLabels(Label label, Vector3 rootPosition, GameObject rootObject)
        {
            Debug.Log("Loading labels from server, ADDING LABEL ...");
            GameObject currentObject = getGameObjectByLevel(label.level, rootObject);
            Vector3 hitPoint = new Vector3(label.coordinates.x, label.coordinates.y, label.coordinates.z);
            GameObject labelObject = Instantiate(Resources.Load(PathConfig.MODEL_TAG) as GameObject); // Normal label

            labelObject.transform.SetParent(currentObject.transform, false);
            labelObject.transform.localScale *=  ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x * ObjectManagerBuildLesson.Instance.OriginScale.x;
            labelObject.transform.GetChild(1).localScale = labelObject.transform.GetChild(1).localScale / ObjectManagerBuildLesson.Instance.FactorScaleInitial;

            GameObject sphere = labelObject.transform.GetChild(2).gameObject;
            var spereRenderer = sphere.GetComponent<Renderer>(); 
            spereRenderer.material.SetColor("_Color", Color.red);
            sphere.transform.localScale = new Vector3 (10f, 10f, 10f) * ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x;
            sphere.transform.position = hitPoint; // Global variable

            GameObject line = labelObject.transform.GetChild(0).gameObject; 
            GameObject labelName = labelObject.transform.GetChild(1).gameObject;  
            labelName.transform.GetChild(0).GetComponent<TextMeshPro>().text = Helper.FormatString(label.labelName, 10);
            Bounds parentBounds = Helper.GetParentBound( ObjectManagerBuildLesson.Instance.OriginObject, rootPosition);
            Bounds objectBounds = currentObject.GetComponent<Renderer>().bounds;

            Vector3 dir = ObjectManagerBuildLesson.Instance.OriginObject.transform.InverseTransformPoint(hitPoint) - ObjectManagerBuildLesson.Instance.OriginObject.transform.InverseTransformPoint(rootPosition);    
            labelName.transform.localPosition = parentBounds.max.magnitude * dir.normalized / ObjectManagerBuildLesson.Instance.OriginScale.x;

            line.GetComponent<LineRenderer>().useWorldSpace = false;
            line.GetComponent<LineRenderer>().widthMultiplier = 0.25f * ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x;
            line.GetComponent<LineRenderer>().SetVertexCount(2);
            line.GetComponent<LineRenderer>().SetPosition(0, labelObject.transform.InverseTransformPoint(hitPoint));
            line.GetComponent<LineRenderer>().SetPosition(1, labelObject.transform.InverseTransformPoint(labelName.transform.position));
            line.GetComponent<LineRenderer>().SetColors(Color.black, Color.black);
            line.GetComponent<LineRenderer>().SetWidth(0.03f, 0.03f);

            this.AddTag(labelObject);
            this.updateCurrentEditingIdx(Helper.FormatString(label.labelName, 10));
            this.positionOriginLabel.Add(labelName.transform.localPosition);
            this.AddLabelId(label.labelId);
        }

        private GameObject getGameObjectByLevel(string level, GameObject rootObject)
        {
            Debug.Log("Loading labels from serrver calling getGameObjectByLevel level " + level);
            Debug.Log("Loading labels from serrver calling getGameObjectByLevel " + rootObject.name);
            // Function to get the GameObject given by the parentId
            int[] levels = Array.ConvertAll(level.Split('-'), s => int.Parse(s));
            // For loop with index 
            for(int i=1; i < levels.Length; i++)
            {
                Debug.Log("Loading labels from serrver calling getGameObjectByLevel label index: " + levels[i]);
                rootObject = rootObject.transform.GetChild(levels[i]).gameObject;
            }
            Debug.Log("Loading labels from serrver calling getGameObjectByLevel " + rootObject.name);
            return rootObject;
        }

        // Function called along with the setter of labelEditTag
        public void updateCurrentEditingIdx(string organName)
        {
            // Update the labelEditTag corresponding to current selected label
            // Use both labelEditTag and the currentEditingIdx to handle
            // LabelEditTag can directly pass into the class instance 
            // Find the index of corresponding normal label that the labelEditTag belong to
            foreach(GameObject tag in addedTags)
            {
                Debug.Log("Traversing tags " + tag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshPro>().text);
                if (tag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TextMeshPro>().text == organName)
                {  
                    Debug.Log("Traversing hit: " + organName); // The currentEditingIdx is updated 
                    currentEditingIdx = addedTags.IndexOf(tag);
                }
            }
        }

        public void deleteCurrentLabel()
        {
            // Remove the Label as well as the correspondingId 
            GameObject x = addedTags[currentEditingIdx];
            Destroy(x);
            addedTags.RemoveAt(currentEditingIdx);
            labelIds.RemoveAt(currentEditingIdx);
            currentEditingIdx = -1;
        }

        public void ResetEditLabelIndex()
        {
            currentEditingIdx = -1;
        }

        public void AddLabelId(int labelId)
        {
            // The same shape as addedTags, will be called after the created request successful 
            labelIds.Add(labelId);
            Debug.Log("Add audio add LABELID, length: " + labelIds.Count);
        }

        public void AddTag(GameObject tag)
        {
            // Add Tag mean add NormalTag, happened when created a new label, update the currentEditingIndx
            addedTags.Add(tag);
            originLabelScale = tag.transform.localScale;
            currentEditingIdx = addedTags.Count - 1;
        }

        public void DeleteTags()
        {
            // Reset the value 
            addedTags.Clear();
            currentEditingIdx = -1;
        }

        public void OnMove()
        {
            foreach (GameObject addedTag in addedTags)
            {
                if (addedTag != null)
                {
                    DenoteTag(addedTag);
                    MoveTag(addedTag);
                }
                // Handler the display of the corresponding 2Dlabel 
                if (currentEditingIdx != -1)
                {
                    Update2DLabelPosition();
                }
            }
        }

        public void Update2DLabelPosition()
        {
            // Based on the currentEditingIdx, get position of the NormalLabel
            rootLabel2D = Camera.main.WorldToScreenPoint(addedTags[currentEditingIdx].transform.GetChild(1).gameObject.transform.position);
            labelEditTag.transform.position = rootLabel2D;
        }

        public void DenoteTag(GameObject addedTag)
        {
            if (addedTag.transform.GetChild(1).transform.position.z > 1f)
            {
                addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = false;
                addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = false;
                addedTag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                addedTag.transform.GetChild(0).gameObject.GetComponent<LineRenderer>().enabled = true;
                addedTag.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = true;
                addedTag.transform.GetChild(1).GetChild(0).gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }

        public void MoveTag(GameObject addedTag)
        {
            addedTag.transform.GetChild(1).transform.LookAt(addedTag.transform.GetChild(1).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
            addedTag.transform.GetChild(1).GetChild(0).transform.LookAt(addedTag.transform.GetChild(1).GetChild(0).position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
        
        public void ShowHideCurrentLabel(bool showLabel)
        {
            Debug.Log("Check current Label index: " + currentEditingIdx);
            if (currentEditingIdx != -1)
            {
                addedTags[currentEditingIdx].transform.GetChild(1).gameObject.SetActive(showLabel);
            }
        }

        public void ShowHideAllLabels(bool showLabel)
        {
            // show the label at currentIdx
            for (int i=0; i < addedTags.Count; i++)
            {
                if ( i != currentEditingIdx)
                {
                    addedTags[i].SetActive(showLabel);
                }
            }
        }

        public void AdjustTag(float scaleFactor)
        {
            for (int i=0; i< addedTags.Count; i++)
            {
                addedTags[i].transform.localScale = originLabelScale / scaleFactor;
                addedTags[i].transform.GetChild(1).localPosition = positionOriginLabel[i];
                // ObjectManagerBuildLesson.Instance.OriginScale.x / ObjectManagerBuildLesson.Instance.OriginObject.transform.localScale.x * ObjectManagerBuildLesson.Instance.OriginScale.x
                addedTags[i].transform.GetChild(0).GetComponent<LineRenderer>().SetPosition(1, addedTags[i].transform.GetChild(1).localPosition * 0.9f);
            }
        }

        public void ShowHideTags(bool isShowing)
        {
            foreach(GameObject tag in addedTags)
            {
                tag.SetActive(isShowing);
            }
        }
    }
}

