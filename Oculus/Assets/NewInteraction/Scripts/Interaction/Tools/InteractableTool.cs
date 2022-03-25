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
        /// 右手交互工具
        /// </summary>
        public bool isRightHandedTool { get; set; }
        /// <summary>
        /// 交互方式（射线，挫）
        /// </summary>
        public abstract InteractableToolTags toolTags { get; }
        /// <summary>
        /// 工具输入状态（按下，按住，抬起）
        /// </summary>
        public abstract ToolInputState toolInputState { get; }
        /// <summary>
        /// 是否为远程交互
        /// </summary>
        public abstract bool isFarFieldTool { get; }
        /// <summary>
        /// 速度
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
        /// 当前交互物体的交互信息字典
        /// </summary>
        private Dictionary<Interactable, InteractableCollisionInfo> m_currInteractableToCollisionInfos
    = new Dictionary<Interactable, InteractableCollisionInfo>();
        /// <summary>
        /// 前一个交互物体的交互信息字典
        /// </summary>
        private Dictionary<Interactable, InteractableCollisionInfo> m_prevInteractableToCollisionInfos
            = new Dictionary<Interactable, InteractableCollisionInfo>();


        #endregion

        #region
        /// <summary>
        /// 下一个可交互物体
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
        /// 不再关注
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
        /// 对于每个相交的可交互对象，更新元数据以仅指示最深的碰撞。
        /// For each intersecting interactable, update meta data to indicate deepest collision only.
        /// </summary>
        public virtual void UpdateCurrentCollisionsBasedOnDepth()
        {
            //清除当前交互物体信息列表
            m_currInteractableToCollisionInfos.Clear();
            //遍历检测到的交互物体列表
            m_currentIntersectingObjects.ForEach(interactableCollisionInfo =>
            {
                var interactable = interactableCollisionInfo.interactableCollider.parentInteractable;
                var depth = interactableCollisionInfo.collisionDepth;
                InteractableCollisionInfo collisionInfoFromMap = null;

                //如果不在当前交互的物体字典中，添加交互碰撞信息
                if (!m_currInteractableToCollisionInfos.TryGetValue(interactable, out collisionInfoFromMap))
                {
                    m_currInteractableToCollisionInfos[interactable] = interactableCollisionInfo;
                }
                else if (collisionInfoFromMap.collisionDepth < depth)//如果比缓存的交互更近，则更新字典中交互信息
                {
                    collisionInfoFromMap.interactableCollider = interactableCollisionInfo.interactableCollider;
                    collisionInfoFromMap.collisionDepth = depth;
                }
            });

        }

        /// <summary>
        /// 如果我们的碰撞信息每帧都发生了变化，请记下它。删除、添加和剩余的对象必须得到它们正确的事件。
        /// If our collision information changed per frame, make note of it.
        /// Removed, added and remaining objects must get their proper events.
        /// </summary>
        public virtual void UpdateLatestCollisionData()
        {

            m_addedInteractables.Clear();
            m_removedInteractables.Clear();
            m_remainingInteractables.Clear();

            //判定当前交互的物体是否为前一帧的交互物体，如果不是则是新增，如果是则需遗留
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

            //前一帧交互的物体是否在当前仍然在交互，如果不是则需移除
            foreach (Interactable key in m_prevInteractableToCollisionInfos.Keys)
            {
                if (!m_currInteractableToCollisionInfos.ContainsKey(key))
                {
                    m_removedInteractables.Add(key);
                }
            }

            //触发移除的可交互对象Exit
            // tell removed interactables that we are gone
            m_removedInteractables.ForEach(removedInteractable =>
            {
                removedInteractable.UpdateCollisionDepth(this,
                    m_prevInteractableToCollisionInfos[removedInteractable].collisionDepth,
                    InteractableCollisionDepth.None);
            });

            //触发新增的可交互对象Enter
            // tell added interactable what state we are now in
            m_addedInteractables.ForEach(addedInteractableKey =>
            {
                var addedInteractable = m_currInteractableToCollisionInfos[addedInteractableKey];
                var collisionDepth = addedInteractable.collisionDepth;
                addedInteractableKey.UpdateCollisionDepth(this, InteractableCollisionDepth.None,
                    collisionDepth);
            });

            //更新遗留的可交互对象深度数据
            // remaining interactables must be updated
            m_remainingInteractables.ForEach(remainingInteractableKey =>
            {
                var newDepth = m_currInteractableToCollisionInfos[remainingInteractableKey].collisionDepth;
                var oldDepth = m_prevInteractableToCollisionInfos[remainingInteractableKey].collisionDepth;
                remainingInteractableKey.UpdateCollisionDepth(this, oldDepth, newDepth);
            });

            //将当前帧的交互对象缓存为上一帧
            m_prevInteractableToCollisionInfos = new Dictionary<Interactable, InteractableCollisionInfo>(
                    m_currInteractableToCollisionInfos);
        }
        #endregion


    }

}

