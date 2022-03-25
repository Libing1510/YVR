using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Button_3D : MonoBehaviour
{
    private Button m_button;
    private Vector3 m_scale;
    private PointerTrigger m_pointerTrigger;
    private bool m_select;
    private bool m_click;
    private bool m_touch;
    [SerializeField]
    private ColliderTrigger m_proximityTrigger;
    [SerializeField]
    private ColliderTrigger m_contactTrigger;
    [SerializeField]
    private ColliderTrigger m_actionTrigger;
    public UnityEngine.Events.UnityEvent onClick;

    private void Start()
    {
        m_button = GetComponentInChildren<Button>();
        m_scale = transform.localScale;
        m_pointerTrigger = GetComponentInChildren<PointerTrigger>();
        m_pointerTrigger.onPointerEnterCallback += pointerData => { m_select = true; m_button.OnPointerEnter(pointerData); };
        m_pointerTrigger.onPointerExitCallback += (pointerData => { m_select = false; m_button.OnPointerExit(pointerData); });
        m_pointerTrigger.onPointerDownCallback += (pointerData => { m_click = true; m_button.OnPointerDown(pointerData); });
        m_pointerTrigger.onPointerUpCallback += (pointerData => { m_click = false; m_button.OnPointerUp(pointerData); });
        m_pointerTrigger.onPointerClickCallback += (pointerData => { Debug.Log($"{transform.name} click"); m_button.OnPointerClick(pointerData);  });


        m_proximityTrigger.onTriggerEnterCallback += (trigger, collider) =>
        {
            if (collider.name.Equals("HandFingerPointer"))
            {
                m_proximityTrigger.enabled = false;
                m_touch = true;
                m_button.OnPointerEnter(new PointerEventData(EventSystem.current));
                OnDown(0.1f);
            }
        };

        m_proximityTrigger.onTriggerExitCallback += (trigger, collider) =>
        {
            if (collider.name.Equals("HandFingerPointer"))
            {
                m_proximityTrigger.enabled = true;
                m_touch = false;
                m_button.OnPointerEnter(new PointerEventData(EventSystem.current));
            }
        };

        m_contactTrigger.onTriggerEnterCallback += (trigger, collider) =>
        {
            if (collider.name.Equals("HandFingerPointer"))
            {
                OnDown(0.5f);
            }
        };
        m_contactTrigger.onTriggerExitCallback += (trigger, collider) =>
        {
            if (collider.name.Equals("HandFingerPointer"))
            {
                OnDown(0.1f);
            }
        };

        m_actionTrigger.onTriggerEnterCallback += (trigger, collider) =>
        {
            if (collider.name.Equals("HandFingerPointer"))
            {
                OnDown(0.85f);
                m_button.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        };

        m_actionTrigger.onTriggerExitCallback += (trigger, collider) =>
        {
            if (collider.name.Equals("HandFingerPointer"))
            {
                OnDown(0.5f);
                m_button.OnPointerClick(new PointerEventData(EventSystem.current));
            }
        };

    }

    private void OnEnable()
    {
        m_touch = false;
    }

    private void Update()
    {
        if (m_touch)
            return;

        if (m_click)
        {
            OnDown(0.85f);
        }
        else if (m_select)
        {
            OnDown(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));
        }
        else
        {
            OnDown(0);
        }
        transform.GetChild(0).localPosition = Vector3.forward * -0.005f;
    }

    private void OnDown(float value)
    {
        m_scale.z = 1 - value;
        transform.localScale = m_scale;
    }


}
