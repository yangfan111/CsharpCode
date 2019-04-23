using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core.Data
{
    class GpuInstancingCamera
    {
        // visibility determination camera
        private Camera _renderingCamera;
        private readonly CameraFrustum _frustum = new CameraFrustum();

        private float _fov = float.MinValue;
        private float _tanHalfFov;

        public void SetRenderingCamera(Camera camera)
        {
            _renderingCamera = camera;
        }

        public CameraFrustum GetCameraFrustum()
        {
            _frustum.Update(_renderingCamera);

            return _frustum;
        }

        public float TanHalfFov
        {
            get
            {
                if (!Helper.AlmostEqual(_fov, _renderingCamera.fieldOfView))
                {
                    _fov = _renderingCamera.fieldOfView;
                    _tanHalfFov = Mathf.Tan(_fov * 0.5f * Mathf.Deg2Rad);
                }

                return _tanHalfFov;
            }
        }

        public Vector3 CameraWorldPosition
        {
            get { return _renderingCamera.transform.position; }
        }

        public Matrix4x4 VpMatrix
        {
            get { return _renderingCamera.projectionMatrix * _renderingCamera.worldToCameraMatrix; }
        }
    }
}
