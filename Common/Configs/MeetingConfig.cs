using System;
using System.Collections;
using System.Collections.Generic;
using Google.XR.ARCoreExtensions;
using UnityEngine;

public static class MeetingConfig
{
    // Is not for multi language
    public static int meetingCodeLength = 9;
    public static int maxNumberOfConnecetionAttempt = 200;
    public static int maxnumberOfStartingAttempt = 150;
    public static int maxNumberOfJoiningAttempt = 150;
    public static string errorConnectionMessage = "Please check your connection";
    public static string failedRoomCreationMessage = "Something went wrong. Please try again";
    public static string invalidMeetingCodeMessage = "Invalid meeting code. Please try again";
    public static string successCopyMeetingCodeMessage = "Copy meeting code successfully";
    public static string hostLeftMessage = "Host finished the meeting";
    public static string finishingMeeting = "Finishing meeting ...";
    public static string startingMeeting = "Starting ...";
    public static string connecting = "Connecting ...";
    public static string joinMeeting = "Joining ...";
    public static string leaveMeetingTitle = "Leave meeting";
    public static string finishMeetingTitle = "Finish meeting";
    public static string leaveMeetingContent = "Are you sure you want to leave meeting?";
    public static string finishMeetingContent = "Are you sure you want to finish meeting?";
    public static string txtBtnLeaveMeeting = "Leave";
    public static string txtBtnFinishMeeting = "Finish";
    public static string lostConnection = "You lost connection. Please reconnect and try another meeting";
    public static string waitingConnection = "Waiting for client to be fully disconnected..";
    public static string clientDisconnection = "Client is disconnected!";
    public static string clientReconnectionSuccessfully = "Successful reconnected!";
    public static string clientReconnectionAndRejoinSuccessfully = "Successful reconnected and joined!";
    public static string clientChangeHostSuccessfully = " is now a host!";

    public static string GetFeatureMapQualityMessage(FeatureMapQuality featureMapQuality, bool isHost)
    {
        string message = "Your environment is good. ";
        if (isHost)
        {
            if (featureMapQuality == FeatureMapQuality.Good)
            {
                message += " Let's tap to place model";
            }
            else if (featureMapQuality == FeatureMapQuality.Sufficient)
            {
                message = "Your environment is just okay. Placing model may take a while. Try to move around more gently to get better invironment";
            }
            else if (featureMapQuality == FeatureMapQuality.Insufficient)
            {
                message = "Your environment is not good. Forcing to place model is not recommended. Try to move your phone around gently or try another environment";
            }
        }  
        else
        {
            if (featureMapQuality == FeatureMapQuality.Good)
            {
                message += " Just wait for host to place model";
            }
            else if (featureMapQuality == FeatureMapQuality.Sufficient)
            {
                message = "Your environment is just okay. The accuracy of the recieving host model will likely be reduced. Try to move around more gently to get better invironment";
            }
            else if (featureMapQuality == FeatureMapQuality.Insufficient)
            {
                message = "Your environment is not good. Recieving model from host may be failed. Try to move your phone around gently or try another environment";
            }
        }      
        return message;
    }

    public static string waitForHostPlaceObject = "Wait for host place object";
    public static float toastDuration = 2f;
    public static float shortToastDuration = 1f;
    public static int meetingPlayerTTL = 40000;
    public static int meetingRoomEmptyTTL = 0;
    public static string IS_MAKING_XRAY_KEY = "IS_MAKING_XRAY_KEY";
    public static string IS_SEPARATING_KEY = "IS_SEPARATING_KEY";
    public static string IS_SHOWING_LABEL_KEY = "IS_SHOWING_LABEL_KEY";
    public static string IS_CLICKED_HOLD_KEY = "IS_CLICKED_HOLD_KEY";
    public static string IS_CLICKED_GUIDE_BOARD_KEY = "IS_CLICKED_GUIDE_BOARD_KEY";
    public static string HOST_ACTOR_NUMBER_KEY = "HOST_ACTOR_NUMBER_KEY";
    public static string IS_MUTE_KEY = "IS_MUTE_KEY";
    public static string IS_ORGANIZER_KEY = "IS_ORGANIZER_KEY";
    public static string IS_MUTING_ALL_KEY = "IS_MUTING_ALL_KEY";
    public static string CURRENT_OBJECT_NAME_KEY = "CURRENT_OBJECT_NAME_KEY";
    public static string EXPERIENCE_MODE_KEY = "EXPERIENCE_MODE_KEY";
    public static string IS_MUTED_BY_HOST = "IS_MUTED_BY_HOST";
    public static bool isMutedByHostDefault = false;
    public static int maxLengthOfPlayerName = 30;
    public static string localPlayerTitle = " (You)";
    public static int localPlayerTitleLength = 6;
    public static string organizerName = " (Organizer)";
    public static int organizerNameLength = 12;
    public static string dot = "...";
    public static int dotLength = 3;
    public static string muteAll = "Mute All";
    public static string unmuteAll = "Unmute All";
    public static string muteYou = "Mute You";
    public static string unmuteYou = "Unmute You";
    public static string unableToHostCloudAnchor = "Unable to share your model in 3D space";
    public static string unableToResolveCloudAnchor = "Unable to get host model";
    public static string resolveCloudAnchorSuccess = "Get host model successfully";
    public static string requestNotification = " sent for host a request to present";
    public static string contentPopupConfirm = "Are you sure you want to change host for ";
    public static string characterQuestion = " ?";
    public static float longToastDuration = 5f;
    public static float timeoutForScanEnvironment = 10f;
    public static string retryToHostCloudAnchorMessage = " And try again";
    public static string retryToResolveCloudAnchorMessage = $" Recieving model will automatically retry in {timeoutForScanEnvironment} seconds";
    public static string GetAnchorStateMessage(CloudAnchorState state)
    {
        switch (state)
        {
            case CloudAnchorState.ErrorHostingServiceUnavailable:
            case CloudAnchorState.ErrorNotAuthorized:
            case CloudAnchorState.ErrorResourceExhausted:
                return "Please ask the admin to check server.";
            case CloudAnchorState.ErrorHostingDatasetProcessingFailed:
                return "Please move your phone around gently to get better environment.";
            case CloudAnchorState.ErrorResolvingCloudIdNotFound:
            case CloudAnchorState.ErrorResolvingPackageTooOld:
            case CloudAnchorState.ErrorResolvingPackageTooNew:
                return "Failed to get shared model from host.";
            default:
                return "Something went wrong. Please try again.";
        }
    }
    public static string clientGuideMessage = "Please move your phone around hosting area gently";
    public static string hostGuideMessage = "Tap to place object";
    public static string placingObjectByHost = "Host is placing object";
    public static string sharingModel = "Sharing your model";
    public static string receivingModel = "Receiving host model";
    public static string lostTrackingSessionMessage = "Your space session is interrupted. Try to move around more gently to get better invironment";

    public static string CLOUD_ANCHOR_ID_KEY = "CLOUD_ANCHOR_ID_KEY";
    public static float switchModeTimeLoading = 0.5f;
    public static float initModeTimeLoadingForResolveCloudAnchor = 1f;
    public static string failedToJoinRoom = "Fail to join meeting room";
    public static string failedToCreateRoom = "Fail to create room";
    public static string failedToConnectPhoton = "Fail to connect meeting server";
    public static byte CustomManualInstantiationEventCode = 1;
    public static byte EmptyPhotonViewID = 0;
    public static string failedToInitPhotonView = "Failed to synchronize transform";
}
