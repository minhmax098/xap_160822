using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BuildLesson 
{ 
    public class HandlerEditLabel : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Game Object Clicked: "); 
            gameObject.transform.parent.gameObject.SetActive(false);
            // TagHandler.Instance.ResetEditLabelIndex();
            TagHandler.Instance.ShowHideCurrentLabel(true);
        }
    }
}
