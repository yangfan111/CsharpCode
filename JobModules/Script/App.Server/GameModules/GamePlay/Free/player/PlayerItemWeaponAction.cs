using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using com.wd.free.util;
using Core.Free;
using System;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerItemWeaponAction : AbstractPlayerAction, IRule
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
            {}

                /*SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ChangeAvatar;

                int itemId = FreeUtil.ReplaceInt("{item.itemId}", args);

                playerEntity.WeaponController().PickUpWeapon(WeaponUtil.CreateScan(itemId));
                //playerEntity.bag.Bag.SetWeaponBullet(30);
                //playerEntity.bag.Bag.SetReservedCount(100);

                message.Ins.Add(itemId);

                message.Ks.Add(2);

                playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);*/
            }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerItemWeaponAction;
        }
        
    }

    }
