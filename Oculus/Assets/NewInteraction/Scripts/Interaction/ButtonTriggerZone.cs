using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YVR.Interaction
{
    public class ButtonTriggerZone : MonoBehaviour, IColliderZone
    {

        [SerializeField]
        private GameObject m_parentInteractableObj = null;

        public Collider collider { get; private set; }

        public Interactable parentInteractable { get; private set; }

        public InteractableCollisionDepth collisionDepth
        {
            get
            {
                var myColliderZone = (IColliderZone)this;
                var depth = parentInteractable.proximityCollider == myColliderZone ? InteractableCollisionDepth.Proximity :
                  parentInteractable.contactCollider == myColliderZone ? InteractableCollisionDepth.Contact :
                  parentInteractable.actionCollider == myColliderZone ? InteractableCollisionDepth.Action :
                  InteractableCollisionDepth.None;
                return depth;
            }
        }

        private void Awake()
        {
            Assert.IsNotNull(m_parentInteractableObj);

            collider = GetComponent<Collider>();

            parentInteractable = m_parentInteractableObj.GetComponent<Interactable>();
        }
    }

}

