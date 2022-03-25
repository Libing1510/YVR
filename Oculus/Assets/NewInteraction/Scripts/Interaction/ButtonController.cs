using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YVR.Interaction
{
    public class ButtonController : Interactable
    {
        #region
        [SerializeField]
        private GameObject m_proximityZone = null;
        [SerializeField]
        private GameObject m_contactZone = null;
        [SerializeField]
        private GameObject m_actionZone = null;

        // for positive side tests, the contact position must be on the positive side of the plane
        // determined by this transform
        [SerializeField]
        private Transform m_buttonPlaneCenter = null;
        // make sure press is coming from "positive" side of button, i.e. above it
        [SerializeField]
        private bool m_makeSureToolIsOnPositiveSide = true;
        // depending on the geometry used, the direction might not always be downwards.
        [SerializeField]
        private Vector3 m_localButtonDirection = Vector3.down;
        [SerializeField]
        private InteractableToolTags[] m_allValidToolsTags = new InteractableToolTags[] { InteractableToolTags.All };
        [SerializeField]
        private bool m_allowMultipleNearFieldInteraction = false;

        private Dictionary<InteractableTool, InteractableState> m_toolToState =
  new Dictionary<InteractableTool, InteractableState>();

        private int m_toolTagsMask;
        public override int validToolTagsMask => m_toolTagsMask;
        public InteractableState currentButtonState { get; private set; } = InteractableState.Default;

        #endregion


        #region
        protected override void Awake()
        {
            base.Awake();
            Assert.IsNotNull(m_proximityZone);
            Assert.IsNotNull(m_contactZone);
            Assert.IsNotNull(m_actionZone);
            Assert.IsNotNull(m_buttonPlaneCenter);
            foreach (var interactableToolTags in m_allValidToolsTags)
            {
                m_toolTagsMask |= (int)interactableToolTags;
            }
            m_proximityZoneCollider = m_proximityZone.GetComponent<IColliderZone>();
            m_contactZoneCollider = m_contactZone.GetComponent<IColliderZone>();
            m_actionZoneCollider = m_actionZone.GetComponent<IColliderZone>();
        }

        private void FireInteractionEventsOnDepth(InteractableCollisionDepth oldDepth,
    InteractableTool collidingTool, InteractionType interactionType)
        {
            Debug.Log($"onaction:{oldDepth},{interactionType}");
            switch (oldDepth)
            {
                case InteractableCollisionDepth.Action:
                    OnActionZoneEvent(new ColliderZoneArgs(actionCollider, Time.frameCount,
                      collidingTool, interactionType));
                    break;
                case InteractableCollisionDepth.Contact:
                    OnContactZoneEvent(new ColliderZoneArgs(contactCollider, Time.frameCount,
                      collidingTool, interactionType));
                    break;
                case InteractableCollisionDepth.Proximity:
                    OnProximityZoneEvent(new ColliderZoneArgs(proximityCollider, Time.frameCount,
                      collidingTool, interactionType));
                    break;
            }
        }


        public override void UpdateCollisionDepth(InteractableTool interactableTool, InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth)
        {

            bool isFarFieldTool = interactableTool.isFarFieldTool;

            // if this is a near field tool and another tool already controls it, bail.
            // (assuming we are not allowing multiple near field tools)
            bool testForSingleToolInteraction = !isFarFieldTool &&
              !m_allowMultipleNearFieldInteraction;

            if (testForSingleToolInteraction && m_toolToState.Keys.Count > 0 && !m_toolToState.ContainsKey(interactableTool))
                return;

            InteractableState oldState = currentButtonState;

            var currButtonDirection = transform.TransformDirection(m_localButtonDirection);
            bool validContact = IsValidContact(interactableTool, currButtonDirection) || interactableTool.isFarFieldTool;

            // in case tool enters contact zone first, we are in proximity as well
            bool toolIsInProximity = newCollisionDepth >= InteractableCollisionDepth.Proximity;
            bool toolInContactZone = newCollisionDepth == InteractableCollisionDepth.Contact;
            bool toolInActionZone = newCollisionDepth == InteractableCollisionDepth.Action;

            bool switchingStates = oldCollisionDepth != newCollisionDepth;
            if (switchingStates)
            {
                FireInteractionEventsOnDepth(oldCollisionDepth, interactableTool,
                    InteractionType.Exit);
                FireInteractionEventsOnDepth(newCollisionDepth, interactableTool,
                    InteractionType.Enter);
            }
            else
            {
                FireInteractionEventsOnDepth(newCollisionDepth, interactableTool,
                    InteractionType.Stay);
            }

            var upcomingState = oldState;
            if (interactableTool.isFarFieldTool)
            {
                upcomingState = toolInContactZone ? InteractableState.ContactState :
                  toolInActionZone ? InteractableState.ActionState : InteractableState.Default;
            }
            else
            {
                // plane describing positive side of button
                var buttonZonePlane = new Plane(-currButtonDirection, m_buttonPlaneCenter.position);

                // skip plane test if the boolean flag tells us not to test it
                bool onPositiveSideOfButton = !m_makeSureToolIsOnPositiveSide ||
                  buttonZonePlane.GetSide(interactableTool.interactionPosition);
                upcomingState = GetUpcomingStateNearField(oldState, newCollisionDepth, toolInActionZone, toolInContactZone, toolIsInProximity, validContact, onPositiveSideOfButton);

            }

            if (upcomingState != InteractableState.Default)
            {
                m_toolToState[interactableTool] = upcomingState;
            }
            else
            {
                m_toolToState.Remove(interactableTool);
            }

            // far field tools depend on max state set(or if proper flag is set for near field tools)
            bool setMaxStateForAllTools = isFarFieldTool || m_allowMultipleNearFieldInteraction;
            if (setMaxStateForAllTools)
            {
                foreach (var toolState in m_toolToState.Values)
                {
                    if (upcomingState < toolState)
                    {
                        upcomingState = toolState;
                    }
                }
            }

            if (oldState != upcomingState)
            {
                currentButtonState = upcomingState;
                var interactionType = !switchingStates ? InteractionType.Stay : newCollisionDepth == InteractableCollisionDepth.None ? InteractionType.Exit : InteractionType.Enter;
                IColliderZone currentCollider = null;
                switch (currentButtonState)
                {
                    case InteractableState.ProximityState:
                        currentCollider = proximityCollider;
                        break;
                    case InteractableState.ContactState:
                        currentCollider = contactCollider;
                        break;
                    case InteractableState.ActionState:
                        currentCollider = actionCollider;
                        break;
                    default:
                        currentCollider = null;
                        break;
                }
                interactableStateChanged?.Invoke(new InteractableStateArgs(this, interactableTool, currentButtonState, oldState, new ColliderZoneArgs(currentCollider, Time.frameCount, interactableTool, interactionType)));

            }

        }

        /// <summary>
        /// 根据上一个状态和触发器获取新的状态
        /// </summary>
        /// <param name="oldState">旧的状态</param>
        /// <param name="newCollisionDepth">新的碰撞器深度</param>
        /// <param name="toolIsInActionZone">触发工具是否在Action区域</param>
        /// <param name="toolIsInContactZone">触发工具是否在接触区域</param>
        /// <param name="toolIsInProximity">触发工具是否在刚接近区域</param>
        /// <param name="validContact">接触是否有效</param>
        /// <param name="onPositiveSideOfInteractable">是否在可交换物正面</param>
        /// <returns></returns>
        private InteractableState GetUpcomingStateNearField(InteractableState oldState, InteractableCollisionDepth newCollisionDepth, bool toolIsInActionZone, bool toolIsInContactZone, bool toolIsInProximity, bool validContact, bool onPositiveSideOfInteractable)
        {
            InteractableState upcomingState = oldState;

            switch (oldState)
            {
                case InteractableState.Default:
                    // test contact, action first then proximity (more important states take precedence)
                    if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
                    {
                        upcomingState = newCollisionDepth == InteractableCollisionDepth.Action ? InteractableState.ActionState : InteractableState.ContactState;
                    }
                    else if (toolIsInProximity)
                    {
                        upcomingState = InteractableState.ProximityState;
                    }
                    break;
                case InteractableState.ProximityState:
                    if (newCollisionDepth < InteractableCollisionDepth.Proximity)
                    {
                        upcomingState = InteractableState.Default;
                    }
                    else if (validContact && onPositiveSideOfInteractable && newCollisionDepth > InteractableCollisionDepth.Proximity)
                    {
                        upcomingState = newCollisionDepth == InteractableCollisionDepth.Action ? InteractableState.ActionState : InteractableState.ContactState;
                    }
                    break;
                case InteractableState.ContactState:
                    if (newCollisionDepth < InteractableCollisionDepth.Contact)
                    {
                        upcomingState = toolIsInProximity ? InteractableState.ProximityState : InteractableState.Default;
                    }
                    // can only go to action state if contact is legal
                    // if tool goes into contact state due to proper movement, but does not maintain
                    // that movement throughout (i.e. a tool/finger presses downwards initially but
                    // moves in random directions afterwards), then don't go into action
                    else if (toolIsInActionZone && validContact && onPositiveSideOfInteractable)
                    {
                        upcomingState = InteractableState.ActionState;
                    }
                    break;
                case InteractableState.ActionState:
                    if (!toolIsInActionZone)
                    {
                        // if retreating from action, can go back into action state even if contact
                        // is not legal (i.e. tool/finger retracts)
                        if (toolIsInContactZone)
                        {
                            upcomingState = InteractableState.ContactState;
                        }
                        else if (toolIsInProximity)
                        {
                            upcomingState = InteractableState.ProximityState;
                        }
                        else
                        {
                            upcomingState = InteractableState.Default;
                        }
                    }
                    break;
                default:
                    break;
            }
            return upcomingState;
        }

        private bool IsValidContact(InteractableTool collidingTool, Vector3 buttonDirection)
        {
            if (m_contactZone == null || collidingTool.isFarFieldTool) return true;

            return false;

        }


        #endregion

    }

}

