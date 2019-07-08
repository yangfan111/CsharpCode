using App.Shared.Player;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;
using Core.Free;
using Shared.Scripts;
using System;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerItemAvatarAction : AbstractPlayerAction, IRule
    {
        private bool takeoff;

        public override void DoAction(IEventArgs args)
        {
            if (string.IsNullOrEmpty(player))
            {
                player = "current";
            }

            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            PlayerEntity playerEntity = (PlayerEntity)fr.GetEntity(player);

            IParable item = args.GetUnit("item");

            if (playerEntity != null && item != null)
            {
                int itemId = FreeUtil.ReplaceInt("{item.itemId}", args);
                if (FreeUtil.ReplaceInt("{item.itemCat}", args) == (int) ECategory.Weapon)
                {
                    if (takeoff)
                    {
                        if (SingletonManager.Get<WeaponResourceConfigManager>().IsArmor(itemId))
                        {
                            playerEntity.gamePlay.ArmorLv = playerEntity.gamePlay.CurArmor = playerEntity.gamePlay.MaxArmor = 0;
                        }

                        if (SingletonManager.Get<WeaponResourceConfigManager>().IsHelmet(itemId))
                        {
                            playerEntity.gamePlay.HelmetLv = playerEntity.gamePlay.CurHelmet = playerEntity.gamePlay.MaxHelmet = 0;
                        }
                    }
                    else
                    {
                        var config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(itemId);
                        if (config.Type == (int) EWeaponType_Config.Armor)
                        {
                            playerEntity.gamePlay.ArmorLv = itemId;
                            playerEntity.gamePlay.CurArmor = FreeUtil.ReplaceInt("{item.count}", args);
                            playerEntity.gamePlay.MaxArmor = config.Durable;
                        }

                        if (config.Type == (int) EWeaponType_Config.Helmet)
                        {
                            playerEntity.gamePlay.HelmetLv = itemId;
                            playerEntity.gamePlay.CurHelmet = FreeUtil.ReplaceInt("{item.count}", args);
                            playerEntity.gamePlay.MaxHelmet = config.Durable;
                        }
                    }
                }

                if (FreeUtil.ReplaceInt("{item.itemCat}", args) == (int) ECategory.Avatar)
                {
                    var resId = SingletonManager.Get<RoleAvatarConfigManager>().GetResId(itemId, playerEntity.GetSex());
                    var avatar = SingletonManager.Get<AvatarAssetConfigManager>().GetAvatarAssetItemById(resId);
                    if (avatar != null)
                    {
                        if (takeoff)
                        {
                            playerEntity.appearanceInterface.Appearance.ClearAvatar((Wardrobe) avatar.AvatarType);
                        }
                        else
                        {
                            playerEntity.appearanceInterface.Appearance.ChangeAvatar(resId);
                        }
                    }
                }
            }

        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerItemAvatarAction;
        }
    }
}
