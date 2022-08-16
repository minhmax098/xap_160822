using UnityEngine;
using System;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
public class MeetingObjectInstantiateObserve : MonoBehaviour, IPunInstantiateMagicCallback
{
    // Declare event triggers for photon instantiate object
    public static event Action<GameObject> onMeetingObjectInstantiate;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Invoke event for photon instantiate object
    /// </summary>
    /// <param name="info"></param>
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        onMeetingObjectInstantiate?.Invoke(this.gameObject);
    }
}
