using System;
using App.Server.GameModules.GamePlay.Free.action.ui;
using com.wd.free.action;
using com.wd.free.@event;
using gameplay.gamerule.free.ui;
using Core.Free;
using Free.framework;
using com.wd.free.util;
using UnityEngine;
using App.Shared.GameModules.Vehicle;
using App.Server.GameModules.GamePlay.free.player;
using com.wd.free.item;
using WeaponConfigNs;
using gameplay.gamerule.free.item;
using com.wd.free.skill;
using App.Server.GameModules.GamePlay.Free.item;
using com.wd.free.action.function;
using System.Collections.Generic;
using com.wd.free.para;
using App.Shared.GameModules.Bullet;
using Core.Enums;
using Assets.XmlConfig;
using App.Server.GameModules.GamePlay.Free.item.config;
using App.Server.GameModules.GamePlay.Free.client;
using App.Shared.GameModules.Player;
using Core;
using App.Shared;
using App.Shared.Util;
using App.Shared.GameModules.Weapon;

namespace App.Server.GameModules.GamePlay.Free.action
{
    [Serializable]
    public class UseCodeAction : AbstractGameAction
    {
        private string code;

        static UseCodeAction()
        {
        }

        public override void DoAction(IEventArgs args)
        {
            HandleScore(args);
            HandleKill(args);
            HandleFrame(args);
            HandleAddFuel(args);
            HandleBullet(args);
            HandleDropWeapon(args);
            HandleBagWeight(args);
            HandleArmor(args);
            HandleTest(args);
            HandleAutoPartPut(args);
            HandleFogDamage(args);
            HandleAddGrenade(args);
        }

        private void HandleAddGrenade(IEventArgs args)
        {
            if (code == "AddGrenade")
            {
                PlayerEntity p = ((FreeRuleEventArgs)args).GetPlayer("current");
                if (p != null)
                {
                    int itemId = FreeUtil.ReplaceInt("{item.itemId}", args);

                    p.WeaponController().TryHoldGrenade(itemId);
               
                }
            }
        }

        private void HandleFogDamage(IEventArgs args)
        {
            FreeData fd = (FreeData)args.GetUnit("current");
            if (fd != null)
            {

            }
        }

        private void HandleAutoPartPut(IEventArgs args)
        {
            if (code == "AutoPart")
            {
                FreeData fd = (FreeData)args.GetUnit("current");
                if (fd != null)
                {
                    ItemPosition[] items = fd.freeInventory.GetInventoryManager().GetDefaultInventory().GetItems();
                    foreach (ItemPosition ip in items)
                    {
                        FreeItemInfo info = FreeItemConfig.GetItemInfo(ip.key.GetKey());
                        if (info.cat == (int)ECategory.WeaponPart)
                        {
                            string inv = PickupItemUtil.AutoPutPart(fd, FreeItemConfig.GetItemInfo(info.cat, info.id));
                            if (inv != null && inv.StartsWith("w" + FreeUtil.ReplaceVar("{key}", args)))
                            {
                                ItemInventoryUtil.MovePosition(ip,
                                    fd.GetFreeInventory().GetInventoryManager().GetInventory(inv), 0, 0, (ISkillArgs)args);
                            }
                        }
                    }
                }
            }
        }

        private void HandleTest(IEventArgs args)
        {
            if ("test" == code)
            {
                ItemDrop id = new ItemDrop();
                id.cat = 2;
                id.id = 1;
                List<ItemDrop> ids = FreeItemDrop.GetExtraItems(id);

                foreach (ItemDrop item in ids)
                {
                    args.GameContext.session.entityFactoryObject.SceneObjectEntityFactory.
                                    CreateSimpleEquipmentEntity((ECategory)item.cat, item.id, item.count, new Vector3(2, 1, 2));
                }
            }

        }

        private PlayerDamageInfo GetDamageInfo(IEventArgs args)
        {
            SimpleParable sp = (SimpleParable)args.GetUnit("damage");
            if (sp != null)
            {
                return
                    (PlayerDamageInfo)((ObjectFields)((SimpleParaList)sp.GetParameters()).GetFieldList()[0]).GetObj();
            }
            else
            {
                return new PlayerDamageInfo();
            }
        }

        private void HandleArmor(IEventArgs args)
        {
            if ("ReduceDamage" == code)
            {
                FreeData fd = (FreeData)args.GetUnit("target");
                if (fd != null)
                {
                    SimpleParable sp = (SimpleParable)args.GetUnit("damage");
                    if (sp != null)
                    {
                        PlayerDamageInfo info = (PlayerDamageInfo)((ObjectFields)((SimpleParaList)sp.GetParameters()).GetFieldList()[0]).GetObj();
                        float da = ReduceDamageUtil.HandleDamage(args, fd, info);
                        FloatPara d = (FloatPara)args.GetDefault().GetParameters().Get("damage");
                        if (d != null)
                        {
                            d.SetValue(da);
                        }
                    }
                }
            }
        }

        private void HandleBagWeight(IEventArgs args)
        {
            if ("BagWeight" == code)
            {
                FreeData fd = (FreeData)args.GetUnit("current");

                if (fd != null)
                {
                    UseCommonAction use = new UseCommonAction();
                    use.key = "updateBagCapacity";
                    use.values = new List<ArgValue>();
                    use.values.Add(new ArgValue("weight", ((int)Math.Ceiling(BagCapacityUtil.GetWeight(fd))).ToString()));
                    use.values.Add(new ArgValue("capacity", BagCapacityUtil.GetCapacity(fd).ToString()));

                    use.Act(args);
                }
            }
        }

        private void HandleDropWeapon(IEventArgs args)
        {
            if ("DropWeapon" == code)
            {
                int key = FreeUtil.ReplaceInt("{key}", args);
                FreeData fd = (FreeData)args.GetUnit("current");

                ItemInventory defInv = fd.freeInventory.GetInventoryManager().GetDefaultInventory();
                for (int i = 1; i <= 5; i++)
                {
                    ItemInventory ii = fd.freeInventory.GetInventoryManager().GetInventory("w" + key + "" + i);
                    if (ii.posList.Count > 0)
                    {
                        ItemPosition ip = ii.posList[0];
                        if (BagCapacityUtil.CanAddToBag(args, fd, ip))
                        {
                            int[] next = defInv.GetNextEmptyPosition(ip.key);
                            ItemInventoryUtil.MovePosition(ip, defInv, next[0], next[1], (ISkillArgs)args);
                        }
                        else
                        {
                            ItemInventory ground = fd.freeInventory.GetInventoryManager().GetInventory("ground");
                            int[] next = ground.GetNextEmptyPosition(ip.key);
                            ItemInventoryUtil.MovePosition(ip, ground, next[0], next[1], (ISkillArgs)args);
                        }
                    }
                }

                CarryClipUtil.AddCurrentClipToBag(key, fd, args);
            }
        }

        private void HandleBullet(IEventArgs args)
        {
            if ("SetWeaponClip" == code)
            {
                FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit("current");
                int ClipType = FreeUtil.ReplaceInt("{item.ClipType}", args);

                fd.Player.WeaponController().SetReservedBullet((EBulletCaliber)ClipType, CarryClipUtil.GetClipCount(ClipType, fd, args));

            }
            else if ("ClearWeaponClip" == code)
            {
                FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit("current");
                int ClipType = FreeUtil.ReplaceInt("{item.ClipType}", args);
                if (fd != null)
                {
                    fd.Player.WeaponController().SetReservedBullet((EBulletCaliber)ClipType, 0);
                }
            }
            else if ("SetItemClip" == code)
            {
                FreeData fd = (FreeData)((FreeRuleEventArgs)args).GetUnit("current");
                int ClipType = FreeUtil.ReplaceInt("{state.ClipType}", args);
                int count = fd.Player.WeaponController().GetReservedBullet((EBulletCaliber)ClipType);
                int itemCount = CarryClipUtil.GetClipCount(ClipType, fd, args);
                int delta = count - itemCount;
                if (delta > 0)
                {
                    CarryClipUtil.AddClip(Math.Abs(delta), ClipType, fd, args);
                }
                else
                {
                    CarryClipUtil.DeleteClip(Math.Abs(delta), ClipType, fd, args);
                }
            }
        }

        private void HandleAddFuel(IEventArgs args)
        {
            if (code == "AddFuel")
            {
                FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
                PlayerEntity p = fr.GetPlayer("current");
                if (p != null && p.IsOnVehicle())
                {
                    VehicleEntity v = fr.GameContext.vehicle.GetEntityWithEntityKey(p.controlledVehicle.EntityKey);
                    if (v != null)
                    {
                        v.GetGameData().RemainingFuel = Math.Min(v.GetGameData().MaxFuel, v.GetGameData().RemainingFuel + 20);
                    }
                }
            }
        }

        private void HandleFrame(IEventArgs args)
        {
            if (code == "RecordFrame")
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.TestFrame;

                SendMessageAction.sender.SendMessage(args, message, 4, string.Empty);
            }
        }

        private void HandleScore(IEventArgs args)
        {
            if (code == "WaitInfo")
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ScoreInfo;
                message.Ks.Add(1);

                message.Ins.Add(FreeUtil.ReplaceInt("{playerCount}", args));

                //Debug.Log(message.ToString());

                SendMessageAction.sender.SendMessage(args, message, 4, string.Empty);
            }
            else if (code == "ScoreInfo")
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ScoreInfo;
                message.Ks.Add(2);

                message.Bs.Add(true);
                message.Ins.Add(FreeUtil.ReplaceInt("{current.killNum}", args));
                message.Ins.Add(FreeUtil.ReplaceInt("{startPlayerCount}", args));

                //Debug.Log(message.ToString());

                SendMessageAction.sender.SendMessage(args, message, 1, "current");

            }
        }

        private void HandleKill(IEventArgs args)
        {
            if (code == "KillInfo")
            {
                SimpleProto message = FreePool.Allocate();
                message.Key = FreeMessageConstant.ScoreInfo;
                message.Ks.Add(3);
                message.Bs.Add(true);

                PlayerDamageInfo damageInfo = GetDamageInfo(args);

                //击杀者姓名
                if (args.GetUnit("killer") == null)
                {
                    message.Ss.Add("");
                    message.Ds.Add(-1);
                    message.Ins.Add(0);
                    message.Ins.Add(damageInfo.KillType);
                    message.Ins.Add(damageInfo.KillFeedbackType);
                }
                else
                {
                    //击杀者姓名
                    message.Ss.Add(FreeUtil.ReplaceVar("{killer.PlayerName}", args));
                    //击杀者队伍ID
                    message.Ds.Add(FreeUtil.ReplaceDouble("{killer.TeamId}", args));
                    //击杀者武器ID
                    message.Ins.Add(damageInfo.weaponId);
                    //击杀方式
                    message.Ins.Add(damageInfo.KillType);
                    //击杀反馈
                    message.Ins.Add(damageInfo.KillFeedbackType);
                }

                //死者姓名
                message.Ss.Add(FreeUtil.ReplaceVar("{killed.PlayerName}", args));
                //死者队伍ID
                message.Ds.Add(FreeUtil.ReplaceDouble("{killed.TeamId}", args));
                //死亡方式
                message.Ins.Add(damageInfo.type);

                //                Debug.Log("KillInfo ..." + message.ToString());

                SendMessageAction.sender.SendMessage(args, message, 4, string.Empty);
            }
        }
    }
}
