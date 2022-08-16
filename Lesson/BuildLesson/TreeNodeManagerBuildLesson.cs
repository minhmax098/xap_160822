using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Text;

namespace BuildLesson
{
    public class TreeNodeManagerBuildLesson : MonoBehaviour
    {
        private static TreeNodeManagerBuildLesson instance;
        public static TreeNodeManagerBuildLesson Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TreeNodeManagerBuildLesson>();
                }
                return instance;
            }
        }

        public static event Action<string> onClickNodeTree;
        Vector3 positionCamera;
        // UI
        public Transform rootTreeTransform;
        public GameObject txtNameOrgan;

        void OnEnable()
        {
            ObjectManagerBuildLesson.onReadyModel += OnReadyModel;
        }

        void OnDisable()
        {
            ObjectManagerBuildLesson.onReadyModel -= OnReadyModel;
        }
        void Start()
        {
            positionCamera = Camera.main.transform.position;
        }

        void OnReadyModel()
        {    
            txtNameOrgan.GetComponent<Text>().text = Helper.ShortString(SaveLesson.lessonTitle, 20);
            CreateChildNodeUI(SaveLesson.lessonTitle, true);
        }

        public void CreateChildNodeUI(string name, bool isHeaderTree = false)
        {
            GameObject nodePrefab = isHeaderTree ? (Resources.Load(PathConfig.MODEL_ITEM_HEADER_TREE) as GameObject) : (Resources.Load(PathConfig.MODEL_ITEM_BODY_TREE) as GameObject);
            GameObject childNode = Instantiate(nodePrefab);
            childNode.transform.SetParent(rootTreeTransform, false);
            childNode.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = name;
            childNode.name = name;
            childNode.transform.GetComponent<Button>().onClick.AddListener(delegate { OnClickedNodeTree(childNode.name); });
        }
        
        public void DisplaySelectedObject(GameObject selectedObject, GameObject parentObject)
        {
            foreach (Transform child in parentObject.transform)
            {
                if (child.gameObject != selectedObject)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }

        public void RemoveItem(string nameSelectedObject)
        {
            bool isDeleted = false;
            for (int i = 0; i < rootTreeTransform.transform.childCount; i++)
            {
                if (rootTreeTransform.transform.GetChild(i).gameObject.name == nameSelectedObject)
                {
                    isDeleted = true;
                    continue;
                }

                if (isDeleted)
                {
                    Destroy(rootTreeTransform.transform.GetChild(i).gameObject);
                }
            }
        }

        public void OnClickedNodeTree(String nameSelectedObject)
        {
            Debug.Log("quyen nameselected: "+ nameSelectedObject);
            onClickNodeTree?.Invoke(nameSelectedObject);
        }

        public void DisplayAllChildSelectedObject(GameObject seletedObject)
        {
            int childCount = seletedObject.transform.childCount;
            Debug.Log("quyen seletedObject display: " + seletedObject);
            Debug.Log("quyen count display: " + childCount);
            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    if (seletedObject.transform.GetChild(i).gameObject.tag == TagConfig.LABEL_TAG)
                    {
                        seletedObject.SetActive(true);
                        return;
                    }
                    else
                    {
                        DisplayAllChildSelectedObject(seletedObject.transform.GetChild(i).gameObject);
                    }  
                }
            }
            else
            {
                seletedObject.SetActive(true);
                return;
            }
        }

        public void ReDisplayTreeIn3D(GameObject _currentObject)
        {
            List<String> listParentName = new List<String>();
            GetNestParent(_currentObject, listParentName);

            // Clear node tree
            ClearAllNodeTree();

            // Add node tree
            int parentCount = listParentName.Count;
            for (int i = parentCount - 1; i >= 0; i--)
            {
                TreeNodeManagerBuildLesson.Instance.CreateChildNodeUI(listParentName[i]);
            }
        }

        public void GetNestParent(GameObject obj, List<String> listNameParent)
        {            
            if (obj.transform.parent == null)
            {
                return;
            }
            else
            {
                listNameParent.Add(obj.name);
                GetNestParent(obj.transform.parent.gameObject, listNameParent);
            }
        }

        public void ClearAllNodeTree()
        {
            for (int i = 1; i < rootTreeTransform.transform.childCount; i++)
            {
                Destroy(rootTreeTransform.transform.GetChild(i).gameObject);
            }
        }
    }
}
