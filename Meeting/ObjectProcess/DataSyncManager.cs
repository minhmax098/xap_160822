using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class DataSyncManager : MonoBehaviourPun, IPunObservable
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare flag for data synchronization
    /// </summary>
    public bool isSynchronizedLocalPosition = true; // is synchronized local position
    public bool isSynchronizedLocalRotation = true; // is synchronized local rotation
    public bool isSynchronizedLocalScale = true; // is synchronized local scale
    public bool isSynchronizedDisplay = true; // is synchronized display

    public void InitDefaultSettings()
    {
        isSynchronizedLocalPosition = true;
        isSynchronizedLocalRotation = true;
        isSynchronizedLocalScale = true;
        isSynchronizedDisplay = true;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Synchronize data betweens all members
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="info"></param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Player made changes
            SendChildTransform(stream, transform);
        }
        else
        {
            // Player received and updated data
            ReceiveChildTransform(stream, transform);
        }
    }

    public void ReceiveAnchorInfo(PhotonStream stream, Transform currentTransform)
    {
        stream.SendNext(currentTransform.gameObject.activeSelf);
    }

    public void SendAnchorInfo(PhotonStream stream, Transform transform)
    {
        throw new NotImplementedException();
    }



    /// <summary>
    /// Author: sonvdh
    /// Purpose: Send transform of child object to sync 
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="currentTransform"></param>
    private void SendChildTransform(PhotonStream stream, Transform currentTransform)
    {
        for (int i = 0; i < currentTransform.childCount; i++)
        {
            if (currentTransform.GetChild(i) != null)
            {
                if (currentTransform.GetChild(i).gameObject.tag == TagConfig.LABEL_TAG)
                {
                    return;
                }
                if (isSynchronizedLocalPosition)
                {
                    stream.SendNext(currentTransform.GetChild(i).localPosition);
                }
                if (isSynchronizedLocalRotation)
                {
                    stream.SendNext(currentTransform.GetChild(i).localRotation);
                }
                if (isSynchronizedLocalScale)
                {
                    stream.SendNext(currentTransform.GetChild(i).localScale);
                }
                if (isSynchronizedDisplay)
                {
                    stream.SendNext(currentTransform.GetChild(i).gameObject.activeSelf);
                }
                if (currentTransform.GetChild(i).childCount > 0)
                {
                    SendChildTransform(stream, currentTransform.GetChild(i));
                }
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Receive transform of child object to sync
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="currentTransform"></param>
    private void ReceiveChildTransform(PhotonStream stream, Transform currentTransform)
    {
        for (int i = 0; i < currentTransform.childCount; i++)
        {
            if (currentTransform.GetChild(i) != null)
            {
                if (currentTransform.GetChild(i).gameObject.tag == TagConfig.LABEL_TAG)
                {
                    return;
                }
                if (isSynchronizedLocalPosition)
                {
                    currentTransform.GetChild(i).localPosition = (Vector3)stream.ReceiveNext();
                }
                if (isSynchronizedLocalRotation)
                {
                    currentTransform.GetChild(i).localRotation = (Quaternion)stream.ReceiveNext();
                }
                if (isSynchronizedLocalScale)
                {
                    currentTransform.GetChild(i).localScale = (Vector3)stream.ReceiveNext();
                }
                if (isSynchronizedDisplay)
                {
                    currentTransform.GetChild(i).gameObject.SetActive((bool)stream.ReceiveNext());
                }
                if (currentTransform.GetChild(i).childCount > 0)
                {
                    ReceiveChildTransform(stream, currentTransform.GetChild(i));
                }
            }
        }
    }
}