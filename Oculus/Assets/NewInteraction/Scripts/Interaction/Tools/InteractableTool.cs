using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace YVR.Interaction
{
    /// <summary>
    /// A tool that can engage interactables.
    /// </summary>
    public abstract class InteractableTool : MonoBehaviour
    {
        #region
        public Transform toolTransform { get { return this.transform; } }
        /// <summary>
        /// ���ֽ�������
        /// </summary>
        public bool isRightHandedTool { get; set; }
        /// <summary>
        /// ������ʽ�����ߣ��죩
        /// </summary>
        public abstract InteractableToolTags toolTags { get; }
        /// <summary>
        /// ��������״̬�����£���ס��̧��
        /// </summary>
        public abstract ToolInputState toolInputState { get; }
        /// <summary>
        /// �Ƿ�ΪԶ�̽���
        /// </summary>
        public abstract bool isFarFieldTool { get; }
        /// <summary>
        /// �ٶ�
        /// </summary>
        public Vector3 velocity { get; protected set; }
        /// <summary>
        /// Sometimes we want the position of a tool for stuff like pokes.
        /// </summary>
        public Vector3 interactionPosition { get; protected set; }

        /// <summary>
        /// List of objects that intersect tool.
        /// </summary>
        protected List<InteractableCollisionInfo> m_currentIntersectingObjects =
            new List<InteractableCollisionInfo>();

        public abstract bool enableState { get; set; }

        // lists created once so that they don't need to be created per frame
        private List<Interactable> m_addedInteractables = new List<Interactable>();
        private List<Interactable> m_removedInteractables = new List<Interactable>();
        private List<Interactable> m_remainingInteractables = new List<Interactable>();
        /// <summary>
        /// ��ǰ��������Ľ�����Ϣ�ֵ�
        /// </summary>
        private Dictionary<Interactable, InteractableCollisionInfo> m_currInteractableToCollisionInfos
    = new Dictionary<Interactable, InteractableCollisionInfo>();
        /// <summary>
        /// ǰһ����������Ľ�����Ϣ�ֵ�
        /// </summary>
        private Dictionary<Interactable, InteractableCollisionInfo> m_prevInteractableToCollisionInfos
            = new Dictionary<Interactable, InteractableCollisionInfo>();


        #endregion

        #region
        /// <summary>
        /// ��һ���ɽ�������
        /// </summary>
        /// <returns></returns>
        public abstract List<InteractableCollisionInfo> GetNextIntersectingObjects();

        /// <summary>
        /// Used to tell the tool to "focus" on a specific object, if
        /// focusing is indeed possible given the tool type.
        /// </summary>
        /// <param name="focusedInteractable">Interactable to focus.</param>
        /// <param name="colliderZone">Collider zone of interactable.</param>
        public abstract void FocusOnInteractable(Interactable focusedInteractable,
            IColliderZone colliderZone);
        /// <summary>
        /// ���ٹ�ע
        /// </summary>
        public abstract void DeFocus();
        public abstract void Initialize();

        public KeyValuePair<Interactable, InteractableCollisionInfo> GetFirstCurrentCollisionInfo()
        {
            return m_currInteractableToCollisionInfos.First();
        }
        public void ClearAllCurrentCollisionInfos()
        {
            m_currInteractableToCollisionInfos.Clear();
        }

        /// <summary>
        /// ����ÿ���ཻ�Ŀɽ������󣬸���Ԫ�����Խ�ָʾ�������ײ��
        /// For each intersecting interactable, update meta data to indicate deepest collision only.
        /// </summary>
        public virtual void UpdateCurrentCollisionsBasedOnDepth()
        {
            //�����ǰ����������Ϣ�б�
            m_currInteractableToCollisionInfos.Clear();
            //������⵽�Ľ��������б�
            m_currentIntersectingObjects.ForEach(interactableCollisionInfo =>
            {
                var interactable = interactableCollisionInfo.interactableCollider.parentInteractable;
                var depth = interactableCollisionInfo.collisionDepth;
                InteractableCollisionInfo collisionInfoFromMap = null;

                //������ڵ�ǰ�����������ֵ��У���ӽ�����ײ��Ϣ
                if (!m_currInteractableToCollisionInfos.TryGetValue(interactable, out collisionInfoFromMap))
                {
                    m_currInteractableToCollisionInfos[interactable] = interactableCollisionInfo;
                }
                else if (collisionInfoFromMap.collisionDepth < depth)//����Ȼ���Ľ���������������ֵ��н�����Ϣ
                {
                    collisionInfoFromMap.interactableCollider = interactableCollisionInfo.interactableCollider;
                    collisionInfoFromMap.collisionDepth = depth;
                }
            });

        }

        /// <summary>
        /// ������ǵ���ײ��Ϣÿ֡�������˱仯�����������ɾ������Ӻ�ʣ��Ķ������õ�������ȷ���¼���
        /// If our collision information changed per frame, make note of it.
        /// Removed, added and remaining objects must get their proper events.
        /// </summary>
        public virtual void UpdateLatestCollisionData()
        {

            m_addedInteractables.Clear();
            m_removedInteractables.Clear();
            m_remainingInteractables.Clear();

            //�ж���ǰ�����������Ƿ�Ϊǰһ֡�Ľ������壬������������������������������
            foreach (Interactable key in m_currInteractableToCollisionInfos.Keys)
            {
                if (!m_prevInteractableToCollisionInfos.ContainsKey(key))
                {
                    m_addedInteractables.Add(key);
                }
                else
                {
                    m_remainingInteractables.Add(key);
                }
            }

            //ǰһ֡�����������Ƿ��ڵ�ǰ��Ȼ�ڽ�����������������Ƴ�
            foreach (Interactable key in m_prevInteractableToCollisionInfos.Keys)
            {
                if (!m_currInteractableToCollisionInfos.ContainsKey(key))
                {
                    m_removedInteractables.Add(key);
                }
            }

            //�����Ƴ��Ŀɽ�������Exit
            // tell removed interactables that we are gone
            m_removedInteractables.ForEach(removedInteractable =>
            {
                removedInteractable.UpdateCollisionDepth(this,
                    m_prevInteractableToCollisionInfos[removedInteractable].collisionDepth,
                    InteractableCollisionDepth.None);
            });

            //���������Ŀɽ�������Enter
            // tell added interactable what state we are now in
            m_addedInteractables.ForEach(addedInteractableKey =>
            {
                var addedInteractable = m_currInteractableToCollisionInfos[addedInteractableKey];
                var collisionDepth = addedInteractable.collisionDepth;
                addedInteractableKey.UpdateCollisionDepth(this, InteractableCollisionDepth.None,
                    collisionDepth);
            });

            //���������Ŀɽ��������������
            // remaining interactables must be updated
            m_remainingInteractables.ForEach(remainingInteractableKey =>
            {
                var newDepth = m_currInteractableToCollisionInfos[remainingInteractableKey].collisionDepth;
                var oldDepth = m_prevInteractableToCollisionInfos[remainingInteractableKey].collisionDepth;
                remainingInteractableKey.UpdateCollisionDepth(this, oldDepth, newDepth);
            });

            //����ǰ֡�Ľ������󻺴�Ϊ��һ֡
            m_prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>(
                    m_currInteractableToCollisionInfos);
        }
        #endregion


    }

}

