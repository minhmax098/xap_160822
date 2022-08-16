using Photon.Pun;
using UnityEngine;

public class InteractionSetupForMember : MonoBehaviour
{
    
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Singleton pattern: Create only one instance of InteractionSetupForMember class
    /// Note: All interactions setup for late player joining will be handled in instance of InteractionSetupForMember class
    /// </summary>
    private static InteractionSetupForMember instance;
    public static InteractionSetupForMember Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InteractionSetupForMember>();
            }
            return instance;
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init all interactions for late member joining
    /// </summary>
    public void InitInteractions()
    {
        InitTouchManager();
        InitSeparateManager();
        InitXRayManager();
        InitLabelManager();
        InitGuideBoardManager();
        // InitVoiceManager();
        InitCurrentObject();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Display current model (parent or child) and build hierarchy tree node (Parent -> level 1 -> ... -> current model)
    /// </summary>
    void InitCurrentObject()
    {
        string currentObjectName = (string)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.CURRENT_OBJECT_NAME_KEY);
        if (!string.IsNullOrEmpty(currentObjectName))
        {
            ObjectManager.Instance.ChangeCurrentObject(GameObject.Find(currentObjectName));
            if (ObjectManager.Instance.CurrentObject.name != ObjectManager.Instance.OriginObject.name)
            {
                TreeNodeManager.Instance.CreateChildNodeUI(currentObjectName);
            }
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Mute player when joining. Setup infos for controller of all player voices according to current state (detect by room property: IS_MUTING_ALL_KEY)
    /// </summary>
    public void InitVoiceManager()
    {
        bool isMuting = (bool)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.IS_MUTING_ALL_KEY);
        MeetingMembersManager.Instance.IsMutingAll = isMuting;
        MeetingMembersManager.Instance.SetPlayerMicroState(true);
        PhotonNetwork.LocalPlayer.MuteByHost(isMuting);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init UI for dragging model according to current state (detect by room property: IS_CLICKED_HOLD_KEY)
    /// </summary>
    void InitTouchManager()
    {
        TouchManager.Instance.IsClickedHoldBtn = (bool)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.IS_CLICKED_HOLD_KEY);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init separation view according to current state (detect by room property: IS_SEPARATING_KEY)
    /// </summary>
    void InitSeparateManager()
    {
        SeparateManager.Instance.IsSeparating = (bool)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.IS_SEPARATING_KEY);
        if (SeparateManager.Instance.IsSeparating)
        {
            SeparateManager.Instance.HandleSeparate(SeparateManager.Instance.IsSeparating);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init XRAY view according to current state (detect by room property: IS_MAKING_XRAY_KEY)
    /// </summary>
    void InitXRayManager()
    {
        XRayManager.Instance.IsMakingXRay = (bool)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.IS_MAKING_XRAY_KEY);
        if (XRayManager.Instance.IsMakingXRay)
        {
            XRayManager.Instance.HandleXRayView(XRayManager.Instance.IsMakingXRay);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide label according to current state (detect by room property: IS_SHOWING_LABEL_KEY)
    /// </summary>
    void InitLabelManager()
    {
        LabelManager.Instance.IsShowingLabel = (bool)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.IS_SHOWING_LABEL_KEY);
        if (LabelManager.Instance.IsShowingLabel)
        {
            LabelManager.Instance.HandleLabelView(LabelManager.Instance.IsShowingLabel);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Show/hide guide board according to current state (detect by room property: IS_CLICKED_GUIDE_BOARD_KEY)
    /// </summary>
    void InitGuideBoardManager()
    {
        PopupManager.Instance.IsClickedGuideBoard = (bool)PhotonConnectionManager.Instance.GetActionStatusValue(MeetingConfig.IS_CLICKED_GUIDE_BOARD_KEY);
        if (PopupManager.Instance.IsClickedGuideBoard)
        {
            PopupManager.Instance.ShowGuideBoard(PopupManager.Instance.IsClickedGuideBoard);
        }
    }
}
