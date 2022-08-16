using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class OwnershipTransferring : MonoBehaviourPun, IPunOwnershipCallbacks
{
     #region Singleton Instantiation
        private static OwnershipTransferring instance;
        public static OwnershipTransferring Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<OwnershipTransferring>();
                }
                return instance;
            }
        }

    #endregion Singleton Instantiation

    #region MonoBehaviourPun method

        private void Awake()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }
        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

    #endregion MonoBehaviourPun method

    #region IPunOwnershipCallbacks method
        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            if (NotifyMeetingManager.Instance.listRequestPlayer.IndexOf(requestingPlayer) == -1)
            {
                NotifyMeetingManager.Instance.listRequestPlayer.Add(requestingPlayer);
                NotifyMeetingManager.Instance.DisplayListRequestPlayers();
                NotifyMeetingManager.Instance.ChangeUIWithNotification();
            }
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            PhotonConnectionManager.Instance.SyncResetMemberListPanel();
            PhotonConnectionManager.Instance.SyncResetUINotify();
            PlayerPrefs.SetString("Host", previousOwner.NickName);
            PlayerPrefs.Save();
            PhotonConnectionManager.Instance.PushNotifyChangeHostSuccessfully();
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            Debug.Log("Error change host failed");
        }

        public void ChangeMaster(PhotonView targetView, Player requestingPlayer) 
        {
            targetView.TransferOwnership(requestingPlayer);
            PhotonNetwork.SetMasterClient(requestingPlayer);
            PhotonConnectionManager.Instance.SetActionStatusValue(MeetingConfig.HOST_ACTOR_NUMBER_KEY, requestingPlayer.ActorNumber);
        }

    #endregion IPunOwnershipCallbacks method
}
