using UnityEngine;
using UnityEngine.UI;

namespace YFramework.LaserBeam
{

    public class DefaultImageCursor : CursorBase
    {
        public Image dot;

        private new GameObject gameObject;
        public override GameObject cursorGameObject => this.gameObject;

        private new Transform transform;
        public override Transform cursorTransform => this.transform;

        void Awake()
        {
            this.transform = base.transform;
            this.gameObject = base.gameObject;
        }

        public override void UpdateEffect(CursorConfiguration configuration, float distance, Vector3 normal, GameObject hitGameObject,bool down)
        {
            float scaleParam = Mathf.Max(configuration.cursorMinScale, distance) * 0.1f;
            if (down)
                scaleParam *= 0.9f;
            transform.localScale = new Vector3(scaleParam, scaleParam, transform.localScale.z);
            transform.localPosition = Vector3.forward * distance;
            transform.forward = normal;
            if (dot)
                dot.color = configuration.cursorDotColor;
        }

        public override void Show(bool display)
        {
            if (gameObject) gameObject.SetActive(display);
        }
    }
}
