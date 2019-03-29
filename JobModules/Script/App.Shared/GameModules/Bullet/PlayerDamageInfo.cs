using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Enums;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Bullet
{
    public struct PlayerDamageInfo
    {
        public float damage;
        public int type;    //EUIDeadType
        public int part;    //EBodyPart
        public int weaponId;
        public bool IsOverWall;
        public bool IsKnife;

        //击杀信息（多状态|） EUIKillType
        public int KillType;
        //击杀反馈（多状态|） EUIKillFeedbackType
        public int KillFeedbackType;

        public PlayerDamageInfo(float damage, int type, int part, int weaponId, bool isOverWall = false, bool isKnife = false)
        {
            this.damage = damage;
            this.type = type;
            this.part = part;
            this.weaponId = weaponId;
            IsOverWall = isOverWall;
            KillType = 0;
            KillFeedbackType = 0;

            WeaponResConfigItem weapon = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
            if(weapon != null)
            {
                IsKnife = weapon.Type == (int)EWeaponType_Config.MeleeWeapon;
            }
            else
            {
                IsKnife = false;
            }
        }

        public EWeaponSubType WeaponType
        {
            get
            {
                if (type == (int)EUIDeadType.Weapon)
                {
                    var config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
                    if (null != config)
                    {
                        return (EWeaponSubType)config.SubType;
                    }
                }
                return EWeaponSubType.None;
            }
        }
    }
}
