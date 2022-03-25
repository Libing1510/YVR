using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YVR.Interaction
{
    public enum InteractionType
    {
        Enter = 0,
        Stay,
        Exit
    }

    /// <summary>
    /// Arguments for object interacting with collider zone.
    /// </summary>
    public class ColliderZoneArgs : EventArgs
    {
        public readonly IColliderZone collider;
        public readonly float frameTime;
        public readonly InteractableTool collidingTool;
        public readonly InteractionType interactionType;

        public ColliderZoneArgs(IColliderZone collider, float frameTime,
          InteractableTool collidingTool, InteractionType interactionType)
        {
            this.collider = collider;
            this.frameTime = frameTime;
            this.collidingTool = collidingTool;
            this.interactionType = interactionType;
        }
    }



}
