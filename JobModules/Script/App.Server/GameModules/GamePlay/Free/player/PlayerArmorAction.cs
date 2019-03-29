using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Utils.Configuration;
using XmlConfig;
using WeaponConfigNs;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.GameModules.Bullet;
using com.wd.free.item;
using App.Shared.FreeFramework.framework.util;
using com.wd.free.para;
using Core.Enums;
using Assets.XmlConfig;
using App.Server.GameModules.GamePlay.Free.item.config;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerArmorAction : AbstractPlayerAction
    {
        private bool initial;

        private static Dictionary<int, ArmorData> armorDic;

        private static void Initial()
        {
            if (armorDic == null)
            {
                armorDic = new Dictionary<int, ArmorData>();

                armorDic.Add(1, new ArmorData(1, 1, 30, 100, new int[] { (int)EBodyPart.Chest, (int)EBodyPart.Stomach, (int)EBodyPart.Pelvis }));
                armorDic.Add(2, new ArmorData(2, 1, 40, 100, new int[] { (int)EBodyPart.Chest, (int)EBodyPart.Stomach, (int)EBodyPart.Pelvis }));
                armorDic.Add(3, new ArmorData(3, 1, 55, 100, new int[] { (int)EBodyPart.Chest, (int)EBodyPart.Stomach, (int)EBodyPart.Pelvis }));

                armorDic.Add(8, new ArmorData(8, 2, 30, 100, new int[] { (int)EBodyPart.Head }));
                armorDic.Add(9, new ArmorData(9, 2, 40, 100, new int[] { (int)EBodyPart.Head }));
                armorDic.Add(10, new ArmorData(10, 2, 55, 100, new int[] { (int)EBodyPart.Head }));
            }
        }

        public override void DoAction(IEventArgs args)
        {
            Initial();

            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (playerEntity != null)
            {
                if (initial)
                {
                    InitialArmor(args, playerEntity);
                }
                else
                {
                    ReduceDamage(args, playerEntity);
                }
            }
        }

        private void SetArmor(PlayerEntity player, int id)
        {
            if (armorDic.ContainsKey(id))
            {
                ArmorData data = armorDic[id];
                if (data.type == 1)
                {
                    player.gamePlay.ArmorLv = id;
                    player.gamePlay.MaxArmor = data.max;
                    player.gamePlay.CurArmor = data.max;
                }
                if (data.type == 2)
                {
                    player.gamePlay.HelmetLv = id;
                    player.gamePlay.MaxHelmet = data.max;
                    player.gamePlay.CurHelmet = data.max;
                }
            }
        }

        private void InitialArmor(IEventArgs args, PlayerEntity player)
        {
            var avatars = player.playerInfo.AvatarIds;
            for (int i = 0; i < avatars.Count; i++)
            {

                SetArmor(player, avatars[i]);

            }
        }

        private void ReduceDamage(IEventArgs args, PlayerEntity player)
        {
            SimpleParable sp = (SimpleParable)args.GetUnit("damage");
            if (sp != null)
            {
                PlayerDamageInfo damage = (PlayerDamageInfo)sp.GetFieldObject(0);
                float da = damage.damage;
                if (damage.part == (int)EBodyPart.Head)
                {
                    int helId = player.gamePlay.HelmetLv;
                    if (armorDic.ContainsKey(helId))
                    {
                        ArmorData ad = armorDic[helId];

                        da = ReduceDamage(args, player, damage, ad.reduce, false);
                    }
                }
                else if (damage.part == (int)EBodyPart.Chest || damage.part == (int)EBodyPart.Stomach || damage.part == (int)EBodyPart.Pelvis)
                {
                    int armor = player.gamePlay.ArmorLv;
                    if (armorDic.ContainsKey(armor))
                    {
                        ArmorData ad = armorDic[armor];

                        da = ReduceDamage(args, player, damage, ad.reduce, true);
                    }
                }

                FloatPara d = (FloatPara)args.GetDefault().GetParameters().Get("damage");
                if (d != null)
                {
                    d.SetValue(da);
                }
            }
        }

        private static float ReduceDamage(IEventArgs args, PlayerEntity player, PlayerDamageInfo damage, int percent, bool armor)
        {
            float readDamage = damage.damage;
            if (damage.type != (int)EUIDeadType.Weapon && damage.type != (int)EUIDeadType.Unarmed)
            {
                return readDamage;
            }

            float reduce = damage.damage * percent / 100;
            float realReduce = reduce;
            if (armor)
            {
                realReduce = Math.Min(player.gamePlay.CurArmor, reduce);
            }
            else
            {
                realReduce = Math.Min(player.gamePlay.CurHelmet, reduce);
            }
            damage.damage -= realReduce;

            FreeData fd = (FreeData)player.freeData.FreeData;

            // 普通帽子不减少
            args.TempUse("current", fd);
            if (realReduce > 0)
            {
                if (armor)
                {
                    player.gamePlay.CurArmor = Math.Max(0, player.gamePlay.CurArmor - (int)readDamage);
                    if (player.gamePlay.CurArmor == 0)
                    {
                        PlayerItemAvatarAction.TakeOff(player, player.gamePlay.ArmorLv);
                        FreeItemInfo info = FreeItemConfig.GetItemInfo((int)ECategory.Avatar, player.gamePlay.ArmorLv);
                        FuntionUtil.Call(args, "showBottomTip", "msg", "{desc:10075," + info.name + "}");
                    }
                }
                else
                {
                    player.gamePlay.CurHelmet = Math.Max(0, player.gamePlay.CurHelmet - (int)readDamage);
                    if (player.gamePlay.CurHelmet == 0)
                    {
                        PlayerItemAvatarAction.TakeOff(player, player.gamePlay.HelmetLv);
                        FreeItemInfo info = FreeItemConfig.GetItemInfo((int)ECategory.Avatar, player.gamePlay.HelmetLv);
                        FuntionUtil.Call(args, "showBottomTip", "msg", "{desc:10075," + info.name + "}");
                    }
                }

                player.statisticsData.Statistics.DefenseDamage += realReduce;
            }
            args.Resume("current");

            return damage.damage;
        }
    }

    class ArmorData
    {
        public int id;
        public int reduce;
        public int max;
        public int[] parts;
        public int type;

        public ArmorData(int id, int type, int reduce, int max, int[] parts)
        {
            this.id = id;
            this.reduce = reduce;
            this.max = max;
            this.parts = parts;
            this.type = type;
        }
    }
}
