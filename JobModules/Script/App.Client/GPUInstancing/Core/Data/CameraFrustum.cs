using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Data
{
    public class CameraFrustum
    {
        public enum Direction
        {
            Left = 0,
            Right = 1,
            Far = 2,
            Top = 3,
            Bottom = 4
        }

        private float _fov = float.MinValue;
        private float _aspect = float.MinValue;

        private readonly Vector3[] _baseClipNormal =
        {
            Vector3.back, Vector3.back, Vector3.back, Vector3.back, Vector3.back
        };

        private Vector3 _viewPoint;
        public Vector3 ViewPoint { get { return _viewPoint; } }
        private Vector3 _eulerAngles = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        private float _eulerYAngle = float.MinValue;

        private readonly Vector3[] _clipNormal = new Vector3[5];
        private readonly Vector3[] _2DClipNormal = new Vector3[3];
        private readonly float[] _clipDistance = { 0, 0, 0, 0, 0 };

        private readonly float[][] _clipNormalArray =
        {
            new float[3],
            new float[3],
            new float[3],
            new float[3],
            new float[3]
        };

        public float[][] ClipNormalArray { get { return _clipNormalArray; } }
        public float FarClipDistance { get { return _clipDistance[(int) Direction.Far]; } }

        public void Update(Camera camera)
        {
            bool basicChanged = false;
            if (!Helper.AlmostEqual(_fov, camera.fieldOfView) || !Helper.AlmostEqual(_aspect, camera.aspect))
            {
                _fov = camera.fieldOfView;
                _aspect = camera.aspect;

                Calculate();
                basicChanged = true;
            }

            var transform = camera.transform;
            var currentEulerAngles = transform.eulerAngles;
            if (!Helper.AlmostEqual(_eulerAngles, currentEulerAngles) || basicChanged)
            {
                _eulerAngles = currentEulerAngles;
                for (int i = 0; i < 5; ++i)
                {
                    _clipNormal[i] = transform.TransformDirection(_baseClipNormal[i]);
                    _clipNormal[i].ConvertToFloatArray(_clipNormalArray[i]);
                }
            }

            if (!Helper.AlmostEqual(_eulerYAngle, currentEulerAngles.y) || basicChanged)
            {
                _eulerYAngle = currentEulerAngles.y;

                var rotationY = Quaternion.AngleAxis(_eulerYAngle, Vector3.up);
                _2DClipNormal[(int) Direction.Left] = rotationY * _baseClipNormal[(int) Direction.Left];
                _2DClipNormal[(int) Direction.Right] = rotationY * _baseClipNormal[(int) Direction.Right];
                _2DClipNormal[(int) Direction.Far] = rotationY * _baseClipNormal[(int) Direction.Far];
            }

            _viewPoint = transform.position;
            _clipDistance[(int) Direction.Far] = -camera.farClipPlane;
        }

        private readonly Vector3[] _shiftedBigAabbPos = new Vector3[8];
        public bool IsDetailNodeVisible(Vector3 min, Vector3 max)
        {
            // 顶面4个点在前
            _shiftedBigAabbPos[0].Set(max.x, max.y, max.z);
            _shiftedBigAabbPos[1].Set(max.x, max.y, min.z);
            _shiftedBigAabbPos[2].Set(min.x, max.y, max.z);
            _shiftedBigAabbPos[3].Set(min.x, max.y, min.z);
            _shiftedBigAabbPos[4].Set(max.x, min.y, max.z);
            _shiftedBigAabbPos[5].Set(max.x, min.y, min.z);
            _shiftedBigAabbPos[6].Set(min.x, min.y, max.z);
            _shiftedBigAabbPos[7].Set(min.x, min.y, min.z);

            for (int i = 0; i < 8; ++i)
            {
                if (Inside3DFrustum(_shiftedBigAabbPos[i]))
                    return true;
            }

            if (_viewPoint.x >= min.x && _viewPoint.x <= max.x && _viewPoint.z >= min.z && _viewPoint.z <= max.z)
                return true;

            // 2m为间隔检查离相机最近的一条/两条边
            float startX = min.x;
            float endX = max.x;
            float startZ = min.z;
            float endZ = max.z;
            float fixedX = 0;
            float fixedZ = 0;
            float pointInterval = 2;

            if (_viewPoint.x < min.x)
                fixedX = min.x;
            else if (_viewPoint.x > max.x)
                fixedX = max.x;
            else
                startZ = endZ;

            if (_viewPoint.z < min.z)
                fixedZ = min.z;
            else if (_viewPoint.z > max.z)
                fixedZ = max.z;
            else
                startX = endX;

            Vector3 p = new Vector3(startX, max.y, fixedZ);
            for (float i = startX; i < endX; i += pointInterval)
            {
                p.x = i;
                if (Inside3DFrustum(p))
                    return true;
            }

            p.Set(fixedX, max.y, startZ);
            for (float i = startZ; i < endZ; i += pointInterval)
            {
                p.z = i;
                if (Inside3DFrustum(p))
                    return true;
            }

            return false;
        }

        public bool Is2DRectVisible(Vector2 basePos, Vector2 rectSize)
        {
            bool ret = false;

            Vector2 shiftedPos = new Vector2(basePos.x - _viewPoint.x, basePos.y - _viewPoint.z);
            ret = Helper.FarthestDistance(_2DClipNormal[(int) Direction.Left], shiftedPos, rectSize) >= _clipDistance[(int) Direction.Left] || ret;
            ret = Helper.FarthestDistance(_2DClipNormal[(int) Direction.Right], shiftedPos, rectSize) >= _clipDistance[(int) Direction.Right] || ret;
            ret = Helper.FarthestDistance(_2DClipNormal[(int) Direction.Far], shiftedPos, rectSize) >= _clipDistance[(int) Direction.Far] || ret;

            return ret;
        }

        private bool Inside3DFrustum(Vector3 p)
        {
            p -= _viewPoint;
            bool inside = _clipNormal[(int)Direction.Left].Dot(p) > _clipDistance[(int)Direction.Left];
            inside = inside && _clipNormal[(int)Direction.Right].Dot(p) > _clipDistance[(int)Direction.Right];
            inside = inside && _clipNormal[(int)Direction.Far].Dot(p) > _clipDistance[(int)Direction.Far];
            inside = inside && _clipNormal[(int)Direction.Top].Dot(p) > _clipDistance[(int)Direction.Top];
            return inside && _clipNormal[(int)Direction.Bottom].Dot(p) > _clipDistance[(int)Direction.Bottom];
        }

        private void Calculate()
        {
            var cosHalfFov = Mathf.Cos(_fov * 0.5f / 180 * Mathf.PI);
            var sinHalfFov = Mathf.Sin(_fov * 0.5f / 180 * Mathf.PI);
            _baseClipNormal[(int) Direction.Top] = new Vector3(0, -cosHalfFov, sinHalfFov);
            _baseClipNormal[(int) Direction.Bottom] = new Vector3(0, cosHalfFov, sinHalfFov);

            var tanHalfWFov = sinHalfFov / cosHalfFov * _aspect;
            var cosHalfWFov = Mathf.Sqrt(1 / (1 + tanHalfWFov * tanHalfWFov));
            var sinHalfWFov = tanHalfWFov * cosHalfWFov;

            _baseClipNormal[(int) Direction.Left] = new Vector3(cosHalfWFov, 0, sinHalfWFov);
            _baseClipNormal[(int) Direction.Right] = new Vector3(-cosHalfWFov, 0, sinHalfWFov);
        }
    }
}
