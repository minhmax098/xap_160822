using System.Collections;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TriLibCore;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using System.Collections.Generic;
using System;
using EasyUI.Toast;

public class UploadModel : MonoBehaviour
{
    public Image imgLoadingFill;
    public GameObject uiBFill;
    public Text txtPercent;
    public Button btnUploadModel3D;
    public Button btnBack;
    public GameObject uiCoat;
    public Text warningFormatFile;
    public static int idModel;
    private bool isCallAPI = false;
    private string[] arrFormatFile = {"FBX", "OBJ"};

    private static UploadModel instance;

    public static UploadModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UploadModel>();
            }
            return instance; 
        }
    }

    private void Start()
    {
        SetEventUI();
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }

    private void Update()
    {
        if(isCallAPI == true && UnityHttpClient.processAPI*2f*100f < 100)
        {
            imgLoadingFill.fillAmount = UnityHttpClient.processAPI * 2f;    
            txtPercent.text=$"{(imgLoadingFill.fillAmount*100f):N0} %";
        }
    }

    public void SetEventUI()
    {
        btnUploadModel3D.onClick.AddListener(HandlerUploadModel);
        btnBack.onClick.AddListener(() => BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name));
    }

    public string GetFormatFile(string path)
    {
        string fullModelName = path.Substring(path.LastIndexOf("/") + 1);
        string formatFile = fullModelName.Substring(fullModelName.LastIndexOf(".") + 1);
        string[] modelName = fullModelName.Split('.');
        return formatFile;
    }

    public void HandlerUploadModel()
    {
        DestroyModel();
        imgLoadingFill.fillAmount = 0f;
        AssetLoaderFilePicker.Create()
            .LoadModelFromFilePickerAsync("load model", 
                x =>
                {
                    Debug.Log("x" + x);
                    if(x == null)
                    {
                        Debug.Log("null");
                    }
                    string path = $"{x.Filename}";
                    Debug.Log("path" + path);
                    string formatFile = GetFormatFile(path).ToUpper();
                    Debug.Log(Array.IndexOf(arrFormatFile, formatFile));
                    if (Array.IndexOf(arrFormatFile, formatFile) < 0)
                    {
                            ReStore();
                            warningFormatFile.enabled = true;     
                    }
                    else
                    {
                        var cam = Camera.main;

                        if (cam != null)
                        {
                            x.RootGameObject.transform.SetParent(cam.transform);
                        }

                        var render = x.RootGameObject.GetComponentsInChildren<MeshRenderer>();

                        foreach (var y in x.MaterialRenderers.Values)
                        {
                            foreach (var mrc in y)
                            {
                                foreach (var r in render)
                                {
                                    if (r.name == mrc.Renderer.name)
                                    {
                                        r.materials = mrc.Renderer.materials;
                                        break;
                                    }
                                }
                            }
                        }

                        x.RootGameObject.tag = "ModelClone";

                        if (x.RootGameObject.transform.parent != null)
                        {
                            x.RootGameObject.transform.SetParent(null);
                        }

                        warningFormatFile.enabled = false;
                        StartCoroutine(HandleUploadModel3D(File.ReadAllBytes(path), path));
                        //UploadModelToServer(File.ReadAllBytes(path), path);
                        DontDestroyOnLoad(x.RootGameObject);   
                    }
                           
                },
                x => { },
                (x, y) => { },
                x => { 
                        if(x == true)
                        {
                            Debug.Log(x);
                            uiCoat.SetActive(true);
                            uiBFill.SetActive(true);
                            txtPercent.text="0%";
                        }
                        else 
                        {
                            ReStore();
                        }
                    },
                x => { 
                    ReStore();
                    warningFormatFile.enabled = true;
                },
                null,
                ScriptableObject.CreateInstance<AssetLoaderOptions>());
    }
    

    async void UploadModelToServer(byte[] fileData, string fileName)
    {
        try 
        {
            Debug.Log("UploadModelToServer");
            ImportModelRequest importModelRequest = new ImportModelRequest();
            importModelRequest.model = fileData;
            isCallAPI = true;
            APIResponse<ResData[]> importModelResponse = await UnityHttpClient.CallAPI<ResData[]>(APIUrlConfig.Upload3DModel, UnityWebRequest.kHttpVerbPOST, importModelRequest);
            Debug.Log("No importModelResponse" + importModelResponse.code);
            isCallAPI = false;
            if (importModelResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {

            }
            else
            {
                throw new Exception(importModelResponse.message);
            }
        }
        catch (Exception exception)
        {
            ReStore();
            Debug.Log("No exception" + exception.Message);   
        }
    }

    public IEnumerator HandleUploadModel3D(byte[] fileData, string fileName)
    {
        var form = new WWWForm();

        form.AddBinaryData("model", fileData, fileName);
        string API_KEY = PlayerPrefs.GetString("user_token");
        using var www = UnityWebRequest.Post("https://api.xrcommunity.org/v1/xap/stores/upload3DModel", form);
        www.SetRequestHeader("Authorization", API_KEY);
        var operation = www.SendWebRequest();   

        while (!operation.isDone)
        {

            imgLoadingFill.fillAmount = operation.progress * 2f;    
            txtPercent.text=$"{(imgLoadingFill.fillAmount*100f):N0} %";
            yield return null;
        }
        
        Debug.Log(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));
        string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
        

        if (www.downloadHandler.text == "Unauthorized" ||
            www.downloadHandler.text.StartsWith("<!DOCTYPE html>"))
        {
            Toast.ShowCommonToast(www.downloadHandler.text, APIUrlConfig.BAD_REQUEST_RESPONSE_CODE);
            yield return new WaitForSeconds(1);
            ReStore();
            yield break;
        }
        else 
        {
            ResUpModel res = JsonUtility.FromJson<ResUpModel>(response);
            Debug.Log(res.ToString());

            if (res != null)
            {
                switch (res.code)
                {
                    case "200":
                        if(res.data[0].file_id != null)
                        {
                            idModel = res.data[0].file_id;
                            ModelStoreManager.InitModelStore(idModel, res.data[0].file_path);
                        } 
                        Toast.ShowCommonToast(res.message, APIUrlConfig.SUCCESS_RESPONSE_CODE);
                        yield return new WaitForSeconds(1f);
                        ReStore();
                        BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.interactiveModel);
                        SceneManager.LoadScene(SceneConfig.interactiveModel);
                        break;
                    case "400" :
                        ReStore();
                        Toast.ShowCommonToast(res.message, APIUrlConfig.BAD_REQUEST_RESPONSE_CODE);
                        break;
                    default:
                        Toast.ShowCommonToast(res.message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
                        ReStore();
                        break;
                }
            }         
        }
    }

    private void DestroyModel()
    {
        GameObject modelClone = GameObject.FindWithTag("ModelClone");
        if (modelClone != null)
        {
            Destroy(modelClone);
        }
    }

    public void ReStore() 
    {
        uiCoat.SetActive(false);
        uiBFill.SetActive(false);
        imgLoadingFill.fillAmount = 0; 
        txtPercent.text = "";
    }
}

[System.Serializable]
class ResUpModel 
{
    
    public string code;
    public string message;
    public ResData[] data;
}

[System.Serializable]
class ResData 
{
    public int type;
    public string extention;
    public double size;
    public string file_name;
    public string file_path;
    public int created_by;
    public string created_date;
    public int file_id;
}

