using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : Button
{
    // Source: https://forum.unity.com/threads/deselect-button-on-mouse-exit.1298619/
    
    UnityEvent onpointerExit { get;set; }
 
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        OnDeselect(eventData);
    }
}
