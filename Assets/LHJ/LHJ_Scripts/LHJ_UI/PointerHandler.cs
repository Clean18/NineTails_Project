using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class PointerHandler : MonoBehaviour , IPointerClickHandler
{
    public event Action<PointerEventData> Click;
    public void OnPointerClick(PointerEventData eventData) => Click?.Invoke(eventData);
}
