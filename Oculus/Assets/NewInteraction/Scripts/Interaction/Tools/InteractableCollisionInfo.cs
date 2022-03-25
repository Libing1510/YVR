using System;
using UnityEngine.Events;

namespace YVR.Interaction
{
    /// <summary>
    /// Describes how the tool will work with interactables. An interactable,
    /// in turn, can tell us which tools they support via their flag bit mask.
    /// </summary>
    [System.Flags]
    public enum InteractableToolTags
    {
        None = 0,
        Ray = 1 << 0,
        Poke = 1 << 2,
        All = ~0
    }

    /// <summary>
    /// Indicates if tool has been activated via some gesture, press, etc.
    /// </summary>
    public enum ToolInputState
    {
        Inactive = 0,
        PrimaryInputDown,
        PrimaryInputDownStay,
        PrimaryInputUp
    }

    /// <summary>
    /// Depth of collision, in order of "furthest" to "closest"
    /// </summary>
    public enum InteractableCollisionDepth
    {
        None = 0,
        Proximity,
        Contact,
        Action,
    }

    public enum InteractableState
    {
        Default = 0,
        ProximityState, // in proximity -- close enough
        ContactState, // contact has been made
        ActionState, // interactable activates
    }

    /// <summary>
    /// Describes tool-to-collision information.
    /// </summary>
    public class InteractableCollisionInfo
    {
        public InteractableCollisionInfo(IColliderZone collider, InteractableCollisionDepth collisionDepth,
  InteractableTool collidingTool)
        {
            interactableCollider = collider;
            this.collisionDepth = collisionDepth;
            this.collidingTool = collidingTool;
        }

        public IColliderZone interactableCollider;
        public InteractableCollisionDepth collisionDepth;
        public InteractableTool collidingTool;
    }

    [Serializable]
    public class InteractableStateArgsEvent : UnityEvent<InteractableStateArgs>
    {
    }
}

