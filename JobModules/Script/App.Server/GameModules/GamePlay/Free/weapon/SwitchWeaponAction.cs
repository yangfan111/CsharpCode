using com.wd.free.action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using Utils.Appearance;
using Core;
using com.wd.free.util;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class SwitchWeaponAction : AbstractPlayerAction
    {
        private string weaponKey;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;
            PlayerEntity playerEntity = (PlayerEntity)fr.GetEntity(player);
            int index = FreeUtil.ReplaceInt(weaponKey, args);
            EWeaponSlotType st = FreeWeaponUtil.GetSlotType(index);

            WeaponToHand(playerEntity, st);
        }

        public static void WeaponToHand(PlayerEntity player, EWeaponSlotType slot)
        {
            var pos = WeaponInPackage.EndOfTheWorld;
            switch (slot)
            {
                case EWeaponSlotType.PrimeWeapon:
                    pos = WeaponInPackage.PrimaryWeaponOne;
                    break;
                case EWeaponSlotType.SecondaryWeapon:
                    pos = WeaponInPackage.PrimaryWeaponTwo;
                    break;
                case EWeaponSlotType.PistolWeapon:
                    pos = WeaponInPackage.SideArm;
                    break;
                case EWeaponSlotType.MeleeWeapon:
                    pos = WeaponInPackage.MeleeWeapon;
                    break;
                case EWeaponSlotType.ThrowingWeapon:
                    pos = WeaponInPackage.ThrownWeapon;
                    break;
                case EWeaponSlotType.TacticWeapon:
                    pos = WeaponInPackage.TacticWeapon;
                    break;
                default:
                    break;
            }
            player.appearanceInterface.Appearance.MountWeaponToHand(pos);
        }
    }
}
