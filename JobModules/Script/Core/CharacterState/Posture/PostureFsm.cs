using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Action;
using Core.CharacterState.Movement;
using Core.Fsm;
using Core.Utils;
using Utils.CharacterState;
using XmlConfig;

namespace Core.CharacterState.Posture
{
    class PostureFsm : FiniteStateMachine, IGetPostureState
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PostureFsm));
        private bool _jumpStart;
        private Action<Action<FsmOutput>> _resetParam;

        public PostureFsm(string name) : base(name)
        {
        }

        #region state

        public bool IsNeedJumpSpeed()
        {
            var ret = _jumpStart;
            _jumpStart = false;
            return ret;
        }

        public bool IsNeedJumpForSync
        {
            get { return _jumpStart; }
            set { _jumpStart = value; }
        }

        public bool IsJumpStart()
        {
            return PostureStateId.JumpStart == (PostureStateId) CurrentState.StateId;
        }

        private bool GetCurrentOrNextState(bool getCurrent, PostureStateId state)
        {
            if (getCurrent)
            {
                return state == (PostureStateId) CurrentState.StateId;
            }

            if (CurrentState.ActiveTransition != null)
            {
                return state == (PostureStateId) CurrentState.ActiveTransition.To;
            }

            return false;
        }

        #endregion

        public void InitAsLeanState(IFsmTransitionHelper infoProvider)
        {
            AddState(PostureState.CreateNoPeekState(), infoProvider);
            AddState(PostureState.CreatePeekLeftState(), infoProvider);
            AddState(PostureState.CreatePeekRightState(), infoProvider);
            _resetParam = ResetLeanStateParam;
        }

        public void InitAsCommonState(IFsmTransitionHelper infoProvider)
        {
            AddState(PostureState.CreateStandState(), infoProvider);
            AddState(PostureState.CreateCrouchState(), infoProvider);
            AddState(PostureState.CreateProneState(), infoProvider);
            AddState(PostureState.CreateJumpStartState(), infoProvider);
            AddState(PostureState.CreateProneTransitState(), infoProvider);
            AddState(PostureState.CreateProneToStandState(), infoProvider);
            AddState(PostureState.CreateProneToCrouchState(), infoProvider);
            AddState(PostureState.CreateJumpEndState(), infoProvider);
            AddState(PostureState.CreateFreefallState(), infoProvider);
            AddState(PostureState.CreateSwimState(), infoProvider);
            AddState(PostureState.CreateDiveState(), infoProvider);
            AddState(PostureState.CreateDyingState(), infoProvider);

            AddState(PostureState.CreateClimbState(), infoProvider);
            
            AddState(PostureState.CreateSlideState(), infoProvider);

            _resetParam = ResetCommonStateParam;
        }

        public override void Reset(Action<FsmOutput> addOutput)
        {
            base.Reset(addOutput);
            if (_resetParam != null)
            {
                _resetParam(addOutput);
            }
        }

        protected override void SetCurrentState(int id, IFsmInputCommand command, string msg,
            Action<FsmOutput> addOutput)
        {
            base.SetCurrentState(id, command, msg, addOutput);

            if (PostureStateId.JumpStart == (PostureStateId) CurrentState.StateId)
            {
                _jumpStart = true;
            }
        }

        private void ResetCommonStateParam(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SlideHash,
                AnimatorParametersHash.Instance.SlideName,
                AnimatorParametersHash.Instance.SlideDisable,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PostureHash,
                AnimatorParametersHash.Instance.PostureName,
                AnimatorParametersHash.Instance.StandValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FrontPostureHash,
                AnimatorParametersHash.Instance.FrontPostureName,
                AnimatorParametersHash.Instance.FrontStand,
                CharacterView.FirstPerson | CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ProneHash,
                AnimatorParametersHash.Instance.ProneName,
                AnimatorParametersHash.Instance.ProneDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceEndProneHash,
                AnimatorParametersHash.Instance.ForceEndProneName,
                AnimatorParametersHash.Instance.ForceEndProneDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ForceToProneHash,
                AnimatorParametersHash.Instance.ForceToProneName,
                AnimatorParametersHash.Instance.ForceToProneDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.JumpStartHash,
                AnimatorParametersHash.Instance.JumpStartName,
                AnimatorParametersHash.Instance.JumpStartDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FreeFallHash,
                AnimatorParametersHash.Instance.FreeFallName,
                AnimatorParametersHash.Instance.FreeFallDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbHash,
                AnimatorParametersHash.Instance.ClimbName,
                AnimatorParametersHash.Instance.ClimbDisableValue,
                CharacterView.ThirdPerson | CharacterView.FirstPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbEndHash,
                AnimatorParametersHash.Instance.ClimbEndName,
                AnimatorParametersHash.Instance.ClimbEndDisableValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ClimbStateHash,
                AnimatorParametersHash.Instance.ClimbStateName,
                0.0f,
                CharacterView.ThirdPerson | CharacterView.FirstPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.SwimLayer,
                AnimatorParametersHash.Instance.SwimDisableValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.DyingLayer,
                AnimatorParametersHash.Instance.DyingDisableValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
        }
        
        private void ResetLeanStateParam(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(FsmOutputType.Peek,
                FsmOutput.NoPeekDegree);
            addOutput(FsmOutput.Cache);
        }

        public PostureInConfig GetCurrentPostureState()
        {
            PostureStateId id = (PostureStateId) CurrentState.StateId;
            return StateIdAdapter.GetPostureStateId(id);
        }

        public PostureInConfig GetNextPostureState()
        {
            PostureStateId id = CurrentState.ActiveTransition == null
                ? (PostureStateId) CurrentState.StateId
                : (PostureStateId) CurrentState.ActiveTransition.To;
            return StateIdAdapter.GetPostureStateId(id);
        }

        public LeanInConfig GetCurrentLeanState()
        {
            PostureStateId id = (PostureStateId) CurrentState.StateId;
            return StateIdAdapter.GetLeanStateId(id);
        }

        public LeanInConfig GetNextLeanState()
        {
            PostureStateId id = CurrentState.ActiveTransition == null
                ? (PostureStateId) CurrentState.StateId
                : (PostureStateId) CurrentState.ActiveTransition.To;
            return StateIdAdapter.GetLeanStateId(id);
        }
    }
}