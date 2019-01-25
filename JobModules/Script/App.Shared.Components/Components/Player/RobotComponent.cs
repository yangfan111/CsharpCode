using System.Collections.Generic;
using Core.Prediction.UserPrediction.Cmd;
using Entitas;
using UnityEngine;
using UnityEngine.AI;

namespace App.Shared.Components.Player
{
    public interface IRobotUserCmdProvider
    {
        bool HasPath { get; set; }
        Vector3 DesirwdVelocity { get; set; }
        Quaternion LookAt { get; set; }
        bool IsLookAt { get; set; }
        int FrameInterval { get; set; }
       
        Vector3 TargetPosition { get; set; }
        float JumpMinSpeed { get; set; }
        float StopDistance { get; set; }
        bool IsJump { get; set; }
        bool IsF { get; set; }
        bool IsLeftAttack { get; set; }
        bool IsCrouch { get; set; }
        bool IsProne { get; set; }
        bool IsPeekLeft { get; set; }
        bool IsPeekRight { get; set; }
        bool IsReload { get; set; }
        void Reset();
    }

    public class DummyRobotUserCmdProvider : IRobotUserCmdProvider
    {
        public bool HasPath { get; set; }
        public Vector3 DesirwdVelocity { get; set; }
        public bool IsLookAt { get; set; }
        public Quaternion LookAt { get; set; }
        public int FrameInterval { get; set; }
       
     
        public bool IsJump { get; set; }
        public float JumpMinSpeed { get; set; }
        public Vector3 TargetPosition { get; set; }
     
        public float StopDistance { get; set; }
        public bool IsF { get; set; }
        public bool IsCrouch { get; set; }
        public bool IsProne { get; set; }
        public bool IsPeekLeft { get; set; }
        public bool IsPeekRight { get; set; }
        public bool IsLeftAttack { get; set; }
        public bool IsReload { get; set; }

        public void Reset()
        {
            HasPath = false;
            IsJump = false;
            LookAt = Quaternion.identity;
            IsJump = false;
            IsCrouch = false;
            IsProne = false;
            IsCrouch = false;
            IsPeekLeft = false;
            IsPeekRight = false;
            IsReload = false;
            IsLeftAttack = false;
        }
    }

    public interface IRobotConfig
    {
        int CalcPathInterval { get; }
    }

    public class DummyRobotConfig : IRobotConfig
    {
        public DummyRobotConfig()
        {
            CalcPathInterval = 200;
        }

        public int CalcPathInterval { get; private set; }
    }

    public interface INavMeshAgentBridgeWapper
    {
        void Run();
        void OnGrounded(bool grounded);
    }

    public interface IRobotSpeedInfo
    {
        float JumpVo { get; }
        float MaxSpeed { get; }
        float JumpMaxHeight { get; }
        float JumpAcceleration { get; }
    }

    public class DummyRobotSpeedInfo : IRobotSpeedInfo
    {
        public float JumpVo
        {
            get { return 3.4f; }
        }

        public float MaxSpeed
        {
            get { return 2; }
        }

        public float JumpMaxHeight
        {
            get { return 5 * JumpVo * JumpVo / 100; }
        }

        public float JumpAcceleration
        {
            get { return 10f; }
        }
    }

    public interface IRobotPlayerWapper
    {
        NavMeshAgent NavMeshAgent { get; }

        PlayerEntity Entity { get; }

        IRobotUserCmdProvider RobotUserCmdProvider { get; }

        IRobotSpeedInfo RobotSpeedInfo { get; }
        IUserCmdGenerator UserCmdGenerator { get; }
        IRobotConfig RobotConfig { get; }
        Vector3 Destination { get; set; }
        Vector3 Position { get; }
        GameObject TargetInSight(float value, float f, HashSet<GameObject> gameObjects);
        Entitas.IContexts GameContexts { get; }
        bool LastIsOnGround { get; set; }
        int LastLifeState { get; }
        bool IsOnGround { get; }
        int LifeState { get; }
        void StopNavMeshAgent();
        IRobotWeaponStat GetCurrentWeaponStat();
       
        bool CanFire();
        bool LineOfSight(Vector3 position, PlayerEntity target, bool checkposition);
    }

    public interface IRobotWeaponStat
    {
        float MaxUseDist { get; }
        float MinUseDist { get; }
    }

    [Player]
    public class RobotComponent : IComponent
    {
        public IRobotPlayerWapper Wapper;
    }
}