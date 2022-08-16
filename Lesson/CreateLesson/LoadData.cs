using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System; 
using System.Threading.Tasks; 
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using EasyUI.Toast;

namespace CreateLesson
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
        public ListOrgans getListOrgans()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(APIUrlConfig.GetListOrgans); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
            Debug.Log("Json response: ");
            Debug.Log(jsonResponse);
            return JsonUtility.FromJson<ListOrgans>(jsonResponse);
        }
    }
}
