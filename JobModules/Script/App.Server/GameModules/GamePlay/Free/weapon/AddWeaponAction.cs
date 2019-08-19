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
using Assets.App.Server.GameModules.GamePlay.Free;
using Free.framework;
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
                SimpleProto message = new SimpleProto();
                message.Key = FreeMessageConstant.PlaySound;
                message.Ks.Add(2);
                message.Ins.Add((int)EAudioUniqueId.PickupWeapon);
                message.Bs.Add(true);
                FreeMessageSender.SendMessage(p, message);
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.NewAddWeaponAction;
        }
    }
}
