using System;
using UnityEngine;


namespace YVR.Interaction
{
    /// <summary>
    /// 交互物体的基类
    /// </summary>
    public abstract class Interactable : MonoBehaviour
    {
        #region
        protected IColliderZone m_proximityZoneCollider = null;
        protected IColliderZone m_contactZoneCollider = null;
        protected IColliderZone m_actionZoneCollider = null;

        /// <summary>
        /// Collider that indicates "am I close?"
        /// </summary>
        public IColliderZone proximityCollider => m_proximityZoneCollider;


        /// <summary>
        /// Collider that indicates that contact has been made.
        /// </summary>
        public IColliderZone contactCollider => m_contactZoneCollider;


        /// <summary>
        /// Indicates interactable has been activated. Like when
        // a button goes "click" and something interesting happens.
        /// </summary>
        public IColliderZone actionCollider => m_actionZoneCollider;

        /// <summary>
        /// What kinds of tools works with this interactable?
        /// </summary>
        public virtual int validToolTagsMask => (int)InteractableToolTags.All;

        public event Action<ColliderZoneArgs> proximityZoneEvent;
        public event Action<ColliderZoneArgs> contactZoneEvent;
        public event Action<ColliderZoneArgs> actionZoneEvent;

        public InteractableStateArgsEvent interactableStateChanged;

        #endregion

        #region
        protected virtual void Awake()
        {
            InteractableRegistry.RegisterInteractable(this);
        }

        protected virtual void OnDestroy()
        {
            InteractableRegistry.UnregisterInteractable(this);
        }

        protected virtual void OnProximityZoneEvent(ColliderZoneArgs args)
        {
            proximityZoneEvent?.Invoke(args);
        }

        protected virtual void OnContactZoneEvent(ColliderZoneArgs args)
        {
            contactZoneEvent?.Invoke(args);
        }

        protected virtual void OnActionZoneEvent(ColliderZoneArgs args)
        {
            actionZoneEvent?.Invoke(args);
        }

        public abstract void UpdateCollisionDepth(InteractableTool interactableTool,
        InteractableCollisionDepth oldCollisionDepth, InteractableCollisionDepth newCollisionDepth);

        #endregion



    }
}