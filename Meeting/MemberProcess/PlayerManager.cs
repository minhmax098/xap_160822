using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviourPunCallbacks
{
    public Text playerName; // player name
    private Player player; // photon player that corresponding to current player of component
    public PlayerVoiceManager playerVoiceManager; // voice manager of current player

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Get current player of component
    /// </summary>
    /// <returns></returns>
    public Player GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Setup voice component of current player
    /// </summary>
    /// <param name="playerObject"></param>
    public void SetPlayer(Player playerObject)
    {
        player = playerObject;
        // Set up micro component
        playerVoiceManager.SetPlayer(playerObject);
        InitPlayer();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Init info for current player
    /// </summary>
    private void InitPlayer()
    {
        SetPlayerName();
    }

    /// <summary>
    /// Author: sonvdh
    /// Purpose: Display player name and title (host/you) of current player
    /// </summary>
    private void SetPlayerName()
    {
        if (player == null)
        {
            return;
        }
        string name = player.NickName;
        int acceptedLength = MeetingConfig.maxLengthOfPlayerName;
        if (player.IsLocal) 
        {
            acceptedLength = acceptedLength -  MeetingConfig.localPlayerTitleLength;
        }
        else if (player.IsOrganizer())
        {
            acceptedLength = acceptedLength - MeetingConfig.organizerNameLength;
        }
        if (name.Length > acceptedLength)
        {
            name = name.Substring(0, acceptedLength - MeetingConfig.dotLength) + MeetingConfig.dot;
        }
        if (player.IsLocal) 
        {
            name += MeetingConfig.localPlayerTitle;
        }
        else if (player.IsOrganizer())
        {
            name += MeetingConfig.organizerName;
        }
        playerName.text = name;
    }
}
