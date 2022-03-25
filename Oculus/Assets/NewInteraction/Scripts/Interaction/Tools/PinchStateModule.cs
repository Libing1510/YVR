using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YVR.Interaction
{
    public class PinchStateModule
    {
        private const float PINCH_STRENGTH_THRESHOLD = 1.0f;

        private enum PinchState
        {
            None = 0,
            PinchDown,
            PinchStay,
            PinchUp
        }

        private PinchState m_currPinchState;
        private Interactable m_firstFocusedInteractable;

        /// <summary>
        /// We want a pinch up and down gesture to be done **while** an object is focused.
        /// We don't want someone to pinch, unfocus an object, then refocus before doing
        /// pinch up. We also want to avoid focusing a different interactable during this process.
        /// While the latter is difficult to do since a person might focus nothing before
        /// focusing on another interactable, it's theoretically possible.
        /// </summary>
        public bool pinchUpAndDownOnFocusedObject
        {
            get
            {
                return m_currPinchState == PinchState.PinchUp && m_firstFocusedInteractable != null;
            }
        }

        public bool pinchSteadyOnFocusedObject
        {
            get
            {
                return m_currPinchState == PinchState.PinchStay && m_firstFocusedInteractable != null;
            }
        }

        public bool pinchDownOnFocusedObject
        {
            get
            {
                return m_currPinchState == PinchState.PinchDown && m_firstFocusedInteractable != null;
            }
        }

        public PinchStateModule()
        {
            m_currPinchState = PinchState.None;
            m_firstFocusedInteractable = null;
        }

        public void UpdateState(OVRHand hand, Interactable currFocusedInteractable)
        {
            float pinchStrength = hand.GetFingerPinchStrength(OVRHand.HandFinger.Index);
            bool isPinching = Mathf.Abs(PINCH_STRENGTH_THRESHOLD - pinchStrength) < Mathf.Epsilon;
            var oldPinchState = m_currPinchState;

            switch (oldPinchState)
            {
                case PinchState.PinchUp:
                    // can only be in pinch up for a single frame, so consider
                    // next frame carefully
                    if (isPinching)
                    {
                        m_currPinchState = PinchState.PinchDown;
                        if (currFocusedInteractable != m_firstFocusedInteractable)
                        {
                            m_firstFocusedInteractable = null;
                        }
                    }
                    else
                    {
                        m_currPinchState = PinchState.None;
                        m_firstFocusedInteractable = null;
                    }
                    break;
                case PinchState.PinchStay:
                    if (!isPinching)
                    {
                        m_currPinchState = PinchState.PinchUp;
                    }
                    // if object is not focused anymore, then forget it
                    if (currFocusedInteractable != m_firstFocusedInteractable)
                    {
                        m_firstFocusedInteractable = null;
                    }
                    break;
                // pinch down lasts for a max of 1 frame. either go to pinch stay or up
                case PinchState.PinchDown:
                    m_currPinchState = isPinching ? PinchState.PinchStay : PinchState.PinchUp;
                    // if the focused interactable changes, then the original one is now invalid
                    if (m_firstFocusedInteractable != currFocusedInteractable)
                    {
                        m_firstFocusedInteractable = null;
                    }
                    break;
                default:
                    if (isPinching)
                    {
                        m_currPinchState = PinchState.PinchDown;
                        // this is the interactable that must be focused through out the pinch up and down
                        // gesture.
                        m_firstFocusedInteractable = currFocusedInteractable;
                    }
                    break;
            }
        }

    }
}



