using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using I2.Loc;
using Utils.Utils;
using XmlConfig;

namespace App.Client.GameModules.Ui.Models.Common.Tip
{

    public class TipConst
    {
        public const string BulletPropertyName = "Bullet";
        public const string MaxBulletPropertyName = "Bulletmax";
        public static Dictionary<EWeaponPartType, string> WeaponPartTypeName;
        private static Dictionary<string, string> _weaponPropertyName;
        public static Dictionary<string, string> WeaponPropertyName
        {
            get
            {
                if (_weaponPropertyName == null)
                {
                    _weaponPropertyName = new Dictionary<string, string>();
                    _weaponPropertyName["Power"] = ScriptLocalization.hall.word459;
                    _weaponPropertyName["Limitcycle"] = ScriptLocalization.hall.word460;
                    _weaponPropertyName["Accurate"] = ScriptLocalization.hall.word461;
                    _weaponPropertyName["Weight"] = ScriptLocalization.hall.word462;
                    _weaponPropertyName["Stability"] = ScriptLocalization.hall.word463;
                    //_weaponPropertyName["Penetrate"] = ScriptLocalization.hall.word464;
                    _weaponPropertyName["Scope"] = ScriptLocalization.hall.word465;
                    _weaponPropertyName["Bullet"] = ScriptLocalization.hall.word466;
                    _weaponPropertyName["Bulletmax"] = ScriptLocalization.hall.word467;
                }
                return _weaponPropertyName;
            }
        }

        internal static string GetWeaponPartTypeName(EWeaponPartType type)
        {
            if (WeaponPartTypeName == null)
            {
                WeaponPartTypeName = new Dictionary<EWeaponPartType, string>(CommonIntEnumEqualityComparer<EWeaponPartType>.Instance);
                WeaponPartTypeName[EWeaponPartType.None] = ScriptLocalization.hall.word437;
                WeaponPartTypeName[EWeaponPartType.Magazine] = ScriptLocalization.hall.word453;
                WeaponPartTypeName[EWeaponPartType.Muzzle] = ScriptLocalization.hall.word454;
                WeaponPartTypeName[EWeaponPartType.UpperRail] = ScriptLocalization.hall.word455;
                WeaponPartTypeName[EWeaponPartType.SideRail] = ScriptLocalization.hall.word456;
                WeaponPartTypeName[EWeaponPartType.LowerRail] = ScriptLocalization.hall.word457;
                WeaponPartTypeName[EWeaponPartType.Stock] = ScriptLocalization.hall.word458;
                WeaponPartTypeName[EWeaponPartType.Bore] = "枪膛";
                WeaponPartTypeName[EWeaponPartType.Feed] = "供弹";
                WeaponPartTypeName[EWeaponPartType.Trigger] = "击发";
                WeaponPartTypeName[EWeaponPartType.Interlock] = "闭锁";
                WeaponPartTypeName[EWeaponPartType.Brake] = "制退";
            }

            var res = string.Empty;
            WeaponPartTypeName.TryGetValue(type, out res);
            return res;
        }

        public static string GetWeaponPropertyName(string pname)
        {
            if (WeaponPropertyName.ContainsKey(pname))
            {
                return WeaponPropertyName[pname];
            }
            return string.Empty;
        }
    }
}
