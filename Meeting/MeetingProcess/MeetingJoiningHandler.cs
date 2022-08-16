using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using System.Collections;
using Photon.Voice.PUN;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.Text;
using EasyUI.Toast;

public class MeetingJoiningHandler : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public Button backBtn;
    public Button joinMeetingBtn;
    public Button resetMeetingCodeBtn;
    public InputField inputFieldMeetingCode;
    public GameObject inputMeetingCodeComponent;
    public GameObject meetingLobbyComponent;
    public Text lobbyMeetingCodeTxt;
    private string meetingRoomCode;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare flags for error connection handler
    /// </summary>
    private int numberOfConnectionAttempt = 0;
    private int numberOfJoiningAttempt = 0;
    private bool isForcedDisconnected = false;
    private bool isConnectedToPhotonServer = false;
    private bool isLoadedLessonInfo = false;
    private bool isJoinedPhotonMeetingRoom = false;

    private static MeetingJoiningHandler instance;
    public static MeetingJoiningHandler Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MeetingJoiningHandler>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
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
        if (isLoadedLessonInfo && isJoinedPhotonMeetingRoom)
        {
            EnterLobbyMeeting();
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init Actions
    /// </summary>
    public void InitActions()
    {
        ConnectToMultiplayerServer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add event triggers for UI
    /// </summary>
    void InitEvents()
    {
        backBtn.onClick.AddListener(BackToPreviousScene);
        resetMeetingCodeBtn.onClick.AddListener(ResetMeetingCodeHandler);
        joinMeetingBtn.onClick.AddListener(JoinMeetingHandler);
        inputFieldMeetingCode.onValueChanged.AddListener(OnMeetingCodeChangedHandler);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Connect to photon server
    /// </summary>
    void ConnectToMultiplayerServer()
    {
        StartCoroutine(DisconnectPhotonServer());
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set up infos for photon session and create meeting room
    /// </summary>
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.NickName = PlayerPrefs.GetString(PlayerPrefConfig.userName);
        isConnectedToPhotonServer = true;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update visibility of reset value UI according to input value length
    /// </summary>
    void OnMeetingCodeChangedHandler(string meetingCodeValue)
    {
        joinMeetingBtn.interactable = meetingCodeValue.Length == MeetingConfig.meetingCodeLength;
        resetMeetingCodeBtn.gameObject.SetActive(meetingCodeValue.Length > 0);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Back to previous scene (HOME scene) from MEETING_JOINING scene
    /// </summary>
    void BackToPreviousScene()
    {
        StartCoroutine(DisconnectPhotonServer());
        BackOrLeaveApp.Instance.BackToPreviousScene(SceneManager.GetActiveScene().name);
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
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Reset meeting code input value
    /// </summary>
    void ResetMeetingCodeHandler()
    {
        inputFieldMeetingCode.text = "";
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Join meeting by code
    /// </summary>
    void JoinMeetingHandler()
    {
        AllowStartMeeting(false, MeetingConfig.joinMeeting);
        JoinRoomFromLogicServer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Join meeting from logic server
    /// </summary>
    async void JoinRoomFromLogicServer()
    {
        try
        {
            if (string.IsNullOrEmpty(inputFieldMeetingCode.text))
            {
                return;
            }
            JoinMeetingRoomRequest joinMeetingRoomRequest = new JoinMeetingRoomRequest();
            joinMeetingRoomRequest.roomCode = Convert.ToInt32(inputFieldMeetingCode.text); // can not use construct, do not why -_-
            APIResponse<LessonDetail[]> meetingJoiningResponse = await UnityHttpClient.CallAPI<LessonDetail[]>(APIUrlConfig.POST_JOIN_MEETING_ROOM, UnityWebRequest.kHttpVerbPOST, joinMeetingRoomRequest);

            if (meetingJoiningResponse.code == APIUrlConfig.SUCCESS_RESPONSE_CODE)
            {
                // from quyennt57
                StaticLesson.SetValueForStaticLesson(meetingJoiningResponse.data[0]);
                isLoadedLessonInfo = true;
                meetingRoomCode = inputFieldMeetingCode.text;
                PhotonNetwork.JoinRoom(meetingRoomCode);
            }
            else
            {
                throw new Exception(meetingJoiningResponse.message);
            }
        }
        catch (Exception exception)
        {
            isLoadedLessonInfo = false;
            AllowStartMeeting(true);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup photon local player (voice) and enter lobby when member joined room successfully
    /// </summary>
    public override void OnJoinedRoom()
    {
        InitVoices();
        isJoinedPhotonMeetingRoom = true;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init voice component
    /// </summary>
    void InitVoices()
    {
        PhotonNetwork.LocalPlayer.MuteByHost(MeetingConfig.isMutedByHostDefault);
        PhotonNetwork.LocalPlayer.Mute();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Rejoin meeting room after falied joining
    /// </summary>
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        numberOfJoiningAttempt++;
        if (numberOfJoiningAttempt >= MeetingConfig.maxNumberOfJoiningAttempt)
        {
            // Show notification and reset joining after many times of rejoining
            Toast.ShowCommonToast(MeetingConfig.failedToJoinRoom, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
            numberOfJoiningAttempt = 0;
            isJoinedPhotonMeetingRoom = false;
            meetingRoomCode = null;
            AllowStartMeeting(true);

        }
        else
        {
            PhotonNetwork.JoinRoom(inputFieldMeetingCode.text);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Enter lobby for meeting
    /// </summary>
    void EnterLobbyMeeting()
    {
        inputMeetingCodeComponent.SetActive(false);
        meetingLobbyComponent.SetActive(true);
        lobbyMeetingCodeTxt.text = inputFieldMeetingCode.text;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set up UI for joining meeting
    /// </summary>
    void InitMeetingJoining()
    {
        inputMeetingCodeComponent.SetActive(true);
        meetingLobbyComponent.SetActive(false);
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
        InitMeetingJoining();
        isConnectedToPhotonServer = false;
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
    /// Purpose: Enable/Disable to start meeting
    /// </summary>
    /// <param name="isEnabled">Is enable to start meeting or not</param>
    void AllowStartMeeting(bool isEnabled, string message = null)
    {
        inputFieldMeetingCode.interactable = isEnabled;
        resetMeetingCodeBtn.interactable = isEnabled;
        if (isEnabled)
        {
            LoadingManager.Instance.HideLoading();
        }
        else
        {
            LoadingManager.Instance.ShowLoading(message);
        }
    }
}
