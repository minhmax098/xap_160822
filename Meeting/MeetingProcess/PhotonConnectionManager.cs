using System;
using System.Collections;
using EasyUI.Toast;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
[RequireComponent(typeof(PhotonView))]
public class PhotonConnectionManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Singleton pattern: Create only one instance of PhotonConnectionManager class
    /// Note: ALL RPC will be called in instance of PhotonConnectionManager
    /// </summary>
    private static PhotonConnectionManager instance;
    public static PhotonConnectionManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PhotonConnectionManager>();
            }
            return instance;
        }
    }

    public GameObject waitingSpinner;
    // Photon view component
    private PhotonView PV;
    public bool IsDisConnectedByHost { get; set; } = false;
    public bool IsReConnectedAndRejoned { get; set; } = false;
    public bool IsForcedToDisconnected { get; set; } = false;
    // Start is called before the first frame update
    void Start()
    {
        IsDisConnectedByHost = false;
        IsForcedToDisconnected = false;
        InitDependedComponents();
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get depended components
    /// </summary>
    void InitDependedComponents()
    {
        PV = GetComponent<PhotonView>();
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Connect to photon server
    /// </summary>
    public void ConnectToMaster()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get room property value by key
    /// </summary>
    /// <param name="key">Key of room property</param>
    /// <returns></returns>
    public object GetActionStatusValue(string key)
    {
        return PhotonNetwork.CurrentRoom.CustomProperties[key];
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set room property value by key
    /// </summary>
    /// <param name="key">Key of room property</param>
    /// <param name="value">value of room property</param>
    public void SetActionStatusValue(string key, object value)
    {
        Hashtable newProperties = new Hashtable() { { key, value } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(newProperties, null, null);
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Clean meeting room and notify to leave meeting for everyone
    /// </summary>
    public void OnHostLeftMeeting()
    {
        IsDisConnectedByHost = true;
        CleanMeetingRoom();
        PhotonNetwork.Disconnect();
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Clear meeting room
    /// </summary>
    void CleanMeetingRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false; // do not accept for new joining
        PhotonNetwork.CurrentRoom.EmptyRoomTtl = 0; // close room immediately after all member left
        PhotonNetwork.CurrentRoom.PlayerTtl = 0; // leave room immediately, do not come inactive state
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handler if host leave meeting
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer.ActorNumber == (int)GetActionStatusValue(MeetingConfig.HOST_ACTOR_NUMBER_KEY))
        {
            OnHostLeftMeeting();
        }
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handle if local player disconnected from phonton server
    /// </summary>
    /// <param name="cause">Reason of disconnection</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (IsForcedToDisconnected)
        {
            BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            StartCoroutine(HandleDisconnection(cause));
        }
    }
    /// <summary>
    /// Author: quyennt
    /// Purpose: Show notification corresponding to reason of disconnection + Back to previous scene
    /// If networking clien disconnected, waiting and rejoin room
    /// </summary>
    /// <param name="cause"></param>
    /// <returns></returns>

    // no
    IEnumerator HandleDisconnection(DisconnectCause cause)
    {
        switch (cause)
        {
            case DisconnectCause.DisconnectByClientLogic:
                BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
                break;
            case DisconnectCause.Exception:
            case DisconnectCause.ExceptionOnConnect:
                waitingSpinner.SetActive(true);
                yield return new WaitForSeconds(MeetingConfig.longToastDuration);
                if (PhotonNetwork.ReconnectAndRejoin())
                {
                    waitingSpinner.SetActive(false);
                }
                if (ObjectManager.Instance.OriginObject != null)
                {
                    Toast.ShowCommonToast(MeetingConfig.clientReconnectionAndRejoinSuccessfully, APIUrlConfig.SUCCESS_RESPONSE_CODE);
                    yield return new WaitForSeconds(MeetingConfig.longToastDuration);
                }
                break;
            default:
                Toast.ShowCommonToast(MeetingConfig.lostConnection, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
                yield return new WaitForSeconds(MeetingConfig.longToastDuration);
                BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
                break;
        }
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Ask specified player to update voice state by host
    /// </summary>
    /// <param name="targetPlayer">Player will be updated voice</param>
    /// <param name="isMute">New state of voice: mute or unmute</param>
    public void AssignPlayerMicroStateByHost(Player targetPlayer, bool isMute)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("SetPlayerMicroStateByHost", targetPlayer, isMute);
        }
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Ask all players to update voice state in meeting
    /// </summary>
    /// <param name="isMute">New state of voice: mute or unmute</param>
    public void ForceUpdateMicroStateByHost(bool isMute)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("SetPlayerMicroStateByHost", RpcTarget.All, isMute);
        }
    }
   /// <summary>
   /// Author: sonvdh
   /// Purpose: Handle to select object by touch or tree mode
   /// </summary>
   /// <param name="objectName">Name of selected object</param>
   /// <param name="isSelectedByTouch">object is selected by touch or not</param>
    public void SyncObjectSelection(string objectName, bool isSelectedByTouch = true)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            string actionName = isSelectedByTouch ? "InitNewChildObject" : "BackToParentObject";
            PV.RPC(actionName, RpcTarget.All, objectName);
        }
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Notify to all players to do specified function
    /// </summary>
    /// <param name="processName">Function need to do in all players</param>
    public void SyncProcessByName(string processName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC(processName, RpcTarget.All);
        }
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get all active players in meeting
    /// </summary>
    /// <returns>Active players array</returns>
    public Player[] GetActivePlayers()
    {
        Player[] activePlayers = Array.FindAll(PhotonNetwork.PlayerList, player => player.IsInactive == false);
        return activePlayers;
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Check if all players were muted
    /// </summary>
    /// <returns></returns>
    public bool IsMutedAllPlayers()
    {
        foreach (Player player in GetActivePlayers())
        {
            if (player.IsMuted() == false)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Check if all player are active voice (unmute micro)
    /// </summary>
    /// <returns></returns>
    public bool IsActiveVoiceAllPlayers()
    {
        foreach (Player player in GetActivePlayers())
        {
            if (player.IsMuted() == true)
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Notify to other players to resolve cloud anchor by id after host player hosted cloud anchor
    /// </summary>
    /// <param name="cloudAnchorId">Cloud anchor id will be resolved</param>
    public void SyncResolveCloudAnchor(string cloudAnchorId)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SetActionStatusValue(MeetingConfig.CLOUD_ANCHOR_ID_KEY, cloudAnchorId);
            PV.RPC("ResolveCloudAnchor", RpcTarget.OthersBuffered, cloudAnchorId);
        }
    }

    public void SyncResetOriginalObject()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PV.RPC("ResetOriginalObject", RpcTarget.All);
        }
    }

    public void SyncResetMemberListPanel()
    {
        PV.RPC("ResetMemberListPanel", RpcTarget.All);
    }

    public void SyncResetUINotify()
    {
        PV.RPC("ResetUINotify", RpcTarget.All);
    }

    public void PushNotifyChangeHostSuccessfully()
    {
        PV.RPC("ToastNotifyChangeHostSuccessfully", RpcTarget.All);
    }
}