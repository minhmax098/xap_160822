using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class VerificationCode : MonoBehaviour
{
    private static char[] constant =
    {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
    };

    public static string SetDeleKey(int Length)
    {
        StringBuilder newRandom = new StringBuilder(62);
        System.Random rd = new System.Random();
        for (int i = 0; i < Length; i++)
        {
            newRandom.Append(constant[rd.Next(62)]); // Rd. next (62) returns a nonnegative random number less than 62, and append concatenates the codes with length times random
        }
        return newRandom.ToString();
    }
}
