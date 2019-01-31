using App.Shared.Components.GenericActions;
using App.Shared.Player;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player.Actions
{
    public class GenericAction : IGenericAction
    {
        private IAction _climbAction = new ClimbUp(); //攀爬
        private IAction _stepAction = new StepUp();  //台阶
        private IAction _vaultAction = new Vault();  //翻越
        private IAction _concretAction;
        
        private Vector3 _capsuleBottom;
        private Vector3 _capsuleUp;
        private float _capsuleRadius;

        public void PlayerReborn(PlayerEntity player)
        {
            if(player.hasThirdPersonAnimator)
                player.thirdPersonAnimator.UnityAnimator.applyRootMotion = false;
            if(player.hasThirdPersonModel)
                player.thirdPersonModel.Value.transform.localPosition.Set(0, 0, 0);
            ResetConcretAction();
        }

        public void PlayerDead(PlayerEntity player)
        {
            if(player.hasThirdPersonAnimator)
                player.thirdPersonAnimator.UnityAnimator.applyRootMotion = false;
            if(player.hasThirdPersonModel)
                player.thirdPersonModel.Value.transform.localPosition.Set(0, 0, 0);
            ResetConcretAction();
        }

        public void Update(PlayerEntity player)
        {
            if(null != _concretAction)
            {
                _concretAction.Update();
                _concretAction.AnimationBehaviour();
            }
        }

        public void ActionInput(PlayerEntity player)
        {
            TestTrigger(player);
            if (null != _concretAction)
            {
                _concretAction.ActionInput(player);
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
            var playerTransform = player.RootGo().transform;
            RaycastHit hit;
            var overlapPos = playerTransform.position;
            
            PlayerEntityUtility.GetCapsule(player, overlapPos, out _capsuleBottom, out _capsuleUp, out _capsuleRadius);
            var capsuleHeight = _capsuleUp.y - _capsuleBottom.y;
            _capsuleBottom.y = (playerTransform.position + playerTransform.up * 0.5f).y;
            if ((null == _concretAction || !_concretAction.PlayingAnimation) &&
                Physics.CapsuleCast(_capsuleBottom, _capsuleUp, 0.001f, playerTransform.forward, out hit, 1,
                UnityLayers.SceneCollidableLayerMask))
            {
                //如果碰撞点到发射点的距离大于0.5
                if (hit.distance < 0.5)
                {
                    //得到碰撞点
                    var sphereCenter = new Vector3(hit.point.x, hit.collider.bounds.center.y + hit.collider.bounds.extents.y + capsuleHeight, hit.point.z);

                    // 检测发射源是否处于碰撞体中
                    if (Physics.OverlapSphere(sphereCenter, 0.3f, UnityLayers.SceneCollidableLayerMask).Length >
                        0) return;
                    
                    RaycastHit sphereHit;
                    Physics.SphereCast(sphereCenter, 0.3f, Vector3.down, out sphereHit,
                        hit.collider.bounds.center.y + hit.collider.bounds.extents.y + capsuleHeight,
                        UnityLayers.SceneCollidableLayerMask);
                    var point = sphereHit.point;
                    
                    var distance = point.y - playerTransform.position.y;
                    if (distance > 1.5 && distance < 2.3)
                    {
                        //一定高度内为climb
                        ResetConcretAction();
                        _concretAction = _climbAction;
                        _concretAction.MatchTarget = point - playerTransform.right * 0.2f;
                        
                        _concretAction.CanTriggerAction = true;
                    }else if (distance > 0.5 && distance <= 1.5)
                    {
                        ResetConcretAction();

                        // 检测翻越过程中障碍
                        _capsuleBottom.y = (point + playerTransform.up * 0.3f).y;
                        _capsuleUp.y = _capsuleBottom.y + capsuleHeight;
                        var canVault = !Physics.CapsuleCast(_capsuleBottom, _capsuleUp, 0.1f, playerTransform.forward,
                            out hit, 1, UnityLayers.SceneCollidableLayerMask);

                        //
                        overlapPos = playerTransform.position + playerTransform.forward * (1.0f + _capsuleRadius) + playerTransform.up * 0.2f;
                        PlayerEntityUtility.GetCapsule(player, overlapPos, out _capsuleBottom, out _capsuleUp, out _capsuleRadius);
                        var casts = Physics.OverlapCapsule(_capsuleBottom, _capsuleUp, _capsuleRadius, UnityLayers.SceneCollidableLayerMask);
                        //
                        if (casts.Length <= 0 && distance > 0.8f && canVault)
                        {
                            _concretAction = _vaultAction;
                        }
                        else
                        {
                            _concretAction = _stepAction;
                        }
                        _concretAction.MatchTarget = point;
                        _concretAction.CanTriggerAction = true;
                    }
                }
                else
                {
                    ResetConcretAction();
                }
            }
            else
            {//没有探测到障碍物
                ResetConcretAction();
            }
        }

        private void ResetConcretAction()
        {
            if (null != _concretAction)
                _concretAction.ResetConcretAction();
            _concretAction = null;
        }
    }

    enum GenericActionKind
    {
        Vault,
        Step,
        Climb
    }
}
