using System;

[Serializable]
public class SignInRequest
{
    public string email;
    public string password;
    public void Init(string _email, string _password)
    {
        email = _email;
        password = _password;
    }
}

