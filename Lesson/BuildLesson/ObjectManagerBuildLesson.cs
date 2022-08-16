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
using UnityEngine.Video;

namespace BuildLesson
{
public class ObjectManagerBuildLesson : MonoBehaviour
{
    private static ObjectManagerBuildLesson instance;
    public static ObjectManagerBuildLesson Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ObjectManagerBuildLesson>();
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

    public static event Action onInitOrganSuccessfully;
    public static event Action<string> onChangeCurrentObject;
    public static event Action<GetLessonDetail> onReadyGetLessonDetail;
    public static event Action onResetObject;

    public OrganInfor OriginOrganData { get; set; }
    public Material OriginOrganMaterial { get; set; }
    public GameObject OriginObject { get; set; }
    public List<Vector3> ListchildrenOfOriginPosition { get; set; }
    public GameObject CurrentObject { get; set; }
    public Vector3 OriginPosition { get; set; }
    public Quaternion OriginRotation { get; set; }
    public Vector3 OriginScale { get; set; }

    private GameObject objectInstance;
    private string lessonId;
    private static string API_KEY;
    private int modelId;

    public void LoadDataOrgan()
    {
        // OriginOrganData = new OrganInfor("", "Brain_Demo", "");
    }
    
    public void InitOriginalExperience()
    {
        StartCoroutine(IEDownloadModel());
        // ObjectManager.Instance.LoadObjectAtRunTime(APIUrlConfig.DOMAIN_SERVER + StaticLesson.ModelFile);
        // LabelManagerBuildLesson.Instance.updateCenterPosition();
    }
    
    public void ScaleObjectWithBound(GameObject objectInstance)
    {
        boundOriginObject = Helper.CalculateBounds(objectInstance);
        FactorScaleInitial = Mathf.Min(Mathf.Min(X_SIZE_BOUND / boundOriginObject.size.x, Y_SIZE_BOUND / boundOriginObject.size.y), Z_SIZE_BOUND / boundOriginObject.size.z);
        objectInstance.transform.localScale = objectInstance.transform.localScale * FactorScaleInitial;
    }

    public void SetCollider(GameObject objectInstance)
    {
        var transforms = objectInstance.GetComponentsInChildren<Transform>();
        if (transforms.Length <= 0)
        {
            return;
        }
        foreach (var item in transforms)
        {
            item.gameObject.AddComponent<MeshCollider>();
            // item.tag = TagConfig.;
        }
    }

    public void InitObject(GameObject newObject)
    {   
        Debug.Log("Init object: " + newObject.name);
        this.OriginObject = newObject;
        this.OriginObject.name = SaveLesson.lessonTitle;
        // OriginOrganMaterial = OriginObject.GetComponent<Renderer>().materials[0];
        this.OriginOrganMaterial = OriginObject.GetComponentInChildren<Renderer>().materials[0];
        ChangeCurrentObject(OriginObject);
        
        this.OriginPosition = OriginObject.transform.position;
        this.OriginRotation = OriginObject.transform.rotation;
        this.OriginScale = OriginObject.transform.localScale;

        ScaleObjectWithBound(OriginObject);    
        SetCollider(OriginObject);

        onInitOrganSuccessfully?.Invoke();
        onReadyModel?.Invoke();
    }

    public void ChangeCurrentObject(GameObject newGameObject)
    {
        this.CurrentObject = newGameObject;
        this.ListchildrenOfOriginPosition = Helper.GetListchildrenOfOriginPosition(CurrentObject);
        onChangeCurrentObject?.Invoke(CurrentObject.name);
    }

    public void Instantiate3DObject()
    {
        OriginObject.transform.position = OriginPosition;
        OriginObject.transform.rotation = OriginRotation;
        OriginObject.transform.localScale = OriginScale;
        OriginObject.SetActive(true);
    }

    // public void Init3DObject
    private IEnumerator IEDownloadModel()
    {
        API_KEY = PlayerPrefs.GetString("user_token");
        Debug.Log(modelId);
        using (var uwr = UnityWebRequest.Get(APIUrlConfig.DOMAIN_SERVER + $"models/get3DModelDetail/{SaveLesson.modelId}"))
        {
            uwr.SetRequestHeader("Authorization", API_KEY);

            var operation = uwr.SendWebRequest();

            while (!operation.isDone)
            {
                yield return null;
            }

            if (uwr.downloadHandler.text == "Unauthorized")
            {
                Debug.Log(uwr.downloadHandler.text);
                yield break;
            }
            string str = System.Text.Encoding.UTF8.GetString(uwr.downloadHandler.data);
            Debug.Log(str);
            Response response = JsonUtility.FromJson<Response>(str);

            ModelData data = response.data[0];
            Debug.Log("minhlh17 model data: " + data.modelFile);
            DownloadFile(APIUrlConfig.DOMAIN_SERVER + data.modelFile);
        }
    }
    
    public void DownloadFile(string url)
    {
        Debug.Log("download file: " + url);
        using var client = new WebClient();
        client.Headers[HttpRequestHeader.Authorization] = PlayerPrefs.GetString("user_token");
        // client.Headers.Add("Authentication-Token", PlayerPrefs.GetString("user_token"));
        var lastIndex = url.LastIndexOf("/", StringComparison.Ordinal);
        var modelName = url.Remove(0, lastIndex + 1);
        var loaded = false;
        Debug.Log(modelName);

        client.DownloadProgressChanged +=
            (sender, args) =>
            {
                Debug.Log($"Percent:{args.ProgressPercentage}");

                if (args.ProgressPercentage == 100 &&
                    !loaded)
                {
                    loaded = true;

                    Debug.Log($"path: {Application.persistentDataPath}/{modelName}");

                    AssetLoader.LoadModelFromFile($"{Application.persistentDataPath}/{modelName}",
                        x =>
                        {
                            // x = x.RootGameObject.transform.GetChild(0);
                            var cam = Camera.main;

                            if (cam != null)
                            {
                                x.RootGameObject.transform.SetParent(cam.transform);
                            }

                            var render = x.RootGameObject.GetComponentsInChildren<MeshRenderer>();

                            foreach (var mrc in x.MaterialRenderers.Values.SelectMany(y => y))
                            {
                                foreach (var r in render)
                                {
                                    if (r.name != mrc.Renderer.name)
                                    {
                                        continue;
                                    }

                                    r.materials = mrc.Renderer.materials;
                                    break;
                                }
                            }

                            x.RootGameObject.transform.localPosition = Vector3.zero;
                            x.RootGameObject.transform.localRotation = Quaternion.Euler(Vector3.up * 180f);

                            x.RootGameObject.AddComponent<Rotate>();

                            if (x.RootGameObject.transform.parent != null)
                            {
                                x.RootGameObject.transform.SetParent(null);
                            }

                            DontDestroyOnLoad(x.RootGameObject);
                            GameObject prefabMainOrgan = x.RootGameObject.transform.GetChild(0).gameObject as GameObject;
                            Debug.Log("PrefabMainOrgan: " + prefabMainOrgan.name);
                            LoadingEffectManager.Instance.ShowLoadingEffect(false);
                            
                            objectInstance = Instantiate(prefabMainOrgan, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
                            objectInstance.tag = TagConfig.ORGAN_TAG;
                            objectInstance.AddComponent<UnityEngine.MeshRenderer>();
                            InitObject(objectInstance);
                        },
                        x => { },
                        (x, y) => { },
                        x => { },
                        null,
                        ScriptableObject.CreateInstance<AssetLoaderOptions>(),
                        false);
                }
            };

        client.DownloadFileAsync(new Uri(url), $"{Application.persistentDataPath}/{modelName}");
    }
}

    [System.Serializable]
    class Response 
    {
        public long code;
        public string message;
        public ModelData[] data;
    }

    [System.Serializable]
    class ModelData 
    {
        public long modelId;
        public string ModelName;
        public long modelFileId;
        public long thumbnailFileId;
        public string modelThumbnail;
        public string modelFile;
        public int type;
    }
}

