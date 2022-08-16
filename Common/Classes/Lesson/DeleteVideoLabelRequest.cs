using System;

[Serializable]
public class DeleteVideoLabelRequest
{
    public int lessonId;
    public int labelId;
    public string video;
    public void Init (int _lessonId, int _labelId, string _video)
    {
        lessonId = _lessonId;
        labelId = _labelId;
        video = _video;
    }
}
