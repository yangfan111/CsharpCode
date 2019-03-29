using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Action.States;
using Core.CharacterState.Posture;
using Core.Fsm;
using Utils.CharacterState;
using XmlConfig;

namespace Core.CharacterState.Action
{
    class ActionFsm : FiniteStateMachine, IGetActionState
    {
        public ActionFsm(string name) : base(name)
        {
        }

        private Action<Action<FsmOutput>> _resetParam;

        public void InitCommon(IFsmTransitionHelper infoProvider)
        {
            AddState(ActionState.CreateCommonNullState(), infoProvider);

            AddState(ActionState.CreateFireState(), infoProvider);
            AddState(ActionState.CreateSpecialFireState(), infoProvider);
            AddState(ActionState.CreateSpecialFireHold(), infoProvider);
            AddState(ActionState.CreateSpecialFireEnd(), infoProvider);
            AddState(ActionState.CreateInjuryState(), infoProvider);
            AddState(ActionState.CreateReloadState(), infoProvider);
            AddState(ActionState.CreateSpecialReloadState(), infoProvider);

            AddState(ActionState.CreateUnarmState(), infoProvider);
            AddState(ActionState.CreateDrawState(), infoProvider);
            AddState(ActionState.CreateSwitchWeaponState(), infoProvider);
            AddState(ActionState.CreatePickUpState(), infoProvider);
            AddState(ActionState.CreateMeleeAttackState(), infoProvider);
            AddState(ActionState.CreateGrenadeState(), infoProvider);
            AddState(ActionState.CreateOpenDoorState(), infoProvider);
            AddState(ActionState.CreatePropsState(), infoProvider);

            AddState(ActionState.CreateGlidingState(), infoProvider);
            AddState(ActionState.CreateParachutingState(), infoProvider);

            AddState(ActionState.CreateBuriedBombState(), infoProvider);
            AddState(ActionState.CreateDismantleBombState(), infoProvider);

            _resetParam = ResetCommon;
        }

        public void InitKeep(IFsmTransitionHelper infoProvider)
        {
            AddState(ActionState.CreateKeepNullState(), infoProvider);

            AddState(ActionState.CreateVehiclesAnimState(), infoProvider);
            AddState(ActionState.CreateSightP3State(), infoProvider);
            AddState(ActionState.CreateRescueState(), infoProvider);

            _resetParam = ResetKeep;
        }

        public bool CanFire()
        {
            return ActionStateId.Reload != (ActionStateId) CurrentState.StateId;
        }

        public override void Reset(Action<FsmOutput> addOutput)
        {
            base.Reset(addOutput);
            if (_resetParam != null)
            {
                _resetParam(addOutput);
            }
        }

        private void ResetCommon(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UpperBodySpeedRatioHash,
                AnimatorParametersHash.Instance.UpperBodySpeedRatioName,
                1.0f,
                CharacterView.FirstPerson | CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);
           
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHash,
                AnimatorParametersHash.Instance.FireName,
                AnimatorParametersHash.Instance.FireDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.FireHoldHash,
                AnimatorParametersHash.Instance.FireHoldName,
                AnimatorParametersHash.Instance.FireHoldDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.InjuryHash,
                AnimatorParametersHash.Instance.InjuryName,
                AnimatorParametersHash.Instance.InjuryEndValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ReloadHash,
                AnimatorParametersHash.Instance.ReloadName,
                AnimatorParametersHash.Instance.ReloadDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.ReloadEmptyHash,
                AnimatorParametersHash.Instance.ReloadEmptyName,
                AnimatorParametersHash.Instance.ReloadEmptyDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SpecialReloadHash,
                AnimatorParametersHash.Instance.SpecialReloadName,
                AnimatorParametersHash.Instance.SpecialReloadDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HolsterHash,
                AnimatorParametersHash.Instance.HolsterName,
                AnimatorParametersHash.Instance.HolsterDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SelectHash,
                AnimatorParametersHash.Instance.SelectName,
                AnimatorParametersHash.Instance.SelectDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.SwitchWeaponHash,
                AnimatorParametersHash.Instance.SwitchWeaponName,
                AnimatorParametersHash.Instance.SwitchWeaponDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.HolsterStateHash,
                AnimatorParametersHash.Instance.HolsterStateName,
                AnimatorParametersHash.Instance.HolsterFromRightValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DrawStateHash,
                AnimatorParametersHash.Instance.DrawStateName,
                AnimatorParametersHash.Instance.DrawRightValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.PickUpHash,
                AnimatorParametersHash.Instance.PickUpName,
                AnimatorParametersHash.Instance.PickUpDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeAttackHash,
                AnimatorParametersHash.Instance.MeleeAttackName,
                AnimatorParametersHash.Instance.MeleeAttackEnd,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
            
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.MeleeStateHash,
                AnimatorParametersHash.Instance.MeleeStateName,
                AnimatorParametersHash.Instance.NullMelee,
                CharacterView.FirstPerson | CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.StartThrowHash,
                AnimatorParametersHash.Instance.StartThrowName,
                AnimatorParametersHash.Instance.StartThrowDisable,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.RescueHash,
                AnimatorParametersHash.Instance.RescueName,
                AnimatorParametersHash.Instance.RescueDisableValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.GlidingHash,
                AnimatorParametersHash.Instance.GlidingName,
                AnimatorParametersHash.Instance.GlidingDisableValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.UseHash,
                AnimatorParametersHash.Instance.UseName,
                AnimatorParametersHash.Instance.UseDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.DismantleHash,
                AnimatorParametersHash.Instance.DismantleName,
                AnimatorParametersHash.Instance.DismantleDisableValue,
                CharacterView.FirstPerson | CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayer,
                AnimatorParametersHash.Instance.ADSDisableValue,
                CharacterView.ThirdPerson);
            addOutput(FsmOutput.Cache);

            FsmOutput.Cache.SetLayerWeight(AnimatorParametersHash.Instance.ADSLayerP1,
                AnimatorParametersHash.Instance.ADSDisableValue,
                CharacterView.FirstPerson);
            addOutput(FsmOutput.Cache);
        }

        private void ResetKeep(Action<FsmOutput> addOutput)
        {
            FsmOutput.Cache.SetValue(AnimatorParametersHash.Instance.VehiclesAnimHash,
                AnimatorParametersHash.Instance.VehiclesAnimName,
                AnimatorParametersHash.Instance.VehiclesAnimDisableValue,
                CharacterView.ThirdPerson, false);
            addOutput(FsmOutput.Cache);
        }

        private bool GetCurrentOrNextState(bool getCurrent, ActionStateId state)
        {
            if (getCurrent)
            {
                return state == (ActionStateId) CurrentState.StateId;
            }

            if (CurrentState.ActiveTransition != null)
            {
                return state == (ActionStateId) CurrentState.ActiveTransition.To;
            }

            return false;
        }

        public ActionInConfig GetActionState()
        {
            var ret = StateIdAdapter.GetActionStateId((ActionStateId) CurrentState.StateId);
            return ret;
        }

        //TODO 确认逻辑是否正确
        public ActionInConfig GetNextActionState()
        {
            var next = CurrentState.ActiveTransition == null
                ? ActionStateId.KeepNull
                : (ActionStateId) CurrentState.ActiveTransition.To;
            var ret = StateIdAdapter.GetActionStateId(next);
            return ret;
        }

        public ActionKeepInConfig GetActionKeepState()
        {
            var ret = StateIdAdapter.GetActionKeepStateId((ActionStateId) CurrentState.StateId);
            //if (ret == ActionKeepInConfig.Null)
            //{
            //    var cur = (ActionStateId)CurrentState.StateId;
            //    var next = CurrentState.ActiveTransition == null
            //        ? (ActionStateId)CurrentState.StateId
            //        : (ActionStateId)CurrentState.ActiveTransition.To;
            //    ret = StateIdAdapter.GetActionKeepStateId(cur, next);
            //}
            return ret;
        }

        public ActionKeepInConfig GetNextActionKeepState()
        {
            var next = CurrentState.ActiveTransition == null
                ? ActionStateId.KeepNull
                : (ActionStateId) CurrentState.ActiveTransition.To;
            return StateIdAdapter.GetActionKeepStateId(next);
        }
    }
}