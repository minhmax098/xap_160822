using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class AuthenConfig
{
    public static string emptyValue = "Please enter the information!";
    public static string invalidCapcha = "Incorrect capcha";
    public static string invalidEmail = "Email format incorrect";
    public static string invalidSecretCode = "6 digitals long";
    public static int secretCodeLength = 6;
    public static string invalidPassword = "8-20 characters long, including uppercase, lowercase letters, numbers and special character";
    public static string invalidPasswordConfirmation = "Confirm password does not match password. ";
    public static Regex emailRgx = new Regex(@"^(([^<>()[\]\\.,;:\s@']+(\.[^<>()[\]\\.,;:\s@']+)*)|('.+'))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$");
    public static Regex passRgx = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$");

}
