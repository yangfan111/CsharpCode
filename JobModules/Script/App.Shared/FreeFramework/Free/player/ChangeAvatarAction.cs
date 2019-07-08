using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Sharpen;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class ChangeAvatarAction : AbstractPlayerAction, IRule
    {
        private int avatar;
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity entity = GetPlayerEntity(args);
            if (entity != null && entity.hasAppearanceInterface) {
                entity.appearanceInterface.Appearance.ChangeAvatar(avatar);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.ChangeAvatarAction;
        }
    }
}
