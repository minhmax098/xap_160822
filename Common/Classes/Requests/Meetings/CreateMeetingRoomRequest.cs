using System;

[Serializable]
public class CreateMeetingRoomRequest
{
    public int lessonId;
}

[Serializable]
public class CreateMeetingRoomResponseInfo
{
    public string roomCode;
}
