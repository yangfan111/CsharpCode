using App.Shared;
using Assets.XmlConfig;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.util;
using Core;
using System;
using System.Collections.Generic;
using Core.Free;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class AdjustWeaponAction : AbstractPlayerAction, IRule
    {
        private string weaponType;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);
            if (player != null)
            {
                if (null == player.playerWeaponServerUpdate.ReservedWeaponSubType)
                {
                    player.playerWeaponServerUpdate.ReservedWeaponSubType = new List<int>();
                }
                else
                {
                    player.playerWeaponServerUpdate.ReservedWeaponSubType.Clear();
                }
                string[] types = weaponType.Split(new string[] {",", "，"}, StringSplitOptions.RemoveEmptyEntries);
                if (types.Length == 0)
                {
                    return;
                }

                foreach (string type in types)
                {
                    if (int.Parse(type) == 0)
                    {
                        player.playerWeaponServerUpdate.ReservedWeaponSubType.Clear();
                        return;
                    }
                    player.playerWeaponServerUpdate.ReservedWeaponSubType.Add(int.Parse(type));
                }

                bool grenadeReserved = false;
                if (player.playerWeaponServerUpdate.ReservedWeaponSubType.Contains((int) EWeaponSubType.Throw))
                {
                    grenadeReserved = true;
                }

                for (int i = (int) EWeaponSlotType.PrimeWeapon; i < (int) EWeaponSlotType.Length; i++)
                {
                    if (i == (int) EWeaponSlotType.ThrowingWeapon && grenadeReserved)
                        continue;
                    if (player.playerWeaponServerUpdate.ReservedWeaponSubType.Contains(player.WeaponController().GetWeaponAgent((EWeaponSlotType) i).WeaponConfigAssy.NewWeaponCfg.SubType))
                        continue;
                    player.WeaponController().DestroyWeapon((EWeaponSlotType)i, -1);
                }

                if (!grenadeReserved)
                {
                    player.WeaponController().GrenadeHandler.ClearCache();
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.AdjustWeaponAction;
        }
    }
}
