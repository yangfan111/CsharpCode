using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Action;
using Core.CharacterState.Posture;
using Core.Fsm;
using Utils.CharacterState;
using XmlConfig;

namespace Core.CharacterState.Movement
{
    class MovementFsm : FiniteStateMachine, IGetMovementState
    {

        private Action<Action<FsmOutput>> _resetParam;

        public MovementFsm(string name) : base(name)
        {
        }

        #region IGetMovementState

        public MovementInConfig GetCurrentMovementState()
        {
            MovementStateId id = (MovementStateId)CurrentState.StateId;
            return StateIdAdapter.GetMovementStateId(id);
        }

        public MovementInConfig GetNextMovementState()
        {
            MovementStateId id = CurrentState.ActiveTransition == null ? (MovementStateId)CurrentState.StateId : (MovementStateId)CurrentState.ActiveTransition.To;
            return StateIdAdapter.GetMovementStateId(id);
        }

        #endregion

        public void Init(IFsmTransitionHelper infoProvider)
        {
            AddState(MovementState.CreateIdleState(), infoProvider);
            AddState(MovementState.CreateWalkState(), infoProvider);
            AddState(MovementState.CreateRunState(), infoProvider);
            AddState(MovementState.CreateSprintState(), infoProvider);
            AddState(MovementState.CreateDiveMoveState(), infoProvider);
            _resetParam = ResetMovement;
        }

        private void ResetMovement(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.IsWalkHash,
                AnimatorParametersHash.Instance.IsWalkName,
                AnimatorParametersHash.Instance.IsWalkDisable,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MotionHash,
                AnimatorParametersHash.Instance.MotionName,
                AnimatorParametersHash.Instance.MotionlessValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
        }

        public override void Reset(Action<FsmOutput> addOutput)
        {
            base.Reset(addOutput);

            if (_resetParam != null)
            {
                _resetParam.Invoke(addOutput);
            }
        }
    }
}
