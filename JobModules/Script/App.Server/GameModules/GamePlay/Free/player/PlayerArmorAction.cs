using App.Shared.GameModules.Attack;
using Assets.App.Server.GameModules.GamePlay.Free;
using Assets.Utils.Configuration;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.para;
using Core.Enums;
using Core.Free;
using Free.framework;
using System;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.player
{
    [Serializable]
    public class PlayerArmorAction : AbstractPlayerAction, IRule
    {
        public override void DoAction(IEventArgs args)
        {
            PlayerEntity playerEntity = GetPlayerEntity(args);
            if (playerEntity != null)
            {
                SimpleParable sp = (SimpleParable) args.GetUnit("damage");
                if (sp != null)
                {
                    FloatPara d = (FloatPara) args.GetDefault().GetParameters().Get("damage");
                    PlayerDamageInfo damage = (PlayerDamageInfo) sp.GetFieldObject(0);

                    if (damage.type != (int) EUIDeadType.Weapon && damage.type != (int) EUIDeadType.Unarmed)
                    {
                        return;
                    }

                    if (playerEntity.gamePlay.CurHelmet > 0)
                    {
                        var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(playerEntity.gamePlay.HelmetLv);
                        if (config != null)
                        {
                            if (config.NewWeaponCfg.ProtectivePartsList.Contains(damage.part))
                            {
                                float readDamage = damage.damage;
                                float reduce = readDamage * config.NewWeaponCfg.DamageReduction / 100;
                                reduce = Math.Min(playerEntity.gamePlay.CurHelmet, reduce);
                                playerEntity.gamePlay.CurHelmet = Math.Max(0, playerEntity.gamePlay.CurHelmet - (int) readDamage);
                                if (reduce > 0 && playerEntity.gamePlay.CurHelmet == 0)
                                {
                                    playerEntity.gamePlay.HelmetLv = playerEntity.gamePlay.MaxHelmet = 0;
                                    SimpleProto msg = FreePool.Allocate();
                                    msg.Key = FreeMessageConstant.ChickenTip;
                                    msg.Ss.Add("word75," + config.NewWeaponCfg.Name);
                                    FreeMessageSender.SendMessage(playerEntity, msg);
                                }

                                damage.damage -= reduce;
                                playerEntity.statisticsData.Statistics.DefenseDamage += reduce;

                                d.SetValue(damage.damage);
                            }
                        }
                    }

                    if (playerEntity.gamePlay.CurArmor > 0)
                    {
                        var config = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(playerEntity.gamePlay.ArmorLv);
                        if (config != null)
                        {
                            if (config.NewWeaponCfg.ProtectivePartsList.Contains(damage.part))
                            {
                                float readDamage = damage.damage;
                                float reduce = readDamage * config.NewWeaponCfg.DamageReduction / 100;
                                reduce = Math.Min(playerEntity.gamePlay.CurArmor, reduce);
                                playerEntity.gamePlay.CurArmor = Math.Max(0,playerEntity.gamePlay.CurArmor - (int) readDamage);
                                if (reduce > 0 && playerEntity.gamePlay.CurArmor == 0)
                                {
                                    playerEntity.gamePlay.ArmorLv = playerEntity.gamePlay.MaxArmor = 0;
                                    SimpleProto msg = FreePool.Allocate();
                                    msg.Key = FreeMessageConstant.ChickenTip;
                                    msg.Ss.Add("word75," + config.NewWeaponCfg.Name);
                                    FreeMessageSender.SendMessage(playerEntity, msg);
                                }

                                damage.damage -= reduce;
                                playerEntity.statisticsData.Statistics.DefenseDamage += reduce;

                                d.SetValue(damage.damage);
                            }
                        }
                    }
                }
            }
        }

        public int GetRuleID()
        {
            return (int) ERuleIds.PlayerArmorAction;
        }
    }
}