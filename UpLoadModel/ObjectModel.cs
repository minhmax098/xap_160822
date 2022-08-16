using System.Security;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Net;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TriLibCore;
using System.Linq;
using System.IO;
using Photon.Pun;
using System.Threading.Tasks;
using EasyUI.Toast;

public class ObjectModel : MonoBehaviour
{
    private static ObjectModel instance;
    public static ObjectModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectModel>();
            }
            return instance;
        }
    }

    public const float X_SIZE_BOUND = 2.5f;
    public const float Y_SIZE_BOUND = 2.5f;
    public const float Z_SIZE_BOUND = 2.5f;
    public float FactorScaleInitial { get; set; }
    Bounds boundOriginObject;

    private const float TIME_SCALE_FOR_APPEARANCE = 0.04f;
    public static event Action onReadyModel;
    public static event Action<string> onChangeCurrentObject;
    public Material OriginOrganMaterial { get; set; }
    public GameObject OriginObject { get; set; }
    public List<Vector3> ListchildrenOfOriginPosition = new List<Vector3>();
    public GameObject CurrentObject { get; set; }
    public Vector3 OriginPosition { get; set; }
    public Quaternion OriginRotation { get; set; }
    public Vector3 OriginScale { get; set; }

    public static event Action<GameObject> onLoadedObjectAtRuntime;
    void Start()
    {
        
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object at specified position/rotation in AR mode
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void InstantiateARObject(Vector3 position, Quaternion rotation, bool isHost)
    {
        OriginObject.transform.position = position;
        OriginObject.transform.rotation = rotation;
        if (isHost && (!ARUIManager.Instance.IsStartAR))
        {
            OriginObject.transform.localScale *= ModelConfig.scaleFactorInARMode;
        }
        OriginObject.SetActive(true);
        AddARAnchorToObject();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show zoom up affect for object
    /// </summary>
    public void ShowZoomUpAffectForObject()
    {
        StartCoroutine(Helper.EffectScaleObject(OriginObject, TIME_SCALE_FOR_APPEARANCE, OriginObject.transform.localScale));
    }

    public void Instantiate3DObject()
    {
        OriginObject.transform.position = OriginPosition;
        OriginObject.transform.rotation = OriginRotation;
        OriginObject.transform.localScale = OriginScale;
        OriginObject.SetActive(true);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add ARAnchor component to object
    /// </summary>
    public void AddARAnchorToObject()
    {
        if (OriginObject != null)
        {
            if (OriginObject.GetComponent<ARAnchor>() == null)
            {
                ARAnchor localAnchor = OriginObject.AddComponent<ARAnchor>();
                localAnchor.destroyOnRemoval = false;
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get ARAnchor component from object
    /// </summary>
    /// <returns></returns>
    public ARAnchor GetARAnchorComponent()
    {
        if (OriginObject == null)
        {
            return null;
        }
        if (OriginObject.GetComponent<ARAnchor>() == null)
        {
            AddARAnchorToObject();
        }
        return OriginObject.GetComponent<ARAnchor>();
    }


    public void SetCollider(GameObject objectInstance)
    {
        objectInstance.tag = TagConfig.ORGAN_TAG;
        var transforms = objectInstance.GetComponentsInChildren<Transform>();
        if (transforms.Length <= 0)
        {
            return;
        }

        foreach (var item in transforms)
        {
            item.gameObject.AddComponent<MeshCollider>();
            item.tag = TagConfig.ORGAN;
        }
    }

    public void ScaleObjectWithBound(GameObject objectInstance)
    {
        boundOriginObject = Helper.CalculateBounds(objectInstance);
        FactorScaleInitial = Mathf.Min(Mathf.Min(X_SIZE_BOUND / boundOriginObject.size.x, Y_SIZE_BOUND / boundOriginObject.size.y), Z_SIZE_BOUND / boundOriginObject.size.z);
        objectInstance.transform.localScale = objectInstance.transform.localScale * FactorScaleInitial * 40;
    }

    /// <summary>
    /// Assign organ model to gameobject
    /// </summary>
    public void InitGameObject(GameObject newGameObject)
    {
        OriginObject = newGameObject;
        ScaleObjectWithBound(OriginObject);
        SetCollider(OriginObject);
        OriginScale = OriginObject.transform.localScale;
        ChangeCurrentObject(OriginObject);
        onReadyModel?.Invoke();
    }

    public void ChangeCurrentObject(GameObject newGameObject)
    {
        try
        {
            CurrentObject = newGameObject;
            ListchildrenOfOriginPosition = Helper.GetListOfInitialPositionOfChildren(CurrentObject);
            onChangeCurrentObject?.Invoke(CurrentObject.name);
        }
        catch (Exception e)
        {
            Debug.Log($"error {e.Message}");
        }
    }

    
}
