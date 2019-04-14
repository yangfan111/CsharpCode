using App.Server.GameModules.GamePlay.free.player;
using App.Shared;
using com.cpkf.yyjd.tools.util;
using com.wd.free.action;
using com.wd.free.@event;
using com.wd.free.unit;
using com.wd.free.util;
using Core;
using System;
using System.Collections.Generic;
using Assets.XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    [Serializable]
    public class NewRemoveWeaponAction : AbstractPlayerAction
    {
        private string weaponKey;
        private string weaponType;

        public override void DoAction(IEventArgs args)
        {
            FreeRuleEventArgs fr = (FreeRuleEventArgs)args;

            IGameUnit unit = GetPlayer(args);

            if (unit != null)
            {
                PlayerEntity p = ((FreeData)unit).Player;

                if (!StringUtil.IsNullOrEmpty(weaponKey))
                {
                    int index = FreeUtil.ReplaceInt(weaponKey, args);
                    EWeaponSlotType currentSlot = index > 0 ? FreeWeaponUtil.GetSlotType(index) : p.WeaponController().HeldSlotType;
                    p.WeaponController().DestroyWeapon(currentSlot, -1);
                }
                else
                {
                    int index = FreeUtil.ReplaceInt(weaponType, args);
                    if (index > 0)
                    {
                        if (index == (int) EWeaponSubType.Throw)
                        {
                            p.WeaponController().DestroyWeapon(EWeaponSlotType.ThrowingWeapon, -1);
                            /*var helper = p.WeaponController().GrenadeHelper;
                            helper.ClearCache();
                            p.playerWeaponUpdate.UpdateHeldAppearance = true;*/
                        }
                        else
                        {
                            for (int i = (int) EWeaponSlotType.PrimeWeapon; i < (int) EWeaponSlotType.Length; i++)
                            {
                                var weaponAgent = p.WeaponController().GetWeaponAgent((EWeaponSlotType) i);
                                if (weaponAgent.WeaponConfigAssy.NewWeaponCfg.SubType == index)
                                    p.WeaponController().DestroyWeapon((EWeaponSlotType) i, -1);
                            }
                        }
                    }
                }
            }
        }
    }
}
