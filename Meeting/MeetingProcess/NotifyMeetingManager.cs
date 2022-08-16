using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using EasyUI.Toast;


public class NotifyMeetingManager : MonoBehaviour
{
    #region Singleton Instantiation
        private static NotifyMeetingManager instance;
        public static NotifyMeetingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<NotifyMeetingManager>();
                }
                return instance;
            }
        }

    #endregion Singleton Instantiation

    #region Identification UI
        public const int MAX_NUMBER_LETTERS = 25;

        public Button btnNotify;
        public GameObject countNotification;
        public GameObject countNotificationGlobal;
        public Button btnNotifyGlobal;
        public GameObject listNotifications;
        public GameObject contentNotifications;
        public Button btnExitListNotifications;

        public GameObject popUpConfirmChangeHost;
        public Button btnCloseExitPopup;
        public Button btnAgree;
        public Button btnCancel;
        public GameObject nodata;

        public Text txtContentPopup;

    #endregion Identification UI

    #region Identification Variable

        Player selectedPlayerChange;
        GameObject selectedItemNotify;
        public List<Player> listRequestPlayer = new List<Player>();

    #endregion Identification Variable

    #region MonoBehaviour methods
        void Start()
        {
            InitUI();
            InitEvents();
        }

        void Update()
        {
            btnNotify.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            // btnNotifyGlobal.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

    #endregion MonoBehaviour methods

    #region Private methods

        void InitUI()
        {
            btnNotify.gameObject.SetActive(PhotonNetwork.IsMasterClient);
            btnNotifyGlobal.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        }

        void InitEvents()
        {
            btnNotify.onClick.AddListener(ShowListNotifications);
            btnNotifyGlobal.onClick.AddListener(ShowListNotifications);
            btnExitListNotifications.onClick.AddListener(HiddenListNotifications);
            btnCloseExitPopup.onClick.AddListener(HiddenPopupConfirmChangeHost);
            btnAgree.onClick.AddListener( delegate { HandleChangeHost(selectedPlayerChange, selectedItemNotify); });
            btnCancel.onClick.AddListener(HiddenPopupConfirmChangeHost);
        }

        void ShowListNotifications()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                listNotifications.SetActive(true);
                countNotification.SetActive(false);
                countNotificationGlobal.SetActive(false);
                if (listRequestPlayer.Count > 0)
                {
                    nodata.SetActive(false);
                    DisplayListRequestPlayers();
                }
                else
                {
                    nodata.SetActive(true); 
                }
            }
        }

        void HiddenListNotifications()
        {
            listNotifications.SetActive(false);
        }

        void HiddenPopupConfirmChangeHost()
        {
            popUpConfirmChangeHost.SetActive(false);
        }

        void HandleChangeHost(Player selectedPlayerChange, GameObject selectedItemNotify)
        {
            OwnershipTransferring.Instance.ChangeMaster(ObjectManager.Instance.OriginObject.GetComponent<PhotonView>(), selectedPlayerChange);
            Destroy(selectedItemNotify);
        }

        void ClearItemNotification()
        {
            foreach (Transform itemNotify in contentNotifications.transform)
            {
                Destroy(itemNotify.gameObject);
            }
        }

        void ShowPopUpConfirm(Player player, GameObject item)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                listNotifications.SetActive(false);
            }
            popUpConfirmChangeHost.SetActive(true);
            txtContentPopup.text = MeetingConfig.contentPopupConfirm + $"<b>{Helper.ShortString(player.NickName, MAX_NUMBER_LETTERS)}</b>" + MeetingConfig.characterQuestion;

            selectedPlayerChange = player;
            selectedItemNotify = item;
        }

        public void ChangeUIWithNotification()
        {
            countNotification.SetActive(true);
            countNotification.transform.GetChild(0).GetComponent<Text>().text = listRequestPlayer.Count.ToString();
            countNotificationGlobal.SetActive(true);
            countNotificationGlobal.transform.GetChild(0).GetComponent<Text>().text = listRequestPlayer.Count.ToString();
        }

    
    #endregion Private methods

    #region Pubblic methods

        public void DisplayListRequestPlayers()
        {
            ClearItemNotification();
            foreach(Player itemPlayer in listRequestPlayer)
            {
                GameObject item = Instantiate(Resources.Load(PathConfig.MODEL_ITEM_NOTIFY) as GameObject);
                item.transform.SetParent(contentNotifications.transform, false);
                item.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = $"<b>{Helper.ShortString(itemPlayer.NickName, MAX_NUMBER_LETTERS)}</b>" + MeetingConfig.requestNotification;
                item.transform.GetComponent<Button>().onClick.AddListener(delegate { ShowPopUpConfirm(itemPlayer, item); });
            }
        }

    #endregion Pubblic methods
    

    #region Photon PUN methods  

        [PunRPC]
        public void ResetUINotify()
        {
            popUpConfirmChangeHost.SetActive(false);
            countNotification.SetActive(false);
            countNotificationGlobal.SetActive(false);
            listRequestPlayer.Clear();       
            ClearItemNotification();
        }

        [PunRPC]
        public void ToastNotifyChangeHostSuccessfully()
        {
            Toast.ShowCommonToast($"<b>{Helper.ShortString(PlayerPrefs.GetString("Host"),MAX_NUMBER_LETTERS)}</b>" +  MeetingConfig.clientChangeHostSuccessfully, APIUrlConfig.SUCCESS_RESPONSE_CODE);
        }
    #endregion Photon PUN methods
}
