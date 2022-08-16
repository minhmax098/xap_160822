using System.Linq.Expressions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using EasyUI.Toast;
using Photon.Realtime;
using UnityEngine.XR.ARFoundation;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Google.XR.ARCoreExtensions;
using System.Threading.Tasks;
using static Photon.Pun.PhotonView;

[RequireComponent(typeof(InteractionSetupForMember))]
[RequireComponent(typeof(PhotonConnectionManager))]
[RequireComponent(typeof(TouchManager))]
[RequireComponent(typeof(XRayManager))]
[RequireComponent(typeof(SeparateManager))]
[RequireComponent(typeof(LabelManager))]
[RequireComponent(typeof(PopupManager))]
[RequireComponent(typeof(MeetingMembersManager))]
public class MeetingExperienceManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public GameObject interactionBtnsGroup;
    public Button btnSeparate;
    public Button btnLabel;
    public Button btnXray;
    public Button btnHold;
    public Button btnShowPopupExit;
    public Button btnClosePopupExit;
    public Button btnExitLesson;
    public Button btnCancelExitLesson;
    public Button btnShowGuideBoard;
    public Button btnExitGuideBoard;
    public Button btnAR;
    public Button btnSwitch;
    public Button btnBacktoMode3D;
    public GameObject cloudAnchorLoadingPanel;
    public Text txtCloudAnchorLoading;
    public Button btnRequestPresentation;
    private bool isResolvedModel = false;

    private bool isFirstTimeJoining = true;
    private string meetingRoomCode;
    private int objectPhotonViewID;
    private bool isPendingResolveCloudAnchor = false;
    private string pendingCloudAnchorId = "";

    public Image processImage;
    public Text processText;
    public Text processTypeText;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare variables for checking feature ma quality of environment
    /// </summary>
    public ARAnchorManager arAnchorManager;
    private Pose cameraPose;
    private FeatureMapQuality featureMapQuality;

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Singleton pattern: Create only one instance of MeetingExperienceManager class
    /// Note: All meeting action handler will be called in instance of PhotonConnectionManager
    /// </summary>
    private static MeetingExperienceManager instance;
    public static MeetingExperienceManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MeetingExperienceManager>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Subscribe events (Observe pattern)
    /// </summary>
    public override void OnEnable()
    {
        base.OnEnable();
        TouchManager.onSelectChildObject += OnSelectChildObject;
        TreeNodeManager.onClickNodeTree += OnClickNodeTree;
        ObjectManager.onReadyModel += OnReadyModel;
        ObjectManager.onChangeCurrentObject += OnChangeCurrentObject;
        ARUIManager.OnARPlaceObject += OnARPlaceObject;
        MeetingCloudAnchorManager.onHostCloudAnchorFailed += OnHostCloudAnchorFailed;
        MeetingCloudAnchorManager.onResolveCloudAnchorFailed += OnResolveCloudAnchorFailed;
        ObjectManager.onLoadedObjectAtRuntime += OnLoadedObjectAtRuntime;
        BackOrLeaveApp.onBackPressed += ToggleExitConfirmationPopup;
        SetPreviousScene();
    }

    void SetupVoice()
    {
        InteractionSetupForMember.Instance.InitVoiceManager();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Unsubscribe events
    /// </summary>
    public override void OnDisable()
    {
        base.OnDisable();
        TouchManager.onSelectChildObject -= OnSelectChildObject;
        TreeNodeManager.onClickNodeTree -= OnClickNodeTree;
        ObjectManager.onReadyModel -= OnReadyModel;
        ObjectManager.onChangeCurrentObject -= OnChangeCurrentObject;
        ARUIManager.OnARPlaceObject -= OnARPlaceObject;
        MeetingCloudAnchorManager.onHostCloudAnchorFailed -= OnHostCloudAnchorFailed;
        MeetingCloudAnchorManager.onResolveCloudAnchorFailed -= OnResolveCloudAnchorFailed;
        ObjectManager.onLoadedObjectAtRuntime -= OnLoadedObjectAtRuntime;
        BackOrLeaveApp.onBackPressed -= ToggleExitConfirmationPopup;
    }

    void SetPreviousScene()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            BackOrLeaveApp.Instance.AddPreviousScene(SceneConfig.meetingJoining, SceneConfig.meetingExperience);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupVoice();
        InitLayoutScreen();
        InitDefaultValueForInteraction();
        InstantiateMeetingObject();
        SetupMode();
        InitEvents();
    }

    void InitDefaultValueForInteraction()
    {
        XRayManager.Instance.IsMakingXRay = false;
        SeparateManager.Instance.IsSeparating = false;
        TouchManager.Instance.IsClickedHoldBtn = false;
        LabelManager.Instance.IsShowingLabel = false;
        PopupManager.Instance.InitPopupMangaer(false, false, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            TouchManager.Instance.HandleTouchInteraction();
        }
        SetupUIForMembers(PhotonNetwork.IsMasterClient);
        EnableFeature();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup mode for meeting: 3D or AR (detect by room property: EXPERIENCE_MODE_KEY)
    /// </summary>
    void SetupMode()
    {
        // ModeManager.MODE_EXPERIENCE currentMode = (ModeManager.MODE_EXPERIENCE)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.EXPERIENCE_MODE_KEY);
        // ModeManager.Instance.CheckModeInitToView(currentMode);
        ModeManager.Instance.CheckModeInitToView(ModeManager.MODE_EXPERIENCE.MODE_3D);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup screen orientation and visibility of status bar, navigation bar
    /// </summary>
    void InitLayoutScreen()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        StatusBarManager.statusBarState = StatusBarManager.States.Hidden;
        StatusBarManager.navigationBarState = StatusBarManager.States.Hidden;
    }
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object by photon
    /// </summary>
    void InstantiateMeetingObject()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ObjectManager.Instance.LoadObjectAtRunTime(APIUrlConfig.DOMAIN_SERVER + StaticLesson.ModelFile);
            StartCoroutine(TrackLoadingModel());
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handle when model was loaded
    /// </summary>
    void OnLoadedObjectAtRuntime(GameObject loadedObject)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InstantiateObjectByHost(loadedObject);
        }
        else
        {
            InstantiateObjectByMember(loadedObject);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object by host
    /// </summary>
    void InstantiateObjectByHost(GameObject loadedObject)
    {
        GameObject mainObject = Instantiate(loadedObject, Vector3.zero, Quaternion.Euler(Vector3.zero));
        PhotonView photonView = mainObject.AddComponent<PhotonView>();

        if (PhotonNetwork.AllocateRoomViewID(photonView))
        {
            object[] data = new object[]
            {
                photonView.ViewID
            };

            RaiseEventOptions raiseEventOptions = new RaiseEventOptions
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new SendOptions
            {
                Reliability = true
            };

            // Raise event: instantiate object with photonViewID
            PhotonNetwork.RaiseEvent(MeetingConfig.CustomManualInstantiationEventCode, data, raiseEventOptions, sendOptions);
            objectPhotonViewID = photonView.ViewID;
            AddExtraComponentsForLoadedObject(mainObject);
        }
        else
        {
            Toast.ShowCommonToast(MeetingConfig.failedToInitPhotonView, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
        }

    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add extra components for object
    /// </summary>
    /// <param name="mainObject">Object was load with photon view component</param>
    void AddExtraComponentsForLoadedObject(GameObject mainObject)
    {
        try
        {
            PhotonView photonView = mainObject.GetComponent<PhotonView>();
            photonView.observableSearch = ObservableSearch.AutoFindAll;
            photonView.OwnershipTransfer = OwnershipOption.Request;
            photonView.ObservedComponents = new List<Component>();

            // For sync position, rotation, scale
            PhotonTransformView photonTransformView = mainObject.AddComponent<PhotonTransformView>();
            photonTransformView.m_SynchronizePosition = true;
            photonTransformView.m_SynchronizeRotation = true;
            photonTransformView.m_SynchronizeScale = true;
            photonView.ObservedComponents.Add(photonTransformView);

            // For sync local postion, local rotation, local scale, display
            DataSyncManager dataSyncManager = mainObject.AddComponent<DataSyncManager>();
            dataSyncManager.InitDefaultSettings();
            photonView.ObservedComponents.Add(dataSyncManager);

            // Fit size, mesh, ...
            ObjectManager.Instance.InitGameObject(mainObject);
        }
        catch (Exception e)
        {
            Toast.ShowCommonToast(e.Message, APIUrlConfig.SYSTEM_ERROR_CODE);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object with specified photonViewID
    /// </summary>
    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == MeetingConfig.CustomManualInstantiationEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            objectPhotonViewID = (int)data[0]; // All object instantiate with the same photonViewID
            LoadObjectAtRuntimeByMember();
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Load object by member
    /// </summary>
    void LoadObjectAtRuntimeByMember()
    {
        ObjectManager.Instance.LoadObjectAtRunTime(APIUrlConfig.DOMAIN_SERVER + StaticLesson.ModelFile);
        StartCoroutine(TrackLoadingModel());
    }

    void InitProcessCircle(string type)
    {
        processTypeText.text = type;
        processText.text = "0%";
        processImage.fillAmount = 0;
    }

    IEnumerator TrackLoadingModel()
    {
        InitProcessCircle(ModelConfig.downloadProcessType);
        while(!ObjectManager.Instance.isFinishDownloading)
        {
            processImage.fillAmount = ObjectManager.Instance.downloadingModelProcess;    
            processText.text = $"{(processImage.fillAmount *100f):N0} %";
            yield return null;
        }
        if (ObjectManager.Instance.isDownloadingSuccess)
        {
            InitProcessCircle(ModelConfig.loadFromLocalProcessType);
            while(ObjectManager.Instance.loadingFromLocalProcess < 1)
            {
                processImage.fillAmount = ObjectManager.Instance.loadingFromLocalProcess;    
                processText.text = $"{(processImage.fillAmount *100f):N0} %";
                yield return null;
            }
            LoadingEffectManager.Instance.ShowLoadingEffect(false);
        }
        else
        {
            Toast.ShowCommonToast(ModelConfig.failedToDownloadModel, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
            LoadingEffectManager.Instance.ShowLoadingEffect(false);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Instantiate object by member
    /// </summary>
    /// <param name="loadedObject">object was load at runtime</param>
    void InstantiateObjectByMember(GameObject loadedObject)
    {
        GameObject mainObject = Instantiate(loadedObject, Vector3.zero, Quaternion.Euler(Vector3.zero));
        PhotonView photonView = mainObject.AddComponent<PhotonView>();
        photonView.ViewID = objectPhotonViewID;
        AddExtraComponentsForLoadedObject(mainObject);
        if (isFirstTimeJoining)
        {
            isFirstTimeJoining = false;
            InteractionSetupForMember.Instance.InitInteractions();
            if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
            {
                ObjectManager.Instance.OriginObject.SetActive(false);
            }
        }
    }

    void OnReadyModel()
    {
        LoadingEffectManager.Instance.ShowLoadingEffect(false);
        ModeManager.MODE_EXPERIENCE currentMode = (ModeManager.MODE_EXPERIENCE)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.EXPERIENCE_MODE_KEY);
        if (currentMode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            HandleSwitchMode(currentMode);
            if (isPendingResolveCloudAnchor && !string.IsNullOrEmpty(pendingCloudAnchorId))
            {
                StartCoroutine(ResolveCloudAnchorForLater());
            }
        }
    }

    IEnumerator ResolveCloudAnchorForLater()
    {
        yield return new WaitForSeconds(MeetingConfig.switchModeTimeLoading);
        StartResolveCloudAnchor(pendingCloudAnchorId);
        isPendingResolveCloudAnchor = false;
        pendingCloudAnchorId = "";
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add event triggers for UI
    /// </summary>
    void InitEvents()
    {
        btnExitLesson.onClick.AddListener(ExitLesson);
        btnShowPopupExit.onClick.AddListener(ToggleExitConfirmationPopup);
        btnClosePopupExit.onClick.AddListener(ToggleExitConfirmationPopup);
        btnCancelExitLesson.onClick.AddListener(ToggleExitConfirmationPopup);

        btnXray.onClick.AddListener(HandleXRayView);
        btnSeparate.onClick.AddListener(HandleSeparation);
        btnHold.onClick.AddListener(HandleHoldAction);
        btnLabel.onClick.AddListener(HandleLabelView);
        btnShowGuideBoard.onClick.AddListener(HandleShowGuideBoard);
        btnExitGuideBoard.onClick.AddListener(HandleShowGuideBoard);

        btnAR.onClick.AddListener(delegate { ToggleMode(ModeManager.MODE_EXPERIENCE.MODE_AR); });
        btnSwitch.onClick.AddListener(delegate { ToggleMode(ModeManager.MODE_EXPERIENCE.MODE_3D); });
        btnBacktoMode3D.onClick.AddListener(delegate { ToggleMode(ModeManager.MODE_EXPERIENCE.MODE_3D); });
       
        btnRequestPresentation.onClick.AddListener(HandleRequestPresentation);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup UI according to player role (host or client)
    /// </summary>
    void SetupUIForMembers(bool isHost)
    {
        interactionBtnsGroup.SetActive(isHost);
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            ARPointerManager.Instance.IsForcedHiddenPointer = !isHost;
            if (ARSession.state == ARSessionState.SessionTracking)
            {
                cameraPose = new Pose(Camera.main.transform.position, Camera.main.transform.rotation);
                featureMapQuality = arAnchorManager.EstimateFeatureMapQualityForHosting(cameraPose);
                ARUIManager.Instance.guideText.GetComponent<Text>().text = MeetingConfig.GetFeatureMapQualityMessage(featureMapQuality, isHost);
            }
            else
            {
                ARPointerManager.Instance.IsForcedHiddenPointer = true;
                ARUIManager.Instance.guideText.GetComponent<Text>().text = MeetingConfig.lostTrackingSessionMessage;
            }
            if (!isHost)
            {
                ARUIManager.Instance.introText.SetActive(false);
                ARUIManager.Instance.guideText.SetActive(true);
            }
        }
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR && (!isHost) && (!isResolvedModel))
        {
            btnRequestPresentation.interactable = false;
        }
        else
        {
            btnRequestPresentation.interactable = true;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide interactive UI according to model
    /// </summary>
    void EnableFeature()
    {
        btnLabel.interactable = (ObjectManager.Instance.CurrentObject != null &&
                                    LabelManager.Instance.CheckAvailableLabel(ObjectManager.Instance.CurrentObject));
        btnXray.interactable = (ObjectManager.Instance.CurrentObject != null);
        btnHold.interactable = (ObjectManager.Instance.CurrentObject != null);
        btnAR.interactable = (ObjectManager.Instance.CurrentObject != null);
        btnSeparate.interactable = (ObjectManager.Instance.CurrentObject != null &&
                                    ObjectManager.Instance.CheckObjectHaveChild(ObjectManager.Instance.CurrentObject));

        if (ObjectManager.Instance.CurrentObject != null)
        {
            if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
            {
                btnAR.gameObject.SetActive(false);
                btnSwitch.gameObject.SetActive(true);
            }
            else if(ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_3D)
            {
                btnAR.gameObject.SetActive(true);
                btnSwitch.gameObject.SetActive(false);
            }   
        }
        btnAR.interactable = (ObjectManager.Instance.CurrentObject != null);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handle exit lesson
    /// </summary>
    public void ExitLesson()
    {
        PopupManager.Instance.ShowPopupExitLesson(false);
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            HandleSwitchMode(ModeManager.MODE_EXPERIENCE.MODE_3D);
        }
        PhotonConnectionManager.Instance.IsForcedToDisconnected = true;
        PhotonNetwork.Disconnect();
    }

    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide exit lesson confimation popup
    /// </summary>
    void ToggleExitConfirmationPopup()
    {
        PopupManager.Instance.IsClickedExitLesson = !PopupManager.Instance.IsClickedExitLesson;
        if (PhotonNetwork.IsMasterClient)
        {
            PopupManager.Instance.SetContentForPopupExitLesson(MeetingConfig.finishMeetingTitle, MeetingConfig.finishMeetingContent, MeetingConfig.txtBtnFinishMeeting);
        }   
        else
        {
            PopupManager.Instance.SetContentForPopupExitLesson(MeetingConfig.leaveMeetingTitle, MeetingConfig.leaveMeetingContent, MeetingConfig.txtBtnLeaveMeeting);
        }
        PopupManager.Instance.ShowPopupExitLesson(PopupManager.Instance.IsClickedExitLesson);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle room property: IS_MAKING_XRAY_KEY
    /// </summary>
    void HandleXRayView()
    {
        XRayManager.Instance.IsMakingXRay = !XRayManager.Instance.IsMakingXRay;
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_MAKING_XRAY_KEY, XRayManager.Instance.IsMakingXRay);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update XRAY view
    /// </summary>
    /// <param name="isMakingXRay">New state of XRAY view</param>
    void SyncXRayView(bool isMakingXRay)
    {
        if (ObjectManager.Instance.CurrentObject != null)
        {
            XRayManager.Instance.HandleXRayView(isMakingXRay);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle room property: IS_SEPARATING_KEY
    /// </summary>
    void HandleSeparation()
    {
        SeparateManager.Instance.IsSeparating = !SeparateManager.Instance.IsSeparating;
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_SEPARATING_KEY, SeparateManager.Instance.IsSeparating);
        SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update separation view
    /// </summary>
    /// <param name="isSeparating">New state of separation view</param>
    void SyncSeparationView(bool isSeparating)
    {
        if (ObjectManager.Instance.CurrentObject != null)
        {
            SeparateManager.Instance.IsSeparating = isSeparating;
            SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);
        }
    }
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle room property: IS_CLICKED_HOLD_KEY: allow/disallow player to drag child object
    /// </summary>
    void HandleHoldAction()
    {
        TouchManager.Instance.IsClickedHoldBtn = !TouchManager.Instance.IsClickedHoldBtn;
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_CLICKED_HOLD_KEY, TouchManager.Instance.IsClickedHoldBtn);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle room property: IS_SHOWING_LABEL_KEY
    /// </summary>
    void HandleLabelView()
    {
        LabelManager.Instance.IsShowingLabel = !LabelManager.Instance.IsShowingLabel;
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_SHOWING_LABEL_KEY, LabelManager.Instance.IsShowingLabel);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update label view
    /// </summary>
    /// <param name="isSHowingLabel">New state of label view</param>
    void SyncLabelView(bool isSHowingLabel)
    {
        if (ObjectManager.Instance.CurrentObject != null)
        {
            LabelManager.Instance.HandleLabelView(isSHowingLabel);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle room property: IS_CLICKED_GUIDE_BOARD_KEY
    /// </summary>
    void HandleShowGuideBoard()
    {
        PopupManager.Instance.IsClickedGuideBoard = !PopupManager.Instance.IsClickedGuideBoard;
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_CLICKED_GUIDE_BOARD_KEY, PopupManager.Instance.IsClickedGuideBoard);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update guide board view
    /// </summary>
    /// <param name="isClickedGuideBoard">New state of guide board view</param>
    void SyncGuideBoardView(bool isClickedGuideBoard)
    {
        PopupManager.Instance.ShowGuideBoard(isClickedGuideBoard);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Switch mode of experience: 3D or AR
    /// </summary>
    /// <param name="mode"></param>
    void ToggleMode(ModeManager.MODE_EXPERIENCE mode)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.CLOUD_ANCHOR_ID_KEY, "");
            PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.EXPERIENCE_MODE_KEY, mode);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handle when host change mode
    /// </summary>
    /// <param name="mode"></param>
    void HandleSwitchMode(ModeManager.MODE_EXPERIENCE mode)
    {
        if (ObjectManager.Instance.CurrentObject != null)
        {
            StopAllCoroutines();
            isResolvedModel = false;
            PhotonTransformView photonTransformView = ObjectManager.Instance.OriginObject.GetComponent<PhotonTransformView>();
            photonTransformView.m_SynchronizePosition = (mode != ModeManager.MODE_EXPERIENCE.MODE_AR);
            ObjectManager.Instance.DestroyARAnchorComponent();
            MeetingCloudAnchorManager.Instance.DestroyCloudAnchor(); 
            ShowLoadingAnchorPanel(false);
            StartCoroutine(AsyncSwitchMode(mode));
        }
        else
        {
            isResolvedModel = false;
            ShowLoadingAnchorPanel(false);
            ModeManager.Instance.CheckModeInitToView(mode);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: wait for clean and switch mode
    /// </summary>
    /// <param name="mode"></param>
    /// <returns></returns>
    IEnumerator AsyncSwitchMode(ModeManager.MODE_EXPERIENCE mode)
    {
        // wait for clean mode
        yield return new WaitForSeconds(MeetingConfig.switchModeTimeLoading);
        ModeManager.Instance.ViewScreenWithMode(mode);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Call function corresponding to changed room property:
    /// </summary>
    /// <param name="propertiesThatChanged">Room properties that changed</param>
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(MeetingConfig.IS_MAKING_XRAY_KEY))
        {
            SyncXRayView((bool)propertiesThatChanged[MeetingConfig.IS_MAKING_XRAY_KEY]);
        }
        if (propertiesThatChanged.ContainsKey(MeetingConfig.IS_SEPARATING_KEY))
        {
            SyncSeparationView((bool)propertiesThatChanged[MeetingConfig.IS_SEPARATING_KEY]);
        }
        if (propertiesThatChanged.ContainsKey(MeetingConfig.IS_CLICKED_HOLD_KEY))
        {
            TouchManager.Instance.IsClickedHoldBtn = (bool)propertiesThatChanged[MeetingConfig.IS_CLICKED_HOLD_KEY];
        }
        if (propertiesThatChanged.ContainsKey(MeetingConfig.IS_SHOWING_LABEL_KEY))
        {
            SyncLabelView((bool)propertiesThatChanged[MeetingConfig.IS_SHOWING_LABEL_KEY]);
        }
        if (propertiesThatChanged.ContainsKey(MeetingConfig.IS_CLICKED_GUIDE_BOARD_KEY))
        {
            SyncGuideBoardView((bool)propertiesThatChanged[MeetingConfig.IS_CLICKED_GUIDE_BOARD_KEY]);
        }
        if (propertiesThatChanged.ContainsKey(MeetingConfig.EXPERIENCE_MODE_KEY))
        {
            HandleSwitchMode((ModeManager.MODE_EXPERIENCE)propertiesThatChanged[MeetingConfig.EXPERIENCE_MODE_KEY]);
        }
    }

    /// <summary>
    /// Author: quyennt57 (sonvdh transfer from original experience to meeting experience)
    /// Purpose: Handler when host selected child object
    /// </summary>
    /// <param name="selectedObject"></param>
    void OnSelectChildObject(GameObject selectedObject)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            OnResetStatusFeature();
            PhotonConnectionManager.Instance.SyncObjectSelection(selectedObject.name, true);
        }
    }

    /// <summary>
    /// Author: quyennt57 (sonvdh transfer from original experience to meeting experience)
    /// Purpose: Handle when player clicked on item level tree
    /// </summary>
    /// <param name="nodeName"></param>
    void OnClickNodeTree(String nodeName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ResetActionStatusByHost();
            PhotonConnectionManager.Instance.SyncObjectSelection(nodeName, false);
        }
    }

    /// <summary>
    /// Author: quyennt57 (sonvdh transfer from original experience to meeting experience)
    /// Purpose: Handler when current object was changed
    /// </summary>
    /// <param name="currentObjectName"></param>
    void OnChangeCurrentObject(string currentObjectName)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.CURRENT_OBJECT_NAME_KEY, currentObjectName);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Reset action status by host
    /// </summary>
    void ResetActionStatusByHost()
    {
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_SHOWING_LABEL_KEY, false);
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_SEPARATING_KEY, false);
        SeparateManager.Instance.HandleSeparate(false);
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_CLICKED_HOLD_KEY, false);
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_MAKING_XRAY_KEY, false);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Reset all state of XRAY, label, separation, hold, media
    /// </summary>
    void OnResetStatusFeature()
    {
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_SHOWING_LABEL_KEY, false);
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_SEPARATING_KEY, false);
        SeparateManager.Instance.HandleSeparate(false);
        PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.IS_CLICKED_HOLD_KEY, false);
        // MediaManager.Instance.StopMedia();
    }

    /// <summary>
    /// Author: quyennt57 (sonvdh transfer from original experience to meeting experience)
    /// </summary>
    // [PunRPC]
    // void SyncEmptyNodeTree()
    // {
    //     TreeNodeManager.Instance.ClearAllNodeTree();
    // }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handler when player place model in AR mode
    /// </summary>
    /// <param name="position">Position to place object</param>
    /// <param name="rotation">Rotation to rotate object</param>
    void OnARPlaceObject(Vector3 position, Quaternion rotation)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ObjectManager.Instance.InstantiateARObject(position, rotation, true);
            SetupUIForHostCloudAnchor();
            MeetingCloudAnchorManager.Instance.HostCloudAnchor(ObjectManager.Instance.GetARAnchorComponent());
        }
        else
        {
            Toast.ShowCommonToast(MeetingConfig.waitForHostPlaceObject, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup UI for hosting cloud anchor
    /// </summary>
    void SetupUIForHostCloudAnchor()
    {
        ARUIManager.Instance.IsStartAR = true;
        ARPointerManager.Instance.IsForcedHiddenPointer = true;
        ARUIManager.Instance.guideText.SetActive(false);
        txtCloudAnchorLoading.text = MeetingConfig.sharingModel;
        ShowLoadingAnchorPanel(true);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handle when hosting cloud anchor was failed
    /// </summary>
    /// <param name="message">Message will be displayed</param>
    public void OnHostCloudAnchorFailed(string message)
    {
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            ObjectManager.Instance.DestroyARAnchorComponent();
            ObjectManager.Instance.OriginObject.SetActive(false);
            SetUIForRehostCloudAnchor(message);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup UI for rehosting cloud anchor
    /// </summary>
    /// <param name="message">Message will be displayed</param>
    void SetUIForRehostCloudAnchor(string message)
    {
        ARPointerManager.Instance.IsForcedHiddenPointer = false;
        ARUIManager.Instance.guideText.SetActive(true);
        message += MeetingConfig.retryToHostCloudAnchorMessage;
        ShowNotificationAnchorPanel(message);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Ask member to resolve cloud anchor
    /// </summary>
    /// <param name="cloudAnchorId">Cloud anchor id will be resolved</param>
    public void ShareCloudAnchorId(string cloudAnchorId)
    {
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            ShowLoadingAnchorPanel(false);
            ARUIManager.Instance.PlaceMeetingARObject();
            PhotonConnectionManager.Instance.SyncResolveCloudAnchor(cloudAnchorId);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup UI for resolving cloud anchor
    /// </summary>
    void SetupUIForResolveCloudAnchor()
    {
        ARUIManager.Instance.guideText.SetActive(false);
        txtCloudAnchorLoading.text = MeetingConfig.receivingModel;
        ShowLoadingAnchorPanel(true);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Handle when resolving cloud anchor was failed
    /// </summary>
    /// <param name="message"></param>
    public void OnResolveCloudAnchorFailed(string message)
    {
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            message += MeetingConfig.retryToResolveCloudAnchorMessage;
            ShowNotificationAnchorPanel(message);
            ARUIManager.Instance.guideText.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(RetryToResolveCloudAnchor());
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Automatically resolve again after several seconds
    /// </summary>
    /// <returns></returns>
    IEnumerator RetryToResolveCloudAnchor()
    {
        yield return new WaitForSeconds(MeetingConfig.timeoutForScanEnvironment); 
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            MeetingCloudAnchorManager.Instance.ResolveCloudAnchorById(MeetingCloudAnchorManager.Instance.cloudAnchorID);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update model transform after resolving cloud anchor successfully
    /// </summary>
    /// <param name="cloudAnchorTransform"></param>
    public void ResolveCloudAnchorTransform(Transform cloudAnchorTransform)
    {
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            ShowLoadingAnchorPanel(false);
            Toast.ShowCommonToast(MeetingConfig.resolveCloudAnchorSuccess, APIUrlConfig.SUCCESS_RESPONSE_CODE);
            // just for test
            ARUIManager.Instance.PlaceARObject(cloudAnchorTransform.position, cloudAnchorTransform.rotation, false);
            isResolvedModel = true;
        }
    }
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide loading panel when hosting/resolving cloud anchor
    /// </summary>
    /// <param name="isLoading"></param>
    public void ShowLoadingAnchorPanel(bool isLoading)
    {
        cloudAnchorLoadingPanel.SetActive(isLoading);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show notification related to cloud anchor
    /// </summary>
    /// <param name="message">Message will be displayed</param>
    public void ShowNotificationAnchorPanel(string message)
    {
        cloudAnchorLoadingPanel.SetActive(false);
        Toast.ShowCommonToast(message, APIUrlConfig.SERVER_ERROR_RESPONSE_CODE);
    }
   
    /// <summary>
    /// Author: quyennt57 (sonvdh transfer from original experience to meeting experience)
    /// Purpose: Init child object (change current object and object level tree)
    /// </summary>
    /// <param name="childObjectName"></param>
    [PunRPC]
    void InitNewChildObject(string childObjectName)
    {
        SeparateManager.Instance.HandleSeparate(false);
        GameObject selectedObject = GameObject.Find(childObjectName);
        TreeNodeManager.Instance.DisplaySelectedObject(selectedObject, ObjectManager.Instance.CurrentObject);
        ObjectManager.Instance.ChangeCurrentObject(selectedObject);
        TreeNodeManager.Instance.CreateChildNodeUI(childObjectName);
    }

    /// <summary>
    /// Author: quyennt57 (sonvdh transfer from original experience to meeting experience)
    /// Purpose: Back to parent object when player clicked on tree level
    /// </summary>
    /// <param name="selectedObjectName"></param>
    [PunRPC]
    public void BackToParentObject(string selectedObjectName)
    {
        if (selectedObjectName != ObjectManager.Instance.CurrentObject.name)
        {
            GameObject newActiveObject = GameObject.Find(selectedObjectName);
            TreeNodeManager.Instance.DisplayAllChildSelectedObject(newActiveObject);
            // Conjoined
            ObjectManager.Instance.CurrentObject = newActiveObject;
            SeparateManager.Instance.HandleSeparate(false);

            ObjectManager.Instance.ChangeCurrentObject(newActiveObject);
            TreeNodeManager.Instance.RemoveItem(selectedObjectName);
            // StartCoroutine(Helper.MoveObject(Camera.main.gameObject, Camera.main.transform.position));
        }
        ObjectManager.Instance.OriginObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Resolve cloud anchor by id
    /// </summary>
    /// <param name="cloudAnchorId"></param>
    [PunRPC]
    public void ResolveCloudAnchor(string cloudAnchorId)
    {
        if (ModeManager.Instance.Mode == ModeManager.MODE_EXPERIENCE.MODE_AR)
        {
            if ((cloudAnchorId.Equals((string)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.CLOUD_ANCHOR_ID_KEY))))
            {
                if (ObjectManager.Instance.CurrentObject != null)
                {
                    StartResolveCloudAnchor(cloudAnchorId);
                }
                else
                {
                    isPendingResolveCloudAnchor = true;
                    pendingCloudAnchorId = cloudAnchorId;
                }
            }
        }
    }

    public void StartResolveCloudAnchor(string cloudAnchorId)
    {
        StopAllCoroutines();
        SetupUIForResolveCloudAnchor();
        StartCoroutine(AsyncResolveCloudAnchor(cloudAnchorId));
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: wait for init and resolve cloud anchor
    /// </summary>
    /// <param name="cloudAnchorId"></param>
    /// <returns></returns>
    IEnumerator AsyncResolveCloudAnchor(string cloudAnchorId)
    {
        // wait for init mode
        yield return new WaitForSeconds(MeetingConfig.initModeTimeLoadingForResolveCloudAnchor);
        MeetingCloudAnchorManager.Instance.ResolveCloudAnchorById(cloudAnchorId);
    }

    /// <summary>
    /// Author: quyennt57
    /// Purpose: Send request to host by PhotonView
    /// </summary>
    void HandleRequestPresentation()
    {
        ObjectManager.Instance.OriginObject.GetComponent<PhotonView>().RequestOwnership();
    }
}