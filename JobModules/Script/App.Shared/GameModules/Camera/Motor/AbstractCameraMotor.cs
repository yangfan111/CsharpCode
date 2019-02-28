using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Player;
using App.Shared.GameModules.Camera;
using Assets.App.Shared.GameModules.Camera;
using Core.Utils;
using UnityEngine;
using XmlConfig;
using System.Collections.Specialized;
using Utils.Configuration;
using Utils.Singleton;

namespace Core.CameraControl.NewMotor
{
    public interface IMotorConfig
    {
        float MinYaw { get; }
        float MaxYaw { get; }
        float MinPitch { get; }
        float MaxPitch { get; }
        float Smoothing { get; }
        bool UseYawLimits { get; }
        bool UsePitchLimits { get; }
    }

    public interface IMotorActive
    {
        bool IsActive(ICameraMotorInput input, ICameraMotorState state);
    }


    public interface ICameraMainMotor
    {
        CameraConfigItem Config { get; }
    }

    public interface ICameraNewMotor
    {
        short ModeId { get; }

        bool IsActive(ICameraMotorInput input, ICameraMotorState state);
        Vector3 FinalArchorOffset { get; }
        Vector3 FinalArchorPostOffset { get; }
        Vector3 FinalEulerAngle { get; }
        Vector3 FinalOffset { get; }
        Vector3 FinalPostOffset { get; }
        float FinalFov { get; }
        int Order { get; }

        void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output,
            ICameraNewMotor last, int clientTime);


        void AddEnterAction(Action<PlayerEntity, ICameraMotorState> action);
        void AddLeaveAction(Action<PlayerEntity, ICameraMotorState> action);

        void CallLeaveActions(PlayerEntity player, ICameraMotorState state);
        void CallEnterActions(PlayerEntity player, ICameraMotorState state);

        HashSet<short> ExcludeNextMotor();

        void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state);
    }

    abstract class AbstractCameraMainMotor : AbstractCameraMotor, ICameraMainMotor
    {
        protected CameraConfigItem _config;

        
        public CameraConfigItem Config
        {
            get { return _config; }
        }
    }

    public abstract class AbstractCameraMotor : ICameraNewMotor
    {
        protected static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractCameraMotor));
        protected Action<PlayerEntity, ICameraMotorState> EnterActions;
        protected Action<PlayerEntity, ICameraMotorState> LeaveActions;
        
        public abstract short ModeId { get; }

        protected static HashSet<short> EmptyHashSet = new HashSet<short>();
        public abstract bool IsActive(ICameraMotorInput input, ICameraMotorState state);

        public virtual Vector3 FinalArchorOffset
        {
            get { return Vector3.zero; }
        }

        public virtual Vector3 FinalArchorPostOffset
        {
            get { return Vector3.zero; }
        }

        public virtual Vector3 FinalEulerAngle
        {
            get { return Vector3.zero; }
        }

        public virtual Vector3 FinalOffset
        {
            get { return Vector3.zero; }
        }

        public virtual Vector3 FinalPostOffset
        {
            get { return Vector3.zero; }
        }

        public virtual float FinalFov
        {
            get { return 0; }
        }

        public virtual int Order
        {
            get { return 1; }
        }


        protected AbstractCameraMotor()
        {

        }

        
        public abstract void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime);

        public abstract void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state,
            PlayerEntity player);

        public void AddEnterAction(Action<PlayerEntity, ICameraMotorState> action)
        {
            EnterActions += (action);
        }

        public void AddLeaveAction(Action<PlayerEntity, ICameraMotorState> action)
        {
            LeaveActions += (action);
        }


        public void CallLeaveActions(PlayerEntity player, ICameraMotorState state)
        {
            if (LeaveActions != null)
            {
                LeaveActions(player, state);
            }
        }

        public void CallEnterActions(PlayerEntity player, ICameraMotorState state)
        {
            if (EnterActions != null)
            {
                EnterActions(player, state);
            }
        }

        protected float EaseInOutQuad(float start, float end, float value)
        {
            value /= .5f;
            end -= start;
            if (value < 1) return end * 0.5f * value * value + start;
            value--;
            return -end * 0.5f * (value * (value - 2) - 1) + start;
        }


        public abstract HashSet<short> ExcludeNextMotor();
        public abstract void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state);

        protected static float CalculateFrameVal(float angle, float delta, ViewLimit limit)
        {
            angle = YawPitchUtility.Normalize(angle);
            return limit.Flag && (limit.Min > -180f || limit.Max < 180f)
                ? Mathf.Clamp(angle + delta, limit.Min, limit.Max)
                : (angle + delta);
        }

        protected static float ElapsedPercent(int clientTime, int enterTime, float tTime)
        {
            var elapsedPercent = tTime > 0f ? (clientTime - enterTime) / tTime : 0f;
            elapsedPercent = elapsedPercent > 1f ? 1 : elapsedPercent;
            elapsedPercent = elapsedPercent < 0f ? 0 : elapsedPercent;
            return elapsedPercent;
        }
    }






    

}