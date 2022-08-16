using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Networking;

public class UploadAudio : MonoBehaviour
{
    public GameObject uiBFill;
    // public Text txtPercent;
    public Image imgLoadingFill;

    public GameObject uiCoat;
    public GameObject pannelUpload;
    public GameObject pannelAddAudio; 

    private string path;
    AudioClip audioClip;
    AudioSource audioSource;
    string[] fileTypes = new string[] { "mp3/*", "wav/*" }; // Valid file types

    private static UploadAudio instance;
    public static UploadAudio Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UploadAudio>();
            }
            return instance; 
        }
    }

    // public void HandleUploadFileAudio()
    // {
    //     if (Application.isEditor)
    //         path = "Assets/";

    //     audioSource = audio;
        
    //     // Find files in directory        
    //     yield GetFilesInDirectory();
        
    //     // Play a clip found in directory
    //     if (audioClips.Count > 0) 
    //     {
    //         audioSource.clip = audioClips[0];
    //         audioSource.Play();
    //     }
    // }

    // void GetFilesInDirectory () 
    // {
    //     try
    //     {
    //         DirectoryInfo info = new DirectoryInfo(path);
    //         files = info.GetFiles();
    //         foreach (FileInfo file in files) 
    //         {
    //             Debug.Log("FILE FULLNAME: " + file.FullName );
    //             string extension = Path.GetExtension(file.FullName);
    //             if (ValidType(extension))
    //             {
    //                 yield LoadFile(file.FullName);
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Debug.Log($"test {ex.Message}");
    //     } 
    // }

    // boolean ValidType (string extension) 
    // {
    //     foreach (string validExtension in fileTypes)
    //     {
    //         if (extension.IndexOf(validExtension) > -1)
    //             return true;
    //     }
    //     return false;
    // }
 
    // void LoadFile (string path ) 
    // {
    //     WWW www  = new WWW("file://"+path);
    //     AudioClip clip = www.audioClip;
    //     while (!clip.isReadyToPlay)
    //         yield;
    //     clip = www.GetAudioClip(false);
    //     String[] parts = path.Split("\\"[0]);
    //     clip.name = parts[parts.Length-1];
    //     audioClips.Add(clip);
    // }   

    public void OpenFileExplore()
    {
        NativeFilePicker.Permission permission = NativeFilePicker.PickFile((p) =>
            {
                if (p == null)
                {
                    Debug.Log("Operation cancelled");
                }
                    
                else
                {
                    Debug.Log("UPLOAD AUDIO - Picked file: " + p);
                    path = p;
                }
            }, fileTypes);
        Debug.Log("test path: " + path);
        StartCoroutine(GetAudio());
    }

    IEnumerator GetAudio()
    {
        yield return null; 
        
        UnityWebRequest webRequest = UnityWebRequest.Get("file:///" + path);
        UnityWebRequestAsyncOperation request = webRequest.SendWebRequest();
        imgLoadingFill.fillAmount = 0f;
        
        while (!request.isDone)
        {
            Debug.Log("UPLOAD AUDIO: " + request.progress);
            imgLoadingFill.fillAmount = request.progress * 2f;
            uiCoat.SetActive(true);
            uiBFill.SetActive(true);
            yield return null;
        }

        if (webRequest.isNetworkError || webRequest.isHttpError)
        {   
            Debug.Log("UPLOAD AUDIO - An error has occur");
            Debug.Log(webRequest.error);
        }
        else
        {
            if (webRequest.isDone)
            {
                uiCoat.SetActive(false);
                uiBFill.SetActive(false);

                Debug.Log("UPLOAD AUDIO ---- DONE");
                byte[] audio = webRequest.downloadHandler.data;
                // Convert to AudioClip 
                AudioClip audioData = Helper.ToAudioClip(audio);
                pannelAddAudio.SetActive(false);
                pannelUpload.SetActive(true);
                
                // Convert 
                pannelUpload.GetComponent<AudioSource>().clip = audioData;
                // AudioManager1.Instance.DisplayAudio(true);

                pannelUpload.GetComponent<AudioSource>().Play();
            }
        }    
    }
}
