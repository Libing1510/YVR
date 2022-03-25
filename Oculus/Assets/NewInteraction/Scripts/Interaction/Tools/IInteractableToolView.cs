using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YVR.Interaction
{
    /// <summary>
    /// The visual abstraction of an interactable tool.
    /// </summary>
    public interface IInteractableToolView
    {
        InteractableTool interactableTool { get; }
        void SetFocusedInteractable(Interactable interactable);

        bool enableState { get; set; }
        // Useful if you want to tool to glow in case it interacts with an object.
        bool toolActivateState { get; set; }
    }
}

