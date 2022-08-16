using System;

[Serializable]
public class ForgotPassRequest
{
    public string email;
    public void Init(string _email)
    {
        email = _email;
    }
}

