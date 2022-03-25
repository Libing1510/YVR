using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YVR.Interaction
{
    /// <summary>
    /// Allows a bone to keep track of interactables that it has touched. This information
    /// can be used by a tool.
    /// </summary>
    public class BoneCapsuleTriggerLogic : MonoBehaviour
    {
        public InteractableToolTags toolTags;
        public HashSet<IColliderZone> collidersTouchingUs = new HashSet<IColliderZone>();
        private List<IColliderZone> m_elementsToCleanUp = new List<IColliderZone>();

        /// <summary>
        /// If we get disabled, clear our colliders. Otherwise, on trigger exit may not get called.
        /// </summary>
        private void OnDisable()
        {
            collidersTouchingUs.Clear();
        }

        private void OnTriggerEnter(Collider other)
        {
            var triggerZone = other.GetComponent<ButtonTriggerZone>();
            if (triggerZone != null && (triggerZone.parentInteractable.validToolTagsMask & (int)toolTags) != 0)
            {
                collidersTouchingUs.Add(triggerZone);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var triggerZone = other.GetComponent<ButtonTriggerZone>();
            if (triggerZone != null && (triggerZone.parentInteractable.validToolTagsMask & (int)toolTags) != 0)
            {
                collidersTouchingUs.Remove(triggerZone);
            }
        }

        /// <summary>
        /// Sometimes colliders get disabled and trigger exit doesn't get called.
        /// Take care of that edge case.
        /// </summary>
        private void CleanUpDeadColliders()
        {
            m_elementsToCleanUp.Clear();
            foreach (IColliderZone colliderTouching in collidersTouchingUs)
            {
                m_elementsToCleanUp.Clear();
                if (!colliderTouching.collider.gameObject.activeInHierarchy)
                {
                    m_elementsToCleanUp.Add(colliderTouching);
                }
            }

            m_elementsToCleanUp.ForEach(colliderZone => { collidersTouchingUs.Remove(colliderZone); });
        }


    }

}
