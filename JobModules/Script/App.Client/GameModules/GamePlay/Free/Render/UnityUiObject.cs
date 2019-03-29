using UnityEngine;

namespace Assets.Sources.Free.Render
{
    public class UnityUiObject : IUiObject
    {
        private RectTransform rectTransform;
        private CanvasRenderer canvasRender;
        private GameObject _gameObject;

        public UnityUiObject(GameObject engineObj)
        {
            _gameObject = engineObj;
            rectTransform = _gameObject.GetComponent<RectTransform>();
            canvasRender = _gameObject.GetComponent<CanvasRenderer>();

            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
        }
        public int width
        {
            get { return (int)rectTransform.sizeDelta.x; }
            set
            {
                var size = rectTransform.sizeDelta;
                size.x = value;
                rectTransform.sizeDelta = size;
            }
        }

        public int height
        {
            get { return (int)rectTransform.sizeDelta.y; }
            set
            {
                var size = rectTransform.sizeDelta;
                size.y = value;
                rectTransform.sizeDelta = size;
            }
        }

        public bool visible
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
            }
        }

        public float x
        {
            get { return rectTransform.localPosition.x; }
            set
            {
                var pos = rectTransform.localPosition;
                pos.x = value;
                rectTransform.localPosition = pos;
            }
        }

        public float y
        {
            get { return -rectTransform.localPosition.y; }
            set
            {
                var pos = rectTransform.localPosition;
                pos.y = -value;
                rectTransform.localPosition = pos;
            }
        }

        public float alpha
        {
            get { return canvasRender.GetAlpha(); }
            set
            {
                canvasRender.SetAlpha(value);
            }
        }

        public float scaleX
        {
            get { return rectTransform.localScale.x; }
            set
            {
                var scale = rectTransform.localScale;
                scale.x = value;
                rectTransform.localScale = scale;
            }
        }

        public float scaleY
        {
            get { return rectTransform.localScale.y; }
            set
            {
                var scale = rectTransform.localScale;
                scale.y = value;
                rectTransform.localScale = scale;
            }
        }

        public float rotation
        {
            get { return rectTransform.localEulerAngles.z; }
            set
            {
                var angle = rectTransform.localEulerAngles;
                angle.z = value;
                rectTransform.localEulerAngles = angle;
            }
        }

        public GameObject gameObject
        {
            get { return _gameObject; }
        }

        public void AddChild(IUiObject child)
        {
            var localPos = child.gameObject.transform.localPosition;
            child.gameObject.transform.parent = gameObject.transform;
            child.gameObject.transform.localPosition = localPos;
        }

        public T FindComponentInChildren<T>() where T : MonoBehaviour
        {
            return gameObject.GetComponentInChildren<T>();
        }

        public T GetComponent<T>() where T : MonoBehaviour
        {
            return gameObject.GetComponent<T>();
        }
    }
}
