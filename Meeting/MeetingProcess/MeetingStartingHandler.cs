using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.Networking;
using System;
using EasyUI.Toast;
using Photon.Voice.PUN;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class MeetingStartingHandler : MonoBehaviourPunCallbacks
{
    enum PhotonGameServerConnectionState { None, Connecting, Connected };

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public Button backBtn;
    public Button copyMeetingCodeBtn;
    public Button launchMeetingBtn;
    public Text meetingCodeTxt;

    private int numberOfConnectionAttempt = 0;
    private int numberOfStartingAttempt = 0;
    private bool isForcedDisconnected = false;
    private bool isConnectedToLogicServer = false;
    private bool isConnectedToPhotonMasterServer = false;
    private PhotonGameServerConnectionState photonGameServerConnectionState = PhotonGameServerConnectionState.None;

    private static MeetingStartingHandler instance;
    public static MeetingStartingHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MeetingStartingHandler>();
            }
            return instance;
        }
    }

    void Start()
    {
        InitScreen();
        InitEvents();
        InitActions();
    }

    void InitScreen()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        StatusBarManager.statusBarState = StatusBarManager.States.TranslucentOverContent;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }
    void Update()
    {
        if (isConnectedToLogicServer && isConnectedToPhotonMasterServer && photonGameServerConnectionState == PhotonGameServerConnectionState.None)
        {
            CreatePhotonMeetingRoom();
        }
        AllowStartMeeting(isConnectedToLogicServer && photonGameServerConnectionState == PhotonGameServerConnectionState.Connected);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add trigger event for interactive UI
    /// </summary>
    void InitEvents()
    {
        launchMeetingBtn.onClick.AddListener(StartMeetingRoom);
        backBtn.onClick.AddListener(BackToPreviousScene);
        copyMeetingCodeBtn.onClick.AddListener(CopyMeetingCodeHandler);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Back to previous scene
    /// </summary>
    void BackToPreviousScene()
    {
        StartCoroutine(DisconnectPhotonServer());
        BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Start meeting room
    /// </summary>
    void StartMeetingRoom()
    {
        LoadingManager.Instance.ShowLoading(MeetingConfig.startingMeeting);
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.LoadLevel(SceneConfig.meetingExperience);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get meeting code and connect to photon server
    /// </summary>
    async void InitActions()
    {
        ConnectToMultiplayerServer();
        await GetMeetingCodeFromLogicServer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get meeting code from logic server
    /// </summary>
    async Task GetMeetingCodeFromLogicServer()
    {
        try
        {
            CreateMeetingRoomRequest createMeetingRoomRequest = new CreateMeetingRoomRequest();
            createMeetingRoomRequest.lessonId = StaticLesson.LessonId; // can not use construct, do not why -_-
            APIResponse<CreateMeetingRoomResponseInfo> createMeetingRoomResponse = await UnityHttpClient.CallAPI<CreateMeetingRoomResponseInfo>(APIUrlConfig.POST_CREATE_MEETING_ROOM, UnityWebRequest.kHttpVerbPOST, createMeetingRoomRequest);
            if (createMeetingRoomResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                if (meetingCodeTxt != null)
                {
                    meetingCodeTxt.text = createMeetingRoomResponse.data.roomCode;
                    isConnectedToLogicServer = true;
                }
            }
            else
            {
                throw new Exception(createMeetingRoomResponse.message);
            }
        }
        catch (Exception exception)
        {
            Toast.ShowCommonToast(exception.Message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Copy meeting code to clipboard to share with other users
    /// </summary>
    void CopyMeetingCodeHandler()
    {
        Helper.CopyToClipboard(meetingCodeTxt.text);
        Toast.ShowCommonToast(MeetingConfig.successCopyMeetingCodeMessage, APIUrlConfig.SUCCESS_RESPONSE_CODE);
    }


    /// <summary>
    /// Author: sonvdh
    /// Purpose: Connect to photon server
    /// </summary>
    public void ConnectToMultiplayerServer()
    {
        try
        {
            StartCoroutine(DisconnectPhotonServer());
            PhotonNetwork.ConnectUsingSettings();
        }
        catch (Exception e)
        {
            // Toast.ShowCommonToast(e.Message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Override callback OnConnectedToMaster photon
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.NickName = PlayerPrefs.GetString(PlayerPrefConfig.userName);
        isConnectedToPhotonMasterServer = true;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Create meeting room in photon
    /// </summary>
    void CreatePhotonMeetingRoom()
    {
        photonGameServerConnectionState = PhotonGameServerConnectionState.Connecting;
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.PlayerTtl = MeetingConfig.meetingPlayerTTL;
        roomOptions.EmptyRoomTtl = MeetingConfig.meetingRoomEmptyTTL;
        PhotonNetwork.JoinOrCreateRoom(meetingCodeTxt.text, roomOptions, TypedLobby.Default);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Override OnJoinedRoom in photon
    /// </summary>
    public override void OnJoinedRoom()
    {
        InitVoices();
        InitRoomProperties();
        photonGameServerConnectionState = PhotonGameServerConnectionState.Connected;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Override OnCreateRoomFailed in photon
    /// </summary>
    public override void OnCreateRoomFailed(short code, string message)
    {
        photonGameServerConnectionState = PhotonGameServerConnectionState.None;
        Toast.ShowCommonToast(MeetingConfig.failedToCreateRoom, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Override OnJoinRoomFailed in photon
    /// </summary>
    public override void OnJoinRoomFailed(short code, string message)
    {
        photonGameServerConnectionState = PhotonGameServerConnectionState.None;
        Toast.ShowCommonToast(MeetingConfig.failedToJoinRoom, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init voice component
    /// </summary>
    void InitVoices()
    {
        PhotonNetwork.LocalPlayer.SetOrganizer();
        PhotonNetwork.LocalPlayer.MuteByHost(MeetingConfig.isMutedByHostDefault);
        PhotonNetwork.LocalPlayer.Mute();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init default values for meeting room properties
    /// </summary>
    void InitRoomProperties()
    {
        Hashtable roomProperties = new Hashtable();
        roomProperties.Add(MeetingConfig.IS_MAKING_XRAY_KEY, false);
        roomProperties.Add(MeetingConfig.IS_SEPARATING_KEY, false);
        roomProperties.Add(MeetingConfig.IS_SHOWING_LABEL_KEY, false);
        roomProperties.Add(MeetingConfig.IS_CLICKED_HOLD_KEY, false);
        roomProperties.Add(MeetingConfig.IS_CLICKED_GUIDE_BOARD_KEY, false);
        roomProperties.Add(MeetingConfig.HOST_ACTOR_NUMBER_KEY, PhotonNetwork.LocalPlayer.ActorNumber);
        roomProperties.Add(MeetingConfig.IS_MUTING_ALL_KEY, true);
        roomProperties.Add(MeetingConfig.CURRENT_OBJECT_NAME_KEY, "");
        roomProperties.Add(MeetingConfig.EXPERIENCE_MODE_KEY, ModeManager.MODE_EXPERIENCE.MODE_3D);
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties, null, null);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Reconnect to photon server after disconnection
    /// </summary>
    /// <param name="cause">Reason of disconnection</param>
    public override void OnDisconnected(DisconnectCause cause)
    {
        if (isForcedDisconnected)
        {
            // if isForcedDisconnected == true (user wants to disconnect) -> do not reconnect
            return;
        }
        isConnectedToPhotonMasterServer = false;
        photonGameServerConnectionState = PhotonGameServerConnectionState.None;
        numberOfConnectionAttempt++;
        if (numberOfConnectionAttempt >= MeetingConfig.maxNumberOfConnecetionAttempt)
        {
            numberOfConnectionAttempt = 0;
            // Toast.ShowCommonToast(MeetingConfig.failedToConnectPhoton, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
        }
        ConnectToMultiplayerServer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Disconnect from photon server
    /// </summary>
    /// <returns></returns>
    public IEnumerator DisconnectPhotonServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            while (PhotonNetwork.IsConnected)
            {
                // repeat if the status is still connected
                yield return null;
            }
            photonGameServerConnectionState = PhotonGameServerConnectionState.None;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Enable/Disable to start meeting
    /// </summary>
    /// <param name="isEnabled">Is enable to start meeting or not</param>
    void AllowStartMeeting(bool isEnabled)
    {
        launchMeetingBtn.interactable = isEnabled;
        if (isEnabled)
        {
            LoadingManager.Instance.HideLoading();
        }
        else
        {
            LoadingManager.Instance.ShowLoading(MeetingConfig.connecting);
        }
    }
}