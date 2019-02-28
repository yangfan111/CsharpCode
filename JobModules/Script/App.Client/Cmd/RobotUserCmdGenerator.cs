using System;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.Robot.Utility;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace Assets.Sources
{
    public interface IRobotUserCmdProviderContainer
    {
        IRobotUserCmdProvider RobotUserCmdProvider { get; }
    }

    public class RobotUserCmdGenerator : IUserCmdGenerator, IRobotUserCmdProviderContainer
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(UnityUserCmdGenerator));
        private readonly UserCmd _lastUserCmd = UserCmd.Allocate();
        private readonly UserCmd _userCmd = UserCmd.Allocate();
        private readonly IRobotUserCmdProvider _userCmdProvider;
        private int _seq;
        public float MaxPitchChangeAngle = 180;
        public float MaxYawChangeAngle = 180;
        private PlayerEntity _playerEntity;

        public RobotUserCmdGenerator(IRobotUserCmdProvider userCmdProvider)
        {
            _userCmdProvider = userCmdProvider;
            Type = CmdGeneratorType.Robot;
        }

        public IRobotUserCmdProvider RobotUserCmdProvider
        {
            get { return _userCmdProvider; }
        }


        public CmdGeneratorType Type { get; private set; }
        private Quaternion lookRotation;

        public UserCmd GenerateUserCmd(IUserCmdOwnAdapter  player,int intverval)
        {
            UpdateLookAt(player, _userCmdProvider.LookAt);


            if (_userCmdProvider.HasPath)
            {
                var vel = player.PlayerTransform.InverseTransformVector(_userCmdProvider.DesirwdVelocity);

                vel.y = 0;
                vel = vel.normalized;
                _userCmd.MoveVertical = vel.z;
                _userCmd.MoveHorizontal = vel.x;
               
               
                

                if (_userCmd.DeltaYaw > 1)
                {
                    _userCmd.IsSlightWalk = true;
                }
                else if (_userCmdProvider.DesirwdVelocity.magnitude > 2)
                {
                    _userCmd.IsRun = true;
                }
                else if (_userCmdProvider.DesirwdVelocity.magnitude > 0.5)
                {
                }
                else
                {
                    _userCmd.IsSlightWalk = true;
                }
            }

            _userCmd.IsCrouch = _userCmdProvider.IsCrouch;
            _userCmd.IsJump = _userCmdProvider.IsJump;
            _userCmd.IsProne = _userCmdProvider.IsProne;
            _userCmd.IsPeekLeft= _userCmdProvider.IsPeekLeft;
            _userCmd.IsPeekRight=_userCmdProvider.IsPeekRight;
            _userCmd.IsF = _userCmdProvider.IsF;
            _userCmd.IsLeftAttack = _userCmdProvider.IsLeftAttack;
            _userCmd.IsReload = _userCmdProvider.IsReload;


            //_userCmdProvider.Reset();
//            _userCmdProvider.LookAt = Quaternion.Euler(0,0,
//                 0);
            _userCmd.FrameInterval = intverval;
            _userCmd.Seq = _seq++;
            var rc = UserCmd.Allocate();
            _userCmd.CopyTo(rc);
            _userCmd.Reset();
            return rc;
        }

        private void UpdateLookAt(IUserCmdOwnAdapter player, Quaternion lookat)
        {
            var forYaw = lookat.eulerAngles.y;
            var forPitch = lookat.eulerAngles.x;
            var lastYaw = player.Yaw;
            var lastPitch = player.Pitch;


            _userCmd.DeltaYaw = RobotUtility.RestrictInnerAngle(forYaw - lastYaw);
            _userCmd.DeltaPitch = RobotUtility.RestrictInnerAngle(forPitch - lastPitch);
        }

        public void SetLastUserCmd(UserCmd userCmd)
        {
            userCmd.CopyTo(_lastUserCmd);
        }

        public void SetUserCmd(Action<UserCmd> cb)
        {
            if (null != cb) cb(_userCmd);
        }

        public void MockUserCmd(Action<UserCmd> cb)
        {

        }
    }
}