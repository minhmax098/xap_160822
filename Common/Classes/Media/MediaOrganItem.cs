using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MediaOrganItem : MonoBehaviour
{
    public string MediaName { get; set; }
    public string VideoURL { get; set; }
    public string AudioURL { get; set; }

    public void SetValueMediaOrganItem(string _mediaName, string _videoUrl, string _audioURL)
    {
        MediaName = _mediaName;
        VideoURL = _videoUrl;
        AudioURL = _audioURL;
    }
}

