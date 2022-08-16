using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public static partial class PhotonPlayerHelper
{
    public static bool Mute(this Player player)
    {
        return player.SetCustomProperties(new Hashtable(1) { { MeetingConfig.IS_MUTE_KEY, true } });
    }

    public static bool Unmute(this Player player)
    {
        return player.SetCustomProperties(new Hashtable(1) { { MeetingConfig.IS_MUTE_KEY, false } });
    }

    public static bool IsMuted(this Player player)
    {
        object temp;
        return player.CustomProperties.TryGetValue(MeetingConfig.IS_MUTE_KEY, out temp) && (bool)temp;
    }

    public static bool MuteByHost(this Player player, bool isMutedByHost)
    {
        return player.SetCustomProperties(new Hashtable(1) { { MeetingConfig.IS_MUTED_BY_HOST, isMutedByHost } });
    }

    public static bool IsMutedByHost(this Player player)
    {
        object temp;
        return player.CustomProperties.TryGetValue(MeetingConfig.IS_MUTED_BY_HOST, out temp) && (bool)temp;
    }

    public static bool IsOrganizer(this Player player)
    {
        object temp;
        return player.CustomProperties.TryGetValue(MeetingConfig.IS_ORGANIZER_KEY, out temp) && (bool)temp;
    }

    public static bool SetOrganizer(this Player player)
    {
        return player.SetCustomProperties(new Hashtable(1) { { MeetingConfig.IS_ORGANIZER_KEY, false } });
    }
}
