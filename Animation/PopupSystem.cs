using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class PopupSystem : MonoBehaviour
{
    public GameObject popUpBox; //
    public Animator animator; //
    public TMP_Text popUpText; 

    // Start is called before the first frame update
    public void PopUp(string text)
    {
        popUpBox.SetActive(true);
        popUpText.text = text; 
        animator.SetTrigger("pop");
        animator.SetTrigger("wait");
        animator.SetTrigger("close");
    }
}
