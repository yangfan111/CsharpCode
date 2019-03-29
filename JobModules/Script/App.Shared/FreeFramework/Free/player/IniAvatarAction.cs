using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Utils.Configuration;
using Free.framework;
using Core.Free;
using App.Shared.Player;
using Utils.Singleton;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class IniAvatarAction : AbstractPlayerAction
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            var ids = player.playerInfo.AvatarIds;

            for(int i = 0; i < ids.Count; i++)
            {
                PutOn(player, ids[i]);
            }
        }

        private void PutOn(PlayerEntity playerEntity, int id)
        {
            var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(id, playerEntity.GetSex());
            var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
            if (avatar != null)
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ChangeAvatar;

                playerEntity.appearanceInterface.Appearance.ChangeAvatar(resId);

                message.Ins.Add(id);
                message.Ks.Add(1);

                //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }
    }
}
