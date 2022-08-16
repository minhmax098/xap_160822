using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System; 
using System.Threading.Tasks; 

namespace List3DStore
{
    public class LoadData : MonoBehaviour
    {
        private string jsonResponse;
        private static LoadData instance; 
        public static LoadData Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<LoadData>();
                }
                return instance; 
            }
        }

        public All3DModel GetList3DModel(int type, string searchValue, int offset, int limit)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetList3DModel, type, searchValue, offset, limit));
            request.Method = "GET";
            request.Headers["Authorization"] = PlayerPrefs.GetString("user_token");
            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("Test API: " + jsonResponse);
            return JsonUtility.FromJson<All3DModel>(jsonResponse);
        }
    }
}
