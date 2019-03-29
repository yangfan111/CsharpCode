using App.Shared.Components.GenericActions;
using App.Shared.GameModules.Player.Actions.ClimbPackage;
using App.Shared.GameModules.Player.Actions.VaultPackage;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions
{
    public class GenericAction : IGenericAction
    {
        private readonly IAction[] _climbClasses = new IAction[(int)GenericActionKind.Null];
        private IAction _concretenessAction;

        public GenericAction()
        {
            RegisterClimbClass();
        }

        public void PlayerReborn(PlayerEntity player)
        {
            if (player.hasThirdPersonAnimator)
                player.thirdPersonAnimator.UnityAnimator.applyRootMotion = false;
            if (player.hasThirdPersonModel)
                player.thirdPersonModel.Value.transform.localPosition.Set(0, 0, 0);
            ResetConcretenessAction();
        }

        public void PlayerDead(PlayerEntity player)
        {
            if (player.hasThirdPersonAnimator)
                player.thirdPersonAnimator.UnityAnimator.applyRootMotion = false;
            if (player.hasThirdPersonModel)
                player.thirdPersonModel.Value.transform.localPosition.Set(0, 0, 0);
            ResetConcretenessAction();
        }

        public void Update(PlayerEntity player)
        {
            if (null != _concretenessAction)
            {
                _concretenessAction.Update();
                _concretenessAction.AnimationBehaviour();
            }
        }

        public void ActionInput(PlayerEntity player)
        {
            TestTrigger(player);
            if (null != _concretenessAction)
            {
                _concretenessAction.ActionInput(player);
            }
        }

        /**
         * 1.人物正前方做CapsuleCast(capsuleBottom向上微抬)
         * 2.hit点向上抬 探出碰撞体高 + 人物高  的距离
         * 3.向下做SphereCast(半径0.3)，目的是人物所站位置与攀爬位置有一定的容错
         * 4.hit点作为攀爬点，做MatchTarget(手到hit点差值)
         * 5.人物站立位置往正前方移动1m，做OverlapCapsule，检测翻越
         */
        private void TestTrigger(PlayerEntity player)
        {
            if (null == player) return;
            
            if (null != _concretenessAction && _concretenessAction.PlayingAnimation ||
                !ClimbUpCollisionTest.ClimbUpFrontDistanceTest(player))
            {
                ResetConcretenessAction();
                return;
            }

            GenericActionKind kind;
            Vector3 matchTarget;
            ClimbUpCollisionTest.ClimbUpTypeTest(player, out kind, out matchTarget);
            CreateConcretenessAction(kind, matchTarget);
        }

        private void CreateConcretenessAction(GenericActionKind kind, Vector3 matchTarget)
        {
            ResetConcretenessAction();
            
            if((int)kind < 0 || (int)kind >= _climbClasses.Length) return;
            
            _concretenessAction = _climbClasses[(int)kind];
            
            if (null == _concretenessAction) return;
            
            _concretenessAction.MatchTarget = matchTarget;
            _concretenessAction.CanTriggerAction = true;
        }

        private void ResetConcretenessAction()
        {
            if (null != _concretenessAction)
                _concretenessAction.ResetConcretAction();
            _concretenessAction = null;
        }

        private void RegisterClimbClass()
        {
            _climbClasses[(int)GenericActionKind.Vault50Cm] = new VaultUp50Cm(GenericActionKind.Vault50Cm);
            _climbClasses[(int)GenericActionKind.Vault1M] = new VaultUp1M(GenericActionKind.Vault1M);
            _climbClasses[(int)GenericActionKind.Vault2M] = new VaultUp2M(GenericActionKind.Vault2M);
            _climbClasses[(int)GenericActionKind.Climb50Cm] = new ClimbUp50Cm(GenericActionKind.Climb50Cm);
            _climbClasses[(int)GenericActionKind.Climb1M] = new ClimbUp1M(GenericActionKind.Climb1M);
            _climbClasses[(int)GenericActionKind.Climb2M] = new ClimbUp2M(GenericActionKind.Climb2M);
        }
    }

    public enum GenericActionKind
    {
        Vault50Cm,
        Vault1M,
        Vault2M,
        Climb50Cm,
        Climb1M,
        Climb2M,
        Null
    }
}