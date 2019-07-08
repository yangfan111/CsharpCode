using App.Shared.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using System;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class IniAvatarAction : AbstractPlayerAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            var ids = player.playerInfo.AvatarIds;
            if (0 == ids.Count) {
                ids = player.playerInfo.CacheAvatarIds;
            }

            for(int i = 0; i < (ids == null ? 0 : ids.Count); i++)
            {
                PutOn(player, ids[i]);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.IniAvatarAction;
        }

        private void PutOn(PlayerEntity playerEntity, int id)
        {
            var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(id, playerEntity.GetSex());
            var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
            if (avatar != null)
            {
                playerEntity.appearanceInterface.Appearance.ChangeAvatar(resId);
            }
        }
    }
}
