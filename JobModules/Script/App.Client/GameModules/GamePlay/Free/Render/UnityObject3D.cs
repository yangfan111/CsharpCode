using UnityEngine;

namespace Assets.Sources.Free.Render
{
    public class UnityObject3D : IObject3D
    {
        private GameObject _gameObject;

        public UnityObject3D(GameObject go)
        {
            _gameObject = go;
        }

        public float x
        {
            get { return _gameObject.transform.localPosition.x; }
            set
            {
                var pos = _gameObject.transform.localPosition;
                pos.x = value;
                _gameObject.transform.localPosition = pos;
            }
        }
        public float y
        {
            get { return -_gameObject.transform.localPosition.y; }
            set
            {
                var pos = _gameObject.transform.localPosition;
                pos.y = value;
                _gameObject.transform.localPosition = pos;
            }
        }
        public float z
        {
            get { return _gameObject.transform.localPosition.z; }
            set
            {
                var pos = _gameObject.transform.localPosition;
                pos.z = value;
                _gameObject.transform.localPosition = pos;
            }
        }

        public float scaleX
        {
            get { return _gameObject.transform.localScale.x; }
            set
            {
                var scale = _gameObject.transform.localScale;
                scale.x = value;
                _gameObject.transform.localScale = scale;
            }
        }

        public float scaleY
        {
            get { return -_gameObject.transform.localScale.y; }
            set
            {
                var scale = _gameObject.transform.localScale;
                scale.y = value;
                _gameObject.transform.localScale = scale;
            }
        }

        public float scaleZ
        {
            get { return _gameObject.transform.localScale.z; }
            set
            {
                var scale = _gameObject.transform.localScale;
                scale.z = value;
                _gameObject.transform.localScale = scale;
            }
        }

        public float rotationX
        {
            get { return _gameObject.transform.localEulerAngles.x; }
            set
            {
                var angle = _gameObject.transform.localEulerAngles;
                angle.x = value;
                _gameObject.transform.localEulerAngles = angle;
            }
        }

        public float rotationY
        {
            get { return -_gameObject.transform.localEulerAngles.y; }
            set
            {
                var angle = _gameObject.transform.localEulerAngles;
                angle.y = -value;
                _gameObject.transform.localEulerAngles = angle;
            }
        }

        public float rotationZ
        {
            get { return _gameObject.transform.localEulerAngles.z; }
            set
            {
                var angle = _gameObject.transform.localEulerAngles;
                angle.z = value;
                _gameObject.transform.localEulerAngles = angle;
            }
        }

        public GameObject GameObject
        {
            get { return _gameObject; }
        }

        public float alpha { get; set; }

        public bool visible
        {
            get { return _gameObject.activeSelf; }
            set
            {
                _gameObject.SetActive(value);
            }
        }

        public void AddChild(IObject3D child)
        {
            var pos = child.GameObject.transform.localPosition;
            var rot = child.GameObject.transform.localRotation;
            var scale = child.GameObject.transform.localScale;
            child.GameObject.transform.parent = _gameObject.transform;
            child.GameObject.transform.localPosition = pos;
            child.GameObject.transform.localRotation = rot;
            child.GameObject.transform.localScale = scale;
        }
    }
}
