using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandlerDropdownClick : MonoBehaviour, IPointerClickHandler
{
    private bool triggered = false;
    // Dropdown dropdown = gameObject.GetComponent<Dropdown>();
    public void OnPointerClick(PointerEventData pointerEventData)
    {
        // Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            // Output to console the clicked GameObject's name and the following message
            // can replace this with your own actions for when clicking the GameObject.
            Debug.Log(name + " Game Object Right Clicked");
        }
        // Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log(name + " Game Object Left Clicked");
            // Remove the first Options
            if (!triggered)
            {
                triggered = !triggered;
                gameObject.GetComponent<Dropdown>().options.RemoveAt(gameObject.GetComponent<Dropdown>().options.Count - 1);
            }
        }
    }
}
