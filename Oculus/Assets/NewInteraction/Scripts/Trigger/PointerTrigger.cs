using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PointerTrigger : ColliderTrigger, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    public Action<PointerEventData> onPointerEnterCallback;
    public Action<PointerEventData> onPointerExitCallback;
    public Action<PointerEventData> onPointerDownCallback;
    public Action<PointerEventData> onPointerUpCallback;
    public Action<PointerEventData> onPointerClickCallback;


    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        onPointerEnterCallback?.Invoke(eventData);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        onPointerDownCallback?.Invoke(eventData);
        Debug.Log($"{triggerName}:{transform.localPosition.ToString("f5")}");
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        onPointerEnterCallback?.Invoke(eventData);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        onPointerExitCallback?.Invoke(eventData);
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        onPointerUpCallback?.Invoke(eventData);
    }
}
