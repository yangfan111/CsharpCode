using Assets.App.Server.GameModules.GamePlay.Free;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Free.framework;
using gameplay.gamerule.free.ui;
using System;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerVisibilityAction : AbstractPlayerAction
    {
        private bool visibility;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (null != playerEntity)
            {
                SimpleProto sp = FreePool.Allocate();
                sp.Key = FreeMessageConstant.PlayerVisibility;
                sp.Ins.Add(playerEntity.entityKey.Value.EntityId);
                sp.Bs.Add(visibility);
                SendMessageAction.sender.SendMessage(args, sp, 4, string.Empty);
            }
        }
    }
}
