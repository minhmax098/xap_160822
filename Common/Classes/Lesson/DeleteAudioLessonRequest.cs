using System;

[Serializable]
public class DeleteAudioLessonRequest
{
    public int lessonId;
    public string audio;
    public void Init(int _lessonId, string _audio)
    {
        lessonId = _lessonId;
        audio = _audio;
    }
}
