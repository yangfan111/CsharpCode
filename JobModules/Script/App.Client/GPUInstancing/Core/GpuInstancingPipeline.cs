using App.Client.GPUInstancing.Core.Data;
using App.Client.GPUInstancing.Core.Utils;
using UnityEngine;

namespace App.Client.GPUInstancing.Core
{
    public abstract class GpuInstancingPipeline
    {
        private readonly GpuInstancingCamera _viewData = new GpuInstancingCamera();
        private Camera _renderingCamera;

        protected readonly ComputeShader VisibilityShader;

        private readonly float[] _cameraWorldPositiotn = new float[3];

        protected GpuInstancingPipeline(ComputeShader visibilityShader)
        {
            VisibilityShader = visibilityShader;
        }

        public void SetRenderingCamera(Camera cam)
        {
            _renderingCamera = cam;
            _viewData.SetRenderingCamera(cam);

            _renderingCamera.depthTextureMode = DepthTextureMode.None;
        }

        public void Draw()
        {
            FrustumCulling();
            CreateInstance();
            InstancingDraw(_renderingCamera);
        }

        protected virtual void FrustumCulling()
        { }

        protected virtual void CreateInstance()
        { }

        protected virtual void InstancingDraw(Camera camera)
        { }

        protected CameraFrustum GetFrustum()
        {
            return _viewData.GetCameraFrustum();
        }

        protected Vector3 GetCameraPosition()
        {
            return _viewData.CameraWorldPosition;
        }

        protected void VisibilityDetermination(InstancingDraw draw)
        {
            _viewData.CameraWorldPosition.ConvertToFloatArray(_cameraWorldPositiotn);
            var frustum = _viewData.GetCameraFrustum();

            draw.VisibilityDetermination(_cameraWorldPositiotn, _viewData.TanHalfFov,
                frustum .ClipNormalArray, frustum.FarClipDistance);
        }
    }
}
