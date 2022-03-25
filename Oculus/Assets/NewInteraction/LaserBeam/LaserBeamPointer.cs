using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static OVRHand;

namespace YFramework.LaserBeam
{
    /// <summary>
    /// The controller of laser beam .
    /// </summary>
    public class LaserBeamPointer : MonoBehaviour
    {

        [SerializeField]
        private Hand _controllerType;

        /// <summary>
        /// X is the min value of the hit distance,Y is the max value of the hit distance
        /// </summary>
        [Tooltip("X is the min value of the hit distance,Y is the max value of the hit distance")]
        public Vector2 hitDistanceRange = new Vector2(0.1f, 1f);

        /// <summary>
        /// The idle configuration of the laser beam and cursor.
        /// </summary>
        public LaserBeamConfiguration idleConfiguration = new LaserBeamConfiguration()
        {
            startWidth = 0.0055f,
            endWidth = 0.0015f,
            startColor = new Color32(255, 255, 255, 205),
            endColor = new Color32(255, 255, 255, 0),
            cursorConfiguration = new CursorConfiguration()
            {
                cursorMinScale = 1f,
                cursorDotColor = Color.white
            }
        };

        /// <summary>
        /// The hold configuration of the laser beam and cursor.
        /// </summary>
        public LaserBeamConfiguration holdConfiguration = new LaserBeamConfiguration()
        {
            startWidth = 0.0041f,
            endWidth = 0.0015f,
            startColor = new Color32(23, 92, 230, 205),
            endColor = new Color32(23, 92, 230, 0),
            cursorConfiguration = new CursorConfiguration
            {
                cursorMinScale = 0.8f,
                cursorDotColor = new Color32(23, 92, 230, 255)
            }
        };

        [SerializeField]
        private GameObject cursorPrefab;
        [SerializeField]
        private GameObject meshEffect;

        private CursorBase _cursor;
        private LineRenderer laserLine;
        private new Transform transform;

        private List<RaycastResult> raycastList = new List<RaycastResult>();

        /// <summary>
        /// The controller type of laser beam.
        /// </summary>
        public Hand controllerType { get => _controllerType; }

        /// <summary>
        /// The cursor of the laser beam.
        /// </summary>
        public CursorBase cursor { get => _cursor; set => _cursor = value; }

        #region Unity

        private void OnDisable()
        {
            //move away the line render
            laserLine?.SetPosition(0, Vector3.one * 9998);
            laserLine?.SetPosition(1, Vector3.one * 9999);
        }
        private void Start()
        {
            this.transform = base.transform;
            laserLine = GetComponent<LineRenderer>();
            if (laserLine == null) laserLine = CreateLaserBeam();

            if (cursor == null)
                _cursor = CreateDefaultCursor();

        }
        private void LateUpdate()
        {
            UpdateRaycast();
        }
        #endregion Unity end

        #region
        private void UpdateRaycast()
        {
            RaycastResult result;
            // while input data provider source does not match the target controller, calculate raycast target manually
            // otherwise, use the result from YVRInputmodule directly
            result = OVRInputModule.Instance.raycastResult;

            var hitSth = result.gameObject != null;
            float distance = hitSth ? Vector3.Distance(result.worldPosition, transform.position) - 0.01f : 0;
            if (cursor) cursor.Show(hitSth);
            Vector3 normal = hitSth ? result.worldNormal * -1 : transform.forward;
            if (normal.Equals(Vector3.zero))
                normal = result.gameObject.transform.forward;

            if (OVRInput.Get(OVRInput.RawTouch.RIndexTrigger))
            {
                UpdateLaserBeam(holdConfiguration, distance, normal);
                if (cursor) cursor.UpdateEffect(holdConfiguration.cursorConfiguration, distance, normal, result.gameObject,true);
            }
            else
            {
                UpdateLaserBeam(idleConfiguration, distance, normal);
                if (cursor) cursor.UpdateEffect(idleConfiguration.cursorConfiguration, distance, normal, result.gameObject,false);
            }

            if (hitSth&&meshEffect!=null&&!ExecuteEvents.GetEventHandler<IPointerClickHandler>(result.gameObject))
            {
                meshEffect.SetActive(true);
                meshEffect.transform.localPosition = Vector3.forward * distance;
                meshEffect.transform.forward = normal;
            }
            else
            {
                meshEffect.SetActive(false);
            }
        }

        private void UpdateLaserBeam(LaserBeamConfiguration configuration, float distance, Vector3 normal)
        {
            if (laserLine)
            {
                laserLine.startColor = configuration.startColor;
                laserLine.endColor = configuration.endColor;
                laserLine.startWidth = configuration.startWidth;
                laserLine.endWidth = configuration.endWidth;

                laserLine.SetPosition(0, transform.position + transform.forward * configuration.startPointOffset);
                float beamLength = Mathf.Clamp(distance, hitDistanceRange.x, hitDistanceRange.y);
                laserLine.SetPosition(1, transform.position + transform.forward * beamLength);
            }
        }

        private LineRenderer CreateLaserBeam()
        {
            var lineRender = gameObject.AddComponent<LineRenderer>();
            lineRender.material = new Material(Shader.Find("YFramework/Unlit/VertexColor"));
            return lineRender;
        }

        private CursorBase CreateDefaultCursor()
        {
            if (cursorPrefab && cursorPrefab.GetComponent<CursorBase>())
            {
                var cur = Instantiate<GameObject>(cursorPrefab, transform);
                return cur.GetComponent<CursorBase>();
            }
            return null;
        }

        #endregion

    }

}