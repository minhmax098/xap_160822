using System.IO;
using System.Net.Mime;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using EasyUI.Toast;
using UnityEngine.SceneManagement;
using System.Threading;


public static class UnityHttpClient
{
    public static string LOCAL_FILE_PATH = "file:///";
    private static int defaultDurationForTokenTimeout = 4000;
    private static List<string> blackListToastAPI = new List<string>()
    {
        APIUrlConfig.POST_CREATE_MEETING_ROOM,
        APIUrlConfig.POST_JOIN_MEETING_ROOM
    };
    public static float processAPI = 0;
    // public static async Task<TResponseType> CallAPI<TResponseType>(string url, string method, object requestData = null)
    // {
    //     try
    //     {
    //         // convert request data to byte array
    //         UnityWebRequest www = new UnityWebRequest(APIUrlConfig.DOMAIN_SERVER + url, method);
    //         if (requestData != null)
    //         {
    //             string requestDataString = JsonUtility.ToJson(requestData);
    //             byte[] requestDataBytes = Encoding.UTF8.GetBytes(requestDataString);
    //             www.uploadHandler = (UploadHandler)new UploadHandlerRaw(requestDataBytes);
    //         }
    //         www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //         www.SetRequestHeader(APIUrlConfig.CONTENT_TYPE_KEY, APIUrlConfig.JSON_CONTENT_TYPE_VALUE);
    //         www.SetRequestHeader(APIUrlConfig.AUTHORIZATION_KEY, PlayerPrefs.GetString(PlayerPrefConfig.userToken));

    //         var operation = www.SendWebRequest();

    //         while (!operation.isDone)
    //         {
    //             await Task.Yield();
    //         }

    //         if (www.result == UnityWebRequest.Result.ConnectionError)
    //         {
    //             // Only for error connection. Bad request will be handled (show message)
    //             throw new Exception(www.error);
    //         }
    //         TResponseType responseData = JsonUtility.FromJson<TResponseType>(www.downloadHandler.text);
    //         return responseData;
    //     }
    //     catch (Exception exception)
    //     {
    //         throw exception;
    //     }
    // }

    public static async Task<APIResponse<TypeData>> CallAPI<TypeData>(string url, string method, object requestData = null, string contentType = null)
    {
        bool isMoveScene = false;
        try
        {
            if (method == UnityWebRequest.kHttpVerbPOST || method == UnityWebRequest.kHttpVerbPUT)
            {
                Exception exception = Network.CheckNetWorkToDisplayToast();
                if (exception != null)
                {
                    throw exception;
                }
            }
            else
            {
                Exception exception = Network.CheckNetWorkMoveScenceForAPI();
                if (exception != null)
                {
                    isMoveScene = true;
                    throw exception;
                }
            }

            // convert request data to byte array
            UnityWebRequest www = new UnityWebRequest(APIUrlConfig.DOMAIN_SERVER + url, method);  
            if (requestData != null)
            {
                string requestDataString = JsonUtility.ToJson(requestData);
                byte[] requestDataBytes = Encoding.UTF8.GetBytes(requestDataString);
                www.uploadHandler = (UploadHandler)new UploadHandlerRaw(requestDataBytes);
            }
            www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            www.SetRequestHeader(APIUrlConfig.CONTENT_TYPE_KEY, "application/octet-stream");
            if (string.IsNullOrEmpty(contentType))
            {
                contentType = APIUrlConfig.JSON_CONTENT_TYPE_VALUE;
            }
            www.SetRequestHeader(APIUrlConfig.CONTENT_TYPE_KEY, contentType);
            www.SetRequestHeader(APIUrlConfig.AUTHORIZATION_KEY, PlayerPrefs.GetString(PlayerPrefConfig.userToken));

            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                processAPI = operation.progress;
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                throw new Exception(APIUrlConfig.NETWORK_ERROR_MESSAGE);
            }
            APIResponse<TypeData> responseData = JsonUtility.FromJson<APIResponse<TypeData>>(www.downloadHandler.text);
            if (responseData.code == APIUrlConfig.TOKEN_TIME_OUT_CODE)
            {
                Toast.ShowCommonToast(responseData.message, responseData.code); 
                await Task.Delay(defaultDurationForTokenTimeout);
                Helper.Logout();
            }
            else if (method != UnityWebRequest.kHttpVerbGET)
            {
                if ((blackListToastAPI.IndexOf(url) == -1) || (blackListToastAPI.IndexOf(url) != -1 && responseData.code != APIUrlConfig.SUCCESS_RESPONSE_CODE))
                {
                    Toast.ShowCommonToast(responseData.message, responseData.code);
                }
            }
            return responseData;
        }
        catch (Exception exception)
        {
            if (isMoveScene)
            {
                SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name);
                Network.CheckNetWorkMoveScence();
            }
            else
            {
                Debug.Log("exception e: " + exception.Message);
                Toast.ShowCommonToast(exception.Message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
                throw exception;
            }
            return null;
        }
    }

    public static async Task<AudioClip> GetAudioClip(string audioURL)
    {
        try
        {
            Exception exception = Network.CheckNetWorkToDisplayToast();
            if (exception != null)
            {
                throw exception;
            }

            UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(APIUrlConfig.DOMAIN_SERVER + audioURL, AudioType.UNKNOWN);
            www.SetRequestHeader("Authorization", PlayerPrefs.GetString("user_token"));

            var operation = www.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result != UnityWebRequest.Result.Success)
            {
                throw new Exception(www.error);
            }
            Debug.Log($"{www}");
            Debug.Log($"{DownloadHandlerAudioClip.GetContent(www)}");
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
            Debug.Log($"{audioClip == null}");
            return audioClip;
        }
        catch (Exception exception)
        {
            throw exception;
        }

    }

    public static IEnumerator LoadRawImageAsync(string imageURL, RawImage targetImage, Action<bool> callback, bool isSetCache = true)
    {
        SceneNameManager.setPrevScene(SceneManager.GetActiveScene().name); 
        Network.CheckNetWorkMoveScence();
        
        int lastIndex = imageURL.LastIndexOf("/", StringComparison.Ordinal);
        string imageName = imageURL.Remove(0, lastIndex + 1);
        string imagePathInLocalSource = PathConfig.MEDIA_CACHE + imageName; 
        bool isExist = File.Exists(imagePathInLocalSource);
        bool isSuccess = false;
        string loadingImageURL = isExist ? (LOCAL_FILE_PATH + imagePathInLocalSource) : imageURL;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(loadingImageURL);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture) www.downloadHandler).texture;
            if (targetImage != null)
            {
                targetImage.texture = texture;
                isSuccess = true;
                if (!isExist && isSetCache)
                {
                    byte[] fileData = texture.EncodeToPNG();
                    new Thread(() => 
                    {
                        Thread.CurrentThread.IsBackground = true; 
                        if (!Directory.Exists(PathConfig.MEDIA_CACHE)) 
                        {
                            Directory.CreateDirectory(PathConfig.MEDIA_CACHE);
                        }
                        using (var fs = new FileStream(imagePathInLocalSource, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(fileData, 0, fileData.Length);
                            fs.Close();
                        }
                    }).Start();
                }
            }
        }
        callback?.Invoke(isSuccess);
    }
}