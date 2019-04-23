﻿using System;
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

        public void InitAsLeanState(IFsmTransitionHelper infoProvider, ICharacterInfoProvider characterInfo)
        {
            AddState(PostureState.CreateNoPeekState(), infoProvider, characterInfo);
            AddState(PostureState.CreatePeekLeftState(), infoProvider, characterInfo);
            AddState(PostureState.CreatePeekRightState(), infoProvider, characterInfo);
            _resetParam = ResetLeanStateParam;
        }

        public void InitAsCommonState(IFsmTransitionHelper infoProvider, ICharacterInfoProvider characterInfo)
        {
            AddState(PostureState.CreateStandState(), infoProvider, characterInfo);
            AddState(PostureState.CreateCrouchState(), infoProvider, characterInfo);
            AddState(PostureState.CreateProneState(), infoProvider, characterInfo);
            AddState(PostureState.CreateJumpStartState(), infoProvider, characterInfo);
            AddState(PostureState.CreateProneTransitState(), infoProvider, characterInfo);
            AddState(PostureState.CreateProneToStandState(), infoProvider, characterInfo);
            AddState(PostureState.CreateProneToCrouchState(), infoProvider, characterInfo);
            AddState(PostureState.CreateJumpEndState(), infoProvider, characterInfo);
            AddState(PostureState.CreateFreefallState(), infoProvider, characterInfo);
            AddState(PostureState.CreateSwimState(), infoProvider, characterInfo);
            AddState(PostureState.CreateDiveState(), infoProvider, characterInfo);
            AddState(PostureState.CreateDyingTransition(), infoProvider, characterInfo);
            AddState(PostureState.CreateDyingState(), infoProvider, characterInfo);

            AddState(PostureState.CreateClimbState(), infoProvider, characterInfo);
            AddState(PostureState.CreatePostureLadderState(), infoProvider, characterInfo);
            
            AddState(PostureState.CreateSlideState(), infoProvider, characterInfo);

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
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderHash,
                AnimatorParametersHash.Instance.LadderName,
                AnimatorParametersHash.Instance.LadderDisableValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderSpeedHash,
                AnimatorParametersHash.Instance.LadderSpeedName, 0.0f,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderEnterStateHash,
                AnimatorParametersHash.Instance.LadderEnterStateName, 
                AnimatorParametersHash.Instance.LadderEnterStateTop,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.LadderExitStateHash,
                AnimatorParametersHash.Instance.LadderExitStateName, 
                AnimatorParametersHash.Instance.LadderExitStateTop,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InjuredMoveHash,
                AnimatorParametersHash.Instance.InjuredMoveName,
                AnimatorParametersHash.Instance.InjuredMoveDisableValue,
                CharacterView.ThirdPerson,
                false);
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

        public bool IsState(short id)
        {
            return CurrentState.StateId == id;
        }
    }
}
