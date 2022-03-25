using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class MoveAndScale : MonoBehaviour
{
    [System.Serializable]
    internal class HandController
    {
        public HandType handType;
        public bool touchMove;
        public bool touchScale;
        public Transform trigger;
        public Transform target;
        public bool press;
        public bool release;
        public bool grip;
    }

    private float m_lastDistance;
    private Vector3 m_lastScale;
    [SerializeField]
    private HandController m_leftHand;
    [SerializeField]
    private HandController m_rightHand;


    public Action<bool> onMoveTrigger;
    public Action<bool> onMove;
    public Action<ColliderTrigger, bool> onScaleTrigger;
    public Action<ColliderTrigger> onScale;

    private void Start()
    {
        m_leftHand = new HandController();
        m_leftHand.handType = HandType.LeftHand;
        m_rightHand = new HandController();
        m_rightHand.handType = HandType.RightHand;

        var triggers = new List<ColliderTrigger>(GetComponentsInChildren<ColliderTrigger>());
        triggers.ForEach(trigger =>
        {
            if (trigger.name.Contains("Move"))
            {
                trigger.onTriggerEnterCallback += OnMoveEnter;
                trigger.onTriggerExitCallback += OnMoveExit;
            }
            if (trigger.name.Contains("Scale"))
            {
                trigger.onTriggerEnterCallback += OnScaleEnter;
                trigger.onTriggerExitCallback += OnScaleExit;
            }
        });
    }

    private void Update()
    {
        UpdateKeyCode();
        if (!UpdateScale())
        {
            if (UpdateMove())
            {
                onMove?.Invoke(true);
                Debug.Log("move");
            }
        }
        else
        {
            Debug.Log("scale");
            onScale?.Invoke(m_leftHand.trigger.GetComponent<ColliderTrigger>());
            onScale?.Invoke(m_rightHand.trigger.GetComponent<ColliderTrigger>());
        }

    }

    private bool UpdateMove()
    {
        bool result = false;
        if (m_rightHand.touchMove && m_rightHand.grip)
        {
            if (transform.parent || !m_rightHand.target.Equals(transform.parent))
            {
                transform.SetParent(m_rightHand.target);
            }
            result = true;
        }
        else if (m_leftHand.touchMove && m_leftHand.grip)
        {
            if (transform.parent == null || !m_leftHand.target.Equals(transform.parent))
            {
                transform.SetParent(m_leftHand.target);
            }
            result = true;
        }
        else
        {
            transform.SetParent(null);
            result = false;
        }
        return result;

    }

    private bool UpdateScale()
    {
        if (m_rightHand.touchScale && m_rightHand.grip && m_leftHand.touchScale && m_leftHand.grip)
        {
            float distance = Vector3.Distance(m_leftHand.target.position, m_rightHand.target.position);

            if (m_lastDistance < 0)
            {
                m_lastDistance = distance;
                m_lastScale = transform.localScale;
            }
            float scale = distance / m_lastDistance;
            transform.localScale = new Vector3(m_lastScale.x * scale, m_lastScale.y * scale, m_lastScale.z * 1);
            Debug.Log($"scale:{scale},{transform.localScale.ToString("f4")}");
            return true;
        }
        else
        {
            if (m_lastDistance > 0)
            {
                m_rightHand.touchScale = false;
                //m_rightHand.trigger.GetComponent<MeshRenderer>().enabled = false;
                onScaleTrigger?.Invoke(m_rightHand.trigger.GetComponent<ColliderTrigger>(), false);
                m_rightHand.trigger = null;
                m_rightHand.target = null;


                m_leftHand.touchScale = false;
                //m_leftHand.trigger.GetComponent<MeshRenderer>().enabled = false;
                onScaleTrigger?.Invoke(m_leftHand.trigger.GetComponent<ColliderTrigger>(), false);
                m_leftHand.trigger = null;
                m_leftHand.target = null;
            }
            m_lastDistance = -1.0f;
        }
        return false;
    }
    public bool left;
    public bool right;

    private void UpdateKeyCode()
    {
        var leftPress = OVRInput.GetDown(OVRInput.RawButton.LHandTrigger) || Input.GetKeyDown(KeyCode.L) || left;
        var leftRelease = OVRInput.GetUp(OVRInput.RawButton.LHandTrigger) || Input.GetKeyUp(KeyCode.L);
        float leftAxis = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger) + (Input.GetKey(KeyCode.L) || left ? 1 : 0);
        m_leftHand.press = leftPress;
        m_leftHand.release = leftRelease;
        m_leftHand.grip = leftAxis > 0.2f;

        var rightPress = OVRInput.GetDown(OVRInput.RawButton.RHandTrigger) || Input.GetKeyDown(KeyCode.R) || right;
        var rightRelease = OVRInput.GetUp(OVRInput.RawButton.RHandTrigger) || Input.GetKeyUp(KeyCode.R);
        float rightAxis = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger) + (Input.GetKey(KeyCode.R) || right ? 1 : 0);
        m_rightHand.press = rightPress;
        m_rightHand.release = rightRelease;
        m_rightHand.grip = rightAxis > 0.2f;
    }



    private void OnMoveEnter(ColliderTrigger trigger, Collider collider)
    {
        var touch = collider.GetComponent<Touch>();
        if (touch)
        {
            HandController handController = GetHandController(touch.hand);
            if (handController != null && !handController.touchScale)
            {
                handController.touchMove = true;
                handController.trigger = trigger.transform;
                handController.target = collider.transform;
                //trigger.GetComponent<MeshRenderer>().enabled = true;
                onMoveTrigger?.Invoke(true);
            }
        }
    }

    private void OnMoveExit(ColliderTrigger trigger, Collider collider)
    {
        var touch = collider.GetComponent<Touch>();
        if (touch)
        {
            HandController handController = GetHandController(touch.hand);
            if (handController != null && handController.touchMove)
            {
                handController.touchMove = false;
                handController.trigger = null;
                handController.target = null;
                //trigger.GetComponent<MeshRenderer>().enabled = false;
                onMoveTrigger?.Invoke(false);
            }
        }
    }

    private void OnScaleEnter(ColliderTrigger trigger, Collider collider)
    {
        var touch = collider.GetComponent<Touch>();
        if (touch)
        {
            HandController handController = GetHandController(touch.hand);
            if (handController != null && !handController.touchMove)
            {
                handController.touchScale = true;
                handController.trigger = trigger.transform;
                handController.target = collider.transform;
                //trigger.GetComponent<MeshRenderer>().enabled = true;
                onScaleTrigger?.Invoke(trigger, true);
            }
        }
    }

    private void OnScaleExit(ColliderTrigger trigger, Collider collider)
    {
        var touch = collider.GetComponent<Touch>();
        if (touch)
        {
            HandController handController = GetHandController(touch.hand);
            if (handController != null && handController.touchScale && m_lastDistance < 0)
            {
                handController.touchScale = false;
                handController.trigger = null;
                handController.target = null;
                //trigger.GetComponent<MeshRenderer>().enabled = false;
                onScaleTrigger?.Invoke(trigger, false);
            }
        }
    }

    private HandController GetHandController(HandType handType)
    {
        switch (handType)
        {
            case HandType.LeftHand:
                return m_leftHand;
            case HandType.RightHand:
                return m_rightHand;
            default:
                return null;
        }
    }



}
