using System;

[Serializable]
public class SignUpRequest
{
    public string email;
    public string fullName;
    public string password;
    public string confirmPassword;
    public void Init(string _email, string _fullName, string _password, string _confirmPassword)
    {
        email = _email;
        fullName = _fullName;
        password = _password;
        confirmPassword = _confirmPassword;
    }
}

