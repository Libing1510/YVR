using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YVR.Interaction
{
    /// <summary>
    /// Visual portion of ray tool.
    /// </summary>
    public class RayToolView : MonoBehaviour, IInteractableToolView
    {
        private const int K_NumRayLinePositions = 25;
        private const float K_DefaultRayCastDistance = 3.0f;

        [SerializeField]
        private Transform m_targetTransform = null;
        [SerializeField]
        private LineRenderer m_lineRenderer = null;
        private bool m_toolActivateState = false;
        private Transform m_focusedTransform = null;
        private Vector3[] m_linePositions = new Vector3[K_NumRayLinePositions];
        private Gradient m_oldColorGradient, m_highLightColorGradient;

        public InteractableTool interactableTool { get; set; }

        public bool enableState { get => m_lineRenderer.enabled; set { m_targetTransform.gameObject.SetActive(value); m_lineRenderer.enabled = value || true; } }
        public bool toolActivateState
        {
            get => m_toolActivateState;
            set
            {
                m_toolActivateState = value;
                m_lineRenderer.colorGradient = m_toolActivateState ? m_highLightColorGradient : m_oldColorGradient;
            }
        }

        private void Awake()
        {
            Assert.IsNotNull(m_targetTransform);
            Assert.IsNotNull(m_lineRenderer);
            m_lineRenderer.positionCount = K_NumRayLinePositions;

            m_oldColorGradient = m_lineRenderer.colorGradient;
            m_highLightColorGradient = new Gradient();
            m_highLightColorGradient.SetKeys(
  new GradientColorKey[] { new GradientColorKey(new Color(0.90f, 0.90f, 0.90f), 0.0f),
          new GradientColorKey(new Color(0.90f, 0.90f, 0.90f), 1.0f) },
  new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
);
        }

        private void Update()
        {
            var myPosition = interactableTool.toolTransform.position;
            var myForward = interactableTool.toolTransform.forward;
            var targetPosition = m_focusedTransform != null
  ? m_focusedTransform.position
  : myPosition + myForward * K_DefaultRayCastDistance;
            var targetVector = targetPosition - myPosition;
            var targetDistance = targetVector.magnitude;
            var p0 = myPosition;
            // make points in between based on my forward as opposed to targetvector
            // this way the curve "bends" toward to target
            var p1 = myPosition + myForward * targetDistance * 0.3333333f;
            var p2 = myPosition + myForward * targetDistance * 0.6666667f;
            var p3 = targetPosition;
            for (int i = 0; i < K_NumRayLinePositions; i++)
            {
                m_linePositions[i] = GetPointOnBezierCurve(p0, p1, p2, p3, i / 25.0f);
            }

            m_lineRenderer.SetPositions(m_linePositions);
            m_targetTransform.position = targetPosition;
        }

        public void SetFocusedInteractable(Interactable interactable)
        {
            m_focusedTransform = interactable == null ? null : interactable.transform;
        }


        /// <summary>
        /// Returns point on four-point Bezier curve.
        /// </summary>
        /// <param name="p0">Beginning point.</param>
        /// <param name="p1">t=1/3 point.</param>
        /// <param name="p2">t=2/3 point.</param>
        /// <param name="p3">End point.</param>
        /// <param name="t">Interpolation parameter.</param>
        /// <returns>Point along Bezier curve.</returns>
        public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;
            var oneMinusTSqr = oneMinusT * oneMinusT;
            var tSqr = t * t;
            return oneMinusT * oneMinusTSqr * p0 + 3f * oneMinusTSqr * t * p1 + 3f * oneMinusT * tSqr * p2 +
                t * tSqr * p3;
        }
    }
}


