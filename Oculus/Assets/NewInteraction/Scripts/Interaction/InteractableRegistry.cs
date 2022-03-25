using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace YVR.Interaction
{
    /// <summary>
    /// In case someone wants to know about all interactables in a scene,
    /// this registry is the easiest way to access that information.
    /// </summary>
    public class InteractableRegistry : MonoBehaviour
    {
        private static HashSet<Interactable> s_interactables = new HashSet<Interactable>();

        public static HashSet<Interactable> Interactables
        {
            get
            {
                return s_interactables;
            }
        }

        public static void RegisterInteractable(Interactable interactable)
        {
            Interactables.Add(interactable);
        }

        public static void UnregisterInteractable(Interactable interactable)
        {
            Interactables.Remove(interactable);
        }
    }

}




