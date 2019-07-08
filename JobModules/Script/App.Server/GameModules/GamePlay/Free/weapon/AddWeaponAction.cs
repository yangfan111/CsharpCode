using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.unit;
using com.wd.free.util;
using Core;
using Core.Utils;
using System;
using Utils.Singleton;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class NewAddWeaponAction : AbstractPlayerAction, IRule
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(NewAddWeaponAction));

        private string weaponKey;
        private string weaponId;
        private string fullAmmo;
        private string replace;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            IGameUnit unit = GetPlayer(args);

            if (unit != null)
            {
                PlayerEntity p = ((FreeData)unit).Player;
                
                int itemId = FreeUtil.ReplaceInt(weaponId, args);
                int index = FreeUtil.ReplaceInt(weaponKey, args);
                EWeaponSlotType st = FreeWeaponUtil.GetSlotType(index);

                WeaponBaseAgent agent = null;
                if (index == 0)
                    agent = p.WeaponController().HeldWeaponAgent;
                else
                    agent = p.WeaponController().GetWeaponAgent(st);
                if (agent != null && agent.IsValid() && !FreeUtil.ReplaceBool(replace, args))
                {
                    return;
                }

                var scan = WeaponUtil.CreateScan(itemId);
                if (FreeUtil.ReplaceBool(fullAmmo, args))
                {
                    var weaponAllConfig = SingletonManager.Get<WeaponConfigManagement>().FindConfigById(itemId);
                    scan.Bullet = weaponAllConfig.PropertyCfg.Bullet;
                    scan.ReservedBullet = weaponAllConfig.PropertyCfg.Bulletmax;
                }
                if (index == 0)
                {
                    p.WeaponController().PickUpWeapon(scan);
                }
                else
                {
                    p.WeaponController().ReplaceWeaponToSlot(st, scan);
                    if (p.stateInterface.State.CanDraw() && p.WeaponController().HeldSlotType == EWeaponSlotType.None)
                    {
                        p.WeaponController().TryArmWeaponImmediately(st);
                    }
                }

                /*SimpleProto message = new SimpleProto();
                message.Ins.Add(itemId);
                if (index > 0)
                {
                    message.Ins.Add((int)st);
                }
                else
                {
                    message.Ins.Add(-1);
                }

                message.Ks.Add(2);
                message.Key = FreeMessageConstant.ChangeAvatar;
                FreeMessageSender.SendMessage(p, message);*/
                //p.network.NetworkChannel.SendReliable((int)EServer2ClientMessage.FreeData, message);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.NewAddWeaponAction;
        }
    }
}
