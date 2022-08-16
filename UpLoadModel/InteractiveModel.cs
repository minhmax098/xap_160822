using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using BuildLesson;
using EasyUI.Toast;
using AdvancedInputFieldPlugin;

public class InteractiveModel : MonoBehaviour
{
    public Button btnCreateThumbnail;
    public Button btnBack;
    public Button btnCompleteConversion;
    public Image imgScreenShot;
    public new Camera camera;
    public Transform parent2;
	public Vector3 offset;
    public AdvancedInputField iModelName;
    public GameObject uiCoat;
    public GameObject NameModelWarning;
    public GameObject PictureWarning;
    public Image imgLoadingFill;
    public GameObject uiBFill;
    public Text txtPercent;
    private static Canvas canvas;

    private int resWidth = 407;
    private int resHeight = 408;

    private int idModel;
    public static string modelName;
    private byte[] thumbnail = null;
   

    private void Start()
    {
        Debug.Log("InteractiveModel");
        Screen.orientation = ScreenOrientation.Portrait; 
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;

        GameObject modelClone = GameObject.FindWithTag("ModelClone");

        

        NameModelWarning.SetActive(false);
        PictureWarning.SetActive(false);
        if (modelClone != null) 
        {
            iModelName.Text = modelClone.name;
            modelClone.transform.localPosition = modelClone.transform.localPosition + new Vector3(0,-20,160);
            ObjectModel.Instance.InitGameObject(modelClone);
        }

        idModel = UploadModel.idModel;
        Debug.Log(idModel);

        InitEvents();   

        Debug.Log(GameObject.Find("ImgRenderModel").transform.position);

    }

    void Update()
    {
        //TouchModel.Instance.TouchEventTest();
        TouchModel.Instance.HandleTouchInteraction();
    }

    void OnValidate()
    {
        btnBack = GameObject.Find("BtnBack").GetComponent<Button>();
        btnCreateThumbnail = GameObject.Find("BtnCreateThumbnail").GetComponent<Button>();
        camera = GameObject.Find("Render Camera").GetComponent<Camera>();
        parent2 = camera.transform;
        imgScreenShot = GameObject.Find("ImgScreenShot").GetComponent<Image>();
        //iModelName=GameObject.Find("IModelName").GetComponent<AdvancedInputField>();
        btnCompleteConversion = GameObject.Find("BtnComplete").GetComponent<Button>();
    }

    private void InitEvents()
    {
        btnBack.onClick.AddListener(() =>{ 
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
            GameObject modelClone = GameObject.FindWithTag("Organ");
            Destroy(modelClone);
        });
        btnCreateThumbnail.onClick.AddListener(HandleCreateThumbnail);
        // btnCompleteConversion.onClick.AddListener(() =>{
        //     (SceneManager.GetActiveScene().name, SceneConfig.createLesson);
        //     btnCompleteConversion.onClick.AddListener(HandleCompleteConversion);
        // });
        btnCompleteConversion.onClick.AddListener(HandleCompleteConversion);
        iModelName.OnValueChanged.AddListener(checkNameModelValid);
    }

    private void checkNameModelValid(string data)
    {
        if(data != "")
        {
            changeUIStatus(iModelName, NameModelWarning, false);
        }
    }

    private void changeUIStatus(AdvancedInputField input, GameObject warning, bool status)
    {
        warning.SetActive(status);
        if(status)
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldWarning);
        }
        else
        {
            input.GetComponent<Image>().sprite = Resources.Load<Sprite>(SpriteConfig.imageInputFieldNormal);
        }
    }

    private void HandleCreateThumbnail()
    {
        LateUpdates();
        StartCoroutine(Capture());
    }

    private void HandleCompleteConversion()
    {
        BackOrLeaveApp.Instance.AddPreviousScene(SceneManager.GetActiveScene().name, SceneConfig.createLesson);
        StartCoroutine(HandleCompleteConversionAPI());
    }

    private IEnumerator HandleCompleteConversionAPI() 
    {
        bool check = true;
        string modelName = iModelName.Text;
        Debug.Log(modelName);
        if(thumbnail == null)
        {
            check = false;
            PictureWarning.SetActive(true);
            uiCoat.SetActive(false);
        }
        else 
        {
            PictureWarning.SetActive(false);
        }

        if(modelName == "")
        {
            check = false;
            uiCoat.SetActive(false);
            changeUIStatus(iModelName, NameModelWarning, true);
        }

        if(check == false) 
        {
            yield return new WaitForSeconds(0f);
            yield break;
        }

        uiCoat.SetActive(true);
        uiBFill.SetActive(true);
        txtPercent.text="0%";

        var form = new WWWForm();

        form.AddField("modelFileId", idModel);
        form.AddField("modelName", iModelName.Text.ToString());
        form.AddBinaryData("thumbnail", thumbnail,$"{modelName}.jpg","image/jpg");
        Debug.Log(thumbnail);
        
        string API_KEY = PlayerPrefs.GetString("user_token");
        using var www = UnityWebRequest.Post(APIUrlConfig.Import3DModel, form);
        //using var www = UnityWebRequest.Post("http://10.30.176.132:8080/import3DModel", form);
        www.SetRequestHeader("Authorization", API_KEY);

        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            imgLoadingFill.fillAmount = operation.progress * 2f;    
            txtPercent.text=$"{(imgLoadingFill.fillAmount*100f):N0} %";
            yield return null;
        }

        string response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
        Debug.Log(response);
        ResImportModel res = JsonUtility.FromJson<ResImportModel>(response);

        if(res != null)
        {
            switch(res.code)
            {
                case "200":
                    ModelStoreManager.InitModelStore(res.data[0].modelId, res.data[0].modelName);
                    Toast.ShowCommonToast(res.message, APIUrlConfig.SUCCESS_RESPONSE_CODE);
                    yield return new WaitForSeconds(0f);
                    SceneManager.LoadScene(SceneConfig.createLesson);
                    ReStore();
                    break;

                case "400":
                    Toast.ShowCommonToast(res.message, APIUrlConfig.BAD_REQUEST_RESPONSE_CODE);
                    ReStore();
                    break;

                case "500":
                    Toast.ShowCommonToast(res.message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
                    ReStore();      
                    break;

                default:
                    Toast.ShowCommonToast(res.message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
                    ReStore();
                    break;
            }
        }        
        yield return new WaitForSeconds(0);
    }

    public static string ScreenShotName(int width, int height, int number = 0)
    {
        return
            $"{Application.dataPath}/ScreenShots/screen_{width}x{height}x{number}_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
    }

    private void LateUpdates()
    {

        Debug.Log("LatesUpadte");
        var rt = new RenderTexture(resWidth, resHeight, 24);

        camera.targetTexture = rt;

        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);

        camera.Render();

        RenderTexture.active = rt;

        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        screenShot.Apply();

        camera.targetTexture = null;
        RenderTexture.active = null; 
        imgScreenShot.sprite = Sprite.Create(screenShot, new Rect(0, 0, resWidth, resHeight), new Vector2(0, 0));
        Destroy(rt);
        

    }

    public IEnumerator Capture()
    {
        yield return new WaitForEndOfFrame();
        GameObject image = GameObject.FindGameObjectWithTag("ImageBackground");
        if(image != null)
        {
            Image i = image.GetComponent<Image>();
            i.enabled = false;

            var imageTransprency = zzTransparencyCapture.captureScreenshot();
            Texture2D result = ResizeImage(imageTransprency,resWidth,resHeight);
            thumbnail = result.EncodeToPNG();

            i.enabled = true;
        }      
        camera.targetTexture = Resources.Load<RenderTexture>("Textures/InteractiveRender");
    }

    Texture2D ResizeImage(Texture2D texture2D,int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        Texture2D result=new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }

    private void ReStore() 
    {
        uiCoat.SetActive(false);
        uiBFill.SetActive(false);
        imgLoadingFill.fillAmount = 0; 
        txtPercent.text = "";
        GameObject modelOrgan = GameObject.FindWithTag("Organ");
        if (modelOrgan != null)
        {
            Destroy(modelOrgan);
        }
    }
}

[System.Serializable]
class ResImportModel 
{
    
    public string code;
    public string message;
    public ResDataImportModel[] data;
}

[System.Serializable]
class ResDataImportModel 
{
    public string modelName;
    public string modelFileId;
    public int thumnailFileId;
    public int createBy;
    public string createDate;
    public int modelId;
}