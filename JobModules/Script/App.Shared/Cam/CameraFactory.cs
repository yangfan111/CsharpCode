using Core.Utils;
using UnityEngine;

namespace App.Shared.Cam
{

    public interface ICameraFactory
    {
        Camera CreateFpCamera(Camera mainCamera);
        Camera CreateFxCamera(Camera mainCamera);
    }

    public class AbstractCameraFactory
    {
        public virtual Camera CreateFpCamera(Camera mainCamera)
        {
          
            var fpCamera = new GameObject("FPCamera").AddComponent<Camera>();
            fpCamera.transform.SetParent(mainCamera.transform);
            fpCamera.transform.localPosition = Vector3.zero;
            fpCamera.transform.localRotation = Quaternion.identity;
            fpCamera.transform.localScale = Vector3.one;
          
            fpCamera.nearClipPlane = 0.01f;

            fpCamera.depth = mainCamera.depth + 1;
            fpCamera.clearFlags = CameraClearFlags.Depth;
            return fpCamera;
        } 

        public virtual Camera CreateFxCamera(Camera mainCamera)
        {
            mainCamera.cullingMask ^= UnityLayerManager.GetLayerMask(EUnityLayerName.CameraFx);
            var fxCamera = new GameObject("FxCamera").AddComponent<Camera>();
            fxCamera.transform.SetParent(mainCamera.transform);
            fxCamera.transform.localPosition = Vector3.zero;
            fxCamera.transform.localRotation = Quaternion.identity;
            fxCamera.transform.localScale = Vector3.one;
            fxCamera.cullingMask = UnityLayerManager.GetLayerMask(EUnityLayerName.CameraFx);
            fxCamera.nearClipPlane = 0.01f;

            fxCamera.depth = mainCamera.depth + 2;
            fxCamera.clearFlags = CameraClearFlags.Depth;
            fxCamera.renderingPath = RenderingPath.Forward;
            fxCamera.useOcclusionCulling = false;
            return fxCamera;
        }
    }

    public class SingleCameraFactory : AbstractCameraFactory, ICameraFactory
    {
        public override Camera CreateFpCamera(Camera mainCamera)
        {
            return mainCamera;
        }

        public override Camera CreateFxCamera(Camera mainCamera)
        {
            return mainCamera;
        }
    }

    public class DualCameraFactory : AbstractCameraFactory, ICameraFactory
    {
    }
}
