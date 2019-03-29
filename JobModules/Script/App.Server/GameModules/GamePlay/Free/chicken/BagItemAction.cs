using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using com.wd.free.util;
using App.Server.GameModules.GamePlay.free.player;
using WeaponConfigNs;
using App.Server.GameModules.GamePlay.Free.item;
using com.wd.free.item;
using Free.framework;
using Core.Free;
using App.Shared;
using com.wd.free.para;
using Assets.App.Server.GameModules.GamePlay.Free;
using Core;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.chicken
{
    /// <summary>
    /// 处理物品加入，移除玩家背包时的一些特性情况的处理
    /// </summary>
    class BagItemAction : AbstractGameAction
    {
        private const string ParaFrom = "{from}";
        private const string ParaTo = "{to}";
        private const string ParaItemCat = "{item.cat}";
        private const string ParaClipType = "{item.ClipType}";
        private const string ParaItemCount = "{item.count}";
        private const string ParaItemId = "{item.itemId}";
        private const string ClipType = "ClipType";

        public override void DoAction(IEventArgs args)
        {
            string from = FreeUtil.ReplaceVar(ParaFrom, args);
            string to = FreeUtil.ReplaceVar(ParaTo, args);
            string itemCat = FreeUtil.ReplaceVar(ParaItemCat, args);
            int clipType = FreeUtil.ReplaceInt(ParaClipType, args);

            IParable item = args.GetUnit(FreeArgConstant.Item);

            if (to == ChickenConstant.BagDefault && itemCat == ChickenConstant.ItemCatGrenadeWeapon)
            {
                HandleAddGrenade(args);
            }
            if (from == ChickenConstant.BagDefault && itemCat == ChickenConstant.ItemCatGrenadeWeapon)
            {
                HandleRemoveGrenade(args);
            }
            if (to == ChickenConstant.BagDefault && item.GetParameters().HasPara(ClipType))
            {
                HandleSetClip(args, clipType);
            }
            else if (from == ChickenConstant.BagDefault && item.GetParameters().HasPara(ClipType))
            {
                HandleClearClip(args, clipType);
            }

            HandleCapacity(args);
        }

        /// <summary>
        /// 扔掉背包中的子弹的时候，清空武器的备弹数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="clipType">子弹类型</param>
        private void HandleClearClip(IEventArgs args, int clipType)
        {
            FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit(FreeArgConstant.PlayerCurrent);
            if (fd != null)
            {
               fd.Player.WeaponController().SetReservedBullet((EBulletCaliber)clipType, 0);
            }
        }

        /// <summary>
        /// 增加背包中的子弹的时候，更新武器的备弹数
        /// </summary>
        /// <param name="args"></param>
        /// <param name="clipType">子弹类型</param>
        private void HandleSetClip(IEventArgs args, int clipType)
        {
            FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit(FreeArgConstant.PlayerCurrent);

            fd.Player.WeaponController().SetReservedBullet((EBulletCaliber)clipType, CarryClipUtil.GetClipCount(clipType, fd, args));
        }

        /// <summary>
        /// 手雷加入背包
        /// </summary>
        /// <param name="args"></param>
        private void HandleAddGrenade(IEventArgs args)
        {
            PlayerEntity playerEntity = ((FreeRuleEventArgs)args).GetPlayer(FreeArgConstant.PlayerCurrent);
            if (playerEntity != null)
            {
                int itemId = FreeUtil.ReplaceInt(ParaItemId, args);
                int count = FreeUtil.ReplaceInt(ParaItemCount, args);
                for (int i = 0; i < count; i++)
                {
                    var helper = playerEntity.WeaponController().GrenadeHelper;
                    playerEntity.WeaponController().TryHoldGrenade(itemId);
                    SimpleProto message = new SimpleProto();
                    message.Ks.Add(8);
                    message.Ins.Add(itemId);
                    message.Bs.Add(true);
                    message.Key = FreeMessageConstant.ChangeAvatar;
                    FreeMessageSender.SendMessage(playerEntity, message);



                    //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
                }
            }
        }

        /// <summary>
        /// 手雷从背包中移除
        /// </summary>
        /// <param name="args"></param>
        private void HandleRemoveGrenade(IEventArgs args)
        {
            PlayerEntity playerEntity = ((FreeRuleEventArgs)args).GetPlayer(FreeArgConstant.PlayerCurrent);
            if (playerEntity != null)
            {
                int itemId = FreeUtil.ReplaceInt(ParaItemId, args);
                int count = FreeUtil.ReplaceInt(ParaItemCount, args);
                for (int i = 0; i < count; i++)
                {
                    playerEntity.WeaponController().RemoveGreande(itemId);
                    SimpleProto message = new SimpleProto();
                    message.Ks.Add(8);
                    message.Ins.Add(itemId);
                    message.Bs.Add(false);
                    message.Key = FreeMessageConstant.ChangeAvatar;
                    FreeMessageSender.SendMessage(playerEntity, message);
                    //playerEntity.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
                }
            }
        }

        /// <summary>
        /// 更新背包的容量
        /// </summary>
        /// <param name="args"></param>
        private void HandleCapacity(IEventArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit(FreeArgConstant.PlayerCurrent);

            if (fd != null)
            {
                ChickenFuncUtil.UpdateBagCapacity(args, (int)Math.Ceiling(BagCapacityUtil.GetWeight(fd)), BagCapacityUtil.GetCapacity(fd));
            }
        }
    }
}
