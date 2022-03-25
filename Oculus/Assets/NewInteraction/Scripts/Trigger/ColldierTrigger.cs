using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ColldierTrigger : MonoBehaviour
{
    public Action<ColldierTrigger, Collider> onTriggerEnterCallback;
    public Action<ColldierTrigger, Collider> onTrriggerStayCallback;
    public Action<ColldierTrigger, Collider> onTriggerExitCallback;

    [SerializeField]
    private string m_triggerName;
    public string triggerName => m_triggerName ??= transform.name;

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
