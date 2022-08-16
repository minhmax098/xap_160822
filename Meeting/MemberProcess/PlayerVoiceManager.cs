using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerVoiceManager : MonoBehaviourPunCallbacks
{
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare interactive UI for event triggers
    /// </summary>
    public Image microImage;
    private Button btnMicro;
    /// <summary>
    /// Author: sonvdh
    /// Purpose: Declare player of component that corresponding to photon player
    /// </summary>
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        InitUI();
        InitEvents();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init UI for micro component
    /// </summary>
    void InitUI()
    {
        if (this.gameObject.GetComponent<Button>() == null)
        {
            this.gameObject.AddComponent<Button>();
        }
        btnMicro = this.gameObject.GetComponent<Button>();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Set photon player that corresponding to current player of component
    /// </summary>
    /// <param name="playerObject"></param>
    public void SetPlayer(Player playerObject)
    {
        player = playerObject;
        UpdateMicroUI(player.IsMuted());
    }


    /// <summary>
    /// Author: sonvdh
    /// Purpose: Add event triggers for UI
    /// </summary>
    void InitEvents()
    {
        btnMicro.onClick.AddListener(ToggleMicro);
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Toggle micro state (by host or by local player)
    /// </summary>
    void ToggleMicro()
    {
        if (player == null)
        {
            return;
        }
        if ((player.IsLocal && !player.IsMutedByHost()) || PhotonNetwork.IsMasterClient)
        {
            bool isMutingMicro = player.IsMuted();
            if (isMutingMicro)
            {
                player.Unmute();
            }
            else
            {
                player.Mute();
            }
            MeetingMembersManager.Instance.SetPlayerVoice(!isMutingMicro, player);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update UI for new micro state
    /// </summary>
    /// <param name="isMute">New state of micro state: mute or unmute</param>
    public void UpdateMicroUI(bool isMute)
    {
        if (isMute)
        {
            microImage.sprite = Resources.Load<Sprite>(PathConfig.MICRO_OFF_IMAGE);
        }
        else
        {
            microImage.sprite = Resources.Load<Sprite>(PathConfig.MICRO_ON_IMAGE);
        }
        if (PhotonNetwork.LocalPlayer == player)
        {
            MeetingMembersManager.Instance.UpdateUIForMyMicro(isMute);
        }
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Update micro UI according to micro state (detect by player property: IS_MUTE_KEY)
    /// </summary>
    /// <param name="targetPlayer">Player have property updated</param>
    /// <param name="changedProps">Properties that changed</param>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer != null && targetPlayer == player)
        {
            if (changedProps.ContainsKey(MeetingConfig.IS_MUTE_KEY))
            {
                UpdateMicroUI((bool)changedProps[MeetingConfig.IS_MUTE_KEY]);
            }
        }
    }
}
