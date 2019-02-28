using Core.CameraControl.NewMotor;
using UnityEngine;

namespace Assets.App.Shared.GameModules.Camera
{
    public class DummyCameraMotorOutput
    {
        public Vector3 ArchorPosition;

        public Vector3 ArchorEulerAngle;
        public Vector3 ArchorOffset;

        public Vector3 ArchorPostOffset;

        public Vector3 EulerAngle;


        public Vector3 Offset;

        public Vector3 PostOffset;

        public float FreeYaw;
        public float FreePitch;
        private float _far;
        private float _fov;
        private float _near;


        public float Far
        {
            get { return _far; }
            set
            {
                IsSetFar = true;
                _far = value;
            }
        }

        public float Fov
        {
            get { return _fov; }
            set
            {
                IsSetFov = true;
                _fov = value;
            }
        }

        public bool IsSetFov { get; set; }
        public bool IsSetFar { get; set; }
        public bool IsSetNear { get; set; }

        public float Near
        {
            get { return _near; }
            set
            {
                IsSetNear = true;
                _near = value;
            }
        }

        public DummyCameraMotorOutput()
        {
            Init();
        }

        public void Init()
        {
            _fov = 75;
            ArchorPosition = ArchorOffset =
                ArchorPostOffset = Offset = PostOffset = ArchorEulerAngle = EulerAngle = Vector3.zero;
            _far = 8000;
            _near = 0.03f;
            IsSetFar = false;
            IsSetFov = false;
            IsSetNear = false;
        }

        public DummyCameraMotorOutput Append(DummyCameraMotorOutput append)
        {
            ArchorEulerAngle += append.ArchorEulerAngle;
            ArchorOffset.x += append.ArchorOffset.x;
            ArchorOffset.y += append.ArchorOffset.y;
            ArchorOffset.z += append.ArchorOffset.z;
            ArchorPostOffset.x += append.ArchorPostOffset.x;
            ArchorPostOffset.y += append.ArchorPostOffset.y;
            ArchorPostOffset.z += append.ArchorPostOffset.z;
            EulerAngle.x = EulerAngle.x + append.EulerAngle.x;
            EulerAngle.y = EulerAngle.y + append.EulerAngle.y;
            EulerAngle.z = EulerAngle.z + append.EulerAngle.z;
            Offset.x += append.Offset.x;
            Offset.y += append.Offset.y;
            Offset.z += append.Offset.z;
            PostOffset.x += append.PostOffset.x;
            PostOffset.y += append.PostOffset.y;
            PostOffset.z += append.PostOffset.z;
            if (append.IsSetFov)
                Fov = append.Fov;
            if (append.IsSetFar)
                Far = append.Far;
            if (append.IsSetNear)
                Near = append.Near;
            return this;
        }
    }
}