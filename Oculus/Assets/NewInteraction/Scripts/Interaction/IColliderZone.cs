using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YVR.Interaction
{
    public interface IColliderZone
    {
        Collider collider { get; }
        // Which interactable do we belong to?
        Interactable parentInteractable { get; }
        InteractableCollisionDepth collisionDepth { get; }
    }
}