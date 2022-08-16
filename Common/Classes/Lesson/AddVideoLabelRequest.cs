using System;

[Serializable]
public class AddVideoLabelRequest
{
    public int labelId;
    public string video;
    public void Init(int _labelId, string _video)
    {
        labelId = _labelId;
        video = _video;
    }
}
