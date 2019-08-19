using App.Shared.Components;
using App.Shared.Player;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using Core.Room;
using System;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.FreeFramework.Free.player
{
    [Serializable]
    public class IniAvatarAction : AbstractPlayerAction, IRule
    {
        private IGameAction action;
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            if (action != null)
            {
                action.Act(args);
            }

            if (player.playerInfo.CampInfo != null && player.playerInfo.CampInfo.Preset.Count > 0 && player.playerInfo.CampInfo.CurrCamp != player.playerInfo.Camp
                && GameRules.ClothesType(args.GameContext.session.commonSession.RoomInfo.ModeId) != (int) EGameModeClothes.Default)
            {
                foreach (Preset p in player.playerInfo.CampInfo.Preset)
                {
                    if (p.camp == player.playerInfo.Camp)
                    {
                        player.gamePlay.NewRoleId = p.roleModelId;
                        break;
                    }
                }
            }
            else
            {
                var ids = (player.playerInfo.AvatarIds == null || player.playerInfo.AvatarIds.Count == 0) ? player.playerInfo.CacheAvatarIds : player.playerInfo.AvatarIds;
                for(int i = 0; i < (ids == null ? 0 : ids.Count); i++)
                {
                    var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(ids[i], player.GetSex());
                    var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
                    if (avatar != null)
                    {
                        player.appearanceInterface.Appearance.ChangeAvatar(resId);
                    }
                }
            }
        }

        public static void InitAvatar(Contexts contexts, PlayerEntity player)
        {
            if(!SharedConfig.IsServer) return;
            if (player == null || !player.hasPlayerInfo || !player.hasAppearanceInterface) return;
            int modeId = contexts.session.commonSession.RoomInfo.ModeId;

            bool @default = (GameRules.ClothesType(modeId) == (int)EGameModeClothes.Default);

            var ids = player.playerInfo.AvatarIds;
            if (@default/* || modeId == 2005*/) // 非阵营模式
            {
                if (0 == ids.Count)
                {
                    player.playerInfo.AvatarIds.AddRange(player.playerInfo.CacheAvatarIds);
                    ids = player.playerInfo.AvatarIds;
                }
            }
            else
            {
                if (0 == ids.Count && null != player.playerInfo.CampInfo)
                {
                    foreach (Preset p in player.playerInfo.CampInfo.Preset)
                    {
                        if (p.camp == player.playerInfo.Camp)
                        {
                            player.playerInfo.CampInfo.CurrCamp = player.playerInfo.Camp;
                            player.playerInfo.AvatarIds.Clear();
                            player.playerInfo.AvatarIds.AddRange(p.avatarIds);
                            ids = player.playerInfo.AvatarIds;
                            break;
                        }
                    }
                }
            }

            for(int i = 0; i < (ids == null ? 0 : ids.Count); i++)
            {
                var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(ids[i], player.GetSex());
                var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
                if (avatar != null)
                {
                    player.appearanceInterface.Appearance.ChangeAvatar(resId);
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.IniAvatarAction;
        }
    }
}
