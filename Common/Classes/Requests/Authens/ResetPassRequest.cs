using System;

[Serializable]
public class ResetPassRequest
{
    public string email;
    public int secretCode;
    public string newPassword;
    public string confirmPassword;
    public void Init(string _email, int _secretCode, string _newPassword, string _confirmPassword)
    {
        email = _email;
        secretCode = _secretCode;
        newPassword = _newPassword;
        confirmPassword = _confirmPassword;
    }
}

