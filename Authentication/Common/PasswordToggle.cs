using System.Collections;
using System.Collections.Generic;
using AdvancedInputFieldPlugin;
using UnityEngine;
using UnityEngine.UI; 

public class PasswordToggle : MonoBehaviour
{
    public Button toggleBtn;
    public AdvancedInputField passwordField;
    private bool isOpen = true;
    void Start()
    {
        toggleBtn = transform.GetComponent<Button>();
        toggleBtn.onClick.AddListener(TogglePasswordField);
    }
    void TogglePasswordField()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            toggleBtn.image.sprite = Resources.Load<Sprite>(SpriteConfig.eyeClose);
            passwordField.ContentType = ContentType.STANDARD;
        }
        else
        {
            toggleBtn.image.sprite =  Resources.Load<Sprite>(SpriteConfig.eyeOpen);
            passwordField.ContentType = ContentType.PASSWORD;
        }
    }
}
