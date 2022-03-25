using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YVR.Interaction
{
    public class RayTool : InteractableTool
    {
        private const float MINIMUM_RAY_CAST_DISTANCE = 0.8f;
        private const float COLLIDER_RADIUS = 0.01f;
        private const int NUM_MAX_PRIMARY_HITS = 10;
        private const int NUM_MAX_SECONDARY_HITS = 25;
        private const int NUM_COLLIDERS_TO_TEST = 20;

        [SerializeField]
        private HandType handType;
        [SerializeField]
        private RayToolView m_rayToolView = null;
        [Range(0.0f, 45.0f)]
        [SerializeField]
        private float m_coneAngleDegrees = 20.0f;
        private float m_coneAngleReleaseDegrees;
        [SerializeField]
        private float m_farFieldMaxDistance = 5f;
        private Interactable m_focusedInteractable;
        private Interactable m_currInteractableCastedAgainst = null;

        private RaycastHit[] m_primaryHits = new RaycastHit[NUM_MAX_PRIMARY_HITS];
        private Collider[] m_secondaryOverlapResults = new Collider[NUM_MAX_SECONDARY_HITS];
        private Collider[] m_collidersOverlapped = new Collider[NUM_COLLIDERS_TO_TEST];
        private bool m_initialized = false;

        public override InteractableToolTags toolTags => InteractableToolTags.Ray;

        #region

        private void Awake()
        {
            Initialize();
        }

        public override void Initialize()
        {
            Assert.IsNotNull(m_rayToolView);
            InteractableToolsInputRouter.instance.RegisterInteractableTool(this);
            m_rayToolView.interactableTool = this;
            m_coneAngleReleaseDegrees = m_coneAngleDegrees * 1.2f;
            m_initialized = true;
            isRightHandedTool = handType == HandType.RightHand;

        }

        private void OnDestroy()
        {
            if (InteractableToolsInputRouter.instance != null)
            {
                InteractableToolsInputRouter.instance.UnregisterInteractableTool(this);
            }
        }
        #endregion


        public override ToolInputState toolInputState
        {
            get
            {
                if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    return ToolInputState.PrimaryInputDown;
                }
                else if (Input.GetKeyUp(KeyCode.Alpha6))
                {
                    return ToolInputState.PrimaryInputUp;
                }
                else if (Input.GetKey(KeyCode.Alpha6))
                {
                    return ToolInputState.PrimaryInputDownStay;
                }
                else
                {
                    return ToolInputState.Inactive;
                }
            }
        }

        public override bool isFarFieldTool => true;

        public override bool enableState { get => m_rayToolView.enableState; set => m_rayToolView.enableState = value; }


        public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
        {
            if (!m_initialized) return m_currentIntersectingObjects;
            m_currentIntersectingObjects.Clear();
            /*
            // if we already have focused on something, keep it until the angle between
            // our forward direction and object vector becomes too large
            if (m_currInteractableCastedAgainst != null && HasRayReleasedInteractable(m_currInteractableCastedAgainst))
                m_currInteractableCastedAgainst = null;
            */
            m_currInteractableCastedAgainst = FindTargetInteractable();

            if (m_currInteractableCastedAgainst != null)
            {
                var targetHitPoint = m_currInteractableCastedAgainst.transform.position;
                int numHits = Physics.OverlapSphereNonAlloc(targetHitPoint, COLLIDER_RADIUS, m_collidersOverlapped);

                for (int i = 0; i < numHits; i++)
                {
                    Collider colliderHit = m_collidersOverlapped[i];
                    var colliderZone = colliderHit.GetComponent<IColliderZone>();
                    if (colliderZone == null) continue;

                    Interactable interactableComponent = colliderZone.parentInteractable;
                    if (interactableComponent == null || interactableComponent != m_currInteractableCastedAgainst)
                        continue;

                    InteractableCollisionInfo collisionInfo = new InteractableCollisionInfo(colliderZone, colliderZone.collisionDepth, this);
                    m_currentIntersectingObjects.Add(collisionInfo);
                }
                // clear intersecting object if no collisions were found
                if (m_currentIntersectingObjects.Count == 0)
                {
                    m_currInteractableCastedAgainst = null;
                }
            }

            return m_currentIntersectingObjects;
        }


        private bool HasRayReleasedInteractable(Interactable focusedInteractable)
        {
            Vector3 ourPosition = transform.position;
            Vector3 forwardDirection = transform.forward;
            var hysteresisDotThreshold = Mathf.Cos(m_coneAngleReleaseDegrees * Mathf.Deg2Rad);
            var vectorToFocusedObject = focusedInteractable.transform.position - ourPosition;
            vectorToFocusedObject.Normalize();
            var hysteresisDotProduct = Vector3.Dot(vectorToFocusedObject, forwardDirection);
            return hysteresisDotProduct < hysteresisDotThreshold;
        }

        /// <summary>
        /// Find all objects from primary ray cast or if that fails, all objects in a
        /// cone around main ray direction via a "secondary" cast.
        /// </summary>
        private Interactable FindTargetInteractable()
        {
            var rayOrigin = GetRayCastOrigin();
            var rayDirection = transform.forward;
            Interactable targetInteractable = null;

            //attmpt primary ray cast
            targetInteractable = FindPrimaryRaycastHit(rayOrigin, rayDirection);

            return targetInteractable;
        }

        /// <summary>
        /// Find first hit that is supports our tool's method of interaction.
        /// </summary>
        private Interactable FindPrimaryRaycastHit(Vector3 rayOrigin, Vector3 rayDirection)
        {
            Interactable interactableCastedAgainst = null;

            // hit order not guaranteed, so find closest
            int numHits = Physics.RaycastNonAlloc(new Ray(rayOrigin, rayDirection), m_primaryHits, Mathf.Infinity);
            float minDistance = 0.0f;
            for (int hitIndex = 0; hitIndex < numHits; hitIndex++)
            {
                RaycastHit raycastHit = m_primaryHits[hitIndex];

                // continue if something occludes it and that object is not an interactable
                var currentHitColliderZone = raycastHit.transform.GetComponent<IColliderZone>();
                if (currentHitColliderZone == null) continue;

                // at this point we have encountered an interactable.
                // Only consider it if it allows interaction with our tool. Otherwise ignore it.
                Interactable currentInteractable = currentHitColliderZone.parentInteractable;
                if (currentInteractable == null || (currentInteractable.validToolTagsMask & (int)toolTags) == 0)
                    continue;

                var vectorToInteractable = currentInteractable.transform.position - rayOrigin;
                var distanceToInteractable = vectorToInteractable.magnitude;
                if (interactableCastedAgainst == null || distanceToInteractable < minDistance)
                {
                    interactableCastedAgainst = currentInteractable;
                    minDistance = distanceToInteractable;
                }
            }

            return interactableCastedAgainst;
        }

        /// <summary>
        /// Avoid hand collider during raycasts so move origin some distance away from where tool is.
        /// </summary>
        /// <returns>Proper raycast origin.</returns>
        private Vector3 GetRayCastOrigin()
        {
            return transform.position + MINIMUM_RAY_CAST_DISTANCE * transform.forward;
        }


        public override void DeFocus()
        {
            m_rayToolView.SetFocusedInteractable(null);
            m_focusedInteractable = null;
        }

        public override void FocusOnInteractable(Interactable focusedInteractable, IColliderZone colliderZone)
        {
            m_rayToolView.SetFocusedInteractable(focusedInteractable);
            m_focusedInteractable = focusedInteractable;
        }




    }
}

