using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System;

namespace Home 
{
    public class LoadData : MonoBehaviour
    {
        // public TextAsset jsonGetListXRLibrary;
        private string jsonResponse;
        private static LoadData instance;
        public static LoadData Instance
        {
            get 
            {
                if (instance == null) 
                {
                    instance = FindObjectOfType<LoadData>();
                }
                return instance;
            }
        }

        public ListXRLibrary GetCategoryWithLesson() 
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrlConfig.GetCategoryWithLesson); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("Json response: ");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<ListXRLibrary>(jsonResponse);
        }
    }
}