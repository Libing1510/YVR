using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ColliderTrigger : MonoBehaviour
{
    public Action<ColliderTrigger, Collider> onTriggerEnterCallback;
    public Action<ColliderTrigger, Collider> onTrriggerStayCallback;
    public Action<ColliderTrigger, Collider> onTriggerExitCallback;

    [SerializeField]
    private string m_triggerName;
    public string triggerName => string.IsNullOrEmpty(m_triggerName) ? m_triggerName = transform.name : m_triggerName;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnterCallback?.Invoke(this, other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTrriggerStayCallback?.Invoke(this, other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExitCallback?.Invoke(this, other);
    }
}
