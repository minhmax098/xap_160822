using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.PUN;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

[RequireComponent(typeof(PhotonConnectionManager))]
[RequireComponent(typeof(PhotonView))]
public class MeetingMembersManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public GameObject memberList;
    public Text meetingCode;
    public Text numberOfMember;
    public  Button btnToggleMeetingMember;
    public Animator toggleMeetingMemberAnimator;
    public Text presenterName;
    public Text presenterTitle;
    public GameObject hostControlPanel;
    public GameObject clientControlPanel;
    public Button btnUpdateAllMicroState;
    public Image allMicroState;
    public Text txtMuteAll;
    public Text txtMuteYou;
    public GameObject btnNotifyGlobal;

    // Declare voice component of presenter 
    public PlayerVoiceManager presenterVoiceManager;

    // Declare voice component of local player
    public PlayerVoiceManager yourVoiceManager;

    // Declare voice connection component for meeting
    private PhotonVoiceNetwork photonVoiceNetwork;
    // Declare flag for muting all players
    public bool IsMutingAll {get; set; } = true;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Singleton pattern: Create only one instance of MeetingMembersManager class
    /// </summary>
    private static MeetingMembersManager instance;
    public static MeetingMembersManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MeetingMembersManager>();
            }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        InitDependedComponents();
        InitMemberListPanel();
        InitEvents();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateControlUI();
        UpdateVoiceControl();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init depended components
    /// </summary>
    void InitDependedComponents()
    {
        if (photonVoiceNetwork == null)
        {
            GameObject photonVoiceResource = PhotonNetwork.Instantiate(PathConfig.MEETING_VOICE_RESOURCE_PATH, Vector3.zero, Quaternion.identity);
            photonVoiceNetwork = FindObjectOfType<PhotonVoiceNetwork>();
            photonVoiceNetwork.PrimaryRecorder = photonVoiceResource.GetComponent<PhotonVoiceView>().RecorderInUse;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update muting all player state
    /// Descriptiom: if all player are muted -> show Unmute all. if all player are unmuted -> show Mute all
    /// </summary>
    void UpdateVoiceControl()
    {
        if (PhotonConnectionManager.Instance.IsMutedAllPlayers())
        {
            UpdateUIForAllMicroStates(true);
        }
        else if (PhotonConnectionManager.Instance.IsActiveVoiceAllPlayers())
        {
            UpdateUIForAllMicroStates(false);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide control panel by role player (host or not)
    /// </summary>
    void UpdateControlUI()
    {
        hostControlPanel.SetActive(PhotonNetwork.IsMasterClient);
        clientControlPanel.SetActive(!PhotonNetwork.IsMasterClient);
        btnNotifyGlobal.SetActive(!toggleMeetingMemberAnimator.GetBool(AnimatorConfig.isShowMeetingMemberList) && PhotonNetwork.IsMasterClient); 
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add event triggers for UI
    /// </summary>
    void InitEvents()
    {
        btnToggleMeetingMember.onClick.AddListener(ToggleMeetingMemberPanel);
        btnUpdateAllMicroState.onClick.AddListener(UpdateAllMicroStatesByHost);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle visibility of meeting member list
    /// </summary>
    void ToggleMeetingMemberPanel()
    {
        toggleMeetingMemberAnimator.SetBool(AnimatorConfig.isShowMeetingMemberList, !toggleMeetingMemberAnimator.GetBool(AnimatorConfig.isShowMeetingMemberList));
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup UI for member list panel and voice component for each member
    /// </summary>
    public void InitMemberListPanel()
    {
        meetingCode.text = PhotonNetwork.CurrentRoom.Name;
        SetPlayerNumber();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.IsMasterClient)
            {
                SetPresenterInfo(player);
            }
            if (player.IsLocal)
            {
                SetYourVoiceManager(player);
            }
            SetPlayerInfo(player);
        }
    }

    /// <summary>
    /// Author: quyennt57
    /// Purpose: Reset members
    /// </summary>
    [PunRPC]
    public void ResetMemberListPanel()
    {
        ClearMemberList();
        InitMemberListPanel();
    }

    /// <summary>
    /// Author: quyennt57
    /// Purpose: Destroy item members
    /// </summary>
    public void ClearMemberList()
    {
        foreach(Transform itemMember in memberList.transform)
        {
            Destroy(itemMember.gameObject);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup voice component for local player
    /// </summary>
    /// <param name="player"></param>
    void SetYourVoiceManager(Player player)
    {
        yourVoiceManager.SetPlayer(player);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup infos (name, title, voice) for presenter
    /// </summary>
    /// <param name="player"></param>
    void SetPresenterInfo(Player player)
    {
        presenterName.text = player.NickName;
        presenterTitle.text = MeetingConfig.organizerName + (player.IsLocal ? MeetingConfig.localPlayerTitle : "");
        presenterVoiceManager.SetPlayer(player);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup (name, voice) for specified player
    /// </summary>
    /// <param name="player"></param>
    void SetPlayerInfo(Player player)
    {
        GameObject playerResource = Instantiate(Resources.Load(PathConfig.MEETING_MEMBER_PATH) as GameObject);
        PlayerManager playerManager = playerResource.GetComponent<PlayerManager>() as PlayerManager;
        playerManager.SetPlayer(player);
        playerResource.transform.SetParent(memberList.transform);
        playerResource.transform.localScale = Vector3.one;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Remove specified player from member list panel
    /// </summary>
    /// <param name="player"></param>
    void RemovePlayerInfo(Player player)
    {   
        foreach (Transform memberTransform in memberList.transform)
        {
            if (memberTransform.GetComponent<PlayerManager>().GetPlayer().ActorNumber == player.ActorNumber)
            {
                Destroy(memberTransform.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set text of total member in meeting
    /// </summary>
    void SetPlayerNumber()
    {
        numberOfMember.text = PhotonConnectionManager.Instance.GetActivePlayers().Length.ToString();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup info (name, voice) for new member and update number of member in meeting
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        SetPlayerInfo(newPlayer);
        SetPlayerNumber();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Remove info of left member and update number of member in meeting
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        RemovePlayerInfo(otherPlayer);
        SetPlayerNumber();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup voice component for specified member
    /// </summary>
    /// <param name="isMute">New state of voice: mute or unmute</param>
    /// <param name="targetPlayer">Player will be updated</param>
    public void SetPlayerVoice(bool isMute, Player targetPlayer)
    {
        if (targetPlayer == PhotonNetwork.LocalPlayer)
        {
            SetPlayerMicroState(isMute);
        }
        else if (PhotonNetwork.IsMasterClient)
        {
            PhotonConnectionManager.Instance.AssignPlayerMicroStateByHost(targetPlayer, isMute);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle voice state of all players (members) by host
    /// </summary>
    void UpdateAllMicroStatesByHost()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            IsMutingAll = !IsMutingAll;
            PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_MUTING_ALL_KEY, IsMutingAll);
            PhotonConnectionManager.Instance.ForceUpdateMicroStateByHost(IsMutingAll);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update UI for controller of all player (members) voices
    /// </summary>
    /// <param name="newMicroState">New state of voice: mute or unmute</param>
    public void UpdateUIForAllMicroStates(bool newMicroState)
    {
        IsMutingAll = newMicroState;
        if (IsMutingAll)
        {
            txtMuteAll.text = MeetingConfig.unmuteAll;
            allMicroState.sprite = Resources.Load<Sprite>(PathConfig.MICRO_OFF_IMAGE);
        }
        else
        {
            txtMuteAll.text = MeetingConfig.muteAll;
            allMicroState.sprite = Resources.Load<Sprite>(PathConfig.MICRO_ON_IMAGE);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set micro state of specified player by host
    /// </summary>
    /// <param name="isMute"></param>

    [PunRPC]
    public void SetPlayerMicroStateByHost(bool isMute)
    {
        SetPlayerMicroState(isMute);
        PhotonNetwork.LocalPlayer.MuteByHost(isMute);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set micro state of local player (member)
    /// </summary>
    /// <param name="isMute"></param>
    [PunRPC]
    public void SetPlayerMicroState(bool isMute)
    {
        if (isMute)
        {
            PhotonNetwork.LocalPlayer.Mute();
        }
        else
        {
            PhotonNetwork.LocalPlayer.Unmute();
        }
        if (photonVoiceNetwork == null)
        {
            InitDependedComponents();
        }
        photonVoiceNetwork.PrimaryRecorder.TransmitEnabled = !isMute;
        photonVoiceNetwork.PrimaryRecorder.IsRecording = !isMute;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update UI for controller of all player voices when IS_MUTING_ALL_KEY from room property changed
    /// </summary>
    /// <param name="propertiesThatChanged">Room properties that changed</param>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);
        if (propertiesThatChanged.ContainsKey(MeetingConfig.IS_MUTING_ALL_KEY))
        {
            UpdateUIForAllMicroStates((bool)propertiesThatChanged[MeetingConfig.IS_MUTING_ALL_KEY]);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update UI for controller of local player voice
    /// </summary>
    /// <param name="isMute">New state of local voice: mute or unmute</param>
    public void UpdateUIForMyMicro(bool isMute)
    {
        txtMuteYou.text = isMute ? MeetingConfig.unmuteYou : MeetingConfig.muteYou;
    }

}
