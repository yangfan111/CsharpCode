using App.Shared;
using Assets.XmlConfig;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using Core.Free;
using com.wd.free.util;
using Core;
using System;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class NewRemoveWeaponAction : AbstractPlayerAction, IRule
    {
        private string weaponKey;
        private string weaponType;

        public override void DoAction(IEventArgs args)
        {
            PlayerEntity player = GetPlayerEntity(args);

            if (player != null)
            {
                if (!StringUtil.IsNullOrEmpty(weaponKey))
                {
                    int index = FreeUtil.ReplaceInt(weaponKey, args);
                    EWeaponSlotType currentSlot = index > 0 ? FreeWeaponUtil.GetSlotType(index) : player.WeaponController().HeldSlotType;
                    player.WeaponController().DestroyWeapon(currentSlot, -1);
                }
                else
                {
                    int index = FreeUtil.ReplaceInt(weaponType, args);
                    if (index > 0)
                    {
                        if (index == (int) EWeaponSubType.Throw)
                        {
                            player.WeaponController().DestroyWeapon(EWeaponSlotType.ThrowingWeapon, -1);
                            player.WeaponController().GrenadeHandler.ClearCache();
                            /*var helper = p.WeaponController().GrenadeHelper;
                            helper.ClearCache();
                            p.playerWeaponUpdate.UpdateHeldAppearance = true;*/
                        }
                        else
                        {
                            for (int i = (int) EWeaponSlotType.PrimeWeapon; i < (int) EWeaponSlotType.Length; i++)
                            {
                                var weaponAgent = player.WeaponController().GetWeaponAgent((EWeaponSlotType)i);
                                if (weaponAgent.WeaponConfigAssy.NewWeaponCfg.SubType == index)
                                    player.WeaponController().DestroyWeapon((EWeaponSlotType) i, -1);
                            }
                        }
                    }
                    else
                    {
                        for (int i = (int)EWeaponSlotType.PrimeWeapon; i < (int)EWeaponSlotType.Length; i++)
                        {
                            player.WeaponController().DestroyWeapon((EWeaponSlotType)i, -1);
                        }
                    }
                }
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.NewRemoveWeaponAction;
        }
    }
}
