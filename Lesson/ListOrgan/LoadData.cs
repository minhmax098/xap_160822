using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Threading.Tasks;

namespace ListOrgan 
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
        public async Task<AllOrgans> GetListLessonsByOrgan(int organId, string searchValue, int offset, int limit)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetListLessonsByOrgan, organId, searchValue, offset, limit)); 
            request.Method = "GET";
            // request.Headers["Authorization"] = PlayerPrefs.GetString("user_token");
            HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("API URL " + String.Format(APIUrlConfig.GetListLessonsByOrgan, organId, searchValue, offset, limit));
            Debug.Log("Json response from server: " + jsonResponse);
            return JsonUtility.FromJson<AllOrgans>(jsonResponse); 
        }
    }
}
