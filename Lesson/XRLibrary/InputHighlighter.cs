using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InputHighlighter : MonoBehaviour
{
    private InputField inputField;
    private bool moveCaret = false;
    void Start()
    {
        inputField = GetComponent<InputField>();
    }
    void Update()
    {
        if (inputField.isFocused){
            inputField.ForceLabelUpdate();
            // Debug.Log("Current Carret Position: " + inputField.selectionAnchorPosition);
        }
    }
}
