using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassField : MonoBehaviour
{
    public InputField _inputField;
    void Start()
    {
        _inputField.asteriskChar = "$!£%&*"[5];
    }
    void Update()
    {
        
    }
}
