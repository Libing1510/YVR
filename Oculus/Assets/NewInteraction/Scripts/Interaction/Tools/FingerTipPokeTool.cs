using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.XR;

namespace YVR.Interaction
{

    public class FingerTipPokeTool : InteractableTool
    {
        private const int k_NumVelocityFrames = 10;

        [SerializeField]
        private FingerTipPokeToolView m_fingerTipPokeToolView = null;
        [SerializeField]
        private HandFinger m_fingerToFollow = HandFinger.Index;
        [SerializeField]
        private BoneCapsuleTriggerLogic[] m_boneCapsuleTriggerLogics;

        /// <summary>
        /// 速度
        /// </summary>
        private Vector3[] m_velocityFrames;
        private int m_currVelocityFrame = 0;
        private bool m_sampledMaxFramesAlready;
        private Vector3 m_position;



        private bool m_isInitialized = false;

        public override InteractableToolTags toolTags => InteractableToolTags.Poke;

        public override ToolInputState toolInputState => ToolInputState.Inactive;

        public override bool isFarFieldTool => false;

        public override bool enableState { get => m_fingerTipPokeToolView.gameObject.activeSelf; set => m_fingerTipPokeToolView.gameObject.SetActive(value); }

        private void Awake()
        {
            Initialize();
        }

        private void Update()
        {
            UpdateAverageVelocity();
        }

        public override void Initialize()
        {
            Assert.IsNotNull(m_fingerTipPokeToolView);
            InteractableToolsInputRouter.instance.RegisterInteractableTool(this);
            m_fingerTipPokeToolView.interactableTool = this;
            m_velocityFrames = new Vector3[k_NumVelocityFrames];
            Array.Clear(m_velocityFrames, 0, k_NumVelocityFrames);

            m_isInitialized = true;
        }

        /// <summary>
        /// 计算平均速度
        /// </summary>
        private void UpdateAverageVelocity()
        {
            Vector3 prevPosition = m_position;
            Vector3 currPosition = transform.position;
            Vector3 currentVelocity = (currPosition - prevPosition) / Time.deltaTime;
            m_position = currPosition;
            m_velocityFrames[m_currVelocityFrame] = currentVelocity;
            //if sampled more than allowed, loop back toward the beginning
            m_currVelocityFrame = (m_currVelocityFrame + 1) % k_NumVelocityFrames;
            velocity = Vector3.zero;

            if (!m_sampledMaxFramesAlready && m_currVelocityFrame == k_NumVelocityFrames - 1)
            {
                m_sampledMaxFramesAlready = true;
            }

            int numFramesToSamples = m_sampledMaxFramesAlready ? k_NumVelocityFrames : m_currVelocityFrame + 1;
            for (int frameIndex = 0; frameIndex < numFramesToSamples; frameIndex++)
            {
                velocity += m_velocityFrames[frameIndex];
            }
            velocity /= numFramesToSamples;
        }

        public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
        {
            m_currentIntersectingObjects.Clear();
            foreach (var boneCapsuleTriggerLogic in m_boneCapsuleTriggerLogics)
            {
                var collidersTouching = boneCapsuleTriggerLogic.collidersTouchingUs;
                foreach (IColliderZone colliderTouching in collidersTouching)
                {
                    m_currentIntersectingObjects.Add(new InteractableCollisionInfo(colliderTouching, colliderTouching.collisionDepth, this));
                }
            }
            Debug.Log(m_currentIntersectingObjects.Count);
            return m_currentIntersectingObjects;
        }

        public override void DeFocus()
        {

        }

        public override void FocusOnInteractable(Interactable focusedInteractable, IColliderZone colliderZone)
        {

        }




    }

}
