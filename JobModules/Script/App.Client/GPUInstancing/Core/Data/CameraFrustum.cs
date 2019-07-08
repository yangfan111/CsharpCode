using App.Client.GPUInstancing.Core.Spatial;
using App.Client.GPUInstancing.Core.Utils;
using Core.Components;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Data
{
    public class CameraFrustum
    {
        public enum Direction
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
            Far = 4,
            EnumLength = 5
        }
        private static readonly int[] PlaneIndexIterator =
        {
            (int) Direction.Left, (int) Direction.Right, (int) Direction.Top, (int) Direction.Bottom, (int) Direction.Far,
            (int) Direction.Left, (int) Direction.Right, (int) Direction.Top, (int) Direction.Bottom, (int) Direction.Far
        };

        private float _fov = float.MinValue;
        private float _aspect = float.MinValue;

        // normal point to the inside of frustum
        private readonly Vector3[] _baseClipNormal = { Vector3.back, Vector3.back, Vector3.back, Vector3.back, Vector3.back };
        private readonly int[] _nearVertexIndices = new int[(int) Direction.EnumLength];

        private Vector3 _viewPoint;
        public Vector3 ViewPoint { get { return _viewPoint + WorldOrigin.Origin; } }
        private Vector3 _eulerAngles = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        private readonly Vector3[] _clipNormal = new Vector3[5];
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
            bool needChange = false;
            if (!Helper.AlmostEqual(_fov, camera.fieldOfView) || !Helper.AlmostEqual(_aspect, camera.aspect))
            {
                _fov = camera.fieldOfView;
                _aspect = camera.aspect;

                Calculate();
                needChange = true;
            }

            var transform = camera.transform;
            var currentEulerAngles = transform.eulerAngles;
            if (needChange || !Helper.AlmostEqual(_eulerAngles, currentEulerAngles))
            {
                _eulerAngles = currentEulerAngles;

                for (int i = 0; i < (int) Direction.EnumLength; ++i)
                {
                    _clipNormal[i] = transform.TransformDirection(_baseClipNormal[i]);
                    _clipNormal[i].ConvertToFloatArray(_clipNormalArray[i]);

                    int nearVertexIndex = (_clipNormal[i].x > 0 ? 1 : 0) +
                                          (_clipNormal[i].y > 0 ? 2 : 0) +
                                          (_clipNormal[i].z > 0 ? 4 : 0);

                    _nearVertexIndices[i] = nearVertexIndex;
                }

                needChange = true;
            }

            if (needChange || !Helper.AlmostEqual(_viewPoint, transform.position))
            {
                _viewPoint = transform.position;
                var cameraPos = ViewPoint;

                _clipDistance[(int) Direction.Left] = Vector3.Dot(_clipNormal[(int) Direction.Left], cameraPos);
                _clipDistance[(int) Direction.Right] = Vector3.Dot(_clipNormal[(int) Direction.Right], cameraPos);
                _clipDistance[(int) Direction.Top] = Vector3.Dot(_clipNormal[(int) Direction.Top], cameraPos);
                _clipDistance[(int) Direction.Bottom] = Vector3.Dot(_clipNormal[(int) Direction.Bottom], cameraPos);
            }
            
            _clipDistance[(int) Direction.Far] = -camera.farClipPlane;
        }

        public bool IsNodeVisible(GpuInstancingNodeIndicator node)
        {
            var start = node.LastOutsidePlaneIndex;

            var vertices = node.Vertices;
            // AABB and frustum intersection test
            for (int i = 0; i < (int) Direction.EnumLength; ++i)
            {
                var planeIndex = PlaneIndexIterator[start + i];

                if (Vector3.Dot(_clipNormal[planeIndex], vertices[_nearVertexIndices[planeIndex]]) < _clipDistance[planeIndex])
                {
                    node.LastOutsidePlaneIndex = planeIndex;
                    return false;
                }
            }
            

            return true;
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
