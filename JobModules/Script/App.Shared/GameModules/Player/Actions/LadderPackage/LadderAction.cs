using System;
using App.Shared.Components.GenericActions;
using App.Shared.Player;
using Core.Animation;
using Core.CharacterState;
using Core.Fsm;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using Utils.Utils;

namespace App.Shared.GameModules.Player.Actions.LadderPackage
{
    public class LadderAction : ILadderAction
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(LadderAction));
        private readonly ClimbLadder _ladder = new ClimbLadder();
        private bool _startLadder = false;
        private LadderLocation _location;
        private FsmOutputCache _directOutputs = new FsmOutputCache();

        public void PlayerReborn(PlayerEntity player)
        {
            ResetPlayerStatus(player);
        }

        public void PlayerDead(PlayerEntity player)
        {
            ResetPlayerStatus(player);
        }

        public void Update(PlayerEntity player, float speed)
        {
            _ladder.SetPlayerData(player);
            _ladder.UpdateLadderSpeed(speed);
            
            if(_startLadder && !_ladder.IsUsingLadder)
                ResetPlayerStatus(player);

            var collisionLadder = true;
            if (_ladder.NeedCheckLadderKind())
            {
                collisionLadder = LadderCollisionTest.CheckLadderKind(player, out _location, !_startLadder);
            }
            
            if(_startLadder && !collisionLadder)
                ResetPlayerStatus(player);
            
            if (collisionLadder && speed > 0.05f)
            {
                _ladder.TestEnterLadder(_location);
                _startLadder = true;
            }
            
            _ladder.Update(_location);
        }
        
        public void Execute(PlayerEntity player, Action<FsmOutput> addOutput)
        {
            FullBodyLadder(player);
            _directOutputs.Apply(addOutput);
        }

        private void FullBodyLadder(PlayerEntity player)
        {
            if (!_startLadder) return;

            if (player.appearanceInterface.Appearance.GetWeaponIdInHand() == UniversalConsts.InvalidIntId)
                _directOutputs.CacheFsmOutput(NetworkAnimatorLayer.PlayerSyncLayer, 0,
                    CharacterView.ThirdPerson);
            else
                _directOutputs.CacheFsmOutput(NetworkAnimatorLayer.PlayerSyncLayer, 1,
                    CharacterView.ThirdPerson);
        }

        private void ResetPlayerStatus(PlayerEntity player)
        {
            if (player.hasThirdPersonAnimator)
                player.thirdPersonAnimator.UnityAnimator.applyRootMotion = false;
            if (player.hasThirdPersonModel)
            {
                player.thirdPersonModel.Value.transform.localPosition =
                    new Vector3(0, -PlayerEntityUtility.CcSkinWidth, 0);
                player.thirdPersonModel.Value.transform.localRotation = Quaternion.identity;
            }

            _startLadder = false;

            _directOutputs.CacheFsmOutput(NetworkAnimatorLayer.PlayerSyncLayer, 0,
                CharacterView.ThirdPerson);

            _ladder.ResetPlayerSetting();
        }
    }

    public enum LadderLocation
    {
        Top,
        Middle,
        Bottom,
        Null
    }
}
