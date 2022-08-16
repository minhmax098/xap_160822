using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System.IO;
using System;

public class LoadJsonFromWeb : MonoBehaviour
{
    private string category_uri = "https://api.xrcommunity.org/api/categories";
    
    public string GetCategoryJSON()
    {
        // create an HttpWebRequest with the specified URL
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(category_uri);
        // WebRequest.KeepAlive = false;
        // HttpWebRequest request = new HttpWebRequest.Create(category_uri)
        // Sends the HTTPWebRequest and waits for the response 
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        // Gets the stream associated with the response 
        StreamReader reader = new StreamReader(response.GetResponseStream());
        // response.Close();
        string jsonResponse = reader.ReadToEnd();
        return jsonResponse;
    }

    public string GetListLessonByCategory(string category_id)
    {
        string jsonResponse = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(String.Format(APIUrlConfig.GetListLessonByCategory, category_id)); 
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader= new StreamReader(response.GetResponseStream());
            jsonResponse = reader.ReadToEnd();
        }
        catch(Exception ex) 
        {
            Debug.Log("Eception occur"); 
        }
        return jsonResponse; 
    }
    
}
