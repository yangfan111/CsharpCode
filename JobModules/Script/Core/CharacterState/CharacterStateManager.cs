using System;
using System.Collections.Generic;
using Core.CharacterState.Action;
using Core.CharacterState.Movement;
using Core.CharacterState.Posture;
using Core.Fsm;
using UnityEngine;
using XmlConfig;
using Core.Utils;
using Utils.Appearance;
using Utils.CharacterState;

namespace Core.CharacterState
{
    public class CharacterStateManager : NewCommandFromCall, ICharacterState, IFsmTransitionHelper
    {
        private static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(CharacterStateManager));
        private readonly PostureManager _posture;
        private readonly MovementManager _movement;
        private readonly ActionManager _action;

        private readonly List<FsmSnapshot> _snapshotsCache;
        private ICharacterSpeed _speed;
        private bool _needReset;

        public CharacterStateManager()
        {
            int snapshotCount = 0;

            _posture = new PostureManager(this);
            snapshotCount += _posture.SnapshotCount();

            _movement = new MovementManager(this);
            snapshotCount += _movement.SnapshotCount();

            _action = new ActionManager(this);
            snapshotCount += _action.SnapshotCount();

            _snapshotsCache = new List<FsmSnapshot>(snapshotCount);
            for (int i = 0; i < snapshotCount; i++)
            {
                _snapshotsCache.Add(new FsmSnapshot());
            }
        }

        public void SetSpeedInterface(ICharacterSpeed charSpeed)
        {
            _speed = charSpeed;
        }

        public ICharacterPostureInConfig GetIPostureInConfig()
        {
            return _posture;
        }

        public ICharacterMovementInConfig GetIMovementInConfig()
        {
            return _movement;
        }

        #region ICharacterState

        private string _name;

        public void SetName(string name)
        {
            _posture.SetName(name);
            _movement.SetName(name);
            _action.SetName(name);
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public void PlayerReborn()
        {
            _needReset = true;
        }

        private bool _moveInWater;

        public void SetMoveInWater(bool value)
        {
            _moveInWater = value;
        }

        public bool IsMoveInWater()
        {
            return _moveInWater;
        }

        private bool _steepSlope;

        public void SetSteepSlope(bool value)
        {
            _steepSlope = value;
        }

        public bool IsSteepSlope()
        {
            return _steepSlope;
        }

        private bool _beenSlowDown;

        public void SetBeenSlowDown(bool value)
        {
            _beenSlowDown = value;
        }

        public bool IsSlowDown()
        {
            return _beenSlowDown;
        }

        private bool _exceedSlopeLimit;

        public void SetExceedSlopeLimit(bool value)
        {
            _exceedSlopeLimit = value;
        }

        public bool IsExceedSlopeLimit()
        {
            return _exceedSlopeLimit;
        }

        private bool _isSlide;

        
        public void SetSlide(bool value)
        {
            _isSlide = value;
        }
        
        public bool IsSlide()
        {
            return _isSlide;
        }

        #endregion

        #region IFsmUpdate

        public void Update(IAdaptiveContainer<IFsmInputCommand> commands,
            int frameInterval,
            Action<FsmOutput> addOutput,
            FsmUpdateType updateType)
        {
            if (_needReset)
            {
                _needReset = false;
                RebindAnimator(addOutput);
                _posture.Reset(addOutput);
                _movement.Reset(addOutput);
                _action.Reset(addOutput);
            }

            if ((updateType & FsmUpdateType.ResponseToInput) != 0)
                ApplyNewCommand(commands, addOutput);

            _posture.Update(commands, frameInterval, addOutput, updateType);
            _movement.Update(commands, frameInterval, addOutput, updateType);
            _action.Update(commands, frameInterval, addOutput, updateType);
            UpdateFullBodySpeedScale(addOutput);
        }

        #endregion

        #region ICharacterPosture

        public PostureInConfig GetCurrentPostureState()
        {
            return _posture.GetCurrentPostureState();
        }

        public PostureInConfig GetNextPostureState()
        {
            return _posture.GetNextPostureState();
        }

        public LeanInConfig GetCurrentLeanState()
        {
            return _posture.GetCurrentLeanState();
        }

        public LeanInConfig GetNextLeanState()
        {
            return _posture.GetNextLeanState();
        }

        public bool IsJumpStart()
        {
            return _posture.IsJumpStart();
        }

        public ActionInConfig GetActionState()
        {
            return _action.GetActionState();
        }

        public ActionInConfig GetNextActionState()
        {
            return _action.GetNextActionState();
        }

        public ActionKeepInConfig GetActionKeepState()
        {
            return _action.GetActionKeepState();
        }

        public ActionKeepInConfig GetNextActionKeepState()
        {
            return _action.GetNextActionKeepState();
        }

        public bool IsNeedJumpSpeed()
        {
            return _posture.IsNeedJumpSpeed();
        }

        public bool IsNeedJumpForSync
        {
            get { return _posture.IsNeedJumpForSync; }
            set { _posture.IsNeedJumpForSync = value; }
        }

        #endregion

        #region ISyncFsmSnapshot

        public IList<FsmSnapshot> GetSnapshots()
        {
            int index = 0;

            _posture.GetSnapshot(_snapshotsCache, index);
            index += _posture.SnapshotCount();

            _movement.GetSnapshot(_snapshotsCache, index);
            index += _movement.SnapshotCount();

            _action.GetSnapshot(_snapshotsCache, index);

            return _snapshotsCache;
        }

        public void TryRewind()
        {
            int index = 0;

            _posture.SetSnapshot(_snapshotsCache, index);
            index += _posture.SnapshotCount();

            _movement.SetSnapshot(_snapshotsCache, index);
            index += _movement.SnapshotCount();

            _action.SetSnapshot(_snapshotsCache, index);
        }

        #endregion

        #region ICharacterSpeed

        // 更新全身动画速率
        private bool _speedRatioChanged = false;
        private float _ratio = 1;

        private void UpdateFullBodySpeedScale(Action<FsmOutput> addOutput)
        {
            _speedRatioChanged =
                SpeedRatio() != _ratio && !float.IsNaN(SpeedRatio()) && !float.IsInfinity(SpeedRatio());
            if (_speedRatioChanged)
            {
                _ratio = SpeedRatio();
                FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FullBodySpeedRatioHash,
                    AnimatorParametersHash.Instance.FullBodySpeedScaleName,
                    _ratio,
                    CharacterView.FirstPerson | CharacterView.ThirdPerson);
                addOutput(FsmOutput.Cache);
            }
        }

        public Vector3 GetSpeed(Vector3 lastVelocity, float deltaTime, float buff)
        {
            return _speed.GetSpeed(lastVelocity, deltaTime, buff);
        }

        public Vector3 GetSpeedOffset(float buff = 0)
        {
            return _speed.GetSpeedOffset(buff);
        }

        public void SetSpeedAffect(float affect)
        {
            _speed.SetSpeedAffect(affect);
        }

        public float SpeedRatio()
        {
            return _speed.SpeedRatio();
        }

        #endregion

        #region ICharacterMovement

        public MovementInConfig GetCurrentMovementState()
        {
            return _movement.GetCurrentMovementState();
        }

        public MovementInConfig GetNextMovementState()
        {
            return _movement.GetNextMovementState();
        }

        public bool IsForth
        {
            get { return _movement.IsForth; }
        }

        public bool IsBack
        {
            get { return _movement.IsBack; }
        }

        public bool IsLeft
        {
            get { return _movement.IsLeft; }
        }

        public bool IsRight
        {
            get { return _movement.IsRight; }
        }

        public bool IsUp
        {
            get { return _movement.IsUp; }
        }

        public bool IsDown
        {
            get { return _movement.IsDown; }
        }

        public float HorizontalValue
        {
            get { return _movement.HorizontalValue; }
        }

        public float VerticalValue
        {
            get { return _movement.VerticalValue; }
        }

        public float UpDownValue
        {
            get { return _movement.UpDownValue; }
        }

        public void UpdateAxis(float horizontalValue, float verticalValue, float upDownValue)
        {
            _movement.UpdateAxis(horizontalValue, verticalValue, upDownValue);
        }

        #endregion

        #region ICharacterAction

        public bool CanFire()
        {
            var moveState = GetCurrentMovementState();
            return _action.CanFire() &&
                   (moveState == MovementInConfig.Idle || moveState == MovementInConfig.Walk ||
                    moveState == MovementInConfig.Run || moveState == MovementInConfig.Sprint) &&
                   GetCurrentPostureState() != PostureInConfig.Dying &&
                   GetCurrentPostureState() != PostureInConfig.Swim;
        }

        public bool CanDraw()
        {
            var curActionState = _action.GetActionState();
            var curKeepState = _action.GetActionKeepState();
            var curPostureState = _posture.GetCurrentPostureState();
            if (curKeepState == ActionKeepInConfig.Drive || curPostureState == PostureInConfig.Dive ||
                curPostureState == PostureInConfig.Swim || curActionState == ActionInConfig.Props)
                return false;
            return true;
        }

        // 是否需要因人物动作改变打断救援
        public bool NeedInterruptRescue(PostureInConfig posture)
        {
            return ActionKeepInConfig.Rescue == GetActionKeepState() &&
                   (ActionInConfig.Null != GetActionState() ||
                    posture != GetCurrentPostureState() ||
                    MovementInConfig.Idle != GetCurrentMovementState());
        }

        // 是否可以进入受击状态
        public bool CanBeenHit()
        {
            return ActionInConfig.Null == GetActionState() &&
                   ActionKeepInConfig.Null == GetActionKeepState();
        }

        #endregion

        #region

        public float GetDurationCoefficient(string id)
        {
            return 1;
        }

        #endregion

        private static void RebindAnimator(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.RebindAnimator, 0);
            addOutput(FsmOutput.Cache);
        }

        public override void ServerUpdate()
        {
            base.ServerUpdate();
        }
    }
}