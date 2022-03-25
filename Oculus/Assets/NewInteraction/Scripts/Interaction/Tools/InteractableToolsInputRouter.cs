using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YVR.Interaction
{
    /// <summary>
    /// Routes all collisions from interactable tools to the interactables themselves.
    /// We want to do this in a top-down fashion, because we might want to disable
    /// far-field interactions if near-field interactions take precendence (for instance).
    /// </summary>
    public class InteractableToolsInputRouter : MonoBehaviour
    {
        private static InteractableToolsInputRouter s_instance;
        public static InteractableToolsInputRouter instance
        {
            get
            {
                if (s_instance == null)
                {
                    var instances = FindObjectsOfType<InteractableToolsInputRouter>();
                    if (instances.Length > 0)
                    {
                        s_instance = instances[0];
                        // remove extras, if any
                        for (int i = 1; i < instances.Length; i++)
                        {
                            GameObject.Destroy(instances[i].gameObject);
                        }
                    }
                }

                return s_instance;
            }
        }

        private HashSet<InteractableTool> m_leftHandNearTools = new HashSet<InteractableTool>();
        private HashSet<InteractableTool> m_leftHandFarTools = new HashSet<InteractableTool>();
        private HashSet<InteractableTool> m_rightHandNearTools = new HashSet<InteractableTool>();
        private HashSet<InteractableTool> m_rightHandFarTools = new HashSet<InteractableTool>();

        /// <summary>
        /// 注册交互工具（射线，碰撞）
        /// </summary>
        /// <param name="interactableTool"></param>
        public void RegisterInteractableTool(InteractableTool interactableTool)
        {
            if (interactableTool.isRightHandedTool)
            {
                if (interactableTool.isFarFieldTool)
                {
                    m_rightHandFarTools.Add(interactableTool);
                }
                else
                {
                    m_rightHandNearTools.Add(interactableTool);
                }
            }
            else
            {
                if (interactableTool.isFarFieldTool)
                {
                    m_leftHandFarTools.Add(interactableTool);
                }
                else
                {
                    m_leftHandNearTools.Add(interactableTool);
                }
            }
        }

        /// <summary>
        /// 取消交互工具
        /// </summary>
        /// <param name="interactableTool"></param>
        public void UnregisterInteractableTool(InteractableTool interactableTool)
        {
            if (interactableTool.isRightHandedTool)
            {
                if (interactableTool.isFarFieldTool)
                {
                    m_rightHandFarTools.Remove(interactableTool);
                }
                else
                {
                    m_rightHandNearTools.Remove(interactableTool);
                }
            }
            else
            {
                if (interactableTool.isFarFieldTool)
                {
                    m_leftHandFarTools.Remove(interactableTool);
                }
                else
                {
                    m_leftHandNearTools.Remove(interactableTool);
                }
            }
        }

        private void Update()
        {
            UpdateTools();
        }

        protected virtual void UpdateTools()
        {
            bool leftHandIsReliable = OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) || true;
            bool rightHandIsReliable = OVRInput.IsControllerConnected(OVRInput.Controller.RTouch) || true;


            //触摸交互
            bool encounteredNearObjectsLeftHand = UpdateToolsAndEnableState(m_leftHandNearTools, leftHandIsReliable);
            //远程射线交互
            // don't interact with far field if near field is touching something
            UpdateToolsAndEnableState(m_leftHandFarTools, !encounteredNearObjectsLeftHand && leftHandIsReliable);

            //触摸交互
            bool encounteredNearObjectsRightHand = UpdateToolsAndEnableState(m_rightHandNearTools, rightHandIsReliable);
            //远程射线交互
            // don't interact with far field if near field is touching something
            UpdateToolsAndEnableState(m_rightHandFarTools, !encounteredNearObjectsRightHand && rightHandIsReliable);
        }


        /// <summary>
        /// 更新工具的开关状态
        /// </summary>
        /// <param name="tools"></param>
        /// <param name="toolsAreEnabledThisFrame"></param>
        /// <returns></returns>
        private bool UpdateToolsAndEnableState(HashSet<InteractableTool> tools, bool toolsAreEnabledThisFrame)
        {
            bool encounteredObjects = UpdateInteractableTools(tools, toolsAreEnabledThisFrame);
            ToggleToolsEnableState(tools, toolsAreEnabledThisFrame);
            return encounteredObjects;
        }

        private bool UpdateInteractableTools(HashSet<InteractableTool> tools, bool resetCollisionData = false)
        {
            bool toolsEncounteredObjects = false;
            foreach (InteractableTool currentInteractableTool in tools)
            {

                List<InteractableCollisionInfo> intersectingObjectsFound = currentInteractableTool.GetNextIntersectingObjects();
                if (intersectingObjectsFound.Count > 0 && resetCollisionData)
                {
                    if (!toolsEncounteredObjects)
                    {
                        toolsEncounteredObjects = intersectingObjectsFound.Count > 0;
                    }

                    //创建地图，指示每个可交互元素遇到的最远对撞机
                    // create map that indicates the furthest collider encountered per interactable element
                    currentInteractableTool.UpdateCurrentCollisionsBasedOnDepth();

                    if (currentInteractableTool.isFarFieldTool)
                    {
                        var firstInteractable = currentInteractableTool.GetFirstCurrentCollisionInfo();
                        // if our tool is activated, make sure depth is set to "action"
                        firstInteractable.Value.interactableCollider = firstInteractable.Key.actionCollider;

                        firstInteractable.Value.collisionDepth = currentInteractableTool.toolInputState == ToolInputState.PrimaryInputUp ? InteractableCollisionDepth.Action : InteractableCollisionDepth.Contact;

                        // far field tools only can focus elements -- pick first (for now)
                        currentInteractableTool.FocusOnInteractable(firstInteractable.Key, firstInteractable.Value.interactableCollider);
                    }
                }
                else
                {
                    currentInteractableTool.DeFocus();
                    currentInteractableTool.ClearAllCurrentCollisionInfos();
                }

                currentInteractableTool.UpdateLatestCollisionData();
            }

            return toolsEncounteredObjects;
        }

        /// <summary>
        /// 设置所有交互工具状态
        /// </summary>
        /// <param name="tools"></param>
        /// <param name="enableState"></param>
        private void ToggleToolsEnableState(HashSet<InteractableTool> tools, bool enableState)
        {
            foreach (InteractableTool tool in tools)
            {
                if (tool.enableState != enableState)
                {
                    tool.enableState = enableState;
                }
            }
        }
    }
}



