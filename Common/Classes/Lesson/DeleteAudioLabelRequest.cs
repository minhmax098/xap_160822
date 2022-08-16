using System;

[Serializable]
public class DeleteAudioLabelRequest
{
    public int lessonId;
    public int labelId;
    public string audio;
    public void Init (int _lessonId, int _labelId, string _audio)
    {
        lessonId = _lessonId;
        labelId = _labelId;
        audio = _audio;
    }
}
