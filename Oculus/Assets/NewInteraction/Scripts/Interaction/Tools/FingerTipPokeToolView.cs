using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YVR.Interaction
{
    /// <summary>
    /// Visual of finger tip poke tool.
    /// </summary>
    public class FingerTipPokeToolView : MonoBehaviour, IInteractableToolView
    {
        [SerializeField]
        private MeshRenderer m_sphereMeshRenderer = null;

        public InteractableTool interactableTool { get; set; }

        public bool enableState { get => m_sphereMeshRenderer.enabled; set => m_sphereMeshRenderer.enabled = value; }
        public bool toolActivateState { get; set; }

        public float sphereRadius { get; private set; }

        private void Awake()
        {
            Assert.IsNotNull(m_sphereMeshRenderer);
            sphereRadius = m_sphereMeshRenderer.transform.localScale.z * 0.5f;
        }

        public void SetFocusedInteractable(Interactable interactable)
        {

        }
    }

}

