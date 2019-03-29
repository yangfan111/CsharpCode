using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XmlConfig;

namespace App.Server.GameModules.GamePlay.Free.weapon
{
    public class FreeWeaponUtil
    {
        private static EWeaponSlotType[] types;
        private static EWeaponPartType[] partTypes;

        static FreeWeaponUtil()
        {
            types = new EWeaponSlotType[7];
            types[1] = EWeaponSlotType.PrimeWeapon;
            types[2] = EWeaponSlotType.SecondaryWeapon;
            types[3] = EWeaponSlotType.PistolWeapon;
            types[4] = EWeaponSlotType.MeleeWeapon;
            types[5] = EWeaponSlotType.ThrowingWeapon;
            types[6] = EWeaponSlotType.TacticWeapon;

            partTypes = new EWeaponPartType[6];
            partTypes[1] = EWeaponPartType.UpperRail;
            partTypes[2] = EWeaponPartType.Muzzle;
            partTypes[3] = EWeaponPartType.LowerRail;
            partTypes[4] = EWeaponPartType.Magazine;
            partTypes[5] = EWeaponPartType.Stock;
        }

        public static int GetWeaponPart(EWeaponPartType partType)
        {
            for (int i = 1; i <= 5; i++)
            {
                if (partTypes[i] == partType)
                {
                    return i;
                }
            }

            return 0;
        }

        public static EWeaponPartType GetPartType(int type)
        {
            if (type > 0 && type < types.Length)
            {
                return partTypes[type];
            }

            return EWeaponPartType.UpperRail;
        }

        public static int GetWeaponKey(EWeaponSlotType st)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (types[i] == st)
                {
                    return i;
                }
            }

            return 0;
        }

        public static EWeaponSlotType GetSlotType(int index)
        {
            if (index >= 0 && index < types.Length)
            {
                return types[index];
            }

            return EWeaponSlotType.PrimeWeapon;
        }
    }
}
