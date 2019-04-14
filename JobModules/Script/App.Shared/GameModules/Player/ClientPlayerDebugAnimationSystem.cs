using System;
using System.Collections.Generic;
using App.Shared.Components.Player;
using App.Shared.GameModules.Player.Animation;
using App.Shared.Util;
using Core.Animation;
using Core.CharacterState;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Player
{
    public class ClientPlayerDebugAnimationSystem:AbstractUserCmdExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientPlayerDebugAnimationSystem));
        
        private static readonly int DefaultLayerIndex = 0;

        protected override bool filter(PlayerEntity playerEntity)
        {
            return SharedConfig.DebugAnimation && playerEntity.hasThirdPersonAnimator;
        }

        protected override void ExecuteUserCmd(PlayerEntity playerEntity, IUserCmd cmd)
        {
            var animator = playerEntity.thirdPersonAnimator.UnityAnimator;
            var seq = cmd.Seq;
            int layerIndex = DefaultLayerIndex;
            try
            {
                Logger.InfoFormat(AnimationHelper.PrintAnimator(animator, layerIndex, seq));
                Logger.InfoFormat(AnimationHelper.PrintAnimatorParam(animator));
                Logger.InfoFormat(AnimationHelper.PrintNetworkAnimator(playerEntity.networkAnimator, layerIndex,seq));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        



        
    }
}